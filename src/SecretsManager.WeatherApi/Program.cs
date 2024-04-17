using Microsoft.AspNetCore.Mvc;
using SecretsManager.WeatherApi.Services;

var builder = WebApplication.CreateSlimBuilder(args);
var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName;

builder.Configuration.AddSecretsManager(configurator: options =>
{
    // in this case, our secret key should look like "Development_SecretsManager.WeatherApi_{SectionName}__{NestedKey}"
    options.SecretFilter = entry => entry.Name.StartsWith($"{env}_{appName}"); 
    options.KeyGenerator = (_, s) => s
        .Replace($"{env}_{appName}_", string.Empty) // remove the part that identifies the environment and the application
        .Replace("__", ":"); // replace the nested key separator
});

{
    builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection(WeatherApiSettings.SectionName));
    var settings = builder.Configuration.GetSection(WeatherApiSettings.SectionName).Get<WeatherApiSettings>()!;
    builder.Services.AddHttpClient(settings.ClientName, httpClient =>
    {
        httpClient.BaseAddress = new Uri(settings.BaseUrl);        
    });

    builder.Services.AddSingleton<IWeatherService, WeatherService>();

}

var app = builder.Build();

{     
    app.MapGet("/weather/{city}", async (
        string city,
        [FromServices] IWeatherService service) =>
    {
        var weather = await service.GetCurrentWeatherAsync(city);
        return weather is not null
            ? Results.Ok(weather)
            : Results.NotFound();
    });
}
app.Run();
