using Microsoft.AspNetCore.Authentication.Cookies;
using Unicorn.Web;
using Unicorn.Web.Components;

var builder = WebApplication.CreateBuilder(args);


// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.Configure<CookieAuthenticationOptions>(options =>
{
	options.Cookie.Name = "Unicorn.Web";
	options.Cookie.HttpOnly = false;
	options.LoginPath = "/login";
	options.LogoutPath = "/logout";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
	options.SlidingExpiration = false;
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICookieAuthenticationService, CookieAuthenticationService>();
builder.Services.AddScoped<HttpClientInterceptorService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.SecurePolicy = CookieSecurePolicy.None;
		options.Cookie.Name = "Unicorn.Web";
		options.Cookie.HttpOnly = false;
		options.Cookie.SameSite = SameSiteMode.Lax;
		options.LoginPath = "/login";
		options.LogoutPath = "/logout";
		options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
		options.SlidingExpiration = false;
	});

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
	// This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
	// Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
	client.BaseAddress = new("https+http://apiservice");
	client.Timeout = TimeSpan.FromSeconds(50);
}).AddHttpMessageHandler<HttpClientInterceptorService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorPages();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();