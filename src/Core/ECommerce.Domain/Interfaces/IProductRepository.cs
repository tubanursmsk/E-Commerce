using ECommerce.Domain.Entities;
using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
         Task<IEnumerable<Product>> GetAllWithCategoryAndBrandAsync();

         Task<IEnumerable<Product>> GetByCompanyIdListAsync(Guid companyId);
         Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredAsync(ProductFilterParams filter);
    }

    
}