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

		internal int GetDiscountedPrice(int currentPrice, int originalCurrentPrice)
		{
			var priceBeforeThisDiscountEffects = currentPrice;

			// 1) set lowest newPrice
			var newPriceDiscounts = _discountEffects.OfType<NewPriceDiscountEffect>();
			if (newPriceDiscounts.Any())
			{
				currentPrice = newPriceDiscounts.Min(d => d.NewPrice);
			}
			var summedAmountDiscount = priceBeforeThisDiscountEffects - currentPrice;

			// 2) sum all percentages and apply
			var percentage = _discountEffects.OfType<PercentageDiscountEffect>().Sum(p => p.Percentage);
			var @decimal = currentPrice*(1 - percentage);
			var round = Math.Round(@decimal, MidpointRounding.AwayFromZero);
			var discounted = (int)round;

			// 3) lower price by all discountAmounts
			var amountSum = _discountEffects.OfType<AmountDiscountEffect>().Sum(p => p.Amount);
			discounted -= amountSum;
			summedAmountDiscount += amountSum;

			// correct for vat
			if (currentPrice != originalCurrentPrice && summedAmountDiscount > 0)
			{
				discounted += (int)Math.Round((1 - (decimal)priceBeforeThisDiscountEffects / originalCurrentPrice) * summedAmountDiscount, MidpointRounding.AwayFromZero);
			}

			return discounted;
		}

		public void Clear()
		{
			_discountEffects.Clear();
		}

		public int GetDiscountAmountWithoutPercentage(int currentPrice)
		{
			var priceBeforeThisDiscountEffects = currentPrice;

			// 1) set lowest newPrice
			var newPriceDiscounts = _discountEffects.OfType<NewPriceDiscountEffect>();
			if (newPriceDiscounts.Any())
			{
				currentPrice = newPriceDiscounts.Min(d => d.NewPrice);
			}

			// 2) sum all percentages and apply
			//var percentage = _discountEffects.OfType<PercentageDiscountEffect>().Sum(p => p.Percentage);
			//var @decimal = currentPrice * (1 - percentage);
			//var round = Math.Round(@decimal, MidpointRounding.AwayFromZero);
			//var discounted = (int)round;

			// 3) lower price by all discountAmounts
			currentPrice -= _discountEffects.OfType<AmountDiscountEffect>().Sum(p => p.Amount);

			return priceBeforeThisDiscountEffects - currentPrice;
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