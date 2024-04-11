using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Unicorn.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlite("Data Source=Unicorn.db");
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	options.SignIn.RequireConfirmedEmail = false;
	options.SignIn.RequireConfirmedPhoneNumber = false;

	options.User.RequireUniqueEmail = true;
});

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
	jwtOptions.SaveToken = true;
	jwtOptions.RequireHttpsMetadata = false;
	jwtOptions.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8dbb64d5-8d48-4706-8984-f09af6c26090")),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true
	};
});

builder.Services.AddAuthorization();


builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
	var forecast = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast
			(
				DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				Random.Shared.Next(-20, 55),
				summaries[Random.Shared.Next(summaries.Length)]
			))
		.ToArray();
	return forecast;
}).RequireAuthorization();

app.MapCarter();
app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}