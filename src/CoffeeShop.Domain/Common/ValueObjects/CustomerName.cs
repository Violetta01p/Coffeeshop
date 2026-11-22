using CoffeeShop.Domain.Common.Exceptions;

namespace CoffeeShop.Domain.Common.ValueObjects;

/// <summary>
/// Value Object для імені гостя (клієнта).
/// Незмінний тип із валідацією.
/// </summary>
public sealed class CustomerName : IEquatable<CustomerName>
{
    public string Value { get; }

    public CustomerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Ім'я клієнта не може бути порожнім.");

        if (value.Length > 100)
            throw new DomainException("Ім'я клієнта не може перевищувати 100 символів.");

        Value = value.Trim();
    }

    public bool Equals(CustomerName? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    public override bool Equals(object? obj) => Equals(obj as CustomerName);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;

    public static bool operator ==(CustomerName? left, CustomerName? right) =>
        Equals(left, right);

    public static bool operator !=(CustomerName? left, CustomerName? right) =>
        !Equals(left, right);
}
