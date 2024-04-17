using Microsoft.Extensions.Options;

namespace SecretsManager.WeatherApi.Services;

internal sealed class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WeatherApiSettings _settings;
    public WeatherService(IHttpClientFactory httpClientFactory, IOptions<WeatherApiSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }
    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {        
        var client = _httpClientFactory.CreateClient(_settings.ClientName);

        var response = await client.GetAsync($"/data/2.5/weather?q={city}&units=metric&appid={_settings.ApiKey}", cancellationToken);
        if (response is null || !response.IsSuccessStatusCode) 
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<WeatherResponse>(cancellationToken: cancellationToken);
    }
}
