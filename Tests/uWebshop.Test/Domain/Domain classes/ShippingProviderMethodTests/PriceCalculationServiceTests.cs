using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using Moq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Domain_classes.ShippingProviderMethodTests
{
	[TestFixture]
	public class PriceCalculationServiceTest
	{
		private Mock<IShippingProviderUpdateService> _mock;
		private ShippingProviderMethod _shippingProviderMethod;

		[SetUp]
		public void Setup()
		{
			_shippingProviderMethod = new ShippingProviderMethod();
			_mock = new Mock<IShippingProviderUpdateService>();
		}

		[Test]
		public void CallingPriceInCents_ShouldCallService()
		{
			_shippingProviderMethod.ShippingProviderUpdateService = _mock.Object;

			var actual = _shippingProviderMethod.PriceInCents;

			_mock.Verify(m => m.Update(_shippingProviderMethod, null), Times.Once());
		}

		[Test]
		public void CallingPriceInCentsOnMethodWithCalculationService_ShouldReturnValueFromCalculationService()
		{
			var service = new TestShippingProviderUpdateService();
			service.Implementation = method => { method.PriceInCents = 123; };
			_shippingProviderMethod.ShippingProviderUpdateService = service;

			var actual = _shippingProviderMethod.PriceInCents;

			Assert.AreEqual(123, actual);
		}

		[Test]
		public void CallingPriceInCentsOnMethodWithoutCalculationService_ShouldReturnPriceInCents()
		{
			_shippingProviderMethod.PriceInCents = 234;

			var actual = _shippingProviderMethod.PriceInCents;

			Assert.AreEqual(234, actual);
		}

		private class TestShippingProviderUpdateService : IShippingProviderUpdateService
		{
			public Action<ShippingProviderMethod> Implementation = shippingProviderMethod => { };

			public void Update(ShippingProviderMethod shippingProviderMethod, OrderInfo order)
			{
				Implementation(shippingProviderMethod);
			}
		}
	}
}