namespace InventoryManagementSystem.Api.Services;

public interface IWeatherForecastService
{
    IReadOnlyList<WeatherForecast> GetForecasts(int days);
}
