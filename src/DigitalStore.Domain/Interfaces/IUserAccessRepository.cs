using DigitalStore.Domain.Entities;

namespace DigitalStore.Domain.Interfaces;

public interface IUserAccessRepository
{
    Task<bool> HasAccessAsync(int userId, int productId, CancellationToken ct = default);
    Task AddAsync(UserAccess access, CancellationToken ct = default);
}
