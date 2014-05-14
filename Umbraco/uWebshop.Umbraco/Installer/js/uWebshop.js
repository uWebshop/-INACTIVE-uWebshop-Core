/* UWEBSHOP CLASS */

if (typeof uWebshop === 'undefined') {
	var uWebshop = {};
}

if (typeof Basket === 'undefined') {
	var Basket = {};
}

if (typeof Catalog === 'undefined') {
	var Catalog = {};
}

if (typeof Customers === 'undefined') {
	var Customers = {};
}

if (typeof Discounts === 'undefined') {
	var Discounts = {};
}

if (typeof Orders === 'undefined') {
	var Orders = {};
}

if (typeof Providers === 'undefined') {
	var Providers = {};
}

if (typeof Store === 'undefined') {
	var Store = {};
}

(function () {

	/* Get the current order */
	uWebshop.Handle = function (formData, success, error) {
		return uWebshopService.basketHandler(formData, success, error);
	};

	/* call any uWebshop.API function with Json */
	uWebshop.GetJSON = function (methodName, formData, success, error) {
		return uWebshopService.callBase(methodName, formData, success, error);
	};
	
	// Basket API
	
	/* Get the current Basket */
	Basket.GetBasket = function (success, error) {
		return uWebshopService.callBase('Basket.GetBasket', null, success, error);
	};
	
	/* Gets the current basket or create a new basket if basket was confirmed. */
	Basket.GetCurrentOrNewBasket = function (success, error) {
		return uWebshopService.callBase('Basket.GetCurrentOrNewBasket', null, success, error);
	};

	/* Gets the fulfillment providers. (ie shipping/pickup/etc) */
	Basket.GetFulfillmentProviders = function(useZone, storeAlias, currencyCode, success, error) {
		var dataArray = [({ key: "useZone", value: useZone }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode })];
		return uWebshopService.callBase('Basket.GetFulfillmentProviders', dataArray, success, error);
	};
	
	/* Gets the payment providers */
	Basket.GetPaymentProviders = function (useZone, storeAlias, currencyCode, success, error) {
		var dataArray = [({ key: "useZone", value: useZone }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode })];
		return uWebshopService.callBase('Basket.GetPaymentProviders', dataArray, success, error);
	};
	
	/* Gets the order discounts */
	Basket.GetAllOrderDiscounts = function (storeAlias, currencyCode, success, error) {
		var dataArray = [({ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode })];
		return uWebshopService.callBase('Basket.GetAllOrderDiscounts', dataArray, success, error);
	};
	
	// Orders API
	/* Get the current order by unique id */
	Orders.GetOrder = function (guidAsString, success, error) {
		var dataArray = [{ key: "guidAsString", value: guidAsString }];
		return uWebshopService.callBase('Orders.GetOrder', dataArray, success, error);
	};
	
	/* Get order by transaction Id */
	Orders.GetOrderByTransactionId = function (transactionId, success, error) {
		var dataArray = [{ key: "transactionId", value: transactionId }];
		return uWebshopService.callBase('Orders.GetOrderByTransactionId', dataArray, success, error);
	};
	
	/* Get all orders (by store) */
	Orders.GetAllOrders = function (storeAlias, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Orders.GetAllOrders', dataArray, success, error);
	};

	/* Get orders (by status) */
	Orders.GetOrdersByStatus = function (status, storeAlias, success, error) {
		var dataArray = [{ key: "status", value: status }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Orders.GetOrders', dataArray, success, error);
	};

	/* Get orders (by last x days) */
	Orders.GetOrdersFromLastDays = function (days, storeAlias, success, error) {
		var dataArray = [{ key: "days", value: days }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Orders.GetOrders', dataArray, success, error);
	};
	
	/* Get orders for customer (by UserName) */
	Orders.GetOrdersForCustomer = function (userName, success, error) {
		var dataArray = [{ key: "userName", value: userName }];
		return uWebshopService.callBase('Orders.GetOrdersForCustomer', dataArray, success, error);
	};
	
	// Catalog API
	
	/* Get All Categories */
	Catalog.GetAllCategories = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetAllCategories', dataArray, success, error);
	};

	/* Get All Products */
	Catalog.GetAllProducts = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetAllProducts', dataArray, success, error);
	};

	/* Get All Product Variants */
	Catalog.GetAllProductVariants = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetAllProducts', dataArray, success, error);
	};

	/* Get a Category by Id */
	Catalog.GetCategory = function (categoryId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "categoryId", value: categoryId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetCategory', dataArray, success, error);
	};

	/* Get all categories that are nested below the given category */
	Catalog.GetCategoriesRecursive = function (categoryId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "categoryId", value: categoryId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetCategoriesRecursive', dataArray, success, error);
	};

	/* Get a Product by Id */
	Catalog.GetProduct = function (productId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "productId", value: productId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetProduct', dataArray, success, error);
	};

	/* Get all proucts that are nested below the given category */
	Catalog.GetProductsRecursive = function (categoryId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "categoryId", value: categoryId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetProductsRecursive', dataArray, success, error);
	};

	/* Get a ProductVariant by Id */
	Catalog.GetProductVariant = function (variantId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "variantId", value: variantId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetProductVariant', dataArray, success, error);
	};

	/* Get all prouct variants that are nested below the given category */
	Catalog.GetProductsVariantsRecursive = function (categoryId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "categoryId", value: categoryId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Catalog.GetProductsVariantsRecursive', dataArray, success, error);
	};
	
	// Customers API
	
	/* Get all customers */
	Customers.GetAllCustomers = function (success, error) {
		return uWebshopService.callBase('Customers.GetAllCustomers', null, success, error);
	};
	
	/* Get all customers from a certain group */
	Customers.GetCustomers = function (group, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "group", value: group }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Customers.GetCustomers', dataArray, success, error);
	};

	/* Get  customers who spend at least a certain amount (in cents) */
	Customers.GetCustomersBySpending = function (amountInCents, storeAlias, success, error) {
		var dataArray = [{ key: "amountInCents", value: amountInCents }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Customers.GetCustomersBySpending', dataArray, success, error);
	};
	
	/* Get Orders from Customer (by customer UserName) */
	Customers.GetOrders = function (userName, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "userName", value: userName }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Customers.GetOrders', dataArray, success, error);
	};
	
	/* Get Wishlist from Customer (by username) */
	Customers.GetWishlist = function (userName, wishlistName, storeAlias, success, error) {
		var dataArray = [{ key: "userName", value: userName }, { key: "wishlistName", value: wishlistName }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Customers.GetWishlist', dataArray, success, error);
	};
	
	/* Get all Wishlists from Customer (by username) */
	Customers.GetWishlist = function (wishlistName, storeAlias, success, error) {
		var dataArray = [{ key: "wishlistName", value: wishlistName }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Customers.GetWishlist', dataArray, success, error);
	};
	
	/* Get all Wishlists from Customer (by username) */
	Customers.GetWishlists = function (userName, storeAlias, success, error) {
		var dataArray = [{ key: "userName", value: userName }, { key: "storeAlias", value: storeAlias }];
		return uWebshopService.callBase('Customers.GetWishlists', dataArray, success, error);
	};

	/* Get the customer value from the current session */
	Customers.GetCustomerValueFromSession = function (fieldName, success, error) {
		var dataArray = [{ key: "fieldName", value: fieldName }];
		return uWebshopService.callBase('Customers.GetCustomerValueFromSession', dataArray, success, error);
	};
	
	/* Get the customer value from the current users profile  */
	Customers.GetCustomerValueFromProfile = function (fieldName, success, error) {
		var dataArray = [{ key: "fieldName", value: fieldName }];
		return uWebshopService.callBase('Customers.GetCustomerValueFromProfile', dataArray, success, error);
	};
	
	/* Get the customer value from the current session with fallback to users profile  */
	Customers.GetCustomerValueFromSessionOrProfile = function (fieldName, success, error) {
		var dataArray = [{ key: "fieldName", value: fieldName }];
		return uWebshopService.callBase('Customers.GetCustomerValueFromSessionOrProfile', dataArray, success, error);
	};
	
	/* Get the customer value from the current session, fallback to the current basket then fallback to users profile  */
	Customers.GetCustomerValueFromSessionOrBasketOrProfile = function (guidAsString, fieldName, success, error) {
		var dataArray = [{ key: "guidAsString", value: guidAsString }, { key: "fieldName", value: fieldName }];
		return uWebshopService.callBase('Customers.GetCustomerValueFromSessionOrBasketOrProfile', dataArray, success, error);
	};
	
	// Discounts API
	
	/* Get all order discounts */
	Discounts.GetAllOrderDiscounts = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Discounts.GetAllOrderDiscounts', dataArray, success, error);
	};

	/* Get order discounts for the given order */
	Discounts.GetDiscountsForOrder = function (orderGuid, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "orderGuid", value: orderGuid }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Discounts.GetDiscountsForOrder', dataArray, success, error);
	};
	
	Discounts.GetDiscountForProduct = function (productId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "productId", value: productId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Discounts.GetDiscountForProduct', dataArray, success, error);
	};
	
	Discounts.GetDiscountForProductVariant = function (variantId, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "variantId", value: variantId }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Discounts.GetDiscountForProductVariant', dataArray, success, error);
	};
	
	// Providers API
	
	/* Get All Payment Providers */
	Providers.GetAllPaymentProviders = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Providers.GetAllPaymentProviders', dataArray, success, error);
	};
	
	/* Get Payment Providers available for the given order*/
	Providers.GetPaymentProvidersForOrder = function (guidAsString, useZone, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "guidAsString", value: guidAsString }, { key: "useZone", value: useZone }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Providers.GetPaymentProvidersForOrder', dataArray, success, error);
	};

    /* Get Shipping/Fulfillment providers available for the given country*/
	Providers.GetPaymentProvidersForCountry = function (countryCode, storeAlias, currencyCode, success, error) {
	    var dataArray = [{ key: "countryCode", value: countryCode }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
	    return uWebshopService.callBase('Providers.GetPaymentProvidersForCountry', dataArray, success, error);
	};
	
	/* Get All Shipping/Fulfillment Providers */
	Providers.GetAllFulfillmentProviders = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Providers.GetAllFulfillmentProviders', dataArray, success, error);
	};
	
	/* Get Shipping/Fulfillment providers available for the given order*/
	Providers.GetFulfillmentProvidersForOrder = function (guidAsString, useZone, storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "guidAsString", value: guidAsString }, { key: "useZone", value: useZone }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Providers.GetFulfillmentProvidersForOrder', dataArray, success, error);
	};

    /* Get Shipping/Fulfillment providers available for the given country*/
	Providers.GetFulfillmentProvidersForCountry = function (countryCode, storeAlias, currencyCode, success, error) {
	    var dataArray = [{ key: "countryCode", value: countryCode }, { key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
	    return uWebshopService.callBase('Providers.GetFulfillmentProvidersForCountry', dataArray, success, error);
	};
	
	// Store API
	
	/* Get the current store */
	Store.GetStore = function (success, error) {
		return uWebshopService.callBase('Store.GetStore', null, success, error);
	};

	/* Get all stores */
	Store.GetAllStores = function (success, error) {
		return uWebshopService.callBase('Store.GetAllStores', null, success, error);
	};
	
	/* Get the currenct localization */
	Store.GetCurrentLocalization = function (success, error) {
		return uWebshopService.callBase('Store.GetCurrentLocalization', null, success, error);
	};
	
	/* Get all countries available for the given store */
	Store.GetAllCountries = function (storeAlias, currencyCode, success, error) {
		var dataArray = [{ key: "storeAlias", value: storeAlias }, { key: "currencyCode", value: currencyCode }];
		return uWebshopService.callBase('Store.GetAllCountries', dataArray, success, error);
	};
	
	/* Get a full country name from a country code */
	Store.GetCountryNameFromCountryCode = function (countryCode, success, error) {
		var dataArray = [{ key: "countryCode", value: countryCode }];
		return uWebshopService.callBase('Store.GetCountryNameFromCountryCode', dataArray, success, error);
	};
	
	/* Get a currencySymbol ($) from the ISOCurrencySymbol Code (USD) */
	Store.GetCurrencySymbol = function (ISOCurrencySymbol, success, error) {
		var dataArray = [{ key: "ISOCurrencySymbol", value: ISOCurrencySymbol }];
		return uWebshopService.callBase('Store.GetCurrencySymbol', dataArray, success, error);
	};
	
	/* UWEBSHOP PRIVATE SERVICE OBJECT */

	var uWebshopService = function () {

		uWebshopService.callBase = function (methodName, data, success, error) {
			$.ajax({
				type: "GET",
				url: uWebshopService.createUrl("JSON/" + methodName),
				data: JSON.stringify(data),
				async: true,
				processData: false,
				success: function (returnedData) {
					if (success) {
						success(returnedData);
					}
				},
				error: function (xmlHttpRequest, status, errorThrown) {
					if (error) {
						error(xmlHttpRequest, status, errorThrown);
					}
				},
				cache: false,
				contentType: "application/json; charset=utf-8",
				dataType: "json"
			});
		};

		uWebshopService.basketHandler = function (data, success, error) {
			$.ajax({
				type: "GET",
				url: uWebshopService.createUrl('Handle'),
				data: data,
				async: true,
				processData: false,
				success: function (returnedData) {
					if (success) {
						success(returnedData);
					}
				},
				error: function (xmlHttpRequest, status, errorThrown) {
					if (error) {
						error(xmlHttpRequest, status, errorThrown);
					}
				},
				cache: false,
				contentType: "application/json; charset=utf-8",
				dataType: "json"
			});
		};

		uWebshopService.createUrl = function (method, args) {
			var baseUrl = '/Base/uWebshopBase/' + method;
			var argList = "";
			if (args) {
				for (var i = 0; i < args.length; i++) {
					argList += '/' + args[i];
				}
			}
			return baseUrl;
		};
	};

	uwbsService = new uWebshopService();

})();