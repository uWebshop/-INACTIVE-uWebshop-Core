using System;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoOrderDiscountRepository : UmbracoMultiStoreEntityRepository<IOrderDiscount, DiscountOrder>, IOrderDiscountRepository
	{
		private readonly IDiscountOrderAliassesService _aliasses;
		private readonly IStoreService _storeService;

		public UmbracoOrderDiscountRepository(IDiscountOrderAliassesService discountOrderAliassesService,IStoreService storeService)
		{
			_aliasses = discountOrderAliassesService;
			_storeService = storeService;
		}

		public override void LoadDataFromPropertiesDictionary(DiscountOrder discount, IPropertyProvider fields, ILocalization localization)
		{
			discount.Localization = localization;
			LoadBaseProperties(discount, fields, localization, _storeService);

			discount.RequiredItemIds = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.items, localization, fields).Split(',').Select(id => Common.Helpers.ParseInt(id)).Distinct().Where(id => id > 0).ToList();
			discount.AffectedOrderlines = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.affectedOrderlines, localization, fields).Split(',').Select(id => Common.Helpers.ParseInt(id)).Distinct().Where(id => id > 0).ToList();
			
			var tagsValue = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.affectedTags, localization, fields);
			discount.AffectedProductTags = InternalHelpers.ParseTagsString(tagsValue);

            DiscountOrderCondition condition;
			var discountOrderConditionParseResult = Enum.TryParse(StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.orderCondition, localization, fields), out condition);
			discount.Condition = discountOrderConditionParseResult ? condition : DiscountOrderCondition.None;

			discount.NumberOfItemsCondition = StoreHelper.GetMultiStoreIntValue(_aliasses.numberOfItemsCondition, localization, fields);

			discount.MinimumOrderAmountInCents = StoreHelper.GetMultiStoreIntValue(_aliasses.minimumAmount, localization, fields);

			var shippingDiscountable = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.shippingDiscountable, localization, fields).ToLower();
			discount.IncludeShippingInOrderDiscountableAmount = shippingDiscountable == "enable" || shippingDiscountable == "1" || shippingDiscountable == "true";

			// for backwards compatibility
			discount.CouponCode = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("couponCode", localization, fields);

			var oncePerCustomer = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.oncePerCustomer, localization, fields).ToLower();
			discount.OncePerCustomer = oncePerCustomer == "enable" || oncePerCustomer == "1" || oncePerCustomer == "true";
		}

		public static void LoadBaseProperties(Domain.BaseClasses.DiscountBase discount, IPropertyProvider fields, ILocalization localization, IStoreService storeService)
		{
			if (storeService == null) throw new Exception("StoreService missing");
			if (localization == null) throw new Exception("Loading data without localization");
			if (fields == null) throw new Exception("Loading data without source");
			if (discount == null) throw new Exception("Loading data without something to load it on");
			
			var cmsApplication = IO.Container.Resolve<ICMSApplication>();
			if (cmsApplication == null) throw new Exception("CMS missing");

			discount.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("title", localization, fields);
			discount.Description = cmsApplication.ParseInternalLinks(
				StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("description", localization, fields));
			discount.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

			discount.RangesString = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("ranges", localization, fields);
			discount.Ranges = StoreHelper.LocalizeRanges(Range.CreateFromString(discount.RangesString), localization);

			DiscountType type;
			var discountTypeParseResult = Enum.TryParse(StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("discountType", localization, fields), out type);
			discount.DiscountType = discountTypeParseResult ? type : DiscountType.Amount;

			discount.DiscountValue = StoreHelper.GetMultiStoreIntValue("discount", localization, fields);
			if (discount.Ranges != null && discount.Ranges.Any())
			{
				var range = discount.Ranges.FirstOrDefault(x => x.From <= 1 && x.PriceInCents != 0);
				if (range != null)
				{
					discount.DiscountValue = range.PriceInCents;
				}
			}
			if (discount.DiscountType != DiscountType.Percentage)
			{
				discount.DiscountValue = StoreHelper.LocalizePrice(discount.DiscountValue, localization);
			}
			
			var counterEnabled = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("countdownEnabled", localization, fields).ToLower();
			if (counterEnabled == "default")
			{
				var store = storeService.GetByAlias(localization.StoreAlias);

				if (store != null)
				{
					discount.CounterEnabled = store.UseCountdown;
				}

			}
			else if (counterEnabled == string.Empty)
			{
				discount.CounterEnabled = false;
			}
			else
			{
				discount.CounterEnabled = counterEnabled == "enable" || counterEnabled == "1" || counterEnabled == "true";
			}

			discount.MemberGroups = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("memberGroups", localization, fields).Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		public override string TypeAlias
		{
			get { return DiscountOrder.NodeAlias; }
		}
	}
}