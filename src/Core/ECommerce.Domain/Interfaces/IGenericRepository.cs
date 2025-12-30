using System.Linq.Expressions;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity // T, BaseEntity türünden bir entity olmalı daha genel kullanım istersek clsass da yapabiliriz
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();

    // Filtreli arama yapmak için (Örn: İsme göre getir)
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity); //tek bir entity eklemek için T entity kullanılır

    //Task AddRangeAsync(IEnumerable<T> entities); //birden fazla entity eklemek için IEnumerable kullanılır
    void Update(T entity);
    void Delete(T entity);
}