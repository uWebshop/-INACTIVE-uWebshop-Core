using System;
using System.Web.Profile;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class CustomerProfile : ProfileBase
	{
		/// <summary>
		///     Customer first name (alias: customerFirstName)
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string FirstName
		{
			get
			{
				object o = base.GetPropertyValue("customerFirstName");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerFirstName", value); }
		}

		/// <summary>
		///     customer last name (alias: customerLastName)
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string LastName
		{
			get
			{
				object o = base.GetPropertyValue("customerLastName");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerLastName", value); }
		}

		/// <summary>
		///     customer country (alias: customerCountry)
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string Country
		{
			get
			{
				object o = base.GetPropertyValue("customerCountry");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerCountry", value); }
		}

		/// <summary>
		///     customer region (alias: customerRegion)
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string Region
		{
			get
			{
				object o = base.GetPropertyValue("customerRegion");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerRegion", value); }
		}

		/// <summary>
		///     customer email (alias: customerEmail)
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string Email
		{
			get
			{
				object o = base.GetPropertyValue("customerEmail");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerEmail", value); }
		}

		/// <summary>
		///     VAT Number
		/// </summary>
		[SettingsAllowAnonymous(false)]
		public string VATNumber
		{
			get
			{
				object o = base.GetPropertyValue("customerVAT");

				if (o == DBNull.Value)
				{
					return string.Empty;
				}

				return (string) o;
			}
			set { base.SetPropertyValue("customerVAT", value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [valid vat].
		/// </summary>
		/// <value>
		///   <c>true</c> if [valid vat]; otherwise, <c>false</c>.
		/// </value>
		public bool ValidVAT
		{
			get
			{
				object o = base.GetPropertyValue("customerValidVAT");

				if (o == DBNull.Value)
				{
					return false;
				}

				return (bool) o;
			}
			set { base.SetPropertyValue("customerValidVAT", value); }
		}
	}
}