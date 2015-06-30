using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderUpdatingService
	{
		/// <summary>
		///     Create, Add, Update the orderline
		/// </summary>
		/// <param name="order"></param>
		/// <param name="orderLineId">The Id of the orderline</param>
		/// <param name="productId">The productId</param>
		/// <param name="action">The action (add, update, delete, deleteall)</param>
		/// <param name="itemCount">The amount of items to be added</param>
		/// <param name="variantsList">The variants ID's added to the pricing</param>
		/// <param name="fields">Custom Fields</param>
		void AddOrUpdateOrderLine(OrderInfo order, int orderLineId, int productId, string action, int itemCount, IEnumerable<int> variantsList, Dictionary<string, string> fields = null);

		/// <summary>
		/// Saves the specified order.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="revalidateOrderOnLoadHack">if set to <c>true</c> [revalidate order configuration load hack].</param>
		/// <param name="validateSaveAction">The validate save action.</param>
		void Save(OrderInfo order, bool revalidateOrderOnLoadHack = false, ValidateSaveAction validateSaveAction = ValidateSaveAction.Order);

		/// <summary>
		/// Removes the discounts with counter zero from order.
		/// </summary>
		/// <param name="order">The order.</param>
		void RemoveDiscountsWithCounterZeroFromOrder(OrderInfo order);

		/// <summary>
		/// Sets the current member.
		/// </summary>
		/// <param name="order">The order.</param>
		void SetCurrentMember(OrderInfo order);

		/// <summary>
		/// Adds the payment provider.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="paymentProviderId">The payment provider unique identifier.</param>
		/// <param name="paymentProviderMethodId">The payment provider method unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		ProviderActionResult AddPaymentProvider(OrderInfo order, int paymentProviderId, string paymentProviderMethodId, ILocalization localization);

		/// <summary>
		/// Adds the coupon.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="couponCode">The coupon code.</param>
		/// <returns></returns>
		CouponCodeResult AddCoupon(OrderInfo order, string couponCode);

		/// <summary>
		/// Adds the shipping provider.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="shippingProviderId">The shipping provider unique identifier.</param>
		/// <param name="shippingProviderMethodId">The shipping provider method unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		ProviderActionResult AddShippingProvider(OrderInfo orderInfo, int shippingProviderId, string shippingProviderMethodId, ILocalization localization);

	    /// <summary>
	    /// Adds the customer fields.
	    /// </summary>
	    /// <param name="orderInfo">The order information.</param>
	    /// <param name="fields">The fields.</param>
	    /// <param name="customerDataType">Type of the customer data.</param>
	    /// <param name="ingnoreNotAllowed">Ignore if order is not allowed to be written to</param>
	    /// <returns></returns>
		bool AddCustomerFields(OrderInfo orderInfo, Dictionary<string, string> fields, CustomerDatatypes customerDataType, bool ingnoreNotAllowed = false);

		/// <summary>
		/// Changes the order automatic incomplete and return false difference not possible.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <returns></returns>
		bool ChangeOrderToIncompleteAndReturnTrueIfNotAllowed(OrderInfo order);

		/// <summary>
		/// Confirms the order.
		/// </summary>
		/// <param name="order">The order information.</param>
		/// <param name="termsAccepted">if set to <c>true</c> [terms accepted].</param>
		/// <param name="confirmationNodeId">The confirmation node unique identifier.</param>
		/// <param name="dontScheduleAlwaysConfirm"></param>
		bool ConfirmOrder(OrderInfo order, bool termsAccepted, int confirmationNodeId, bool dontScheduleAlwaysConfirm = false);

		/// <summary>
		/// Changes the store.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="localization">The localization.</param>
		void ChangeLocalization(OrderInfo order, ILocalization localization);

		/// <summary>
		/// Updates the product information discount information.
		/// </summary>
		/// <param name="product">The product.</param>
		void UpdateProductInfoDiscountInformation(ProductInfo product);
	}
}