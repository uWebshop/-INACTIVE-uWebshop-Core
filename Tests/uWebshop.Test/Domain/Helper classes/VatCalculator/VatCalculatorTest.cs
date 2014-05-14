using System;
using NUnit.Framework;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Domain.Helper_classes.VatCalculatorTest
{
	[TestFixture]
	public class VatCalculatorTest
	{
		[TestCase(100)]
		[TestCase(101)]
		public void CaclulatingBackAndForthShouldReturnSamePrice(int amount)
		{
			Assert.AreEqual(amount, VatCalculator.WithoutVat(VatCalculator.WithVat(amount, 19), 19));
			Console.WriteLine(VatCalculator.WithoutVat(amount, 19));
			Assert.AreEqual(amount, VatCalculator.WithVat(VatCalculator.WithoutVat(amount, 19), 19));
		}

		[Test]
		public void WithoutVat()
		{
			Assert.AreEqual(84, VatCalculator.WithoutVat(100, 19));
			Assert.AreEqual(85, VatCalculator.WithoutVat(101, 19));
		}

		[TestCase(97, 15)]
		[TestCase(98, 16)]
		[TestCase(99, 16)]
		[TestCase(100, 16)]
		[TestCase(101, 16)]
		[TestCase(102, 16)]
		[TestCase(103, 16)]
		[TestCase(104, 17)]
		public void VatAmountFromWithVat(int amountWithVat, int vatAmount)
		{
			Assert.AreEqual(vatAmount, VatCalculator.VatAmountFromWithVat(amountWithVat, 19));
		}

		[TestCase(97, 18)]
		[TestCase(98, 19)]
		[TestCase(99, 19)]
		[TestCase(100, 19)]
		[TestCase(101, 19)]
		[TestCase(102, 19)]
		[TestCase(103, 20)]
		[TestCase(104, 20)]
		public void VatAmountFromWithoutVat(int amountWithVat, int vatAmount)
		{
			Assert.AreEqual(vatAmount, VatCalculator.VatAmountFromWithoutVat(amountWithVat, 19));
		}

		[TestCase(97, 115)]
		[TestCase(98, 117)]
		[TestCase(99, 118)]
		[TestCase(100, 119)]
		[TestCase(101, 120)]
		[TestCase(102, 121)]
		[TestCase(103, 123)]
		[TestCase(104, 124)]
		public void WithVatFromWithoutVat(int amountWithoutVat, int amountWithVat)
		{
			Assert.AreEqual(amountWithVat, VatCalculator.WithVat(amountWithoutVat, 19));
		}

		[TestCase(97, 82)]
		[TestCase(98, 82)]
		[TestCase(99, 83)]
		[TestCase(100, 84)]
		[TestCase(101, 85)]
		[TestCase(102, 86)]
		[TestCase(103, 87)]
		[TestCase(104, 87)]
		public void WithoutVatFromWithVat(int amountWithVat, int amountWithoutVat)
		{
			Assert.AreEqual(amountWithoutVat, VatCalculator.WithoutVat(amountWithVat, 19));
		}
	}
}