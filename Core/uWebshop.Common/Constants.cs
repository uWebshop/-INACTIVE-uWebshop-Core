namespace uWebshop.Common
{
	public static class Constants
	{
		public const string ErrorMessagesSessionKey = "uwbsErrorMessages";

		public const string CreateMemberSessionKey = "AccountCreate";
		public const string CreateMemberSessionKeyAddition = "AccountCreateAddition";

		public const string CouponCodeResultSessionKey = "CouponCodeResult";
		public const string CouponCodeSessionKey = "CouponCode";

		public const string StorePickerAlias = "uwbsStorePicker";

		public const string UpdateMemberSessionKey = "AccountUpdate";
		public const string UpdateMemberSessionKeyAddition = "AccountUpdateAddition";

		public static string ChangePasswordSessionKey = "ChangePassword";

		public const string SignInMemberSessionKey = "AccountSignIn";

		public const string SignOutMemberSessionKey = "AccountSignOut";

		public const string RequestPasswordSessionKey = "AccountRequestPassword";

		public const string PaymentProviderSessionKey = "PaymentProvider";

		public const string ShippingProviderSessionKey = "ShippingProvider";

		public const string OrderedItemcountHigherThanStockKey = "OrderedItemcountHigherThanStockKey";

		public const string NonMultiStoreAlias = "AllStores";

		public static string BasketActionResult = "BasketActionResult";

		public static string PostedFieldsKey = "PostedValues";

		public static string WishlistActionResult = "Wishlist";

		public static string WishlistToBasketActionResult = "WishlistToBasket";

		public static string WishlistRemoveActionResult = "WishlistRemove";

		public static string WishlistRenameActionResult = "WishlistRename";
		
		public static string ConfirmOrderKey = "ConfirmOrder";
		public static string CustomerInformation = "CustomerInformation";

		public static string ValidateOrderlineResult = "ValidateOrderlineResult";
		public static string ValidateStockResult = "ValidateStockResult";
		public static string ValidateCustomResult = "ValidateCustomResult";
		public static string ValidateCustomerResult = "ValidateCustomerResult";
		public static string ValidateOrderResult = "ValidateOrderResult";

		public static string ChangeCurrencyResult = "ChangeCurrencyResult";
	}
	public static class RegistrationOrder
	{
		public const int Logging = 2000;
		public const int InternalNoDependencies = 3000;
	}
	public static class InitializationOrder
	{
		public const int ContentTypeAliasses = 1000;
		public const int Settings = 1100;

		public const int InternalNoDependencies = 3000;
	}
}