namespace CoffeeShop.Application.Common;

/// <summary>
/// Контракт патерну Unit of Work.
/// Координує виконання черги операцій репозиторіїв та гарантує атомарне фіксування змін у сховищі.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
