namespace DigitalStore.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string FullName, string Email, string Password);

public record AuthResponse(string Token, string FullName, string Email, string Role);
