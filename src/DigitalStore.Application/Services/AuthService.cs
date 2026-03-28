using DigitalStore.Application.Common.Exceptions;
using DigitalStore.Application.Common.Interfaces;
using DigitalStore.Application.DTOs.Auth;
using DigitalStore.Domain.Entities;
using DigitalStore.Domain.Interfaces;

namespace DigitalStore.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository users, IJwtTokenService jwt, IPasswordHasher hasher)
    {
        _users = users;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(req.Email, ct))
            throw new InvalidOperationException("Email này đã được đăng ký.");

        var user = new User
        {
            FullName = req.FullName,
            Email = req.Email,
            PasswordHash = _hasher.Hash(req.Password)
        };

        var created = await _users.CreateAsync(user, ct);
        var token = _jwt.GenerateToken(created.Id, created.Email, created.Role);
        return new AuthResponse(token, created.FullName, created.Email, created.Role);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct)
                   ?? throw new NotFoundException("Email hoặc mật khẩu không đúng.");

        if (!user.IsActive)
            throw new ForbiddenException("Tài khoản đã bị vô hiệu hóa.");

        if (!_hasher.Verify(req.Password, user.PasswordHash))
            throw new NotFoundException("Email hoặc mật khẩu không đúng.");

        var token = _jwt.GenerateToken(user.Id, user.Email, user.Role);
        return new AuthResponse(token, user.FullName, user.Email, user.Role);
    }
}

