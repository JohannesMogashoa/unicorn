using Microsoft.AspNetCore.Identity;

namespace Unicorn.ApiService;

public class ApplicationUser : IdentityUser
{
	public string? DisplayName { get; set; }
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
}