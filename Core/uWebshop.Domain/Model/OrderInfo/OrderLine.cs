using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Name="OrderLine", Namespace = "")]
	[Serializable]
	public class OrderLine
	{
		[XmlIgnore] internal IOrderInfo Order;

		/// <summary>
		/// The order line unique identifier
		/// </summary>
		[DataMember] public int OrderLineId;

		/// <summary>
		/// The product information
		/// </summary>
		[DataMember]
		public ProductInfo ProductInfo
		{
			get { return _productInfo; }
			set
			{
				if (_productInfo != value)
				{
					value.ItemCountChanged += e => _sellableUnits = null;
				}
				_productInfo = value;
			}
		}

		private ProductInfo _productInfo;

		private IEnumerable<SellableUnit> _sellableUnits;

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<SellableUnit> SellableUnits
		{
			get
			{
				if (_sellableUnits == null)
				{
					var sellableUnits = new List<SellableUnit>();
					for (int i = 0; i < ProductInfo.Quantity; i++)
					{
						sellableUnits.Add(new SellableUnit(ProductInfo));
					}
					_sellableUnits = sellableUnits;
					var order = (Order as OrderInfo);
					//if (order != null) order.ResetDiscounts();
				}
				return _sellableUnits;
			}
			set { }
		}

		[XmlIgnore] [NonSerialized] internal XDocument _customData;

		private OrderLine()
		{
			ProductInfo = new ProductInfo();// { ItemCount = 0 }; evt of 1?
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderLine"/> class.
		/// </summary>
		/// <param name="productInfo">The product information.</param>
		/// <param name="order">The order.</param>
		public OrderLine(ProductInfo productInfo, IOrderInfo order)
		{
			Order = order;
			productInfo.Order = order;
			ProductInfo = productInfo;
		}

		internal IDiscountedRangedPrice Amount
		{
			get
			{
				return new SummedPrice(SellableUnits.Select(su => su.Price), Order.PricesAreIncludingVAT, ProductInfo.Vat, Order.Localization, (Order as OrderInfo).VatCalculationStrategy);
			}
		}

		public int GetAmount(bool inclVat, bool discounted, bool ranged)
		{
			return SellableUnits.Sum(su => su.GetAmount(inclVat, discounted, ranged));
		}

		public int GetOriginalAmount(bool discounted, bool ranged)
		{
			return SellableUnits.Sum(su => su.GetOriginalAmount(discounted, ranged));
		}

		/// <summary>
		/// Gets or sets the custom data.
		/// </summary>
		/// <value>
		/// The custom data.
		/// </value>
		[DataMember]
		public XElement CustomData
		{
			get { return _customData != null ? _customData.Root : null; }
			set
			{
				_customData = new XDocument();
				_customData.Add(value);
			}
		}

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		[DataMember]
		public double Weight
		{
			get { return (ProductInfo.Weight + ProductInfo.ProductVariants.Sum(variant => variant.Weight))*ProductInfo.ItemCount.GetValueOrDefault(1); }
			set { }
		}

		/// <summary>
		/// Gets or sets the order line weight.
		/// </summary>
		/// <value>
		/// The order line weight.
		/// </value>
		[DataMember]
		public double OrderLineWeight
		{
			get { return (ProductInfo.Weight + ProductInfo.ProductVariants.Sum(variant => variant.Weight))*ProductInfo.ItemCount.GetValueOrDefault(1); }
			set { }
		}

		/// <summary>
		/// Gets or sets the amount in cents without order discount substracted.
		/// </summary>
		/// <value>
		/// The amount in cents.
		/// </value>
		[DataMember]
		public int AmountInCents
		{
			get
			{
				return GetAmount(true, false, true);
				return ProductInfo.PriceInCents*ProductInfo.ItemCount.GetValueOrDefault(1);
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the discount in cents.
		/// </summary>
		/// <value>
		/// The discount in cents.
		/// </value>
		[DataMember]
		public int DiscountInCents
		{
			get { return AmountInCents - DiscountedAmountInCents; }
			set{}
		}

		internal int DiscountedAmountInCents
		{
			get { return GetAmount(true, true, true); }
		}

		internal int DiscountedAmountWithoutVatInCents
		{
			get { return GetAmount(false, true, true); }
		}

		internal int DiscountedVatAmountInCents
		{
			get { return DiscountedAmountInCents - DiscountedAmountWithoutVatInCents; }
		}

		/// <summary>
		/// Orderline vat amount
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		public decimal Vat
		{
			get { return ProductInfo.Vat; }
			set { }
		}

		/// <summary>
		/// Gets or sets the sub total in cents.
		/// </summary>
		/// <value>
		/// The sub total in cents.
		/// </value>
		[DataMember]
		public int SubTotalInCents
		{
			get { return GetAmount(false, true, true); }
			set { }
		}

		/// <summary>
		/// Gets or sets the grand total in cents.
		/// </summary>
		/// <value>
		/// The grand total in cents.
		/// </value>
		[DataMember]
		public int GrandTotalInCents
		{
			get { return GetAmount(true, true, true); }
			set { }
		}

		/// <summary>
		/// Gets or sets the vat amount in cents.
		/// </summary>
		/// <value>
		/// The vat amount in cents.
		/// </value>
		[DataMember]
		public int VatAmountInCents
		{
			get { return GrandTotalInCents - SubTotalInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the order line vat amount in cents.
		/// </summary>
		/// <value>
		/// The order line vat amount in cents.
		/// </value>
		[DataMember]
		public int OrderLineVatAmountInCents
		{
			get { return ProductInfo.VatAmountInCents*ProductInfo.ItemCount.GetValueOrDefault(1); }
			set { }
		}

		/// <summary>
		/// Gets or sets the order line vat amount after order discount in cents.
		/// </summary>
		/// <value>
		/// The order line vat amount after order discount in cents.
		/// </value>
		[DataMember]
		public int OrderLineVatAmountAfterOrderDiscountInCents
		{
			get
			{
				return DiscountedVatAmountInCents; // todo: check and if right make this property obsolete
				//return ProductInfo.OrderVatAmountInCents * ProductInfo.ItemCount.GetValueOrDefault(1);
			}
			set { }
		}

		internal bool VariantsMatch(IEnumerable<int> variants)
		{
			return variants.OrderBy(v => v).SequenceEqual(ProductInfo.ProductVariants.Select(variant => variant.Id).OrderBy(v => v));
		}

		/// <summary>
		/// Determines whether [has variant with unique identifier] [the specified unique identifier].
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public bool HasVariantWithId(int id)
		{
			return ProductInfo.ProductVariants.Any(usedVariant => id == usedVariant.Id);
		}
		
		#region non-cents variants

		/// <summary>
		///     Orderline amount without vat
		/// </summary>
		[DataMember]
		public decimal SubTotal
		{
			get { return SubTotalInCents/100m; }
			set { }
		}

		/// <summary>
		///     Orderline vat amount
		/// </summary>
		[DataMember]
		public decimal VatAmount
		{
			get { return OrderLineVatAmountInCents/100m; }
			set { }
		}

		/// <summary>
		///     Orderline amount with vat
		/// </summary>
		[DataMember]
		public decimal GrandTotal
		{
			get { return GrandTotalInCents / 100m; }
			set { }
		}

		#endregion


	}
}