using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Name = "ShippingProviderMethod", Namespace = "")]
	internal class ShippingMethodFulfillmentAdaptor : IFulfillmentProviderMethod
	{
		private readonly ShippingProviderMethod _shippingProviderMethod;
		private readonly bool _pricesIncludingVat;
		private readonly ILocalization _localization;
		private readonly OrderInfo _order;

		public ShippingMethodFulfillmentAdaptor(ShippingProviderMethod shippingProviderMethod, bool pricesIncludingVat, ILocalization localization, OrderInfo order)
		{
			_shippingProviderMethod = shippingProviderMethod;
			_pricesIncludingVat = pricesIncludingVat;
			_localization = localization;
			_order = order;
			Id = shippingProviderMethod.Id;
			Title = shippingProviderMethod.Title;
			Description = shippingProviderMethod.Description;
			Name = shippingProviderMethod.Name;
			Disabled = shippingProviderMethod.Disabled;
		}
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public int SortOrder { get { return _shippingProviderMethod.NodeId != 0 ? _shippingProviderMethod.SortOrder : 0; } set { } }
		[DataMember]
		public string Title { get; set; }

		public string Description { get; set; }
		public bool Disabled { get; set; }

		public string Name { get; set; }

		[IgnoreDataMember]
		public IDiscountedPrice Amount
		{
			get { return Price.CreateDiscountedRanged(_shippingProviderMethod.PriceInCents, null, _pricesIncludingVat, _order != null ? _order.AverageOrderVatPercentage : _shippingProviderMethod.Vat, null, i => (_order != null && _order.FreeShipping) ? 0 : i, _localization); }
		}

		[IgnoreDataMember]
		public Image Image { get { return _shippingProviderMethod.Image; } }
		[DataMember(Name = "Amount")]
		[JsonProperty(PropertyName = "Amount")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice AmountFlat { get { return new FlatVatPrice(Amount); } set { } }
	}
}