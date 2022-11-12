using Likya.Core.Models;
using Likya.Core.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Likya.Core.EntityFramework
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<EntityLog> EntityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasKey(m => new { m.Id });
            builder.Entity<EntityLog>().HasKey(x => new { x.Id });
            base.OnModelCreating(builder);
        }
    }
}
