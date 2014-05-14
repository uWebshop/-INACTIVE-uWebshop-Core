using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using VATChecker;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic.VATChecking;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;
using uWebshop.Test.Mocks;
using Moq;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class VATCheckerOrderInfoCouplingTest
	{
		private OrderInfo _incompleteOrderInfo;
		private OrderInfo _confirmedOrderInfo;
		private Mock<IVATCheckService> _vatCheckerMock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.VATCheckService.Mock(out _vatCheckerMock);
			_incompleteOrderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo();
			_incompleteOrderInfo.CustomerInfo.CountryCode = "GB";
			_incompleteOrderInfo.VATCheckService = _vatCheckerMock.Object;
			_incompleteOrderInfo.CustomerInfo.VATNumber = "something";
			_confirmedOrderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
			_confirmedOrderInfo.VATCheckService = _vatCheckerMock.Object;
			_confirmedOrderInfo.CustomerInfo.CountryCode = "GB";
		}

		[Test]
		public void CallingVATChargedOnceOnOrderInfo_ShouldCallCheckerOnce()
		{
			var vatCharged = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), _incompleteOrderInfo), Times.Once());
		}

		[Test]
		public void CallingVATChargedTwiceOnOrderInfo_ShouldCallCheckerOnce()
		{
			var vatCharged = _incompleteOrderInfo.VATCharged;
			var vatCharged2 = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), _incompleteOrderInfo), Times.Once());
		}

		[Test]
		public void CallingVATChargedOnceOnOrderInfo_ShouldGiveAssignedVATNumber()
		{
			_incompleteOrderInfo.CustomerInfo.VATNumber = "theNumber";
			var vatCharged = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid("theNumber", _incompleteOrderInfo), Times.Once());
		}

		[TestCase(false)]
		[TestCase(true)]
		public void CallingVATChargedOnceOnOrderInfo_ShouldGiveResultFromVATChecker(bool argument)
		{
			_vatCheckerMock.Setup(m => m.VATNumberValid(It.IsAny<string>(), _incompleteOrderInfo)).Returns(argument);

			var actual = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), _incompleteOrderInfo), Times.Once());
			Assert.AreEqual(!argument, actual);
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithoutVATNumber_ShouldNotCallVATChecker()
		{
			_incompleteOrderInfo.CustomerInfo.VATNumber = null;
			var vatCharged = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Never());
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithoutVATNumber_ShouldReturnTrue()
		{
			_incompleteOrderInfo.CustomerInfo.VATNumber = null;

			Assert.True(_incompleteOrderInfo.VATCharged);
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithStoreCountryEqualToShippingCountry_ShouldNotCallVATChecker()
		{
			_incompleteOrderInfo.CustomerInfo.CountryCode = "NL";
			_incompleteOrderInfo.StoreInfo.CountryCode = "NL";

			var vatCharged = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Never());
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithStoreCountryEqualToShippingCountry_ShouldReturnTrue()
		{
			_incompleteOrderInfo.CustomerInfo.CountryCode = "NL";
			_incompleteOrderInfo.StoreInfo.CountryCode = "NL";

			var vatCharged = _incompleteOrderInfo.VATCharged;

			Assert.True(vatCharged);
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithStoreCountryInEUAndShippingCountryOutsideEU_ShouldNotCallVATChecker()
		{
			_incompleteOrderInfo.CustomerInfo.CountryCode = "MX";
			_incompleteOrderInfo.StoreInfo.CountryCode = "NL";

			var vatCharged = _incompleteOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Never());
		}

		[Test]
		public void CallingVATChargedOnOrderInfoWithStoreCountryInEUAndShippingCountryOutsideEU_ShouldReturnFalse()
		{
			_incompleteOrderInfo.CustomerInfo.CountryCode = "MX";
			_incompleteOrderInfo.StoreInfo.CountryCode = "NL";

			var vatCharged = _incompleteOrderInfo.VATCharged;

			Assert.False(vatCharged);
		}

		//[Test]
		//public void CallingVATChargedOnOrderInfoWithStoreCountryOutsideEU_ShouldNotCallVATChecker()
		//{
		//	_incompleteOrderInfo.StoreInfo.CountryCode = "MX";

		//	var vatCharged = _incompleteOrderInfo.VATCharged;

		//	_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Never());
		//}

		[Test]
		public void CallingVATChargedOnOrderInfoWithStoreCountryOutsideEU_ShouldReturnTrue()
		{
			_incompleteOrderInfo.StoreInfo.CountryCode = "MX";

			var vatCharged = _incompleteOrderInfo.VATCharged;

			Assert.True(vatCharged);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void VATChargedOfConfirmedOrder_ShouldReturnStoredValue(bool value)
		{
			_confirmedOrderInfo.VATCheckService = new FixedValueIvatChecker(!value);

			var vatCharged = _confirmedOrderInfo.VATCharged;

			Assert.AreEqual(value, vatCharged);
		}

		[Test]
		public void CallingVATChargedOfConfirmedOrder_ShouldCallVatChecker()
		{
			var vatCharged = _confirmedOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Once());
		}

		[Test]
		public void CallingVATChargedOfConfirmedOrderWithCountryCodeChanged_ShouldCallVatChecker()
		{
			// test might not be accurate anymore
			_confirmedOrderInfo.CustomerInfo.ShippingCountryCode = "GB";
			_confirmedOrderInfo.StoreInfo.CountryCode = "NL";

			var vatCharged = _confirmedOrderInfo.VATCharged;

			_vatCheckerMock.Verify(m => m.VATNumberValid(It.IsAny<string>(), It.IsAny<OrderInfo>()), Times.Once());
		}
	}
}