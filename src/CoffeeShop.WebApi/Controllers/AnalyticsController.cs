using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IOrderAnalyticsService _analyticsService;

    public AnalyticsController(IOrderAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Отримати зведену бізнес-статистику замовлень, накопичену обробниками подій (Event-Driven).
    /// </summary>
    [HttpGet]
    public ActionResult<OrderAnalyticsDto> GetStatistics()
    {
        var statistics = _analyticsService.GetStatistics();
        return Ok(statistics);
    }
}
