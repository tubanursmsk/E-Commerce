using ECommerce.Domain.Entities;
using System.Linq.Expressions;

namespace ECommerce.Domain.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    // User verisiyle birlikte tüm müşterileri getirir
    Task<IEnumerable<Customer>> GetAllWithUserAsync();

    // Filtreleme yaparak User verisiyle birlikte müşterileri getirir (Arama için)
    Task<IEnumerable<Customer>> FindWithUserAsync(Expression<Func<Customer, bool>> predicate);
}