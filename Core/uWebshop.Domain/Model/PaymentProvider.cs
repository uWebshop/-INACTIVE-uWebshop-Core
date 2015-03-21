using System;
using System.Collections.Generic;
using System.Dynamic;
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
	///     Class representing a payment provider in Umbraco
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	[ContentType(ParentContentType = typeof(PaymentProviderSectionContentType), Name = "Payment Provider", Description = "#PaymentProviderDescription", Alias = "uwbsPaymentProvider", IconClass = IconClass.creditcard, Icon = ContentIcon.CreditCard, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(PaymentProviderMethod) })]
	public class PaymentProvider : uWebshopEntity
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The payment provider repository node alias
		/// </summary>
		public static string PaymentProviderRepositoryNodeAlias { get { return PaymentProviderRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// The payment provider section node alias
		/// </summary>
		public static string PaymentProviderSectionNodeAlias { get { return PaymentProviderSectionContentType.NodeAlias; } }

		/// <summary>
		/// The payment provider zone section node alias
		/// </summary>
		public static string PaymentProviderZoneSectionNodeAlias { get { return PaymentProviderZoneSectionContentType.NodeAlias; } }
		
		internal ILocalization Localization;

		private List<PaymentProviderMethod> _paymentProviderMethods;
		private List<Zone> _paymentProviderZones;

		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentProvider" /> class.
		/// </summary>
		public PaymentProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentProvider" /> class.
		/// </summary>
		/// <param name="id">NodeId of the node</param>
		public PaymentProvider(int id) : base(id)
		{
			IO.Container.Resolve<IPaymentProviderService>().LoadData(this, StoreHelper.CurrentLocalization);
		}

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
		/// Id of the Payment Provider Image
		/// </summary>
		/// <value>
		/// The image unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "image", DataType = DataType.MediaPicker, Tab = ContentTypeTab.Global, Name = "#Image", Description = "#ImageDescription", SortOrder = 3)]
		public int ImageId { get; set; }

		/// <summary>
		/// Gets a list with the payment provider types
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "type", DataType = DataType.PaymentProviderType, Tab = ContentTypeTab.Details, Name = "#PaymentProviderType", Description = "#PaymentProviderTypeDescription", SortOrder = 4)]
		public PaymentProviderType Type { get; set; }

		/// <summary>
		/// Zones for this payment provider
		/// </summary>
		/// <value>
		/// The zone.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "zone", DataType = DataType.MultiContentPickerPaymentZones,
			Tab = ContentTypeTab.Details, Name = "#Zone", Description = "#ZoneDescription", SortOrder = 6)]
		public List<Zone> Zones { get; set; }
	   

		/// <summary>
		/// Gets or sets the succes node unique identifier.
		/// </summary>
		/// <value>
		/// The succes node unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "successNode", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Details, Name = "#SuccessNode", Description = "#SuccessNodeDescription", SortOrder = 7)]
		public string SuccesNodeId { get; set; }

		/// <summary>
		/// Gets or sets the error node unique identifier.
		/// </summary>
		/// <value>
		/// The error node unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "errorNode", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Details, Name = "#ErrorNode", Description = "#ErrorNodeDescription", SortOrder = 8)]
		public string ErrorNodeId { get; set; }

		/// <summary>
		/// Gets or sets the error node unique identifier.
		/// </summary>
		/// <value>
		/// The error node unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "cancelNode", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Details, Name = "#CancelNode", Description = "#CancelNodeDescription", SortOrder = 9)]
		public string CancelNodeId { get; set; }


		/// <summary>
		/// Provider in testmode?
		/// true = testmode enabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "testMode", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Details, Name = "#TestMode", Description = "#TestModeDescription", SortOrder = 10)]
		public bool TestMode { get; set; }

		/// <summary>
		/// Full .dll name of the Payment Provider
		/// </summary>
		/// <value>
		/// The name of the DLL.
		/// </value>
		[DataMember]
		public string DLLName { get; set; }

		/// <summary>
		/// VAT percentage for the payment provider
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		public decimal Vat { get; set; }

		/// <summary>
		/// Gets or sets the control node unique identifier.
		/// </summary>
		/// <value>
		/// The control node unique identifier.
		/// </value>
		[DataMember]
		public string ControlNodeId { get; set; }


		/// <summary>
		/// Gets a list of payment methods
		/// </summary>
		/// <value>
		/// The payment provider methods.
		/// </value>
		[DataMember]
		public IEnumerable<PaymentProviderMethod> PaymentProviderMethods
		{
			get
			{
				if (_paymentProviderMethods != null)
				{
					return _paymentProviderMethods;
				}

				var paymentProviderMethodList = IO.Container.Resolve<IPaymentProviderMethodRepository>().GetAll(Localization).Where(pm => pm.ParentId == Id).ToList();

				foreach (var shippingProviderMethod in paymentProviderMethodList)
				{
					shippingProviderMethod.ProviderName = Title;
				}

				var currentIPaymentProvider = PaymentProviderHelper.GetAllIPaymentProviders().FirstOrDefault(x => x.GetName().ToLowerInvariant() == Name.ToLowerInvariant());

				if (currentIPaymentProvider == null)
				{
					Log.Instance.LogDebug("currentIPaymentProvider == null + Node.Name (Could be ok if NOT connecting to payment provider API): " + Name);
				}
				else
				{
					// todo: op een of andere manier is het mogelijk dat in GetAllPaymentMethods(Id) deze PaymentProvider niet te vinden is op Id...
					if (GetPaymentProvider(Id) == null)
					{
						Log.Instance.LogDebug("GetPaymentProvider(Id) == null + Node.Name: " + Name);
						return paymentProviderMethodList;
					}

					var paymentProviderMethods = currentIPaymentProvider.GetAllPaymentMethods(Id).ToList();

					var removeListProviderMethods = new List<PaymentProviderMethod>();

					var transformedMethods = paymentProviderMethods.Where(method => paymentProviderMethodList.Any(m => m.Title.ToLowerInvariant() == method.Title.ToLowerInvariant())).Select(paymentProviderMethod =>
						{
							var paymentMethodNodeItem = paymentProviderMethodList.First(m => m.Title.ToLowerInvariant() == paymentProviderMethod.Title.ToLowerInvariant());

							removeListProviderMethods.Add(paymentProviderMethod);

							paymentProviderMethodList.Remove(paymentMethodNodeItem);

							return new PaymentProviderMethod {Id = paymentProviderMethod.Id, Title = paymentMethodNodeItem.Title, Image = paymentMethodNodeItem.Image, ProviderName = Title, Disabled = paymentMethodNodeItem.Disabled, PriceInCents = StoreHelper.LocalizePrice(paymentMethodNodeItem.PriceInCents, Localization), Vat = paymentMethodNodeItem.Vat, AmountType = paymentMethodNodeItem.AmountType};
						}).ToList();

					foreach (var item in removeListProviderMethods)
					{
						paymentProviderMethods.Remove(item);
					}

					paymentProviderMethodList.AddRange(paymentProviderMethods);
					paymentProviderMethodList.AddRange(transformedMethods);
				}

				if (!paymentProviderMethodList.Any())
				{
					var paymentMethodDummy = new PaymentProviderMethod {Id = Id.ToString(), Title = Title, ProviderName = Title, PriceInCents = 0};

					Log.Instance.LogDebug(string.Format("PaymentProvider: {0} Without Methods, fallback to code created dummy method", Title));
					paymentProviderMethodList.Add(paymentMethodDummy);
				}

				_paymentProviderMethods = paymentProviderMethodList;
				return _paymentProviderMethods;
			}
			set { }
		}

		/// <summary>
		/// Get the amountcost of the payment provider
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[DataMember]
		public int PriceInCents
		{
			get
			{
				var paymentProviderMethod = PaymentProviderMethods.OrderBy(x => x.PriceWithVatInCents).FirstOrDefault();
				if (paymentProviderMethod != null)
					return PaymentProviderMethods != null ? paymentProviderMethod.PriceWithVatInCents : 0;
				return 0;
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the vat amount in cents.
		/// </summary>
		/// <value>
		/// The vat amount in cents.
		/// </value>
		[IgnoreDataMember]
		public decimal VatAmountInCents
		{
			get { return VatCalculator.VatAmountFromOriginal(IO.Container.Resolve<ISettingsService>().IncludingVat, PriceInCents, Vat); }
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

		/// <summary>
		/// Gets the payment provider.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static PaymentProvider GetPaymentProvider(int id, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IPaymentProviderService>().GetById(id, StoreHelper.GetLocalization(storeAlias, currencyCode) ?? StoreHelper.CurrentLocalization);
		}

		internal static bool IsAlias(string @alias)
		{
			return @alias.StartsWith(NodeAlias);
		}
	}
}