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
	public class DiscountCoupling
	{
		[Test]
		public void BeforeDiscount_ShouldRemoveDiscountCalculation()
		{
			IOC.UnitTest();

			var price = Price.CreateDiscountedRanged(1000, Enumerable.Empty<Range>(), true, 0, null, i => i / 2, null);
			Assert.AreEqual(500, price.ValueInCents());

			var beforeDiscount = price.BeforeDiscount;
			Assert.AreEqual(1000, beforeDiscount.ValueInCents());
		}
	}
}
