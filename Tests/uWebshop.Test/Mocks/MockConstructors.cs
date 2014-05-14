using System.Collections.Generic;
using System.Linq;
using Moq;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Mocks
{
	public class MockConstructors
	{
		public static Mock<IOrderInfo> CreateOrderInfoMock(bool pricesIncludingVat = true)
		{
			var mock = new Mock<IOrderInfo>();
			mock.SetupGet(m => m.OrderLines).Returns(new List<OrderLine>());
			mock.SetupGet(m => m.PricesAreIncludingVAT).Returns(pricesIncludingVat);
			return mock;
		}

		public const int DiscountMockRange1PriceInCents = 234;
		public const int DiscountMockRange2PriceInCents = 567;
		public const int DiscountMockRange3PriceInCents = 890;

		public static Mock<IOrderDiscount> CreateDiscountMock()
		{
			var discountMock = new Mock<IOrderDiscount>();
			var range1 = new Range {From = 0, To = 5, PriceInCents = DiscountMockRange1PriceInCents};
			var range2 = new Range {From = 5, To = 10, PriceInCents = DiscountMockRange2PriceInCents};
			var range3 = new Range {From = 10, To = 15, PriceInCents = DiscountMockRange3PriceInCents};
			var ranges = new List<Range> {range1, range2, range3};
			discountMock.SetupGet(m => m.Ranges).Returns(ranges);
			discountMock.SetupGet(m => m.RequiredItemIds).Returns(new List<int>());
			discountMock.SetupGet(m => m.AffectedOrderlines).Returns(new List<int>());
			return discountMock;
		}

		internal static Mock<ICMSEntityRepository> CreateMockCMSEntityRepository(params uWebshop.Domain.Helpers.UwbsNode[] uwbsNodes)
		{
			var cmsEntityRepoMock = new Mock<ICMSEntityRepository>();
			cmsEntityRepoMock.Setup(m => m.GetAll()).Returns(uwbsNodes);
			cmsEntityRepoMock.Setup(m => m.GetByGlobalId(It.IsAny<int>())).Returns((int id) => uwbsNodes.FirstOrDefault(entity => entity.Id == id));
			return cmsEntityRepoMock;
		}

		public static TServiceType CreateMockEntityService<TServiceType, TEntityType>() where TServiceType : class, IEntityService<TEntityType>
		{
			var mock = new Mock<TServiceType>();
			mock.Setup(m => m.GetAll(It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns(new List<TEntityType>());
			return mock.Object;
		}

		internal static IUwebshopRequestService CreateMockUwebshopRequestService()
		{
			var uwebshopRequest = new UwebshopRequest();
			var mock = new Mock<IUwebshopRequestService>();
			mock.SetupGet(m => m.Current).Returns(uwebshopRequest);
			return mock.Object;
		}
	}
}
