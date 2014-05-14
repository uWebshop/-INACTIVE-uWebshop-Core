using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;

namespace uWebshop.Test.Domain.Businesslogic.PriceTests
{
	[TestFixture]
	public class RangedCalculation
	{
		[Test]
		public void TestingDefaultRangeAndUsingRangesWithCount()
		{
			IOC.UnitTest();
			UwebshopRequest.Current.OrderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo();

			const int orderCount = 15;
			var ranges = new List<Range> {new Range {From = 1, To = 10, PriceInCents = 500}, new Range {From = 10, To = 20, PriceInCents = 300}};
			var price = Price.CreateDiscountedRanged(1000, ranges, true, 0, o => orderCount, i => i, null);
			Assert.AreEqual(500, price.ValueInCents());
			
			var ranged = price.Ranged;
			Assert.AreEqual(300, ranged.ValueInCents());
		}
	}
}
