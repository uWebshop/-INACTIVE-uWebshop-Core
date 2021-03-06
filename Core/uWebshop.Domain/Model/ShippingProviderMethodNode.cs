﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	///     Shipping method of a Shipping Provider
	/// </summary>
	[DataContract(Namespace = "")]
	[ContentType(ParentContentType = typeof (ShippingProviderSectionContentType), Name = "Shipping Provider Method", Description = "#ShippingProviderMethodDescription", Alias = NodeAlias, Icon = ContentIcon.MagnetSmall, Thumbnail = ContentThumbnail.Folder)]
	public class ShippingProviderMethodNode : uWebshopEntity
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public const string NodeAlias = "uwbsShippingProviderMethod";
		private string _description;
		private Image _image;

		/// <summary>
		///     Gets the title of the method
		/// </summary>
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription")]
		public string Title
		{
			get { return StoreHelper.GetMultiStoreItem(Id, "title"); }
			set { }
		}

		/// <summary>
		///     Gets the description of the method
		/// </summary>
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription")]
		public string Description
		{
			get { return IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(_description ?? (_description = StoreHelper.GetMultiStoreItem(Id, "description") ?? string.Empty)); }
			set { }
		}

		/// <summary>
		///     Image of the method
		/// </summary>
		[ContentPropertyType(Alias = "image", DataType = DataType.MediaPicker, Tab = ContentTypeTab.Global, Name = "#Image", Description = "#ImageDescription")]
		public Image Image
		{
			get
			{
				if (_image != null) return _image;
				
				int id = Common.Helpers.ParseInt(StoreHelper.GetMultiStoreItem(Id, "image"));

				if (id == 0) return _image;

				var image = IO.Container.Resolve<ICMSContentService>().GetImageById(id);

				if (image != null && !String.IsNullOrEmpty(image.RelativePathToFile))
				{
					_image = image;
				}

				return _image;
			}
			set { }
		}


		/// <summary>
		///     Is this  disabled for the current store?
		/// </summary>
		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription")]
		public override bool Disabled
		{
			get
			{
				return StoreHelper.GetMultiStoreDisable(Id);
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>
		/// The settings.
		/// </value>
		public Dictionary<string, string> Settings { get; set; }

		/// <summary>
		/// Gets or sets the price information cents.
		/// </summary>
		/// <value>
		/// The price information cents.
		/// </value>
		[ContentPropertyType(Alias = "price", DataType = DataType.Price, Tab = ContentTypeTab.Price, Name = "#Price", Description = "#PriceDescription")]
		public int PriceInCents
		{
			get
			{
				string value = StoreHelper.GetMultiStoreItem(Id, "price");

				return value.Any(Char.IsLetter) ? 0 : Int32.Parse(value);
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the price with vat.
		/// </summary>
		/// <value>
		/// The price with vat.
		/// </value>
		public decimal PriceWithVat
		{
			get { return PriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price with vat information cents.
		/// </summary>
		/// <value>
		/// The price with vat information cents.
		/// </value>
		public int PriceWithVatInCents
		{
			get { return IO.Container.Resolve<ISettingsService>().IncludingVat ? PriceInCents : VatCalculator.WithVat(PriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat.
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		public decimal PriceWithoutVat
		{
			get { return PriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat information cents.
		/// </summary>
		/// <value>
		/// The price without vat information cents.
		/// </value>
		public int PriceWithoutVatInCents
		{
			get { return IO.Container.Resolve<ISettingsService>().IncludingVat ? VatCalculator.WithoutVat(PriceInCents, Vat) : PriceInCents; }
			set { }
		}

		/// <summary>
		///     The taxpercentage of the method
		/// </summary>
		/// todo: VAT moet komen uit de AverageOrderVatPercentage ipv op te geven (in orderinfo is dit al wel goed)
		[ContentPropertyType(Alias = "vat", DataType = DataType.VatPicker, Tab = ContentTypeTab.Price, Name = "#VAT", Description = "#VatDescription")]
		public decimal Vat
		{
			get
			{
				decimal vatPercentage = 0;

				string property = StoreHelper.GetMultiStoreItem(Id, "vat");

				if (!String.IsNullOrEmpty(property))
				{
					Decimal.TryParse(property, out vatPercentage);

					return vatPercentage;
				}

				Store store = StoreHelper.GetCurrentStore();

				if (store != null) vatPercentage = store.GlobalVat;

				return vatPercentage;
			}
		}

		/// <summary>
		/// Gets the vat amount.
		/// </summary>
		/// <value>
		/// The vat amount.
		/// </value>
		[IgnoreDataMember]
		public decimal VatAmount
		{
			get { return VatAmountInCents/100m; }
		}

		/// <summary>
		/// Gets the vat amount information cents.
		/// </summary>
		/// <value>
		/// The vat amount information cents.
		/// </value>
		[IgnoreDataMember]
		public int VatAmountInCents
		{
			get { return VatCalculator.VatAmountFromOriginal(IO.Container.Resolve<ISettingsService>().IncludingVat, PriceInCents, Vat); }
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.Category class
		/// </summary>
		/// <param name="id">NodeId of the category</param>
		public ShippingProviderMethodNode(int id) : base(id)
		{
		}
	}
}