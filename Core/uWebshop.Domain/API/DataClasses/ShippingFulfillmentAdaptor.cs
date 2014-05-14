using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(ShippingMethodFulfillmentAdaptor))]
	[DataContract(Name="ShippingProviders", Namespace = "")]
	internal class ShippingFulfillmentAdaptor : IFulfillmentProvider
	{
		private readonly ShippingProvider _shippingProvider;

		public ShippingFulfillmentAdaptor(ShippingProvider shippingProvider, bool pricesIncludingVat, ILocalization localization, OrderInfo order)
		{
			_shippingProvider = shippingProvider;
			Id = shippingProvider.Id;
			Title = shippingProvider.Title;
		    Description = shippingProvider.Description;
			Methods = new List<IFulfillmentProviderMethod>(shippingProvider.ShippingProviderMethods.Select(m => new ShippingMethodFulfillmentAdaptor(m, pricesIncludingVat, localization, order)));
			Type = shippingProvider.Type;
		    Zones = new List<Zone> {shippingProvider.Zone};
		    Disabled = shippingProvider.Disabled;
		}

		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int SortOrder { get { return _shippingProvider.Id != 0 ? _shippingProvider.SortOrder : 0; } }
		[DataMember]
		public string Title { get; set; }

	    public string Description { get; set; }
	    public bool Disabled { get; set; }

	    public List<Zone> Zones { get; set; }

	    [IgnoreDataMember]
		public ShippingProviderType Type { get; set; }
		[DataMember(Name = "Type")]
		public string TypeText { get { return Type.ToString(); } set { } }
		[DataMember]
		public IEnumerable<IFulfillmentProviderMethod> Methods { get; set; }
	}
}