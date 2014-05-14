using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using Moq;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class ProductRangePriceTest
	{
		[Test]
		public void CachingShouldHaveNoEffectOnRangeCalculation()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(product);
			product.Ranges = new List<Range> {new Range {From = 3, PriceInCents = 900, To = 999999999}};

			Assert.AreEqual(1000, product.PriceInCents);

			product.ItemCount = 3;
			Assert.AreEqual(900, product.PriceInCents);
		}
	}
}