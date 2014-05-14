namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPrice
	{
		/// <summary>
		/// Gets the decimal value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		decimal Value { get; }
		/// <summary>
		/// Gets the price in cents.
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		int ValueInCents { get; }
		/// <summary>
		/// Produces the currency string based on the current currency culture.
		/// </summary>
		/// <returns></returns>
		string ToCurrencyString();
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IVatPrice// : IPrice
	{
		/// <summary>
		/// Gets the price with vat.
		/// </summary>
		/// <value>
		/// The price with vat.
		/// </value>
		IPrice WithVat { get; }

		/// <summary>
		/// Gets the price without vat.
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		IPrice WithoutVat { get; }

		/// <summary>
		/// Gets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		IPrice Vat { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IDiscountedPrice : IVatPrice
	{
		/// <summary>
		/// Gets the original price before the discount.
		/// </summary>
		/// <value>
		/// The original price before the discount.
		/// </value>
		IVatPrice BeforeDiscount { get; }

		/// <summary>
		/// Gets the discount.
		/// </summary>
		/// <value>
		/// The discount.
		/// </value>
		IVatPrice Discount { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IRangedPrice : IVatPrice
	{
		/// <summary>
		/// Gets the ranged.
		/// </summary>
		/// <value>
		/// The ranged.
		/// </value>
		IVatPrice Ranged { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IDiscountedRangedPrice : IDiscountedPrice, IRangedPrice
	{
		/// <summary>
		/// Gets the price adjusted to the current order.
		/// </summary>
		/// <value>
		/// The ranged price.
		/// </value>
		new IDiscountedPrice Ranged { get; }
		/// <summary>
		/// Gets the original price before the discount.
		/// </summary>
		/// <value>
		/// The original price before the discount.
		/// </value>
		new IRangedPrice BeforeDiscount { get; }
	}
}