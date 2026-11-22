using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Pricing;

public sealed record DiscountApplicationResult(
    Money DiscountAmount,
    string? PolicyName);

/// <summary>
/// Доменний контракт для сервісу розрахунку ціноутворення та знижок.
/// </summary>
public interface IOrderPricingService
{
    DiscountApplicationResult CalculateBestDiscount(Order order);
}
