using System;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	internal sealed class SimplePrice : IDiscountedRangedPrice, IPrice
	{
		internal enum YesNoDifference
		{
			Yes, No, Difference
		}
		private readonly IAmountUnit _source;
		private readonly ILocalization _localization;
		private YesNoDifference? _vat;
		private YesNoDifference _discount;
		private bool _ranged;
		public SimplePrice(IAmountUnit source, ILocalization localization)
		{
			if (localization == null) throw new ArgumentNullException("lo");
			//_ranged = true;
			_source = source;
			_localization = localization;
			_discount = YesNoDifference.Yes;
		}

		public SimplePrice(SimplePrice previous)
		{
			_localization = previous._localization;
			_source = previous._source;
			_vat = previous._vat;
			_discount = previous._discount;
			_ranged = previous._ranged;
		}

		public decimal Value { get { return ValueInCents/100m; }}

		public int ValueInCents
		{
			get
			{
				if (!_vat.HasValue)
				{
					_vat = IO.Container.Resolve<ISettingsService>().IncludingVat ? YesNoDifference.Yes : YesNoDifference.No;
				}
				return Amount(_discount, _vat.Value, _source, _ranged);
			}
		}

		internal int Amount(YesNoDifference discount, YesNoDifference vatVal, IAmountUnit source, bool ranged)
		{
			if (discount == YesNoDifference.Difference)
			{
				var inclVat = vatVal != YesNoDifference.No;
				return source.GetAmount(inclVat, false, ranged) - _source.GetAmount(inclVat, true, ranged);
			}

			var discounted = discount == YesNoDifference.Yes;
			if (vatVal == YesNoDifference.Difference)
			{
				return source.GetAmount(true, discounted, ranged) - _source.GetAmount(false, discounted, ranged);
			}

			var vat = vatVal == YesNoDifference.Yes;

			return source.GetAmount(vat, discounted, ranged);
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

		public IRangedPrice BeforeDiscount
		{
			get
			{
				var price = new SimplePrice(this);
				price._discount = YesNoDifference.No;
				return price;
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
				var price = new SimplePrice(this);
				price._discount = YesNoDifference.Difference;
				return price;
			}
		}

		IVatPrice IRangedPrice.Ranged
		{
			get { return Ranged; }
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

		public IPrice WithoutVat
		{
			get
			{
				var price = new SimplePrice(this);
				price._vat = YesNoDifference.No;
				return price;
			}
		}

		public IPrice Vat
		{
			get
			{
				var price = new SimplePrice(this);
				price._vat = YesNoDifference.Difference;
				return price;
			}
		}
	}
}