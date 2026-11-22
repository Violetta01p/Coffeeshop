namespace CoffeeShop.Domain.Common.Exceptions;

/// <summary>
/// Базовий виняток для порушення бізнес-правил предметної області.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
