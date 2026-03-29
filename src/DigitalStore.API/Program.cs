using DigitalStore.Application.Common.Interfaces;
using DigitalStore.API.Middleware;
using DigitalStore.API.Services;
using DigitalStore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure (DB + Repositories + Application Services) ──
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);

// ── HTTP Context & CurrentUserService ────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ── JWT Authentication ─────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// ── Swagger (dev only) ────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CORS (điều chỉnh origin khi deploy) ──────────────────
builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000", "https://digitalstore-web.onrender.com", "https://digitalstore-admin.onrender.com")
         .AllowAnyHeader()
         .AllowAnyMethod()));

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();   // ← Phải trước UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "DigitalStore API is running!");
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// ── DB Init với retry ────────────────────────────────────
for (int i = 0; i < 3; i++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DigitalStore.Infrastructure.Data.AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("✅ Database initialized successfully.");
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ DB init attempt {i + 1} failed: {ex.Message}");
        if (i < 2) await Task.Delay(3000);
    }
}

app.Run();
