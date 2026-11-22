using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Pricing;

/// <summary>
/// Стратегія (політика) розрахунку знижки для замовлення.
/// Відповідає принципу Open/Closed (OCP): нові види знижок додаються
/// створенням нових класів без зміни існуючої бізнес-логіки.
/// </summary>
public interface IDiscountPolicy
{
    string Name { get; }
    Money CalculateDiscount(Order order);
}
