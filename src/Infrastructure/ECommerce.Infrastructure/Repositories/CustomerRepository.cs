using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    private new readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllWithUserAsync()
    {
        return await _context.Customers
            .Include(c => c.User) // User tablosunu bağla
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> FindWithUserAsync(Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers
            .Include(c => c.User) // User tablosunu bağla
            .Where(predicate)
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }
}