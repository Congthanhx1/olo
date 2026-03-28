using DigitalStore.Domain.Enums;

namespace DigitalStore.Application.DTOs.Products;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    ProductType Type,
    decimal Price,
    string? ThumbnailUrl,
    bool IsPublished,
    string CategoryName
);
