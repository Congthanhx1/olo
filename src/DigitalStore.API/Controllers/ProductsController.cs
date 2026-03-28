using DigitalStore.Application.Services;
using DigitalStore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalStore.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _products;
    public ProductsController(ProductService products) => _products = products;

    /// <summary>GET /api/products — Danh sách sản phẩm công khai</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _products.GetAllAsync(ct);
        return Ok(list);
    }

    /// <summary>
    /// GET /api/products/{id} — Chi tiết + trạng thái quyền của user
    /// Response gồm hasAccess: true/false để frontend ẩn/hiện nút Download/Xem video
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetail(int id, CancellationToken ct)
    {
        // Lấy userId nếu đã login, 0 nếu chưa
        int userId = 0;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(claim, out userId);

        try
        {
            var (product, hasAccess) = await _products.GetDetailAsync(userId, id, ct);
            return Ok(new { product, hasAccess });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
