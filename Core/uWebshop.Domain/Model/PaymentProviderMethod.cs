using System;
using System.Collections;
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
	[ContentType(ParentContentType = typeof(PaymentProviderSectionContentType), Name = "Payment Provider Method", Description = "#PaymentProviderMethodDescription", Alias = "uwbsPaymentProviderMethod", IconClass = IconClass.link, Icon = ContentIcon.MagnetSmall, Thumbnail = ContentThumbnail.Folder)]
	public class PaymentProviderMethod : uWebshopEntity
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;
		
		internal ILocalization Localization;
		private Image _image;

		internal int NodeId;
		public new string Id { get; set; }

		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription")]
		public override bool Disabled { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the provider.
		/// </summary>
		/// <value>
		/// The name of the provider.
		/// </value>
		[DataMember]
		public string ProviderName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [test mode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool TestMode { get; set; }

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		[DataMember]
		public Dictionary<string, string> Settings { get; set; }

		/// <summary>
		/// Gets or sets the image unique identifier.
		/// </summary>
		/// <value>
		/// The image unique identifier.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "image", DataType = DataType.MediaPicker, Tab = ContentTypeTab.Global, Name = "#Image", Description = "#ImageDescription")]
		public int ImageId { get; set; }

		/// <summary>
		/// Gets or sets the price in cents.
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "price", DataType = DataType.Price, Tab = ContentTypeTab.Price, Name = "#Price", Description = "#PriceDescription")]
		public int PriceInCents { get; set; }

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		public IVatPrice Price
		{
			get { return Businesslogic.Price.CreateSimplePrice(PriceInCents, IO.Container.Resolve<ISettingsService>().IncludingVat, Vat, Localization); }
		}

		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		[DataMember]
		public Image Image
		{
			get
			{
				if (_image != null) return _image;

				var id = Common.Helpers.ParseInt(StoreHelper.GetMultiStoreItem(NodeId, "image"));

				if (id == 0) return _image;

				var image = IO.Container.Resolve<ICMSContentService>().GetImageById(id);

				if (image != null && !string.IsNullOrEmpty(image.RelativePathToFile))
				{
					_image = image;
				}

				return _image;
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "vat", DataType = DataType.VatPicker, Tab = ContentTypeTab.Price, Name = "#VAT", Description = "#VatDescription")]
		public decimal Vat { get; set; }

		/// <summary>
		/// Gets the type to calculate the payment amount for the order.
		/// </summary>
		/// <value>
		/// The type to calculate the payment amount for the order.
		/// </value>
		[ContentPropertyType(Alias = "amountType", DataType = DataType.PaymentProviderAmountType, Tab = ContentTypeTab.Price, Name = "#AmountType", Description = "#AmountTypeDescription")]
		public PaymentProviderAmountType AmountType { get; set; }

		/// <summary>
		/// Gets or sets the price with vat.
		/// </summary>
		/// <value>
		/// The price with vat.
		/// </value>
		[DataMember]
		public decimal PriceWithVat
		{
			get { return PriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price with vat in cents.
		/// </summary>
		/// <value>
		/// The price with vat in cents.
		/// </value>
		[DataMember]
		public int PriceWithVatInCents
		{
			get { return Price.WithVat.ValueInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat.
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		[DataMember]
		public decimal PriceWithoutVat
		{
			get { return PriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat in cents.
		/// </summary>
		/// <value>
		/// The price without vat in cents.
		/// </value>
		[DataMember]
		public int PriceWithoutVatInCents
		{
			get { return Price.WithoutVat.ValueInCents; }
			set { }
		}

		internal static bool IsAlias(string alias)
		{
			return alias.StartsWith(NodeAlias);
		}
	}
}