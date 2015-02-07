using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(BillingMethodFulfillmentAdaptor))]
	[DataContract(Name = "PaymentProvider", Namespace = "")]
	internal class BillingFulfillmentAdaptor : IBillingProvider
	{
		private readonly PaymentProvider _paymentProvider;

		public BillingFulfillmentAdaptor(PaymentProvider paymentProvider, bool pricesIncludingVat, ILocalization localization)
		{
			_paymentProvider = paymentProvider;
			Id = paymentProvider.Id;
			Title = paymentProvider.Title;
			Description = paymentProvider.Description;
			Methods = new List<IBillingProviderMethod>(paymentProvider.PaymentProviderMethods.Select(m => new BillingMethodFulfillmentAdaptor(m, pricesIncludingVat, localization)));
			Type = paymentProvider.Type;
			Zones = paymentProvider.Zones;
			Disabled = paymentProvider.Disabled;
		}

		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int SortOrder { get { return _paymentProvider.Id != 0 ? _paymentProvider.SortOrder : 0; } set { } }
		[DataMember]
		public string Title { get; set; }

		public string Description { get; set; }
		public bool Disabled { get; set; }

		public List<Zone> Zones { get; set; }

		[IgnoreDataMember]
		public PaymentProviderType Type { get; set; }
		[DataMember(Name = "Type")]
		public string TypeText { get { return Type.ToString(); } set { } }
		[DataMember]
		public IEnumerable<IBillingProviderMethod> Methods { get; set; }
	}
}