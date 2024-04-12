namespace Unicorn.Web;

public record LoginResult(string? Token, string? RefreshToken, DateTime? RefreshTokenExpiryTime);

public class LoginRequest
{
	public required string Username { get; set; } = string.Empty;
	public required string Password { get; set; } = string.Empty;
}

public static class Constants
{
	public const string AuthCookieToken = "Token";
}