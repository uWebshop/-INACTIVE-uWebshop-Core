namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderLine
	{
		/// <summary>
		/// Gets the quantity.
		/// </summary>
		/// <value>
		/// The quantity.
		/// </value>
		int Quantity { get;}
		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		IOrderedProduct Product { get; }
		/// <summary>
		/// Gets the amount.
		/// </summary>
		/// <value>
		/// The amount.
		/// </value>
		IDiscountedRangedPrice Amount { get; }
		/// <summary>
		/// Gets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		int Id { get; }
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		T GetValue<T>(string fieldName);
	}
}