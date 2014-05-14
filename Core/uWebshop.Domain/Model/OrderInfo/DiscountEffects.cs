using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Model
{
	public class DiscountEffects
	{
		private List<IDiscountEffect> _discountEffects = new List<IDiscountEffect>();

		internal void AddEffect(IDiscountEffect effect)
		{
			_discountEffects.Add(effect);
		}

		internal int GetDiscountedPrice(int currentPrice)
		{
			var priceBeforeThisDiscountEffects = currentPrice;

			// 1) set lowest newPrice
			var newPriceDiscounts = _discountEffects.OfType<NewPriceDiscountEffect>();
			if (newPriceDiscounts.Any())
			{
				currentPrice = newPriceDiscounts.Min(d => d.NewPrice);
			}

			// 2) sum all percentages and apply
			var percentage = _discountEffects.OfType<PercentageDiscountEffect>().Sum(p => p.Percentage);
			var @decimal = currentPrice*(1 - percentage);
			var round = Math.Round(@decimal, MidpointRounding.AwayFromZero);
			var discounted = (int)round;

			// 3) lower price by all discountAmounts
			discounted -= _discountEffects.OfType<AmountDiscountEffect>().Sum(p => p.Amount);

			return discounted;
		}

		public void Clear()
		{
			_discountEffects.Clear();
		}
	}
	class NewPriceDiscountEffect : IDiscountEffect
	{
		public int NewPrice;
	}
	class PercentageDiscountEffect : IDiscountEffect
	{
		public decimal Percentage;
	}
	class AmountDiscountEffect : IDiscountEffect
	{
		public int Amount;
	}
}