using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Unicorn.ApiService;

public interface IIdentityService
{
	Task<LoginResult> LoginAsync(LoginRequest request);
}

public class IdentityService : IIdentityService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;

	public IdentityService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory)
	{
		_userManager = userManager;
		_claimsPrincipalFactory = claimsPrincipalFactory;
	}

	public async Task<LoginResult> LoginAsync(LoginRequest request)
	{
		var user = await _userManager.FindByNameAsync(request.UserName);

		if (user == null)
		{
			return new LoginResult(null, null, null);
		}

		var passwordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);

		if (!passwordCorrect)
		{
			return new LoginResult(null, null, null);
		}

		user.RefreshToken = GenerateRefreshToken();
		user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(45);

		await _userManager.UpdateAsync(user);

		var token = await GenerateJwtToken(user);

		return new LoginResult(token, user.RefreshToken, user.RefreshTokenExpiryTime);
	}

	private async Task<string> GenerateJwtToken(ApplicationUser user)
	{
		var signingKey = new SigningCredentials(new SymmetricSecurityKey("8dbb64d5-8d48-4706-8984-f09af6c26090"u8.ToArray()), SecurityAlgorithms.HmacSha256);
		var principal = await _claimsPrincipalFactory.CreateAsync(user);

		var token = new JwtSecurityToken(
			claims: principal.Claims,
			expires: DateTime.UtcNow.AddMinutes(45),
			signingCredentials: signingKey
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private static string GenerateRefreshToken()
	{
		byte[] randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}