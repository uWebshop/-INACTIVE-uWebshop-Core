using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Services.OrderUpdatingsServiceTests;

namespace uWebshop.Test.Domain.Domain_classes.ProductTests
{
	[TestFixture]
	public class OrderablePropertyTests
	{
		[Test]
		public void Orderable_StockTrueVariantFalseBackorderFalseStock100_ShouldReturnTrue()
		{
			IOC.UnitTest();
			IOC.StockService.SetupNewMock().Setup(m => m.GetStockForUwebshopEntityWithId(It.IsAny<int>(), null)).Returns(100);
			var product = new Product { UseVariantStock = false, StockStatus = true, BackorderStatus = false, VariantGroups = Enumerable.Empty<IProductVariantGroup>(), ProductVariantGroupsFactory = () => new List<IProductVariantGroup>(), };

			Assert.IsTrue(product.Orderable);
		}

		[TestCase(true, false, 100, true)]
		[TestCase(true, false, 0, false)]
		[TestCase(true, true, 100, true)]
		[TestCase(true, true, 0, true)]
		[TestCase(false, true, 100, true)]
		[TestCase(false, true, 0, true)]
		[TestCase(false, false, 100, true)]
		[TestCase(false, false, 0, true)]
		public void OrderableTestCases_NoVariants(bool stockStatus, bool backOrderStatus, int stock, bool expected)
		{
			IOC.UnitTest();
			IOC.StockService.SetupNewMock().Setup(m => m.GetStockForUwebshopEntityWithId(It.IsAny<int>(), null)).Returns(stock);
			IOC.ProductService.Actual();
			var product = new Product { UseVariantStock = false, StockStatus = stockStatus, BackorderStatus = backOrderStatus, VariantGroups = Enumerable.Empty<IProductVariantGroup>(), ProductVariantGroupsFactory = () => new List<IProductVariantGroup>(), };

			Assert.AreEqual(expected, product.Orderable);
		}
	}
}