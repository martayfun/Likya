using System.Collections.Generic;
using System.Threading.Tasks;
using Likya.Core.EntityFramework;
using Likya.Core.Models;
using Likya.Core.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Likya.Data.Repositories
{
    public class ProductRepository : RepositoryBase
    {
        public ProductRepository(ApplicationContext pContext) : base(pContext)
        {
        }
        public async Task<List<Product>> GetAllNameOrdered()
        {
            return await _dbContext.Products.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync();
        } 
    }
}
