using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain;

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
	}
}
