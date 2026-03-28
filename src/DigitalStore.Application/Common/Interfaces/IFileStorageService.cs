namespace DigitalStore.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<(Stream FileStream, string ContentType, string FileName)> GetFileAsync(
        string relativePath, CancellationToken ct = default);
    Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default);
}
