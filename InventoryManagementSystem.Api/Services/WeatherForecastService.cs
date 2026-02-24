namespace InventoryManagementSystem.Api.Services;

public sealed class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly IDateTimeProvider _dateTimeProvider;

    public WeatherForecastService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public IReadOnlyList<WeatherForecast> GetForecasts(int days)
    {
        return Enumerable.Range(1, days)
            .Select(index =>
            {
                var date = DateOnly.FromDateTime(_dateTimeProvider.UtcNow.AddDays(index));

                return new WeatherForecast
                {
                    Date = date,
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                };
            })
            .ToArray();
    }
}
