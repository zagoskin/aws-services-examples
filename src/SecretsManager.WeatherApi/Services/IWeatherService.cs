using System.Text.Json.Serialization;

namespace SecretsManager.WeatherApi.Services;

internal record WeatherResponse(
    long Id,
    string Name,
    string Base,
    List<WeatherDescription> Weather,
    WeatherDetails Main,
    double Visibility);

internal record WeatherDescription(
    long Id,
    string Main,
    string Description,
    string Icon);
          
internal sealed class WeatherDetails
{
    public double Temp { get; init; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; init; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; init; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; init; }
    public double Pressure { get; init; }
    public double Humidity { get; init; }

    [JsonPropertyName("sea_level")]
    public double SeaLevel { get; init; }

    [JsonPropertyName("grnd_level")]
    public double GroundLevel { get; init; }
}

internal interface IWeatherService
{
    Task<WeatherResponse?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default);
}
