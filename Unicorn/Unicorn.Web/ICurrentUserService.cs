using System.Security.Claims;

namespace Unicorn.Web;

public interface ICurrentUserService
{
	bool IsAuthenticated();
	string GetUserId();
	string GetUserName();
	AuthenticatedUser? GetAuthenticatedUser();
}

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public bool IsAuthenticated()
	{
		return _httpContextAccessor.HttpContext is { User.Identity.IsAuthenticated: true };
	}

	public string GetUserId()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null) return "";

		return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
	}

	public string GetUserName()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null) return "";

		return user.FindFirstValue(ClaimTypes.Name) ?? "";
	}

	public AuthenticatedUser? GetAuthenticatedUser()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null)
		{
			return new AuthenticatedUser();
		}

		return new AuthenticatedUser
		{
			Email = user.FindFirstValue(ClaimTypes.Email),
			GivenName = user.FindFirstValue(ClaimTypes.GivenName),
			Name = user.FindFirstValue(ClaimTypes.Name),
			NameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier),
		};
	}
}