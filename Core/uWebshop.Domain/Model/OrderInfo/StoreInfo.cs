using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class StoreInfo
	{
		/// <summary>
		/// Gets or sets the language code.
		/// </summary>
		/// <value>
		/// The language code.
		/// </value>
		[DataMember]
		public string LanguageCode { get; set; }

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		[DataMember]
		public string Alias { get; set; }

		/// <summary>
		/// Gets or sets the country code.
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		[DataMember]
		public string CountryCode { get; set; }

		/// <summary>
		/// Gets or sets the culture.
		/// </summary>
		/// <value>
		/// The culture.
		/// </value>
		[DataMember]
		public string Culture { get; set; }

		/// <summary>
		/// Gets or sets the currency culture.
		/// </summary>
		/// <value>
		/// The currency culture.
		/// </value>
		[DataMember]
		public string CurrencyCulture { get; set; }

		/// <summary>
		///     Returns the CultureInfo based on the CurrencyCulture
		/// </summary>
		[XmlIgnore]
		public CultureInfo CurrencyCultureInfo
		{
			get { return CurrencyCulture != null ? new CultureInfo(CurrencyCulture) : new CultureInfo("en-US"); }
		}

		/// <summary>
		///     store Culture
		/// </summary>
		[XmlIgnore]
		public CultureInfo CultureInfo
		{
			get { return Culture != null ? new CultureInfo(Culture) : new CultureInfo("en-US"); }
		}

		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <value>
		/// The store.
		/// </value>
		[XmlIgnore]
		public Store Store
		{
			get
			{
				var byAlias = StoreHelper.GetByAlias(Alias);
				return byAlias ?? new Store { Alias = Alias, CurrencyCulture = CurrencyCulture, Culture = Culture, CountryCode = CountryCode, };
			}
		}
	}
}