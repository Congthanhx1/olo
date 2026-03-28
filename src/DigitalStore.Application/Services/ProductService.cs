using DigitalStore.Application.DTOs.Products;
using DigitalStore.Domain.Interfaces;

namespace DigitalStore.Application.Services;

public class ProductService
{
    private readonly IProductRepository _products;
    private readonly IUserAccessRepository _access;

    public ProductService(IProductRepository products, IUserAccessRepository access)
    {
        _products = products;
        _access = access;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _products.GetAllPublishedAsync(ct);
        return list.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Type, p.Price,
            p.ThumbnailUrl, p.IsPublished, p.Category.Name));
    }

    public async Task<(ProductDto Product, bool HasAccess)> GetDetailAsync(int userId, int productId, CancellationToken ct)
    {
        var p = await _products.GetByIdAsync(productId, ct)
                ?? throw new Common.Exceptions.NotFoundException($"Sản phẩm {productId} không tồn tại.");

        bool hasAccess = await _access.HasAccessAsync(userId, productId, ct);

        return (new ProductDto(p.Id, p.Name, p.Description, p.Type, p.Price,
                               p.ThumbnailUrl, p.IsPublished, p.Category.Name), hasAccess);
    }
}
