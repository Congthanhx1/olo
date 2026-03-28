using DigitalStore.Domain.Enums;

namespace DigitalStore.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProductType Type { get; set; }
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FileStoragePath { get; set; }   // Relative path trong SecureStorage/
    public string? VideoStreamUrl { get; set; }    // HLS manifest URL
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Category Category { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
}
