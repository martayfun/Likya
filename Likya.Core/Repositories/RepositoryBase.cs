using Likya.Core.EntityFramework;

namespace Likya.Core.Repositories
{
    public class RepositoryBase
    {
        public readonly ApplicationContext _dbContext;
        public RepositoryBase(ApplicationContext pContext)
        {
            this._dbContext = pContext;
        }
    }
}