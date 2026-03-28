using DigitalStore.Application.Common.Exceptions;
using DigitalStore.Application.Common.Interfaces;
using DigitalStore.Domain.Interfaces;

namespace DigitalStore.Application.Services;

public record FileDownloadResult(Stream FileStream, string ContentType, string FileName);

public class DownloadService
{
    private readonly IUserAccessRepository _accessRepo;
    private readonly IProductRepository _productRepo;
    private readonly IFileStorageService _storage;

    public DownloadService(
        IUserAccessRepository accessRepo,
        IProductRepository productRepo,
        IFileStorageService storage)
    {
        _accessRepo = accessRepo;
        _productRepo = productRepo;
        _storage = storage;
    }

    public async Task<FileDownloadResult> GetDownloadAsync(int userId, int productId, CancellationToken ct)
    {
        // 1. Kiểm tra sản phẩm tồn tại
        var product = await _productRepo.GetByIdAsync(productId, ct)
                      ?? throw new NotFoundException($"Sản phẩm {productId} không tồn tại.");

        // 2. Xác nhận có file để tải (không phải Course-only)
        if (string.IsNullOrEmpty(product.FileStoragePath))
            throw new InvalidOperationException("Sản phẩm này không có file downloadable.");

        // 3. Kiểm tra quyền truy cập trong bảng UserAccess
        bool hasAccess = await _accessRepo.HasAccessAsync(userId, productId, ct);
        if (!hasAccess)
            throw new ForbiddenException("Bạn chưa mua công cụ này hoặc thời hạn truy cập đã kết thúc.");

        // 4. Lấy file từ SecureStorage
        var (stream, contentType, fileName) = await _storage.GetFileAsync(product.FileStoragePath, ct);
        return new FileDownloadResult(stream, contentType, fileName);
    }
}
