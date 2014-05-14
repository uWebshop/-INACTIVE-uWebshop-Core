using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Model.ProductVariantInfoTests
{
	[TestFixture]
	public class RangedPriceInCentsTests
	{
		private ProductInfo _product;
		private ProductVariantInfo _variant;

		[SetUp]
		public void Setup()
		{
			_variant = new ProductVariantInfo();
			_variant.Ranges = new List<Range> {new Range {From = 0, To = 2, PriceInCents = 100}, new Range {From = 3, To = 999, PriceInCents = 50}};
			_variant.PriceInCents = 200;

			_product = new ProductInfo {ProductVariants = new List<ProductVariantInfo> {_variant}, ItemCount = 5};
			_variant.Product = _product;
		}

		[Test]
		public void VerifyRangesCoupling()
		{
			Assert.AreEqual(50, _variant.RangedPriceInCents);
		}

		[Test]
		public void VerifyOrderTotalItemCount()
		{
			var order = new OrderInfo();
			order.OrderLines = new List<OrderLine> {new OrderLine(_product, order), new OrderLine(_product, order)};
			_product.Order = order;

			Assert.AreEqual(10, _variant.OrderTotalItemCount);
		}
	}
}