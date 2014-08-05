using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// Discount over a product
	/// </summary>
	[Serializable]
	[DataContract(Namespace = "", IsReference = true)]
	[ContentType(ParentContentType = typeof(DiscountProductSectionContentType), Name = "Discount Product", Description = "#DiscountProductDescription", Alias = "uwbsDiscountProduct", IconClass = IconClass.tag, Icon = ContentIcon.PriceTagMinus, Thumbnail = ContentThumbnail.Folder)]
	public class DiscountProduct : DiscountBase, IProductDiscount
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The section node alias
		/// </summary>
		public static string SectionNodeAlias { get { return DiscountProductSectionContentType.NodeAlias; } }

		/// <summary>
		/// List of products this discount applies to
		/// </summary>
		/// <value>
		/// The items.
		/// </value>
		internal IEnumerable<int> Items { get; set; }

		/// <summary>
		/// List of products this discount applies to
		/// </summary>
		/// <value>
		/// The products.
		/// </value>
        [ContentPropertyType(Alias = "products", DataType = DataType.MultiContentPickerCatalog, Tab = ContentTypeTab.Details, Name = "#Products", Description = "#ProductsDescription", SortOrder = 3)]
		public IEnumerable<IProduct> Products { get; set; }

		/// <summary>
		/// List of products variants this discount applies to
		/// </summary>
		/// <value>
		/// The product variants.
		/// </value>
		public IEnumerable<IProductVariant> ProductVariants { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [exclude variants].
		/// </summary>
		/// <value>
		///   <c>true</c> if [exclude variants]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "excludeVariants", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Details, Name = "#ExcludeVariants", Description = "#ExcludeVariantsDescription", SortOrder = 4)]
		public bool ExcludeVariants { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [is active].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is active]; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive
		{
			get { return !Disabled && (!CounterEnabled || Counter > 0); }
			set { }
		}

		// todo: centralize and test
		/// <summary>
		/// The discount value in cents, based on catalog information
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <returns></returns>
		public int GetDiscountAmountInCents(int productId = 0, OrderInfo order = null)
		{
			var product = DomainHelper.GetProductById(productId);
			var productPrice = 0;
			if (product != null)
			{
				if (product.Ranges != null && product.Ranges.Any())
				{
					var range = product.Ranges.FirstOrDefault(x => x.From <= 1 && x.PriceInCents != 0);

					if (range != null)
					{
						productPrice = range.PriceInCents;
					}
				}
				else
				{
					//productPrice = product.Price.BeforeDiscount.ValueInCents; this doesn't work, because IsDiscounted will call this function, creating a loop
				}
			}

			if (order == null) order = OrderHelper.GetOrder();
			var orderCount = 0;
			if (order != null)
			{
				orderCount = order.OrderLines.Select(l => l.ProductInfo).Where(p => p.OriginalId == productId).Sum(p => p.Quantity);
			}
			var discountValue = RangedDiscountValue(orderCount);

			if (DiscountType == DiscountType.NewPrice)
			{
				return discountValue - productPrice;
			}
			if (DiscountType == DiscountType.Amount)
			{
				return Math.Max(discountValue, productPrice);
			}
			if (DiscountType == DiscountType.Percentage)
			{
				return DiscountHelper.PercentageCalculation(discountValue, productPrice);
			}
			return 0;
		}


		/// <summary>
		/// Gets a value indicating whether the discount is once per customer.
		/// </summary>
		/// <value>
		/// <c>true</c> if the discount is once per customer; otherwise, <c>false</c>.
		/// </value>
		public bool OncePerCustomer
		{
			get { return false; }
		}

		public DiscountType Type { get { return DiscountType; } }

		internal static bool IsAlias(string alias)
		{
			return alias.StartsWith(NodeAlias);
		}
	}
}