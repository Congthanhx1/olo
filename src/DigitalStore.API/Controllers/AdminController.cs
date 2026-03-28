using DigitalStore.Application.Common.Interfaces;
using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Enums;
using DigitalStore.Domain.Interfaces;
using DigitalStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace DigitalStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // Uncomment when roles are implemented
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public AdminController(AppDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpPost("products/{id}/upload")]
    public async Task<IActionResult> UploadFile(int id, IFormFile file)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var relativePath = $"{product.Type.ToString().ToLower()}s/{file.FileName}";
        var securePath = Path.Combine(Directory.GetCurrentDirectory(), "SecureStorage", relativePath);
        
        var directory = Path.GetDirectoryName(securePath);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

        using (var stream = new FileStream(securePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        product.FileStoragePath = relativePath;
        await _context.SaveChangesAsync();

        return Ok(new { FilePath = relativePath });
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
