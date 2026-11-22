namespace CoffeeShop.Application.Common.Diagnostics;

/// <summary>
/// Маркерний інтерфейс для демонстрації життєвого циклу Transient.
/// </summary>
public interface ITransientService
{
    Guid InstanceId { get; }
}

/// <summary>
/// Маркерний інтерфейс для демонстрації життєвого циклу Scoped.
/// </summary>
public interface IScopedService
{
    Guid InstanceId { get; }
}

/// <summary>
/// Маркерний інтерфейс для демонстрації життєвого циклу Singleton.
/// </summary>
public interface ISingletonService
{
    Guid InstanceId { get; }
}

public class TransientService : ITransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

public class ScopedService : IScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

public class SingletonService : ISingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}
