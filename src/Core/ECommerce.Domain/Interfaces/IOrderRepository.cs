using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Müşteri ve User bilgileriyle birlikte filtreleyerek getirir
        Task<IEnumerable<Order>> GetByCustomerIdWithDetailsAsync(Guid customerId, Guid? companyId);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync(Guid? companyId);
        Task<Order?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Order>> GetAllWithItemsAsync(Guid? companyId);
    }
}