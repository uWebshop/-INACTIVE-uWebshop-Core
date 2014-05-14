using System;
using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBasket : IOrderBasketShared
	{
		/// <summary>
		/// Gets a value indicating whether [terms accepted].
		/// </summary>
		/// <value>
		///   <c>true</c> if [terms accepted]; otherwise, <c>false</c>.
		/// </value>
		bool TermsAccepted { get; }

		/// <summary>
		/// Gets the validation results.
		/// </summary>
		/// <value>
		/// The validation results.
		/// </value>
		IValidationResults ValidationResults { get; }
	}
	interface IBasketChangingoid
	{
		// actions
		void RegisterCustomOrderValidation(Predicate<IBasket> condition, Func<IBasket, string> errorDictionaryItem); // hmm
		void AddOrUpdateOrderLine(int orderLineId, int productId, string action, int itemCount, IEnumerable<int> variantsList, Dictionary<string, string> fields = null);
		bool ConfirmOrder(bool termsAccepted, int confirmationNodeId);
		ProviderActionResult AddPaymentProvider(int paymentProviderId, string paymentProviderMethodId);
		ProviderActionResult AddShippingProvider(int shippingProviderId, string shippingProviderMethodId);
		bool AddCustomerFields(Dictionary<string, string> fields, CustomerDatatypes customerDataType);
		CouponCodeResult AddCoupon(string couponCode);

		void Save(); // ????
	}
}