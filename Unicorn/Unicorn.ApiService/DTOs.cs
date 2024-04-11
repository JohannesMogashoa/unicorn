namespace Unicorn.ApiService;

public record LoginRequest(string UserName, string Password);

public record RegisterRequest(string UserName, string Password, string Email);

public record LoginResult(string? Token, string? RefreshToken, DateTime? RefreshTokenExpiryTime);