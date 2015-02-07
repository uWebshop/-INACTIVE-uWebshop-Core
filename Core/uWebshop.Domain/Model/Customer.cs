using System.Collections.Generic;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain
{
	/// <summary>
	///     Class representing a customer in Umbraco
	/// </summary>
	public class Customer : MemberBase
	{
		/// <summary>
		///     Construcor of the customer class
		/// </summary>
		/// <param name="email">E-mail address of the customer</param>
		public Customer(string email) : base(email)
		{
		}

		/// <summary>
		///     Construcor of the customer class
		/// </summary>
		/// <param name="id">NodeId of the customer</param>
		public Customer(int id) : base(id)
		{
		}

		private new CustomerProfile Profile
		{
			get { return (CustomerProfile) base.Profile; }
		}

		/// <summary>
		///     Gets or sets the first name of the customer
		/// </summary>
		public string FirstName
		{
			get
			{
				return Profile.FirstName;
				//return Profile["customerFirstName"].ToString();o
				//return Member.getProperty("customerFirstName").Value.ToString();
			}
			set
			{
				Profile.FirstName = value;
				//Member.getProperty("customerFirstName").Value = value;
			}
		}

		/// <summary>
		///     Gets or sets the last name of the customer
		/// </summary>
		public string LastName
		{
			get
			{
				return Profile.LastName;
				//return Profile["customerLastName"].ToString();
				//return Member.getProperty("customerLastName").Value.ToString();
			}
			set
			{
				Profile.LastName = value;
				//Member.getProperty("customerLastName").Value = value;
			}
		}

		/// <summary>
		///     Gets the full name of the customer
		/// </summary>
		public string FullName
		{
			get { return string.Format("{0} {1}", FirstName, LastName); }
		}

		/// <summary>
		///     Gets or sets the country of the customer
		/// </summary>
		public string Country
		{
			get
			{
				return Profile.Country;
				//return Profile["customerCountry"].ToString();
				//return Member.getProperty("customerCountry").Value.ToString();
			}
			set
			{
				Profile.Country = value;
				//Member.getProperty("customerCountry").Value = value;
			}
		}

		/// <summary>
		///     Gets or sets the region of the customer
		/// </summary>
		public string Region
		{
			get
			{
				return Profile.Region;
				//return Profile["customerCountry"].ToString();
				//return Member.getProperty("customerCountry").Value.ToString();
			}
			set
			{
				Profile.Region = value;
				//Member.getProperty("customerCountry").Value = value;
			}
		}

		/// <summary>
		///     Gets or sets the e-mail of the customer
		/// </summary>
		public string Email
		{
			get { return Member.Email; }
			set { Member.Email = value; }
		}

		/// <summary>
		///     Gets or sets the e-mail address of the customer
		/// </summary>
		public string EmailAddress
		{
			get
			{
				return Profile.Email;
				//return Member.getProperty("customerEmail").Value.ToString();
			}
			set
			{
				Profile.Email = value;
				//Member.getProperty("customerEmail").Value = value;
			}
		}

		/// <summary>
		///     Is Customer VAT Valid?
		/// </summary>
		public bool ValidVAT
		{
			get { return Profile.ValidVAT; }
			set { Profile.ValidVAT = value; }
		}

		/// <summary>
		///     VAT Number
		/// </summary>
		public string VATNumber
		{
			get { return Profile.VATNumber; }
			set { Profile.VATNumber = value; }
		}

		/// <summary>
		///     Gets a list of orders related to the customer
		/// </summary>
		public IEnumerable<OrderInfo> Orders
		{
			get { return OrderHelper.GetOrdersForCustomer(Id); }
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
			Profile.Save();
		}
	}
}