using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	internal class SummedPrice : IDiscountedRangedPrice, IPrice
	{
		// todo: adjust for SimplePrice when all Price properties have been altered to that
		private readonly IVatPrice[] _prices;
		private readonly bool _originalPricesIncludingVat;
		private readonly decimal _vat;
		private readonly ILocalization _localization;
		private readonly IVatCalculationStrategy _vatCalculationStrategy;
		private int? _valueInCents;

		internal SummedPrice(IEnumerable<IVatPrice> prices, bool includingVat, decimal vat, ILocalization localization, IVatCalculationStrategy vatCalculationStrategy)
		{
			_prices = prices.ToArray();
			_originalPricesIncludingVat = includingVat;
			_vat = vat;
			_localization = localization;
			_vatCalculationStrategy = vatCalculationStrategy;
		}

		public IPrice WithVat
		{
			get
			{
				var originalTotal = _prices.Sum(p => p.ValueInCents()) * Factor();
				var withVatTotal = _prices.Sum(p => p.WithVat.ValueInCents) * Factor();
				ValueInCents = _vatCalculationStrategy.WithVat(_originalPricesIncludingVat, originalTotal, _vat, withVatTotal);
				return this;
			}
		}

		public IPrice WithoutVat
		{
			get
			{
				var originalTotal = _prices.Sum(p => p.ValueInCents()) * Factor();
				var withoutVatTotal = _prices.Sum(p => p.WithoutVat.ValueInCents) * Factor();
				ValueInCents = _vatCalculationStrategy.WithoutVat(_originalPricesIncludingVat, originalTotal, _vat, withoutVatTotal);
				return this;
			}
		}

		public IPrice Vat
		{
			get
			{
				var originalTotal = _prices.Sum(p => p.ValueInCents()) * Factor();
				var vatTotal = _prices.Sum(p => p.Vat.ValueInCents) * Factor();
				ValueInCents = _vatCalculationStrategy.Vat(_originalPricesIncludingVat, originalTotal, _vat, vatTotal);
				return this;
			}
		}

		public IDiscountedPrice Ranged
		{
			get
			{
				for (int i = 0; i < _prices.Length; i++)
				{
					var price = _prices[i] as IRangedPrice;
					if (price != null)
					{
						_prices[i] = price.Ranged;
					}
				}
				return this;
			}
		}

		public IRangedPrice BeforeDiscount
		{
			get
			{
				for (int i = 0; i < _prices.Length; i++)
				{
					var price = _prices[i] as IDiscountedPrice;
					if (price != null)
					{
						_prices[i] = price.BeforeDiscount;
					}
				}
				return this;
			}
		}

		public IVatPrice Discount
		{
			get
			{
				for (int i = 0; i < _prices.Length; i++)
				{
					var price = _prices[i] as IDiscountedPrice;
					if (price != null)
					{
						_prices[i] = price.Discount;
					}
				}
				return this;
			}
		}

		IVatPrice IDiscountedPrice.BeforeDiscount { get { return BeforeDiscount; } }
		IVatPrice IRangedPrice.Ranged { get { return Ranged; } }
		public decimal Value { get { return ValueInCents / 100m; } }
		public string ToCurrencyString()
		{
			return (ValueInCents / 100m).ToString("C", StoreHelper.GetCurrencyCulture(_localization));
		}

		public int ValueInCents
		{
			get
			{
				if (!_valueInCents.HasValue)
				{
					_valueInCents = _prices.Sum(p => p.ValueInCents()) * Factor();
				}
				return _valueInCents.GetValueOrDefault();
			}
			private set { _valueInCents = value; }
		}

		protected virtual int Factor()
		{
			return 1;
		}
	}

	internal class PriceOrderline : SummedPrice
	{
		private readonly int _lineItemCount;

		public PriceOrderline(IDiscountedRangedPrice price, bool includingVat, decimal vat, ILocalization localization
			   , IVatCalculationStrategy vatCalculationStrategy, int lineItemCount)
			: base(new[] { price }, includingVat, vat, localization, vatCalculationStrategy)
		{
			_lineItemCount = lineItemCount;
		}

		protected override int Factor()
		{
			return _lineItemCount;
		}
	}

	internal sealed class Price : IDiscountedRangedPrice, IPrice
	{
		// todo: replace any usages by SimplePrice
		private readonly int _originalPrice;
		private readonly IEnumerable<Range> _ranges;
		private readonly Func<IOrderInfo, int> _orderCountCalculation;
		private readonly bool _originalPricesIncludingVat;
		private readonly decimal _vat;
		private readonly ILocalization _localization;
		private readonly Func<int, int> _originalApplyDiscount;

		private int _currentOriginalPrice;
		private Func<int, int> _applyDiscount;
		private Func<int, int, int> _applyDiscountRanged;
		private Func<int, int> _calculateVat;
		private bool _ranged;

		internal static IPrice CreateBasicPrice(int price, ILocalization localization)
		{
			return new Price(price, localization);
		}
		internal static IVatPrice CreateSimplePrice(int price, bool includingVat, decimal vat, ILocalization localization)
		{
			return new Price(price, includingVat, vat, localization);
		}
		internal static IDiscountedRangedPrice CreateDiscountedRanged(int price, IEnumerable<Range> ranges, bool includingVat, decimal vat, Func<IOrderInfo, int> orderCountCalculation, Func<int, int> applyDiscount, ILocalization localization)
		{
			return new Price(price, ranges, includingVat, vat, orderCountCalculation, applyDiscount, localization);
		}
		internal static IDiscountedRangedPrice CreateDiscountedRanged(int price, IEnumerable<Range> ranges, bool includingVat, decimal vat, Func<IOrderInfo, int> orderCountCalculation, Func<int, int, int> applyDiscount, ILocalization localization)
		{
			return new Price(price, ranges, includingVat, vat, orderCountCalculation, applyDiscount, localization);
		}

		private Price(int price, bool includingVat, decimal vat, ILocalization localization)
			: this(price, localization)
		{
			_originalPrice = price;
			_originalPricesIncludingVat = includingVat;
			_vat = vat;
		}

		private Price(int price, IEnumerable<Range> ranges, bool includingVat, decimal vat, Func<IOrderInfo, int> orderCountCalculation, Func<int, int> applyDiscount, ILocalization localization)
			: this(price, includingVat, vat, localization)
		{
			if (ranges != null && ranges.Any())
			{
				var defaultRange = ranges.GetRangeAmountForValue(1);
				if (defaultRange != null)
				{
					_currentOriginalPrice = defaultRange.Value;
				}
			}
			_applyDiscount = applyDiscount;
			_originalApplyDiscount = applyDiscount;
			_ranges = ranges;
			_orderCountCalculation = orderCountCalculation;
		}
		private Price(int price, IEnumerable<Range> ranges, bool includingVat, decimal vat, Func<IOrderInfo, int> orderCountCalculation, Func<int, int, int> applyDiscount, ILocalization localization)
			: this(price, includingVat, vat, localization)
		{
			if (ranges != null)
			{
				var defaultRange = ranges.GetRangeAmountForValue(1);
				if (defaultRange != null)
				{
					_currentOriginalPrice = defaultRange.Value;
				}
			}
			_applyDiscountRanged = applyDiscount;
			_applyDiscount = i => _applyDiscountRanged(i, 1);
			_ranges = ranges;
			_orderCountCalculation = orderCountCalculation;
		}

		private Price(int price, ILocalization localization)
		{
			_localization = localization;

			_calculateVat = Nop;
			_currentOriginalPrice = price;
			_applyDiscount = Nop;
			_applyDiscountRanged = NopRanged;
			_ranged = false;
		}

		public int ValueInCents
		{
			get
			{
				var valueInCents = _calculateVat(_applyDiscount(_currentOriginalPrice));
				_applyDiscount = _originalApplyDiscount ?? _applyDiscount;
				return valueInCents;
			}
		}

		public IPrice WithVat
		{
			get
			{
				_calculateVat = _originalPricesIncludingVat ? (Func<int, int>)Nop : (i => VatCalculator.WithVat(i, _vat));
				return this;
			}
		}

		public IPrice WithoutVat
		{
			get
			{
				_calculateVat = _originalPricesIncludingVat ? (i => VatCalculator.WithoutVat(i, _vat)) : (Func<int, int>)Nop;
				return this;
			}
		}

		public IPrice Vat
		{
			get
			{
				_calculateVat = _originalPricesIncludingVat ? i => VatCalculator.VatAmountFromWithVat(i, _vat) : (Func<int, int>)(i => VatCalculator.VatAmountFromWithoutVat(i, _vat));
				return this;
			}
		}

		public IDiscountedPrice Ranged
		{
			get
			{
				_ranged = true;
				_currentOriginalPrice = AdjustPriceAccordingToRanges();
				return this;
			}
		}
		public IRangedPrice BeforeDiscount
		{
			get
			{
				_applyDiscountRanged = NopRanged;
				_applyDiscount = Nop;
				return this;
			}
		}

		public IVatPrice Discount
		{
			get
			{
				_currentOriginalPrice = _currentOriginalPrice - _applyDiscount(_currentOriginalPrice);
				_applyDiscountRanged = NopRanged;
				_applyDiscount = Nop;
				return this;
			}
		}

		private int AdjustPriceAccordingToRanges()
		{
			var order = OrderHelper.GetOrder(); // todo questionable, what of the Info's? => implemented correctly in SimplePrice
			if (order != null && _orderCountCalculation != null)
			{
				var totalCount = _orderCountCalculation(order);
				if (_applyDiscount != Nop)
				{
					_applyDiscount = i => _applyDiscountRanged(i, totalCount);
				}

				var rangedPrice = _ranges.GetRangeAmountForValue(totalCount) ?? ValueInCents;
				return rangedPrice;
			}

			return ValueInCents;
		}

		IVatPrice IDiscountedPrice.BeforeDiscount { get { return BeforeDiscount; } }
		IVatPrice IRangedPrice.Ranged { get { return Ranged; } }

		public decimal Value { get { return ValueInCents / 100m; } }
		public string ToCurrencyString()
		{
			return (ValueInCents / 100m).ToString("C", StoreHelper.GetCurrencyCulture(_localization));
		}
		private static int Nop(int i) { return i; }
		private static int NopRanged(int i, int j) { return i; }

	}
	internal class CombiPrice : IDiscountedRangedPrice, IPrice
	{
		private readonly Price _price1;
		private readonly Price _price2;
		private readonly ILocalization _localization;

		public CombiPrice(Price price1, Price price2, ILocalization localization)
		{
			_price1 = price1;
			_price2 = price2;
			_localization = localization;
		}

		public int ValueInCents
		{
			get { return _price1.ValueInCents + _price2.ValueInCents; }
		}
		public decimal Value { get { return ValueInCents / 100m; } }
		public string ToCurrencyString()
		{
			return (ValueInCents / 100m).ToString("C", StoreHelper.GetCurrencyCulture(_localization));
		}

		public IPrice WithVat
		{
			get
			{
				var a = _price1.WithVat;
				var b = _price2.WithVat;
				return this;
			}
		}

		public IPrice WithoutVat
		{
			get
			{
				var a = _price1.WithoutVat;
				var b = _price2.WithoutVat;
				return this;
			}
		}
		public IPrice Vat
		{
			get
			{
				var a = _price1.Vat;
				var b = _price2.Vat;
				return this;
			}
		}
		public IDiscountedPrice Ranged
		{
			get
			{
				var a = _price1.Ranged;
				var b = _price2.Ranged;
				return this;
			}
		}
		public IRangedPrice BeforeDiscount
		{
			get
			{
				var a = _price1.BeforeDiscount;
				var b = _price2.BeforeDiscount;
				return this;
			}
		}

		IVatPrice IDiscountedPrice.BeforeDiscount
		{
			get { return BeforeDiscount; }
		}

		public IVatPrice Discount
		{
			get
			{
				var a = _price1.Discount;
				var b = _price2.Discount;
				return this;
			}
		}

		IVatPrice IRangedPrice.Ranged
		{
			get { return Ranged; }
		}
	}
}