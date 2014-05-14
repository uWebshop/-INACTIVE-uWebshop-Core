using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(ShippingChosenFulfillmentAdaptor))]
	[DataContract(Namespace = "")]
	internal class BasketFulfillment : IFulfillment
	{
		[DataMember]
		public bool Fulfilled { get { return Providers.All(p => p.Fulfilled); } set {}}
		[DataMember]
		public IEnumerable<IChosenFulfillmentProvider> Providers { get; set; }
	}
}