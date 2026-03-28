using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Interfaces;
using DigitalStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalStore.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => await _db.Products.Include(p => p.Category)
                              .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Product>> GetAllPublishedAsync(CancellationToken ct)
        => await _db.Products.Include(p => p.Category)
                              .Where(p => p.IsPublished)
                              .ToListAsync(ct);
}
