namespace uWebshop.Common
{
	public enum ValidateSaveAction
	{
		Order,
		Customer,
		ShippingCustomer,
		Shipping,
		Payment,
		Stock,
		Orderlines,
		CustomValidation
	}

	public enum BasketActions
	{
		OrderCountTooHighError
	}

	public enum CouponCodeResult
	{
		Success,
		Failed,
		NotFound,
		OutOfStock,
		AlreadyUsed,
		OncePerCustomer,
		MinimumOrderAmount,
		NotPermitted
	}

	public enum BasketActionResult
	{
		Success,
		Failed,
		NoProductOrOrderLineId,
		OrderNull,
		AlreadyExists
	}

	public enum ProviderActionResult
	{
		Success,
		NoCorrectInput,
		ProviderIdZero,
		NotPermitted
	}

	public enum AccountActionResult
	{
		MinRequiredPasswordLengthError,
		MemberExists,
		Success,
		PasswordMismatch,
		Failed,
		PasswordStrengthRegularExpressionError,
		MinRequiredNonAlphanumericCharactersError,
		DefaultMemberTypeAliasError,
		DefaultMemberTypeAliasNonExistingError,
		CustomerEmailEmpty,
		MemberNotExists,
		ValidateUserError,
		AccountForgotPasswordEmailNotConfigured,
		EnablePasswordResetDisabled,
		ChangePasswordError,
		EmailAddressNotValid,
		CustomerUserNameEmpty,
		CurrentpasswordError,
		UserNameInvalid,
		NoUserNameInput,
		SuccessMemberExists
	}

	public enum ConfirmOrderResults
	{
		Success,
		Failed
	}

	public enum CustomerInformationResult
	{
		Success,
		Failed
	}

	public enum OrderStatus
	{
		Cancelled,
		Closed,
		PaymentFailed,
		Incomplete,
		Confirmed,
		OfflinePayment,
		Pending,
		ReadyForDispatch,
		ReadyForDispatchWhenStockArrives,
		Dispatched,
		Undeliverable,
		WaitingForPayment,
		WaitingForPaymentProvider,
		Returned,
		Wishlist
	}

	public enum EmailType
	{
		Unknown,
		Account_Creation,
		Order_Confirmation_Customer,
		Order_Confirmation_Shop,
		Order_Confirmation_Offline_Customer,
		Order_Confirmation_Offline_Shop,
		Payment_Confirmation_Customer,
		Payment_Confirmation_Shop,
		Payment_Failed_Customer,
		Payment_Failed_Shop,
		Order_Completed_Customer,
		Order_Completed_Shop,
		Order_NotInStock_Customer,
		Order_NotInStock_Shop,
		Order_Returned_Customer,
		Order_Returned_Shop
	}

	public enum PaymentProviderType
	{
		Unknown,
		OnlinePayment,
		OfflinePaymentInStore,
		OfflinePaymentAtCustomer
	}

	public enum PaymentTransactionMethod
	{
		QueryString,
		Form,
		Custom,
		ServerPost,
		WebClient,
		Inline
	}

	public enum ShippingProviderType
	{
		Unknown,
		Shipping,
		Pickup
	}

	public enum ShippingRangeType
	{
		Quantity,
		OrderAmount,
		Weight,
		None
	}

	public enum ShippingTransactionMethod
	{
		QueryString,
		Form,
		Custom,
		ServerPost
	}

	public enum DiscountType
	{
		Percentage,
		Amount,
		FreeShipping,
		NewPrice
	}

	public enum PaymentProviderAmountType
	{
		Amount,
		OrderPercentage,
	}

	public enum DiscountOrderCondition
	{
		None,
		OnTheXthItem,
		PerSetOfXItems
	}

	public enum CustomerDatatypes
	{
		Customer,
		Shipping,
		Extra
	}
}