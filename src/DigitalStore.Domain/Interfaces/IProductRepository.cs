using DigitalStore.Domain.Entities;

namespace DigitalStore.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetAllPublishedAsync(CancellationToken ct = default);
}
