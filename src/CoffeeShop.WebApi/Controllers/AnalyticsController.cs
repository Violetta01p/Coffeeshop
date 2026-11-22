using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IOrderAnalyticsReader _analyticsReader;

    public AnalyticsController(IOrderAnalyticsReader analyticsReader)
    {
        _analyticsReader = analyticsReader;
    }

    /// <summary>
    /// Отримати зведену бізнес-статистику (через вузький інтерфейс IOrderAnalyticsReader - ISP/DIP).
    /// </summary>
    [HttpGet]
    public ActionResult<OrderAnalyticsDto> GetStatistics()
    {
        var statistics = _analyticsReader.GetStatistics();
        return Ok(statistics);
    }
}
