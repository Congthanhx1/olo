using DigitalStore.Application.Common.Exceptions;
using DigitalStore.Application.DTOs.Orders;
using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Enums;
using DigitalStore.Domain.Interfaces;

namespace DigitalStore.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly IUserAccessRepository _access;

    public OrderService(
        IOrderRepository orders,
        IProductRepository products,
        IUserAccessRepository access)
    {
        _orders = orders;
        _products = products;
        _access = access;
    }

    /// <summary>Tạo đơn hàng pending, chờ Payment Gateway callback.</summary>
    public async Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest req, CancellationToken ct)
    {
        var order = new Order
        {
            UserId = userId,
            PaymentGateway = req.PaymentGateway,
            Status = OrderStatus.Pending,
        };

        decimal total = 0;
        foreach (var productId in req.ProductIds)
        {
            var product = await _products.GetByIdAsync(productId, ct)
                          ?? throw new NotFoundException($"Sản phẩm {productId} không tồn tại.");

            order.Details.Add(new OrderDetail
            {
                ProductId = productId,
                PriceAtPurchase = product.Price
            });
            total += product.Price;
        }

        order.TotalAmount = total;
        var created = await _orders.CreateAsync(order, ct);
        return new OrderResponse(created.Id, created.TotalAmount, created.Status.ToString(), created.CreatedAt);
    }

    /// <summary>
    /// Gọi khi Payment Gateway webhook xác nhận thanh toán thành công.
    /// Tự động cấp quyền (UserAccess) cho tất cả sản phẩm trong đơn hàng.
    /// </summary>
    public async Task ConfirmPaymentAsync(int orderId, string transactionId, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Đơn hàng {orderId} không tồn tại.");

        if (order.Status == OrderStatus.Paid)
            return; // idempotent — đã xử lý rồi, bỏ qua

        order.Status = OrderStatus.Paid;
        order.PaymentTransactionId = transactionId;
        order.PaidAt = DateTime.UtcNow;

        // Cấp quyền truy cập cho từng sản phẩm trong đơn
        foreach (var detail in order.Details)
        {
            await _access.AddAsync(new UserAccess
            {
                UserId = order.UserId,
                ProductId = detail.ProductId,
                OrderId = orderId,
                ExpiresAt = null // vĩnh viễn
            }, ct);
        }

        await _orders.UpdateAsync(order, ct);
    }
}
