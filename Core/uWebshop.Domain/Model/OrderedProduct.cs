using System;
using System.Collections.Generic;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[ContentType(ParentContentType = typeof (OrderRepositoryContentType), Name = "Ordered Product", Description = "#OrderedProductDescription", Alias = "uwbsOrderedProduct", IconClass = IconClass.box, Icon = ContentIcon.BoxLabel, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] {typeof (OrderedProductVariant)})]
	public class OrderedProduct : DocumentBase
	{
		// possible todo: make internal
		// currently nothing more than an interface to edit orderline data in the umbraco backend

		/// <summary>
		/// The general node type alias
		/// </summary>
		public static string NodeAlias;

		/// <param name="id">NodeId of the orderedProduct</param>
		public OrderedProduct(int id) : base(id)
		{
			//		public static void CreateOrderDocument(OrderInfo orderInfo, int parentDocumentId = 0)
		}

		/// <summary>
		/// Gets the specific doctype alias.
		/// </summary>
		/// <value>
		/// The doctype alias.
		/// </value>
		public string DoctypeAlias
		{
			get { return Document.NodeTypeAlias; }
		}

		/// <summary>
		/// Gets the product unique identifier.
		/// </summary>
		/// <value>
		/// The product unique identifier.
		/// </value>
		[ContentPropertyType(Alias = "productId", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Global, Name = "#ProductId", Description = "#ProductIdDescription")]
		public int ProductId
		{
			get
			{
				var property = Document.getProperty("productId");
				if (property == null) throw new Exception("OrderedProduct document type is missing property with alias productId, please add");
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
				if (property == null) throw new Exception("OrderedProduct document type is missing property with alias ranges, please add");
				return property.Value ?? string.Empty;
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
				int value = 0;
				if (property != null)
				{
					int.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[ContentPropertyType(Alias = "vat", DataType = DataType.VatPicker, Tab = ContentTypeTab.Price, Name = "#VAT", Description = "#VatDescription")]
		public decimal Vat
		{
			get
			{
				var property = Document.getProperty("vat").Value;
				decimal value = 0;
				if (property != null)
				{
					decimal.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets the item count.
		/// </summary>
		/// <value>
		/// The item count.
		/// </value>
		[ContentPropertyType(Alias = "itemCount", DataType = DataType.Numeric, Tab = ContentTypeTab.Price, Name = "#ItemCount", Description = "#ItemCountDescription")]
		public int? ItemCount
		{
			get
			{
				var property = Document.getProperty("itemCount").Value;
				int value = 0;
				if (property != null)
				{
					int.TryParse(property, out value);
				}
				return value;
			}
		}

		/// <summary>
		/// Gets or sets the ordered product discount percentage.
		/// </summary>
		/// <value>
		/// The ordered product discount percentage.
		/// </value>
		[ContentPropertyType(Alias = "orderedProductDiscountPercentage", DataType = DataType.String, Tab = ContentTypeTab.Price, Name = "#OrderedProductDiscountPercentage", Description = "#OrderedProductDiscountPercentageDescription")]
		public decimal OrderedProductDiscountPercentage
		{
			get
			{
				if (Document.GetProperty("orderedProductDiscountPercentage") != null && !string.IsNullOrEmpty(Document.GetProperty("orderedProductDiscountPercentage").Value))
				{
					return Document.GetProperty<int>("orderedProductDiscountPercentage");
				}
				return 0;
			}
			set
			{
				if (Document.GetProperty("orderedProductDiscountPercentage") != null)
				{
					Document.SetProperty("orderedProductDiscountPercentage", value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the ordered product discount amount.
		/// </summary>
		/// <value>
		/// The ordered product discount amount.
		/// </value>
		[ContentPropertyType(Alias = "orderedProductDiscountAmount", DataType = DataType.String, Tab = ContentTypeTab.Price, Name = "#OrderedProductDiscountAmount", Description = "#OrderedProductDiscountAmountDescription")]
		public int OrderedProductDiscountAmount
		{
			get
			{
				if (Document.GetProperty("orderedProductDiscountAmount") != null && !string.IsNullOrEmpty(Document.GetProperty("orderedProductDiscountAmount").Value))
				{
					return Document.GetProperty<int>("orderedProductDiscountAmount");
				}
				return 0;
			}
			set
			{
				if (Document.GetProperty("orderedProductDiscountAmount") != null)
				{
					Document.SetProperty("orderedProductDiscountAmount", value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the ordered product discount excluding variants.
		/// </summary>
		[ContentPropertyType(Alias = "orderedProductDiscountExcludingVariants", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Price, Name = "#OrderedProductDiscountExcludingVariants", Description = "#OrderedProductDiscountExcludingVariantsDescription")]
		public bool OrderedProductDiscountExcludingVariants
		{
			get
			{
				if (Document.GetProperty("orderedProductDiscountExcludingVariants") != null && !string.IsNullOrEmpty(Document.GetProperty("orderedProductDiscountExcludingVariants").Value))
				{
					return Document.GetProperty<bool>("orderedProductDiscountAmount");
				}
				// todo: should this be false by default?
				return false;
			}
			set { Document.SetProperty("orderedProductDiscountExcludingVariants", value); }
		}

		/// <summary>
		/// Gets the default properties.
		/// </summary>
		/// <value>
		/// The default properties.
		/// </value>
		public static List<string> DefaultProperties
		{
			get { return new List<string> {"productId", "title", "sku", "price", "ranges", "vat", "itemCount", "length", "width", 
				"height", "weight", "image", "text", "orderedProductDiscountAmount", "orderedProductDiscountPercentage", "url",}; }
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