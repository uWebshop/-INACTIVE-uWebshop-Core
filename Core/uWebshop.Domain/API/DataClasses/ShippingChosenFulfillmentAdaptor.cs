using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class ShippingChosenFulfillmentAdaptor : IChosenFulfillmentProvider
	{
		private readonly ShippingProvider _shippingProvider;
		private readonly string _methodId;
		private readonly string _methodTitle;

		public ShippingChosenFulfillmentAdaptor(ShippingProvider shippingProvider, string methodId, string methodTitle)
		{
			_shippingProvider = shippingProvider;
			_methodId = methodId;
			_methodTitle = methodTitle;
		}

		[DataMember]
		public int Id { get { return _shippingProvider != null ? _shippingProvider.Id : 0; } set {}}
		[DataMember]
		public string Title { get { return _shippingProvider != null ? _shippingProvider.Title : string.Empty; } set { } }
		[DataMember]
		public string MethodId { get { return _methodId; } set { } }
		[DataMember]
		public string MethodTitle { get { return _methodTitle; } set { } }
		[DataMember]
		public bool Fulfilled { get { return false; } set { } }
		[IgnoreDataMember]
		public FulfillmentStatus Status { get { return FulfillmentStatus.Pending; } }
		[DataMember(Name = "Status")]
		public string StatusText { get { return Status.ToString(); } set { } }
		[DataMember]
		public string TrackingToken { get { return string.Empty; } set { } }
	}
}