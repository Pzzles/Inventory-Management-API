using InventoryManagementSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _forecastService;

    public WeatherForecastController(IWeatherForecastService forecastService)
    {
        _forecastService = forecastService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
    public IEnumerable<WeatherForecast> Get()
    {
        return _forecastService.GetForecasts(5);
    }
}
