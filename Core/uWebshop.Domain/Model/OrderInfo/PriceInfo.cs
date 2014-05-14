using System;
using System.Runtime.Serialization;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class PriceInfo
	{
		private readonly int _basePrice;
		private readonly bool _basePriceInclVat;
		private readonly decimal _vat;

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceInfo"/> class.
		/// </summary>
		public PriceInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceInfo"/> class.
		/// </summary>
		/// <param name="basePrice">The base price.</param>
		/// <param name="basePriceInclVat">if set to <c>true</c> [base price incl vat].</param>
		/// <param name="vat">The vat.</param>
		public PriceInfo(int basePrice, bool basePriceInclVat, decimal vat)
		{
			_basePrice = basePrice;
			_basePriceInclVat = basePriceInclVat;
			_vat = vat;
		}

		/// <summary>
		/// Gets or sets the with vat in cents.
		/// </summary>
		/// <value>
		/// The with vat in cents.
		/// </value>
		[DataMember]
		public int WithVATInCents
		{
			get { return _basePriceInclVat ? _basePrice : VatCalculator.WithVat(_basePrice, _vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the with vat.
		/// </summary>
		/// <value>
		/// The with vat.
		/// </value>
		[DataMember]
		public decimal WithVAT
		{
			get { return WithVATInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the without vat in cents.
		/// </summary>
		/// <value>
		/// The without vat in cents.
		/// </value>
		[DataMember]
		public int WithoutVATInCents
		{
			get { return _basePriceInclVat ? VatCalculator.WithoutVat(_basePrice, _vat) : _basePrice; }
			set { }
		}

		/// <summary>
		/// Gets or sets the without vat.
		/// </summary>
		/// <value>
		/// The without vat.
		/// </value>
		[DataMember]
		public decimal WithoutVAT
		{
			get { return WithoutVATInCents/100m; }
			set { }
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class ChargedShippingCosts : PriceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChargedShippingCosts"/> class.
		/// </summary>
		public ChargedShippingCosts()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChargedShippingCosts"/> class.
		/// </summary>
		/// <param name="basePrice">The base price.</param>
		/// <param name="basePriceInclVat">if set to <c>true</c> [base price incl vat].</param>
		/// <param name="vat">The vat.</param>
		public ChargedShippingCosts(int basePrice, bool basePriceInclVat, decimal vat) : base(basePrice, basePriceInclVat, vat)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class PaymentCosts : PriceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentCosts" /> class.
		/// </summary>
		public PaymentCosts()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentCosts"/> class.
		/// </summary>
		/// <param name="basePrice">The base price.</param>
		/// <param name="basePriceInclVat">if set to <c>true</c> [base price incl vat].</param>
		/// <param name="vat">The vat.</param>
		public PaymentCosts(int basePrice, bool basePriceInclVat, decimal vat) : base(basePrice, basePriceInclVat, vat)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class OrderLineTotal : PriceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrderLineTotal"/> class.
		/// </summary>
		public OrderLineTotal()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderLineTotal"/> class.
		/// </summary>
		/// <param name="basePrice">The base price.</param>
		/// <param name="basePriceInclVat">if set to <c>true</c> [base price incl vat].</param>
		/// <param name="vat">The vat.</param>
		public OrderLineTotal(int basePrice, bool basePriceInclVat, decimal vat) : base(basePrice, basePriceInclVat, vat)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class DiscountAmount : PriceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DiscountAmount"/> class.
		/// </summary>
		public DiscountAmount()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DiscountAmount"/> class.
		/// </summary>
		/// <param name="basePrice">The base price.</param>
		/// <param name="basePriceInclVat">if set to <c>true</c> [base price incl vat].</param>
		/// <param name="vat">The vat.</param>
		public DiscountAmount(int basePrice, bool basePriceInclVat, decimal vat) : base(basePrice, basePriceInclVat, vat)
		{
		}
	}
}