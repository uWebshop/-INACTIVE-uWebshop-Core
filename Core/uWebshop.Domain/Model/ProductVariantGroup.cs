using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// Product variants are grouped by a group title
	/// </summary>
	[DataContract(Namespace = "", IsReference = true)]
	[ContentType(ParentContentType = typeof(Catalog), Name = "Product Variant Group", Description = "#ProductVariantGroupDescription", Alias = "uwbsProductVariantGroup", IconClass = IconClass.folder, Icon = ContentIcon.FolderOpenTable, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ProductVariant) })]
	public class ProductVariantGroup : uWebshopEntity, IProductVariantGroup
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;
		internal ILocalization Localization;
		internal Func<IEnumerable<IProductVariant>> ProductVariantFactory;
		private IEnumerable<IProductVariant> _variants;

		public ProductVariantGroup()
		{
			Variants = Enumerable.Empty<IProductVariant>();
			ProductVariants = Enumerable.Empty<ProductVariant>();
		}
		internal ProductVariantGroup(string title, IEnumerable<ProductVariant> productVariants, int id)
		{
			Title = title;
			ProductVariants = productVariants ?? Enumerable.Empty<ProductVariant>();
			Variants = (productVariants ?? Enumerable.Empty<ProductVariant>()).ToList();
			Id = id;
		}

		/// <summary>
		/// Product Variants in this group
		/// </summary>
		/// <value>
		/// The product variants.
		/// </value>
		public IEnumerable<ProductVariant> ProductVariants { get; set; }

		/// <summary>
		/// Product Variants in this group
		/// </summary>
		/// <value>
		/// The product variants.
		/// </value>
		public IEnumerable<IProductVariant> Variants
		{
			get
			{  
				return (_variants ?? (_variants = ProductVariantFactory()));
			}
			set { _variants = value; }
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		
		/// <summary>
		/// Gets a value indicating whether the variant group is required.
		/// </summary>
		/// <value>
		///   <c>true</c> if the variant group is required; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "required", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global,
			Name = "#RequiredVariantGroup", Description = "#RequiredVariantGroupDescription", SortOrder = 2)]
		public bool Required
		{
			get
			{
				return _requiredFromDatabase ?? ProductVariants.Any(pv => pv.Required);
			}
			set
			{
				_requiredFromDatabase = value;
				foreach (var variant in ProductVariants)
				{
					variant.Required = value;
				}
			}
		}

		private bool? _requiredFromDatabase;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Global, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 3)]
		public string Description { get; set; }

	}
}