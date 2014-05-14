using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic.VATChecking
{
	/// <summary>
	/// 
	/// </summary>
	public class FixedValueIvatChecker : IVATCheckService
	{
		private readonly bool _noVATCharged;

		/// <summary>
		/// Initializes a new instance of the <see cref="FixedValueIvatChecker"/> class.
		/// </summary>
		/// <param name="noVATCharged">if set to <c>true</c> [no vat charged].</param>
		public FixedValueIvatChecker(bool noVATCharged)
		{
			_noVATCharged = noVATCharged;
		}

		/// <summary>
		/// Vats the number valid.
		/// </summary>
		/// <param name="number">The number.</param>
		/// <param name="order">The order.</param>
		/// <returns></returns>
		public bool VATNumberValid(string number, OrderInfo order)
		{
			return _noVATCharged;
		}
	}
}