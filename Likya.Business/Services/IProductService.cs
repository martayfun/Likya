using System.Collections.Generic;
using System.Threading.Tasks;
using Likya.Core.Models;

namespace Likya.Business.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllNameOrdered();
        Task Save(Product product);
    }
}
