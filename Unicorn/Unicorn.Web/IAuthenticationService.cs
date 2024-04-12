using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Unicorn.Web;

public interface IAuthenticationService
{
	Task<LoginResult> LoginAsync(string username, string password);
	Task LogoutAsync();
}

public class AuthenticationService : IAuthenticationService
{
	private readonly ICookieService _cookieService;
	private readonly ICookieAuthenticationService _cookieAuthenticationService;
	private readonly WeatherApiClient _weatherApiClient;

	public AuthenticationService(ICookieAuthenticationService cookieAuthenticationService,
		ICookieService cookieService, WeatherApiClient weatherApiClient1)
	{
		_cookieAuthenticationService = cookieAuthenticationService;
		_cookieService = cookieService;
		_weatherApiClient = weatherApiClient1;
	}

	public async Task<LoginResult> LoginAsync(string username, string password)
	{
		// Authenticate the user
		var user = await _weatherApiClient.LoginAsync(username, password);

		// If the user is authenticated, sign them in
		_cookieService.Set(key: Constants.AuthCookieToken, value: user.Token);

		var autheduser = JwtParser.ParseJwt(user.Token);

		await _cookieAuthenticationService.SignInAsync(autheduser);

		return user;
	}

	public async Task LogoutAsync()
	{
		await _cookieAuthenticationService.SignOutAsync();
	}
}

public static class JwtParser
{
	public static AuthenticatedUser ParseJwt(string userToken)
	{
		var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(userToken);

		var user = new AuthenticatedUser
		{
			NameIdentifier = securityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
			Name = securityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
			Email = securityToken.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
		};

		return user;
	}
}