using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Pricing;

/// <summary>
/// Доменний сервіс ціноутворення (OCP).
/// Приймає колекцію політик розрахунку знижок і знаходить найбільш вигідну для клієнта.
/// </summary>
public class OrderPricingService : IOrderPricingService
{
    private readonly IEnumerable<IDiscountPolicy> _discountPolicies;

    public OrderPricingService(IEnumerable<IDiscountPolicy> discountPolicies)
    {
        _discountPolicies = discountPolicies;
    }

    public DiscountApplicationResult CalculateBestDiscount(Order order)
    {
        var bestDiscount = Money.Zero;
        string? bestPolicyName = null;

        foreach (var policy in _discountPolicies)
        {
            var discount = policy.CalculateDiscount(order);
            if (discount.Amount > bestDiscount.Amount)
            {
                bestDiscount = discount;
                bestPolicyName = policy.Name;
            }
        }

        return new DiscountApplicationResult(bestDiscount, bestPolicyName);
    }
}
