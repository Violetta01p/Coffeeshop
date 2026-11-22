using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Pricing;

/// <summary>
/// Політика знижки для великих чеків: 5% знижки, якщо сума замовлення перевищує 300 грн.
/// </summary>
public class LargeOrderDiscountPolicy : IDiscountPolicy
{
    public string Name => "Знижка 5% на замовлення понад 300 грн";

    public Money CalculateDiscount(Order order)
    {
        if (order.Subtotal.Amount > 300m)
        {
            var discountAmount = Math.Round(order.Subtotal.Amount * 0.05m, 2);
            return new Money(discountAmount);
        }

        return Money.Zero;
    }
}
