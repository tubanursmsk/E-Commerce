using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Repositories;

public class CargoRepository : GenericRepository<Cargo>, ICargoRepository
{
    private new readonly AppDbContext _context;
    public CargoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }


    
}
