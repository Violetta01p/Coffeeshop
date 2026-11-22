using CoffeeShop.Application.Common.Diagnostics;
using CoffeeShop.Application.Common.Time;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly ITransientService _transient1;
    private readonly ITransientService _transient2;
    private readonly IScopedService _scoped1;
    private readonly IScopedService _scoped2;
    private readonly ISingletonService _singleton1;
    private readonly ISingletonService _singleton2;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DiagnosticsController(
        ITransientService transient1,
        ITransientService transient2,
        IScopedService scoped1,
        IScopedService scoped2,
        ISingletonService singleton1,
        ISingletonService singleton2,
        IDateTimeProvider dateTimeProvider)
    {
        _transient1 = transient1;
        _transient2 = transient2;
        _scoped1 = scoped1;
        _scoped2 = scoped2;
        _singleton1 = singleton1;
        _singleton2 = singleton2;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Демонстрація життєвих циклів сервісів (Transient, Scoped, Singleton) у DI-контейнері.
    /// </summary>
    [HttpGet("lifetimes")]
    public IActionResult GetLifetimes()
    {
        return Ok(new
        {
            timestampUtc = _dateTimeProvider.UtcNow,
            transient = new
            {
                instance1 = _transient1.InstanceId,
                instance2 = _transient2.InstanceId,
                areSameInRequest = _transient1.InstanceId == _transient2.InstanceId,
                description = "Transient: новий екземпляр при кожному запиті залежності з контейнера."
            },
            scoped = new
            {
                instance1 = _scoped1.InstanceId,
                instance2 = _scoped2.InstanceId,
                areSameInRequest = _scoped1.InstanceId == _scoped2.InstanceId,
                description = "Scoped: один спільний екземпляр у межах одного HTTP-запиту, але новий у кожному окремому запиті."
            },
            singleton = new
            {
                instance1 = _singleton1.InstanceId,
                instance2 = _singleton2.InstanceId,
                areSameInRequest = _singleton1.InstanceId == _singleton2.InstanceId,
                description = "Singleton: єдиний екземпляр на весь час роботи програми для всіх запитів."
            }
        });
    }
}
