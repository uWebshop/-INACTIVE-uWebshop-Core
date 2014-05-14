namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IVATCheckService
	{
		/// <summary>
		/// Vats the number valid.
		/// </summary>
		/// <param name="number">The number.</param>
		/// <param name="order">The order.</param>
		/// <returns></returns>
		bool VATNumberValid(string number, OrderInfo order);
	}
}