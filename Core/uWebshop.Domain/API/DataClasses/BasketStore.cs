using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Namespace = "")]
	internal class BasketStore : IStore
	{
		private readonly Domain.Store _store;

		public BasketStore(Domain.Store store)
		{
			_store = store;
			Id = store.Id;
            Key = store.Key;
			Alias = store.Alias;
			TypeAlias = store.TypeAlias;
		}

		public BasketStore(StoreInfo info) : this(info.Store)
		{
			Alias = info.Alias;
		}

		[DataMember]
		public int Id { get; set; }

        [DataMember]
        public Guid Key { get; set; }

        [DataMember]
		public string TypeAlias { get; private set; }

		[DataMember]
		public string Culture
		{
			get { return _store.Culture; }
		}

		[DataMember]
		public string Alias { get; set; }

		[DataMember]
		public string CountryCode
		{
			get { return _store.CountryCode; }
		}

		[DataMember]
		public string DefaultCountryCode
		{
			get { return _store.DefaultCountryCode; }
		}

		[DataMember]
		public string CurrencyCulture
		{
			get { return _store.CountryCode; }
		}

		[IgnoreDataMember]
		public IEnumerable<ICurrency> Currencies
		{
			get { return _store.Currencies; }
		}

		[IgnoreDataMember]
		public CultureInfo DefaultCurrencyCultureInfo
		{
			get { return _store.DefaultCurrencyCultureInfo; }
		}

		[DataMember]
		public string DefaultCurrencyCultureSymbol
		{
			get { return _store.DefaultCurrencyCultureSymbol; }
		}

		[IgnoreDataMember]
		public CultureInfo CultureInfo
		{
			get { return _store.CultureInfo; }
		}

		[DataMember]
		public decimal GlobalVat
		{
			get { return _store.GlobalVat; }
		}

		[DataMember]
		public bool Testmode
		{
			get { return _store.Testmode; }
		}

		[DataMember]
		public string StoreUrlWithoutDomain
		{
			get { return _store.StoreUrlWithoutDomain; }
		}

		[DataMember]
		public string EmailAddressFrom
		{
			get { return _store.EmailAddressFrom; }
		}

		[DataMember]
		public string EmailAddressFromName
		{
			get { return _store.EmailAddressFromName; }
		}

		[DataMember]
		public string EmailAddressTo
		{
			get { return _store.EmailAddressTo; }
		}

		[DataMember]
		public string AccountChangePasswordUrl
		{
			get { return _store.AccountChangePasswordUrl; }
		}

		[DataMember]
		public string StoreURL
		{
			get { return _store.StoreURL; }
		}

		public IEnumerable<int> GetConnectedNodes
		{
			get { return _store.GetConnectedNodes; }
		}

		[IgnoreDataMember]
		public bool Disabled
		{
			get { return _store.Disabled; }
		}

		[IgnoreDataMember]
		public DateTime CreateDate
		{
			get { return _store.CreateDate; }
		}

		[IgnoreDataMember]
		public DateTime UpdateDate
		{
			get { return _store.UpdateDate; }
		}

		[DataMember]
		public int SortOrder
		{
			get { return _store.SortOrder; }
		}
	}
}