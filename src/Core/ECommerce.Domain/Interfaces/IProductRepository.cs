using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
         Task<IEnumerable<Product>> GetAllWithCategoryAndBrandAsync();

         Task<IEnumerable<Product>> GetByCompanyIdListAsync(Guid companyId);

        // Task<IEnumerable<Product>> GetAllWithBrandAsync();
    }
}