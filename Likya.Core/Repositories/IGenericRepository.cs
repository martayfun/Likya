using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Likya.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(params object[] keyValue);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddIfNotExists(Expression<Func<T, bool>> predicate, T entity);
        Task AddIfNotExists(Expression<Func<T, bool>> predicate, Func<T> entity);
        void Update(T entity);
        void Delete(T entity);
        Task Delete(int id);
    }
}
