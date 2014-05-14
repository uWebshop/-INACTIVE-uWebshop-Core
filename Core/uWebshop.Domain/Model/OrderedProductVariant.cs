using System;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof(OrderRepositoryContentType), Name = "Ordered Product Variant", Description = "#OrderedProductVariantDescription", Alias = "uwbsOrderedProductVariant", IconClass = IconClass.link, Icon = ContentIcon.MagnetSmall, Thumbnail = ContentThumbnail.Folder)]
	public class OrderedProductVariant : DocumentBase
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <param name="id">NodeId of the orderedProductVariant</param>
		internal OrderedProductVariant(int id) : base(id)
		{
			//		public static void CreateOrderDocument(OrderInfo orderInfo, int parentDocumentId = 0)
		}

		/// <summary>
		/// Gets the variant unique identifier.
		/// </summary>
		/// <value>
		/// The variant unique identifier.
		/// </value>
		[ContentPropertyType(Alias = "variantId", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Global, Name = "#VariantId", Description = "#VariantIdDescription")]
		public int VariantId
		{
			get
			{
				var property = Document.getProperty("variantId");
				if (property == null) throw new Exception("OrderedProductVariant document type is missing property with alias variantId, please add");
				int value;
				int.TryParse(property.Value, out value);
				return value;
			}
		}

		/// <summary>
		/// Gets the Type Alias.
		/// </summary>
		/// <value>
		/// The Type Alias.
		/// </value>
		[ContentPropertyType(Alias = "typeAlias", DataType = DataType.Label, Tab = ContentTypeTab.Global, Name = "#TypeAlias", Description = "#TypeAliasDescription")]
		public string TypeAlias
		{
			get
			{
				var cmsProperty = Document.getProperty("typeAlias");
				if (cmsProperty != null) return cmsProperty.Value;
				return null;
			}
			set { Document.SetProperty("typeAlias", value); }
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription")]
		public string Title
		{
			get { return Document.getProperty("title").Value; }
		}

		/// <summary>
		/// Gets the sku.
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		[ContentPropertyType(Alias = "sku", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#SKU", Description = "#SKUDescription")]
		public string SKU
		{
			get { return Document.getProperty("sku").Value; }
		}

		/// <summary>
		/// Gets the group.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
		[ContentPropertyType(Alias = "group", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Group", Description = "#GroupDescription")]
		public string Group
		{
			get { return Document != null ? Document.getProperty("group").Value : string.Empty; }
		}


		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		[ContentPropertyType(Alias = "length", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Length", Description = "#LengthDescription")]
		public double Length
		{
			get
			{
				var property = Document.getProperty("length").Value;
				double value = 0;
				if (property != null)
				{
					double.TryParse(property, out value);
				}

				return value;
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		[ContentPropertyType(Alias = "width", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Width", Description = "WidthDescription")]
		public double Width
		{
			get
			{
				var property = Document.getProperty("width").Value;
				double value = 0;
				if (property != null)
				{
					double.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		[ContentPropertyType(Alias = "height", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Height", Description = "HeightDescription")]
		public double Height
		{
			get
			{
				var property = Document.getProperty("height").Value;
				double value = 0;
				if (property != null)
				{
					double.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the weight.
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		[ContentPropertyType(Alias = "weight", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Weight", Description = "#WeightDescription")]
		public double Weight
		{
			get
			{
				var property = Document.getProperty("weight").Value;
				double value = 0;
				if (property != null)
				{
					double.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the price in cents.
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[ContentPropertyType(Alias = "price", DataType = DataType.Price, Tab = ContentTypeTab.Price, Name = "#Price", Description = "#PriceDescription")]
		public int PriceInCents
		{
			get
			{
				var property = Document.getProperty("price").Value;
				var value = 0;
				if (property != null)
				{
					int.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the ranges string.
		/// </summary>
		/// <value>
		/// The ranges string.
		/// </value>
		[ContentPropertyType(Alias = "ranges", DataType = DataType.Ranges, Tab = ContentTypeTab.Price, Name = "#Ranges", Description = "#RangesDescription")]
		public string RangesString
		{
			get
			{
				var property = Document.getProperty("ranges");
				if (property == null) throw new Exception("OrderedProductVariant document type is missing property with alias ranges, please add");
				return property.Value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets the document type alias.
		/// </summary>
		/// <value>
		/// The document type alias.
		/// </value>
		public string DocTypeAlias
		{
			get { return Document.NodeTypeAlias; }
		}

		/// <summary>
		/// Gets or sets the discount percentage.
		/// </summary>
		/// <value>
		/// The discount percentage.
		/// </value>
		[ContentPropertyType(Alias = "discountPercentage", DataType = DataType.String, Tab = ContentTypeTab.Price, Name = "#OrderedProductVariantDiscountPercentage", Description = "#OrderedProductVariantDiscountPercentageDescription")]
		public decimal DiscountPercentage
		{
			get { return Document.GetProperty<decimal>("discountPercentage"); }
			set { Document.SetProperty("discountPercentage", value); }
		}

		/// <summary>
		/// Gets or sets the discount amount.
		/// </summary>
		/// <value>
		/// The discount amount.
		/// </value>
		[ContentPropertyType(Alias = "discountAmount", DataType = DataType.String, Tab = ContentTypeTab.Price, Name = "#OrderedProductVariantDiscountAmount", Description = "#OrderedProductVariantDiscountAmountDescription")]
		public int DiscountAmount
		{
			get { return Document.GetProperty<int>("discountAmount"); }
			set { Document.SetProperty("discountAmount", value); }
		}

		/// <summary>
		/// Determines whether the specified alias is alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static bool IsAlias(string alias)
		{
			return alias != null && alias.StartsWith(NodeAlias);
		}
	}
}