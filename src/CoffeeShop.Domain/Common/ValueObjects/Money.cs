using CoffeeShop.Domain.Common.Exceptions;

namespace CoffeeShop.Domain.Common.ValueObjects;

/// <summary>
/// Value Object для представлення грошової суми.
/// Незмінний тип із вбудованою валідацією.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Грошова сума не може бути від'ємною.");

        Amount = Math.Round(amount, 2);
    }

    public static Money Zero => new(0m);

    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Кількість не може бути від'ємною.");

        return new Money(Amount * quantity);
    }

    public bool Equals(Money? other) =>
        other is not null && Amount == other.Amount;

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => Amount.GetHashCode();

    public override string ToString() => $"{Amount:F2} грн";

    public static bool operator ==(Money? left, Money? right) =>
        Equals(left, right);

    public static bool operator !=(Money? left, Money? right) =>
        !Equals(left, right);
}
