using System.Configuration;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using uWebshop.Common;
using uWebshop.Domain;
using VATChecker;

namespace uWebshop.Test.Domain.Businesslogic.ViesVatCheckServiceTests
{
	[TestFixture]
	public class VatNumberCorrectionLogicTests
	{
		[TestCase("NL123456", "NL", "123456")]
		[TestCase(" GB  123456 ", "GB", "123456")]
		[TestCase("GRasf213", "GR", "asf213")]
		[TestCase("G1Rasf213", null, "G1Rasf213")]
		[TestCase("123455", null, "123455")]
		public void ExtractCountryCodeFromVatNumber(string number, string countryCode, string correctedNumber)
		{
			var checker = new ViesVatCheckService();
			checker.ExtractCountryCodeFromVatNumber(number);

			Assert.AreEqual(countryCode, checker.CountryCode);
			Assert.AreEqual(correctedNumber, checker.VATNumber);
		}

		[Test]
		public void GetCountryCodeFromOrderIfNotIncludedInVatNumber_NoCountrySet_ShouldUseCountryFromOrder()
		{
			var checker = new ViesVatCheckService {CountryCode = null};
			var order = CreateOrder("NL");

			checker.GetCountryCodeFromOrderIfNotIncludedInVatNumber(order);

			Assert.AreEqual("NL", checker.CountryCode);
		}

		[Test]
		public void GetCountryCodeFromOrderIfNotIncludedInVatNumber_CountrySet_ShouldNotUseCountryFromOrder()
		{
			var checker = new ViesVatCheckService {CountryCode = "DK"};
			var order = CreateOrder("NL");

			checker.GetCountryCodeFromOrderIfNotIncludedInVatNumber(order);

			Assert.AreEqual("DK", checker.CountryCode);
		}

		[Test]
		public void GetCountryCodeFromOrderIfNotIncludedInVatNumber_CountryIsGR_ShouldBeCorrectedToEL()
		{
			var checker = new ViesVatCheckService {CountryCode = ""};
			var order = CreateOrder("GR");

			checker.GetCountryCodeFromOrderIfNotIncludedInVatNumber(order);

			Assert.AreEqual("EL", checker.CountryCode);
		}

		private static OrderInfo CreateOrder(string countryCode)
		{
			var order = new OrderInfo();
			var xElement = new XElement(CustomerDatatypes.Customer.ToString());
			xElement.AddFirst(new XElement("customerCountry", new XCData("")));
			order.CustomerInfo.customerInformation = new XDocument(xElement);
			order.CustomerCountry = countryCode;
			return order;
		}
	}
}