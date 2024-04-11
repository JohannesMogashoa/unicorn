namespace Unicorn.Web;

public class WeatherApiClient(HttpClient httpClient)
{
	public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10,
		CancellationToken cancellationToken = default)
	{
		List<WeatherForecast>? forecasts = null;

		await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast",
			               cancellationToken))
		{
			if (forecasts?.Count >= maxItems)
			{
				break;
			}

			if (forecast is not null)
			{
				forecasts ??= [];
				forecasts.Add(forecast);
			}
		}

		return forecasts?.ToArray() ?? [];
	}

	public async Task<LoginResult?> LoginAsync(string username, string password)
	{
		var response = await httpClient.PostAsJsonAsync("api/auth/login", new { username, password });

		if(response.IsSuccessStatusCode)
			return await response.Content.ReadFromJsonAsync<LoginResult>();

		return null;
	}
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}