using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Services
{
	[TestFixture]
	public class PaymentProviderServiceTests
	{
		[Test]
		public void LoadData_ShouldCallReloadDataOnRepository()
		{
			IOC.UnitTest();
			var mock = IOC.PaymentProviderRepository.SetupNewMock();
			var paymentProvider = new PaymentProvider(1234);

			IOC.PaymentProviderService.Actual().Resolve().LoadData(paymentProvider, Stubs.StubLocalization.Default);

			mock.Verify(m => m.ReloadData(paymentProvider, It.IsAny<ILocalization>()));
		}
	}
}