using DigitalStore.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace DigitalStore.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _secureRoot;

    public LocalFileStorageService(IHostEnvironment env)
    {
        _secureRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "SecureStorage"));
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> GetFileAsync(
        string relativePath, CancellationToken ct)
    {
        var fullPath = GetSafePath(relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File không tồn tại trên server.", fullPath);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read,
                                    FileShare.Read, bufferSize: 4096, useAsync: true);
        var fileName = Path.GetFileName(fullPath);
        return (stream, "application/octet-stream", fileName);
    }

    public Task<bool> ExistsAsync(string relativePath, CancellationToken ct)
    {
        var fullPath = GetSafePath(relativePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    private string GetSafePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_secureRoot, relativePath));

        // Chống Path Traversal
        if (!fullPath.StartsWith(_secureRoot, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Đường dẫn file không hợp lệ.");

        return fullPath;
    }
}
