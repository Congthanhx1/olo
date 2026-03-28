using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Interfaces;
using DigitalStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalStore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        => await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
        => await _db.Users.FindAsync(new object[] { id }, ct);

    public async Task<User> CreateAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        => await _db.Users.AnyAsync(u => u.Email == email, ct);
}
