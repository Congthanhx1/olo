using DigitalStore.Domain.Enums;

namespace DigitalStore.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? PaymentGateway { get; set; }          // "VNPay" | "Stripe"
    public string? PaymentTransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    public ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
}
