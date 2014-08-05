using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Businesslogic.PriceTests
{
	[TestFixture]
	public class VatCalculation
	{
		[Test]
		public void PragmaticallyCombinedTestingAllVatOperationsForWithoutVat()
		{
			var price = Price.CreateSimplePrice(1000, false, 10m, null);

			Assert.AreEqual(1000, price.ValueInCents());
			var vatPrice = price.WithVat;
			Assert.AreEqual(1100, vatPrice.ValueInCents);
			vatPrice = price.WithoutVat;
			Assert.AreEqual(1000, vatPrice.ValueInCents);
			vatPrice = price.Vat;
			Assert.AreEqual(100, vatPrice.ValueInCents);
		}

		[Test]
		public void PragmaticallyCombinedTestingAllVatOperationsForWithVat()
		{
			var price = Price.CreateSimplePrice(1000, true, 10m, null);

			Assert.AreEqual(1000, price.ValueInCents());
			var vatPrice = price.WithVat;
			Assert.AreEqual(1000, vatPrice.ValueInCents);
			vatPrice = price.WithoutVat;
			Assert.AreEqual(909, vatPrice.ValueInCents);
			vatPrice = price.Vat;
			Assert.AreEqual(91, vatPrice.ValueInCents);
		}

        [Test]
        public void CreateDiscountedRanged()
        {
            var amount = 40000;
			var discount = 1000;

	        Assert.AreEqual(40000, Price.CreateDiscountedRanged(amount, null, false, 10, null
                                                                , i => i - discount, null).BeforeDiscount.WithoutVat.ValueInCents);

            Assert.AreEqual(3900, Price.CreateDiscountedRanged(amount, null, false, 10, null
                                                               , i => i - discount, null).Vat.ValueInCents);

            Assert.AreEqual(42900, Price.CreateDiscountedRanged(amount, null, false, 10, null
                                                                , i => i - discount, null).WithVat.ValueInCents);

            Assert.AreEqual(39000, Price.CreateDiscountedRanged(amount, null, false, 10, null
                                                                , i => i - discount, null).WithoutVat.ValueInCents);

        }
		[Test]
		public void WeNeedToFixMultipleUsagesOfTheSamePrice()
		{
			// todo: CreateDiscountedRanged is broken depending on order how things are called
			// todo: replace CreateDiscountedRanged with simpleprice?!?
			var amount = 40000;
			var discount = 1000;
			var price = Price.CreateDiscountedRanged(amount, null, false, 10, null
					, i => i - discount, null);

			Assert.AreEqual(40000, price.BeforeDiscount.WithoutVat.ValueInCents);

			Assert.AreEqual(3900, price.Vat.ValueInCents);

			Assert.AreEqual(42900, price.WithVat.ValueInCents);

			Assert.AreEqual(39000, price.WithoutVat.ValueInCents);

		}
	}
}
