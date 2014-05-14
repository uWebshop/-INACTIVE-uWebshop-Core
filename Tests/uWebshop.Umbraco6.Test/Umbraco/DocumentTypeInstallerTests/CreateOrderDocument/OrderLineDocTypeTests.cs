using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using uWebshop.Domain;
using uWebshop.Umbraco6.Test;

namespace uWebshop.Test.Umbraco.DocumentTypeInstallerTests.CreateOrderDocument
{
	[TestFixture]
	public class OrderLineDocTypeTests
	{
		private Mock<IContentService> _contentServiceMock;
		private Mock<IContentTypeService> _mockContentTypeService;
		private OrderInfo _orderInfo;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_contentServiceMock = new IOCBuilder<IContentService>().SetupNewMock(); //  IOC.ContentService.SetupNewMock();
			_mockContentTypeService = new IOCBuilder<IContentTypeService>().SetupNewMock(); //IOC.ContentTypeService.SetupNewMock();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 1);
			productInfo.DocTypeAlias = "uwbsProductCustom";
			productInfo.Title = "Nikes";
			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);
			_orderInfo.OrderNumber = "2013001";
			var contentType = new ContentType(1) {Id = 1234};
			_mockContentTypeService.Setup(m => m.GetContentType(Order.OrderRepositoryNodeAlias)).Returns(contentType);
			_contentServiceMock.Setup(m => m.GetContentOfContentType(1234)).Returns(new List<IContent> {new Content("", 1, contentType)});
			_contentServiceMock.Setup(m => m.GetChildren(It.IsAny<int>())).Returns(new List<IContent>());
			_contentServiceMock.Setup(m => m.CreateContent("2013001", It.IsAny<IContent>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new Content("2013001", 1, contentType));
			_contentServiceMock.Setup(m => m.CreateContent("Nikes", It.IsAny<IContent>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new Content("Nikes", 1, contentType));
		}

		[Test]
		public void CreateOrderLine_WithCustomProductDocTypeThatExistsAsCustomOrderedProductDoctype_ShouldUseCorrespondingCustomOrderedProductDocType()
		{
			_mockContentTypeService.Setup(m => m.GetAllContentTypes()).Returns(new List<IContentType> {new ContentType(1) {Alias = "uwbsOrderedProductCustom"}});

			var umbracoDocumentTypeInstaller = IOC.UmbracoDocumentTypeInstaller.Actual().Resolve();
			umbracoDocumentTypeInstaller.CreateOrderDocument(_orderInfo);

			_contentServiceMock.Verify(m => m.CreateContent("Nikes", It.IsAny<IContent>(), "uwbsOrderedProductCustom", It.IsAny<int>()));
		}

		[Test]
		public void CreateOrderLine_WithCustomProductDocTypeNotHavingOrderenProductCounter_ShouldUseOrderedProductNodeAlias()
		{
			_mockContentTypeService.Setup(m => m.GetAllContentTypes()).Returns(new List<IContentType>());

			var umbracoDocumentTypeInstaller = IOC.UmbracoDocumentTypeInstaller.Actual().Resolve();
			umbracoDocumentTypeInstaller.CreateOrderDocument(_orderInfo);

			_contentServiceMock.Verify(m => m.CreateContent("Nikes", It.IsAny<IContent>(), OrderedProduct.NodeAlias, It.IsAny<int>()));
		}
	}
}