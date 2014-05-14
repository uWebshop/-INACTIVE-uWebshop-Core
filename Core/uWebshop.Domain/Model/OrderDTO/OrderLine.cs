using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace uWebshop.Domain.OrderDTO
{
#pragma warning disable 1591
	public class OrderLine
	{
		public int DiscountAmount;
		public int DiscountId;
		public decimal DiscountPercentage;
		public string DocTypeAlias;

		//public int OrderLineId; ?

		public double Height;
		public int ImageId;
		public double Length;
		public int OrderLineId;

		public int OriginalPrice;
		public int OriginalProductId;
		public List<ProductVariantInfo> ProductVariants; // entity - entity relation
		public int Quantity;
		public string RangesString;
		public string SKU;
		public string[] Tags;

		public string Text;
		public string Title;
		public decimal Vat;
		public double Weight;
		public double Width;

		public bool ExcludingVariants;

		[XmlIgnore] internal XDocument _customData;

		public OrderLine()
		{
		}

		public OrderLine(Domain.OrderLine line)
		{
			var productInfo = line.ProductInfo;
			OriginalProductId = productInfo.Id;
			Quantity = productInfo.ItemCount.GetValueOrDefault(1);
			Title = productInfo.Title;
			SKU = productInfo.SKU;
			Tags = productInfo.Tags ?? new string[0];
			Weight = productInfo.Weight;
			Length = productInfo.Length;
			Height = productInfo.Height;
			Width = productInfo.Width;
			OriginalPrice = productInfo.OriginalPriceInCents;
			RangesString = productInfo.RangesString;
			Vat = productInfo.Vat;
			OrderLineId = line.OrderLineId;
			DiscountId = productInfo.DiscountId;
			_customData = line._customData;

			ProductVariants = productInfo.ProductVariants;
			DiscountPercentage = productInfo.DiscountPercentage;
			DiscountAmount = productInfo.DiscountAmountInCents;
			ExcludingVariants = productInfo.DiscountExcludingVariants;

			DocTypeAlias = productInfo.DocTypeAlias;
		}

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

		public Domain.OrderLine ToOrderLine(OrderInfo orderInfo)
		{
			var productInfo = new ProductInfo();
			productInfo.Id = OriginalProductId;
			productInfo.ItemCount = Quantity;
			productInfo.Title = Title;
			productInfo.SKU = SKU;
			productInfo.Tags = Tags ?? new string[0];
			productInfo.Weight = Weight;
			productInfo.Length = Length;
			productInfo.Height = Height;
			productInfo.Width = Width;
			productInfo.OriginalPriceInCents = OriginalPrice;
			productInfo.RangesString = RangesString;
			productInfo.Vat = Vat;
			productInfo.DiscountId = DiscountId;

			productInfo.ProductVariants = ProductVariants ?? new List<ProductVariantInfo>();
			productInfo.DiscountPercentage = DiscountPercentage;
			productInfo.DiscountAmountInCents = DiscountAmount;
			productInfo.DiscountExcludingVariants = ExcludingVariants;

			productInfo.DocTypeAlias = DocTypeAlias;

			return new Domain.OrderLine(productInfo, orderInfo) {OrderLineId = OrderLineId, _customData = _customData};
		}

		
	}
}