using DigitalStore.Application.Common.Interfaces;
using DigitalStore.Application.Services;
using DigitalStore.Domain.Interfaces;
using DigitalStore.Infrastructure.Data;
using DigitalStore.Infrastructure.Repositories;
using DigitalStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        // ── EF Core (PostgreSQL) ─────────────────────────────
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        // ── Repositories ─────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserAccessRepository, UserAccessRepository>();

        // ── Infrastructure Services ───────────────────────────
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // ── Application Services ──────────────────────────────
        services.AddScoped<AuthService>();
        services.AddScoped<DownloadService>();
        services.AddScoped<OrderService>();
        services.AddScoped<ProductService>();

        return services;
    }
}
