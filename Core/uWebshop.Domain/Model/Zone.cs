using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	[ContentType(ParentContentType = typeof(PaymentProviderZoneSectionContentType), SortOrder = 99, Name = "Payment Provider Zone", Description = "#PaymentProviderZoneDescription", Alias = "uwbsPaymentProviderZone", Icon = ContentIcon.MapPin, Thumbnail = ContentThumbnail.Folder)]
	internal class PaymentProviderZone : Zone
	{
		public static string NodeAlias;
	}

	[ContentType(ParentContentType = typeof(ShippingProviderZoneSectionContentType), SortOrder = 99, Name = "Shipping Provider Zone", Description = "#ShippingProviderZoneDescription", Alias = "uwbsShippingProviderZone", Icon = ContentIcon.MapPin, Thumbnail = ContentThumbnail.Folder)]
	internal class ShippingProviderZone : Zone
	{
		public static string NodeAlias;
	}
	
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	public class Zone : uWebshopEntity
	{
		/// <summary>
		/// The payment zone node alias
		/// </summary>
		public static string PaymentZoneNodeAlias { get { return PaymentProviderZone.NodeAlias; } }

		/// <summary>
		/// The shipping zone node alias
		/// </summary>
		public static string ShippingZoneNodeAlias { get { return ShippingProviderZone.NodeAlias; } }

		private List<PaymentProvider> _paymentProviders;
		private List<ShippingProvider> _shippingProviders;

		/// <summary>
		/// Initializes a new instance of the <see cref="Zone"/> class.
		/// </summary>
		public Zone()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Zone"/> class.
		/// </summary>
		/// <param name="id">NodeId of the node</param>
		public Zone(int id) : base(id)
		{
		}

		/// <summary>
		///     Get the countrycodes of the zone
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "zone", DataType = DataType.Zones, Tab = ContentTypeTab.Global, Name = "#Zone", Description = "#ZoneDescription")]
		public List<string> CountryCodes
		{
			get
			{
				if (_countryCodes == null)
				{
					_countryCodes =  new List<string>(StoreHelper.GetMultiStoreItem(Id, "zone").Split(','));
				}

				return _countryCodes;
			}
			set { _countryCodes = value; }
		}

		private List<string> _countryCodes;

			/// <summary>
		///     Get the shippingproviders of the zone
		/// </summary>
		[IgnoreDataMember]
		public IEnumerable<ShippingProvider> ShippingProviders
		{
			get { return _shippingProviders ?? (_shippingProviders = ShippingProviderHelper.GetAllShippingProviders().Where(x => x.Zone.Id == Id).ToList()); }
		}

		/// <summary>
		///     Get the paymentproviders for the zone
		/// </summary>
		[IgnoreDataMember]
		public IEnumerable<PaymentProvider> PaymentProviders
		{
			get { return _paymentProviders ?? (_paymentProviders = PaymentProviderHelper.GetAllPaymentProviders().Where(x => x.Zones.Any(z => z.Id == Id)).ToList()); }
		}
	}
}