using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
         Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    }
}