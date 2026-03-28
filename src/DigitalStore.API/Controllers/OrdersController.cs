using DigitalStore.Application.Services;
using DigitalStore.Application.Common.Exceptions;
using DigitalStore.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalStore.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;
    public OrdersController(OrderService orders) => _orders = orders;

    /// <summary>POST /api/orders — Tạo đơn hàng mới (yêu cầu login)</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

        try
        {
            var result = await _orders.CreateOrderAsync(userId, req, ct);
            return CreatedAtAction(nameof(CreateOrder), new { orderId = result.OrderId }, result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/orders/webhook/confirm
    /// Payment Gateway (VNPay/Stripe) gọi vào đây sau khi thanh toán thành công.
    /// Tự động cấp UserAccess cho tất cả sản phẩm trong đơn.
    /// </summary>
    [HttpPost("webhook/confirm")]
    [AllowAnonymous] // Gateway gọi, không có JWT — bảo mật bằng secret header
    public async Task<IActionResult> ConfirmPayment(
        [FromBody] PaymentWebhookRequest req,
        [FromHeader(Name = "X-Webhook-Secret")] string? secret,
        [FromServices] IConfiguration config,
        CancellationToken ct)
    {
        // Xác thực webhook secret để tránh giả mạo
        if (secret != config["Webhook:Secret"])
            return Unauthorized(new { message = "Invalid webhook secret." });

        try
        {
            await _orders.ConfirmPaymentAsync(req.OrderId, req.TransactionId, ct);
            return Ok(new { message = "Thanh toán xác nhận thành công." });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record PaymentWebhookRequest(int OrderId, string TransactionId);
