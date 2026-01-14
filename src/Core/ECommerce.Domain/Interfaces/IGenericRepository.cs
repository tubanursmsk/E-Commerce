using System.Linq.Expressions;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity // T, BaseEntity türünden bir entity olmalı daha genel kullanım istersek clsass da yapabiliriz
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();

    // Filtreli arama yapmak için (Örn: İsme göre getir)
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Filtreye göre sayı dönecek metot imzası - Dashboard için tanımladım
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // Decimal değerlerin toplamını döner bu şekilde aylık satış tutarını hesaplıyoruz
    Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector);

    Task AddAsync(T entity); //tek bir entity eklemek için T entity kullanılır

    //Task AddRangeAsync(IEnumerable<T> entities); //birden fazla entity eklemek için IEnumerable kullanılır
    void Update(T entity);
    void Delete(T entity);

}