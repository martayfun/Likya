using System.Collections.Generic;
using System.Threading.Tasks;
using Likya.Core.Models;
using Likya.Core.UnitOfWork;
using Likya.Data.Repositories;

namespace Likya.Business.Services
{
    public class ProductService : IProductService
    {
        IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Product>> GetAllNameOrdered()
        {
            return await _unitOfWork.GetRepository<ProductRepository>().GetAllNameOrdered();
        }

        public async Task Save(Product product)
        {
            if (product.Id == 0)
            {
                await _unitOfWork.GetGenericRepository<Product>().AddAsync(product);
            }
            else
            {
                _unitOfWork.GetGenericRepository<Product>().Update(product);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
