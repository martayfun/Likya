using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Likya.Core.Attributes;
using Likya.Core.EntityFramework;
using Likya.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace Likya.Core.UnitOfWork
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _dbContext;

        private IDbContextTransaction _tran;
        private readonly IServiceProvider _services;

        public SqlUnitOfWork(ApplicationContext pDbContext, IServiceProvider services)
        {
            this._dbContext = pDbContext ?? throw new ArgumentException("DbContext Can Not Null");
            this._services = services;
        }

        public Guid? GetUserId()
        {
            IHttpContextAccessor ca = (IHttpContextAccessor)_services.GetService(typeof(IHttpContextAccessor));
            if (ca != null)
            {
                if (ca?.HttpContext?.User != null && ca.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Parse(ca.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            return null;
        }

        #region SaveChanges
        public async Task<int> SaveChangesAsync(bool pUseTransaction = false)
        {
            IDbContextTransaction tran = null;
            try
            {
                tran = pUseTransaction ? await BeginTransaction() : null;

                Guid? UserId = GetUserId();

                Dictionary<EntityEntry, EntityState> Entries = _dbContext.ChangeTracker.Entries().Where(e => e.State != Microsoft.EntityFrameworkCore.EntityState.Unchanged).ToDictionary(k => k, v => v.State);

                int Result = await _dbContext.SaveChangesAsync();

                await CheckTracables(Entries, UserId);

                if (tran != null)
                    CommitTransaction();

                return Result;
            }
            catch (Exception ex)
            {
                if (tran != null)
                    RollBackTransaction();
                throw ex;
            }
        }

        private async Task CheckTracables(Dictionary<EntityEntry, EntityState> entries, Guid? userId)
        {
            if (entries == null || !entries.Any())
                return;

            bool Logged = false;
            try
            {
                foreach (var entry in entries)
                {
                    if (entry.Key.Entity.GetType().GetCustomAttributes(typeof(TrackableAttribute), false).FirstOrDefault() is TrackableAttribute attr)
                    {
                        if (attr.States.Contains(entry.Value))
                        {
                            WaitNewEntities(entries);

                            Logged = true;
                            await _dbContext.EntityLogs.AddAsync(new Models.EntityLog
                            {
                                UserId = userId,
                                CreateAt = DateTime.Now,
                                State = entry.Value,
                                Data = attr.Detailed ? JsonConvert.SerializeObject(entry.Key.Entity, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) : null,
                                RecordId = entry.Key.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue.ToString(),
                                TableName = entry.Key.Entity.GetType().Name
                            });
                        }
                    }
                }
                if (Logged)
                    await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Şimdilik boş kalacak
            }
        }

        private void WaitNewEntities(Dictionary<EntityEntry, EntityState> entries)
        {
            var MustWait = entries
                .Any(e => e.Value == EntityState.Added && e.Key.Properties.Any(p => p.Metadata.Name == "Id" && !p.CurrentValue.GetType().Equals(typeof(Guid)) && Convert.ToInt64(p.CurrentValue) < 1));

            while (MustWait)
                Task.Delay(500);
        }
        #endregion

        #region Transaction
        public async Task<IDbContextTransaction> BeginTransaction()
        {
            if (!IsTransactionActive)
                _tran = await _dbContext.Database.BeginTransactionAsync();
            return _tran;
        }

        public void CommitTransaction()
        {
            if (IsTransactionActive)
                _tran.Commit();
        }

        public void RollBackTransaction()
        {
            if (IsTransactionActive)
                _tran.Rollback();
        }

        public bool IsTransactionActive => _tran != null;

        #endregion

        #region Repository Resolvers
        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(_dbContext);
        }

        public T GetRepository<T>() where T : RepositoryBase
        {
            return (T)Activator.CreateInstance(typeof(T), this._dbContext);
        }
        #endregion

        #region Disposing
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_tran != null)
                        _tran.Dispose();
                    _dbContext.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
