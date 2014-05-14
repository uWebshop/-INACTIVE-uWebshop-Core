using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.API
{
	[DataContract(Name="PaymentProviderMethod", Namespace = "")]
	internal class BillingMethodFulfillmentAdaptor : IBillingProviderMethod
	{
		private readonly PaymentProviderMethod _paymentProviderMethod;
		private readonly bool _pricesIncludingVat;
		private readonly ILocalization _localization;

		public BillingMethodFulfillmentAdaptor(PaymentProviderMethod paymentProviderMethod, bool pricesIncludingVat, ILocalization localization)
		{
			_paymentProviderMethod = paymentProviderMethod;
			_pricesIncludingVat = pricesIncludingVat;
			_localization = localization;
			Id = paymentProviderMethod.Id;
            Title = paymentProviderMethod.Title;
		    Description = paymentProviderMethod.Description;
		    Disabled = paymentProviderMethod.Disabled;
		    //int i;
		    //if (int.TryParse(Id, out i) && i > 0)
		    //{
		    //    Name = paymentProviderMethod.Name;
		    //}
		    //          Error Loading Razor Script (file: Checkout) Request for Node but no nodeId    at uWebshop.Domain.BaseClasses.uWebshopEntity.get_Node()
		    //at uWebshop.Domain.BaseClasses.uWebshopEntity.get_Name()
		    //at uWebshop.API.BillingMethodFulfillmentAdaptor..ctor(PaymentProviderMethod paymentProviderMethod, Boolean pricesIncludingVat, ILocalization localization)
		}

		[DataMember]
		public int SortOrder { get { return _paymentProviderMethod.NodeId != 0 ? _paymentProviderMethod.SortOrder : 0; } set {} }
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public string Title { get; set; }

	    public string Description { get; set; }
	    public bool Disabled { get; set; }

	    public string Name { get
	    {
	        try
	        {
	            return _paymentProviderMethod.Name;
	        }
	        catch (Exception)
	        {
	            return "Error";
	        }
	    }
	    set { }
		}

		[IgnoreDataMember]
		public IVatPrice Amount
		{
			get { return Price.CreateSimplePrice(_paymentProviderMethod.PriceInCents, _pricesIncludingVat, _paymentProviderMethod.Vat, _localization); }
		}

		[IgnoreDataMember]
		public Image Image { get { return _paymentProviderMethod.Image; } }

		[DataMember(Name = "Amount")]
		[JsonProperty(PropertyName = "Amount")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public FlatVatPrice AmountFlat { get { return new FlatVatPrice(Amount); } set { } }
	}
}