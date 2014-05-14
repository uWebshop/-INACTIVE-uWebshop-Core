using System;
using System.Runtime.Serialization;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.Domain.Interfaces
{
	[Serializable]
	[DataContract(Namespace = "")]
	public class FlatPrice
	{
		public FlatPrice(IPrice price)
		{
			Value = price.Value;
			ValueInCents = price.ValueInCents;
			ToCurrencyString = price.ToCurrencyString();
		}

		[DataMember]
		public decimal Value;
		[DataMember]
		public int ValueInCents;
		[DataMember]
		public string ToCurrencyString;
	}
	[Serializable]
	[DataContract(Namespace = "")]
	public class FlatVatPrice
	{
		public FlatVatPrice(IVatPrice price)
		{
			WithVat = new FlatPrice(price.WithVat);
			WithoutVat = new FlatPrice(price.WithoutVat);
			Vat = new FlatPrice(price.Vat);
		}
		[DataMember]
		public FlatPrice WithVat;
		[DataMember]
		public FlatPrice WithoutVat;
		[DataMember]
		public FlatPrice Vat;
	}
	[Serializable]
	[DataContract(Namespace = "")]
	public class FlatRangedPrice
	{
		public FlatRangedPrice(IRangedPrice price)
		{
			WithVat = new FlatPrice(price.WithVat);
			WithoutVat = new FlatPrice(price.WithoutVat);
			Vat = new FlatPrice(price.Vat);
			Ranged = new FlatVatPrice(price.Ranged);
		}
		[DataMember]
		public FlatPrice WithVat;
		[DataMember]
		public FlatPrice WithoutVat;
		[DataMember]
		public FlatPrice Vat;
		[DataMember]
		public FlatVatPrice Ranged;
	}
	[Serializable]
	[DataContract(Namespace = "")]
	public class FlatDiscountedPrice
	{
		public FlatDiscountedPrice(IDiscountedPrice price)
		{
			WithVat = new FlatPrice(price.WithVat);
			WithoutVat = new FlatPrice(price.WithoutVat);
			Vat = new FlatPrice(price.Vat);
			BeforeDiscount = new FlatVatPrice(price.BeforeDiscount);
			Discount = new FlatVatPrice(price.Discount);
		}

		[DataMember]
		public FlatPrice WithVat;
		[DataMember]
		public FlatPrice WithoutVat;
		[DataMember]
		public FlatPrice Vat;
		[DataMember]
		public FlatVatPrice BeforeDiscount;
		[DataMember]
		public FlatVatPrice Discount;
	}
	[Serializable]
	[DataContract(Namespace = "")]
	public class FlatDiscountedRangedPrice
	{
		public FlatDiscountedRangedPrice(IDiscountedRangedPrice price)
		{
			WithVat = new FlatPrice(price.WithVat);
			WithoutVat = new FlatPrice(price.WithoutVat);
			Vat = new FlatPrice(price.Vat);
			BeforeDiscount = new FlatRangedPrice(price.BeforeDiscount);
			Discount = new FlatVatPrice(price.Discount);
			Ranged = new FlatDiscountedPrice(price.Ranged);
		}

		[DataMember]
		public FlatPrice WithVat;
		[DataMember]
		public FlatPrice WithoutVat;
		[DataMember]
		public FlatPrice Vat;
		[DataMember]
		public FlatDiscountedPrice Ranged;
		[DataMember]
		public FlatRangedPrice BeforeDiscount;
		[DataMember]
		public FlatVatPrice Discount;
	}
}