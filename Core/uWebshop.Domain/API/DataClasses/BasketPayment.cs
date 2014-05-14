using System.Collections.Generic;
using System.Runtime.Serialization;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(BasketChosenPaymentProviderAdaptor))]
	[DataContract(Name="PaymentProviders", Namespace = "")]
	internal class BasketPayment : IPayment
	{
		[DataMember]
		public IEnumerable<IChosenPaymentProvider> Providers { get; set; }
	}
}