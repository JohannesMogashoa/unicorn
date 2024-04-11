using Carter;
using Microsoft.AspNetCore.Identity;

namespace Unicorn.ApiService;

public class AuthEndpoints : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/auth");

		group.MapPost("/login", async (LoginRequest request, IIdentityService identityService) =>
		{
			var result = await identityService.LoginAsync(request);

			return result.Token == null
				? Results.Problem("Failed to login", statusCode: 400)
				: Results.Ok(result);
		}).WithOpenApi();

		group.MapPost("/register", async (
			RegisterRequest request,
			UserManager<ApplicationUser> userManager) =>
		{
			var user = new ApplicationUser
			{
				UserName = request.UserName,
				Email = request.Email,
				EmailConfirmed = true
			};

			var identityResult = await userManager.CreateAsync(user, request.Password);

			return !identityResult.Succeeded ? Results.Problem("Failed to create user", statusCode: 400) : Results.Ok(identityResult);
		}).WithOpenApi();
	}
}