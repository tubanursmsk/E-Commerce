using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
         Task<IEnumerable<Review>> GetAllWithDetailsAsync(Guid? companyId);
    }
}