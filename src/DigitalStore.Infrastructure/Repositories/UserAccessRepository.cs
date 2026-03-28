using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Interfaces;
using DigitalStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalStore.Infrastructure.Repositories;

public class UserAccessRepository : IUserAccessRepository
{
    private readonly AppDbContext _db;
    public UserAccessRepository(AppDbContext db) => _db = db;

    public async Task<bool> HasAccessAsync(int userId, int productId, CancellationToken ct)
    {
        return await _db.UserAccesses
            .AnyAsync(a => a.UserId == userId
                        && a.ProductId == productId
                        && (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow), ct);
    }

    public async Task AddAsync(UserAccess access, CancellationToken ct)
    {
        // Dùng upsert để tránh duplicate khi webhook gọi lại
        var existing = await _db.UserAccesses
            .FirstOrDefaultAsync(a => a.UserId == access.UserId && a.ProductId == access.ProductId, ct);

        if (existing is null)
        {
            _db.UserAccesses.Add(access);
            await _db.SaveChangesAsync(ct);
        }
    }
}
