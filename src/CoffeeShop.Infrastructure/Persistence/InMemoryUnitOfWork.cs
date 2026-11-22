using CoffeeShop.Application.Common;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Persistence;

/// <summary>
/// In-Memory реалізація Unit of Work (Scoped).
/// Акумулює чергу операцій мутації стану сховища в межах одного HTTP-запиту
/// та атомарно виконує їх при виклику SaveChangesAsync.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly List<Action> _pendingActions = new();
    private readonly ILogger<InMemoryUnitOfWork> _logger;

    public InMemoryUnitOfWork(ILogger<InMemoryUnitOfWork> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Реєстрація відкладеної операції запису в чергу.
    /// </summary>
    public void RegisterAction(Action operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _pendingActions.Add(operation);
    }

    /// <summary>
    /// Атомарне застосування всіх накопичених операцій.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var count = _pendingActions.Count;
        if (count == 0)
        {
            return Task.FromResult(0);
        }

        try
        {
            foreach (var action in _pendingActions)
            {
                action();
            }

            _logger.LogInformation("💾 [UnitOfWork] Успішно зафіксовано {Count} операцій(і) запису в сховище.", count);
            return Task.FromResult(count);
        }
        finally
        {
            _pendingActions.Clear();
        }
    }
}
