using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace VATChecker
{
	/// <summary>
	///     This class is a wrapper for the checkVAT webservice proxy which uses the synchron version
	///     of the check VAT method. The class adds a U character for Austrian VAT numbers if missing
	/// </summary>
	/// <remarks>
	///     This code was implemented by MasterSoft Software Solutions Ltd.
	///     Visit the <see href="http://www.mastersoft.at">MasterSoft website</see> for
	///     further information.
	///     <para>Dev: BGH -- Mag. (FH) Christian Kleinheinz</para>
	/// </remarks>
	public class ViesVatCheckService : IVATCheckService
	{
		/// <summary>
		///     The VAT number to check for
		/// </summary>
		public string VATNumber { get; set; }

		/// <summary>
		///     The country code of the uid to check
		/// </summary>
		/// <remarks>
		///     This parameter can be one of these country codes
		///     country --> code to use
		///     ************************************************
		///     Austria --> AT
		///     Belgium --> BE
		///     Bulgaria --> BG
		///     Cyprus --> CY
		///     Czech Republic --> CZ
		///     Germany --> DE
		///     Denmark --> DK
		///     Estonia EE
		///     Greece EL
		///     Spain ES
		///     Finland FI
		///     France FR
		///     United Kingdom GB
		///     Hungary HU
		///     Ireland IE
		///     Italy IT
		///     Lithuania LT
		///     Luxembourg LU
		///     Malta MT
		///     The Netherlands NL
		///     Poland PL
		///     Portugal PT
		///     Romania RO
		///     Sweden SE
		///     Slovenia SI
		///     Slovakia SK
		/// </remarks>
		public string CountryCode { get; set; }

		/// <summary>
		/// Gets or sets the address.
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [is valid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is valid]; otherwise, <c>false</c>.
		/// </value>
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the ret date.
		/// </summary>
		/// <value>
		/// The ret date.
		/// </value>
		public DateTime RetDate { get; set; }

		/// <summary>
		///     Check if a VAT number is valid
		/// </summary>
		/// <param name="number"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public bool VATNumberValid(string number, OrderInfo order)
		{
			ExtractCountryCodeFromVatNumber(number);

			GetCountryCodeFromOrderIfNotIncludedInVatNumber(order);

			try
			{
				return CheckVat();
			}
			catch (Exception)
			{
				Log.Instance.LogError("VAT check failed on external service");
				return false;
			}
		}

		/// <summary>
		///     Check if the VAT number is valid or not
		/// </summary>
		/// <returns>True if the VAT number could be validated otherwise false</returns>
		public bool CheckVat()
		{
			if (string.IsNullOrEmpty(VATNumber) || string.IsNullOrEmpty(CountryCode))
			{
				Log.Instance.LogDebug("CheckVat() FALSE: string.IsNullOrEmpty(VATNumber) || string.IsNullOrEmpty(CountryCode)");
				return (false);
			}

			//If the country code is AT for Austria we need a U before the UID
			if (CountryCode.ToUpper().Equals("AT"))
			{
				if (!VATNumber.StartsWith("U"))
					VATNumber = "U" + VATNumber;
			}

			string strVat = VATNumber;
			string strCountry = CountryCode;

			try
			{
				var visService = new checkVatService();

				bool bValid;
				string strName;
				string strAddress;
				RetDate = visService.checkVat(ref strCountry, ref strVat, out bValid, out strName, out strAddress);
				IsValid = bValid;
				Name = strName;
				Address = strAddress;

				Log.Instance.LogDebug("CheckVat() IsValid: " + IsValid);
				return (IsValid);
			}
			catch (Exception err)
			{
				Trace.TraceError(err.ToString());
				Log.Instance.LogDebug("CheckVat() FALSE: EXCEPTION");
				return (false);
			}
		}

		/// <summary>
		/// Gets the country code from order difference not included information vat number.
		/// </summary>
		/// <param name="order">The order.</param>
		public void GetCountryCodeFromOrderIfNotIncludedInVatNumber(OrderInfo order)
		{
			if (!string.IsNullOrWhiteSpace(CountryCode)) return;

			string customerCountry = order.CustomerCountry;
			if (customerCountry.ToUpper() == "GR")
			{
				customerCountry = "EL";
			}
			CountryCode = customerCountry;
		}

		/// <summary>
		/// Extracts the country code from vat number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void ExtractCountryCodeFromVatNumber(string number)
		{
			// regex to check if string stats with alphanummeric: ^[A-Za-z]{2}
			// if match, then remove the countrycode before the vat number.
			number = number.Trim().Replace(" ", string.Empty);

			if (Regex.IsMatch(number, "^[A-Za-z]{2}"))
			{
				VATNumber = number.Remove(0, 2);
				CountryCode = number.Remove(2);
			}
			else
			{
				VATNumber = number;
			}
		}
	}
}