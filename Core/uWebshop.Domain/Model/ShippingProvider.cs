using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	[ContentType(ParentContentType = typeof(ShippingProviderSectionContentType), Name = "Shipping Provider", Description = "#ShippingProviderDescription", Alias = "uwbsShippingProvider", IconClass = IconClass.truck, Icon = ContentIcon.BaggageCartBoxLabel, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ShippingProviderMethod) })]
	public class ShippingProvider : uWebshopEntity
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The shipping provider repository node alias
		/// </summary>
		public static string ShippingProviderRepositoryNodeAlias { get { return ShippingProviderRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// The shipping provider section node alias
		/// </summary>
		public static string ShippingProviderSectionNodeAlias { get { return ShippingProviderSectionContentType.NodeAlias; } }

		/// <summary>
		/// The shipping provider zone section node alias
		/// </summary>
		public static string ShippingProviderZoneSectionNodeAlias { get { return ShippingProviderZoneSectionContentType.NodeAlias; } }

		internal ILocalization Localization;

		private List<ShippingProviderMethod> _shippingProviderMethods;

		/// <summary>
		/// Determines whether [is applicable automatic order] [the specified order information].
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		public bool IsApplicableToOrder(OrderInfo orderInfo)
		{
			switch (TypeOfRange)
			{
				case ShippingRangeType.Quantity:
					int quantity = orderInfo.OrderLines.Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1)); // orderInfo.OrderLines.Aggregate<OrderLine, decimal>(0, (current, orderLine) => (decimal) (current + orderLine.ProductInfo.ItemCount));
					return (quantity >= RangeFrom && quantity < RangeTo);
				case ShippingRangeType.OrderAmount:
					int subTotal = orderInfo.OrderLines.Sum(orderLine => orderLine.AmountInCents);
					return (subTotal >= RangeFrom && subTotal < RangeTo);
				case ShippingRangeType.Weight:
					double weight = orderInfo.OrderLines.Sum(orderLine => orderLine.OrderLineWeight);
					return (weight >= Convert.ToDouble(RangeFrom) && weight < Convert.ToDouble(RangeTo));
				case ShippingRangeType.None:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Loads the shipping methods.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ShippingProviderMethod> LoadShippingMethods()
		{
			//var shippingMethodList = new List<ShippingProviderMethod>();
			var shippingProviders = ShippingProviderHelper.GetAllShippingProvidersIncludingCustomProviders();
			return shippingProviders.Where(shippingProvider => shippingProvider.GetName().ToLowerInvariant() == Title.ToLowerInvariant()).SelectMany(shippingProvider => shippingProvider.GetAllShippingMethods(Id));

		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		public string GetName()
		{
			return Name;
		}

		#region properties

		/// <summary>
		/// Payment Provider Title
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", SortOrder = 1)]
		public string Title { get; set; }

		/// <summary>
		/// Payment Provider Description
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", SortOrder = 2)]
		public string Description { get; set; }

		/// <summary>
		/// Id of the Shipping Provider Image
		/// </summary>
		/// <value>
		/// The image unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "image", DataType = DataType.MediaPicker, Tab = ContentTypeTab.Global, Name = "#Image", Description = "#ImageDescription", SortOrder = 3)]
		public int ImageId { get; set; }

		/// <summary>
		/// The type of shipping provider
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "type", DataType = DataType.ShippingProviderType, Tab = ContentTypeTab.Details, Name = "#ShippingProviderType", Description = "#ShippingProviderTypeDescription", SortOrder = 4)]
		public ShippingProviderType Type { get; set; }

		/// <summary>
		/// Get the rangetype for the shipping provider
		/// </summary>
		/// <value>
		/// The type of range.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "rangeType", DataType = DataType.ShippingProviderRangeType, Tab = ContentTypeTab.Details, Name = "#ShippingProviderRangeType", Description = "#ShippingProviderRangeTypeDescription", SortOrder = 5)]
		public ShippingRangeType TypeOfRange { get; set; }

		/// <summary>
		/// Get the range start point for this shipping provider
		/// </summary>
		/// <value>
		/// The range from.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "rangeStart", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#RangeStart", Description = "#RangeStartDescription", SortOrder = 6)]
		public decimal RangeFrom { get; set; }

		/// <summary>
		/// Get the range endpoint of this shipping provider
		/// </summary>
		/// <value>
		/// The range automatic.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "rangeEnd", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#RangeEnd", Description = "#RangeEndDescription", SortOrder = 7)]
		public decimal RangeTo { get; set; }

		/// <summary>
		/// Check if the shipping provider should overrule all others
		/// This can be used to set free shipping even if there is weight in the basket.
		/// </summary>
		/// <value>
		///   <c>true</c> if [overrule]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "overrule", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Details, Name = "#Overrule", Description = "#OverruleDescription", SortOrder = 9)]
		public bool Overrule { get; set; }

		/// <summary>
		/// Zones for this payment provider
		/// </summary>
		/// <value>
		/// The zone.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "zone", DataType = DataType.MultiContentPickerShippingZones, Tab = ContentTypeTab.Details, Name = "#Zone", Description = "#ZoneDescription", SortOrder = 10)]
		public Zone Zone { get; set; }

		/// <summary>
		/// Provider in testmode?
		/// true = testmode enabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "testMode", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Details, Name = "#TestMode", Description = "#TestModeDescription", SortOrder = 11)]
		public bool TestMode { get; set; }

		/// <summary>
		/// Get the amountcost of the shipping provider
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[IgnoreDataMember]
		public int PriceInCents
		{
			get
			{
				// todo: costly!
				var shippingProviderMethod = ShippingProviderMethods.OrderBy(x => x.PriceWithVatInCents).FirstOrDefault();
				return shippingProviderMethod != null ? shippingProviderMethod.PriceWithVatInCents : 0;
			}
			set { }
		}

		/// <summary>
		/// Full filename of the shippping provider DLL;
		/// </summary>
		/// <value>
		/// The name of the DLL.
		/// </value>
		[IgnoreDataMember]
		public string DLLName { get; set; }

		/// <summary>
		/// Vat percentage for this shipping provider
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		public decimal Vat { get; set; }

		/// <summary>
		/// Gets or sets the vat amount in cents.
		/// </summary>
		/// <value>
		/// The vat amount in cents.
		/// </value>
		[IgnoreDataMember]
		public decimal VatAmountInCents
		{
			get { return PriceWithVatInCents - PriceWithoutVatInCents; }
			set { }
		}

		/// <summary>
		/// Gets a list of shipping methods
		/// </summary>
		/// <value>
		/// The shipping provider methods.
		/// </value>
		[DataMember]
		public IEnumerable<ShippingProviderMethod> ShippingProviderMethods
		{
			get
			{
				if (_shippingProviderMethods != null)
				{
					return _shippingProviderMethods;
				}

				var shippingProviderMethodList = new List<ShippingProviderMethod>();

				shippingProviderMethodList.AddRange(
					IO.Container.Resolve<IShippingProviderMethodRepository>()
						.GetAll(Localization)
						.Where(pm => pm.ParentId == Id)
						.ToList());


				foreach (var shippingProviderMethod in shippingProviderMethodList)
				{
					shippingProviderMethod.Name = shippingProviderMethod.Title;
					shippingProviderMethod.ProviderName = Title;

                }

				shippingProviderMethodList.AddRange(LoadShippingMethods());

				if (!shippingProviderMethodList.Any())
				{
					var shippingMethodDummy = new ShippingProviderMethod
					{
						Id = Id.ToString(),
                        Key = Key,
						Name = Title,
						Title = Title,
						ProviderName = Title,
						PriceInCents = 0
					};

					Log.Instance.LogDebug(
						string.Format("ShippingProvider: {0} Without Methods, fallback to code created dummy method", Title));
					shippingProviderMethodList.Add(shippingMethodDummy);
				}

				return _shippingProviderMethods = shippingProviderMethodList;

			}
			set { }
		}

		/// <summary>
		/// Shipping Provider Amount EXCLUDING Vat
		/// </summary>
		/// <value>
		/// The price without vat in cents.
		/// </value>
		[IgnoreDataMember]
		public int PriceWithoutVatInCents
		{
			get { return IO.Container.Resolve<ISettingsService>().IncludingVat ? VatCalculator.WithoutVat(PriceInCents, Vat) : PriceInCents; }
		}

		/// <summary>
		/// Shipping Provider Amount INCLUDING Vat
		/// </summary>
		/// <value>
		/// The price with vat in cents.
		/// </value>
		[IgnoreDataMember]
		public int PriceWithVatInCents
		{
			get { return IO.Container.Resolve<ISettingsService>().IncludingVat ? PriceInCents : VatCalculator.WithVat(PriceInCents, Vat); }
		}

		#endregion

		internal static bool IsAlias(string alias)
		{
			return alias.StartsWith(NodeAlias);
		}
	}
}