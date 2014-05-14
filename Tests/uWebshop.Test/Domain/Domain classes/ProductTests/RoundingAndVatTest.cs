using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Test.Mocks;
using uWebshop.Test.Services.OrderUpdatingsServiceTests;

namespace uWebshop.Test.Domain.Domain_classes.ProductTests
{
	[TestFixture]
	public class RoundingAndVatTest
	{
		private Product _product;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();

			_product = new Product();
			_product.Localization = StoreHelper.CurrentLocalization;
			_product.Id = 1;
			_product.Ranges = new List<Range>();
			_product.PricesIncludingVat = false;
			_product.ProductDiscount = null;
		}

		[Test]
		public void NoRoundingIsRequired()
		{
			_product.OriginalPriceInCents = 100;
			_product.Vat = 19;

			Assert.AreEqual(19, _product.VatAmountInCents);
			Assert.AreEqual(119, _product.OriginalPriceWithVatInCents);
		}

		[Test]
		public void ThatVATAmounIsRoundedDown()
		{
			_product.OriginalPriceInCents = 101;
			_product.Vat = 19;

			Assert.AreEqual(19, _product.VatAmountInCents);
			Assert.AreEqual(101, _product.OriginalPriceWithoutVatInCents);
			Assert.AreEqual(120, _product.OriginalPriceWithVatInCents);
		}

		[Test]
		public void ThatVATAmountIsRoundedDownWithPricesIncludingVat()
		{
			_product.PricesIncludingVat = true;
			_product.OriginalPriceInCents = 120;
			_product.Vat = 19;

			Assert.AreEqual(19, _product.VatAmountInCents);
			Assert.AreEqual(101, _product.OriginalPriceWithoutVatInCents);
			Assert.AreEqual(120, _product.OriginalPriceWithVatInCents);
		}
	}
}