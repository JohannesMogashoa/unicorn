using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Unicorn.Web;

public interface ICookieAuthenticationService
{
	Task SignInAsync(AuthenticatedUser user, bool isPersistent = false);
	Task SignOutAsync();
}

public class CookieAuthenticationService : ICookieAuthenticationService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CookieAuthenticationService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task SignInAsync(AuthenticatedUser user, bool isPersistent = false)
	{
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>
		{
			new(ClaimTypes.Name, user.Name),
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.NameIdentifier, user.NameIdentifier)
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
			IsPersistent = isPersistent,
			IssuedUtc = DateTime.Now
		};

		if (_httpContextAccessor.HttpContext != null)
		{
			await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
		}
	}

	public async Task SignOutAsync()
	{
		if (_httpContextAccessor.HttpContext != null)
			await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	}
}