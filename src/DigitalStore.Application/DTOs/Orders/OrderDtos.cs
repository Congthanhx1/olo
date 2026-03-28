namespace DigitalStore.Application.DTOs.Orders;

public record CreateOrderRequest(List<int> ProductIds, string PaymentGateway);

public record OrderResponse(int OrderId, decimal TotalAmount, string Status, DateTime CreatedAt);
