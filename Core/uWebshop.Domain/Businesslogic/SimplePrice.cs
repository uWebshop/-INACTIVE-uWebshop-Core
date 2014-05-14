using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	internal sealed class SimplePrice : IDiscountedRangedPrice, IPrice
	{
		private enum YesNoDifference
		{
			Yes, No, Difference
		}
		private readonly IAmountUnit _source;
		private readonly ILocalization _localization;
		private YesNoDifference _vat;
		private YesNoDifference _discount;
		private bool _ranged;
		public SimplePrice(IAmountUnit source, ILocalization localization)
		{
			//_ranged = true;
			_source = source;
			_localization = localization;
			_discount = YesNoDifference.Yes;
		}

		public SimplePrice(SimplePrice previous)
		{
			_source = previous._source;
			_vat = previous._vat;
			_discount = previous._discount;
			_ranged = previous._ranged;
		}

		public IPrice WithVat
		{
			get
			{
				var price = new SimplePrice(this);
				price._vat = YesNoDifference.Yes;
				return price;
			}
		}

		public IPrice WithoutVat { get
		{
			var price = new SimplePrice(this);
			price._vat = YesNoDifference.No;
			return price;
		} }
		public IPrice Vat { get
		{
			var price = new SimplePrice(this);
			price._vat = YesNoDifference.Difference;
			return price;
		} }

		public decimal Value { get { return ValueInCents/100m; }}

		public int ValueInCents
		{
			get
			{
				if (_discount == YesNoDifference.Difference)
				{
					var inclVat = _vat != YesNoDifference.No;
					return _source.GetAmount(inclVat, false, _ranged) - _source.GetAmount(inclVat, true, _ranged);
				}

				var discounted = _discount == YesNoDifference.Yes;
				if (_vat == YesNoDifference.Difference)
				{
					return _source.GetAmount(true, discounted, _ranged) - _source.GetAmount(false, discounted, _ranged);
				}

				var vat = _vat == YesNoDifference.Yes;

				return _source.GetAmount(vat, discounted, _ranged);
			}
		}

		public string ToCurrencyString()
		{
			return (ValueInCents / 100m).ToString("C", StoreHelper.GetCurrencyCulture(_localization));
		}

		public IDiscountedPrice Ranged
		{
			get
			{
				var price = new SimplePrice(this);
				price._ranged = true;
				return price;
			}
		}

		public IRangedPrice BeforeDiscount { get
		{
			var price = new SimplePrice(this);
			price._discount = YesNoDifference.No;
			return price;
		}}

		IVatPrice IDiscountedPrice.BeforeDiscount
		{
			get { return BeforeDiscount; }
		}

		public IVatPrice Discount { get
		{
			var price = new SimplePrice(this);
			price._discount = YesNoDifference.Difference;
			return price;
		} }
		IVatPrice IRangedPrice.Ranged
		{
			get { return Ranged; }
		}
	}
}