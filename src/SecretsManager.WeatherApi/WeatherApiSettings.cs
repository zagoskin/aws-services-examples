internal sealed class WeatherApiSettings
{
    public const string SectionName = "WeatherApiSettings";    
    public string BaseUrl { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}