using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculator
{
	[TestFixture]
	public class PercentageCalculationOverflowRegressionTest
	{
		[Test]
		public void LargePriceLargePercentage_ShouldHaveNoInternalOverflow()
		{
			var discount = new DiscountProduct{ DiscountType = DiscountType.Percentage, DiscountValue = 5000};
			var actual = discount.GetAdjustedPrice(1000000);

			Assert.AreEqual(500000, actual);
		}
	}
}
