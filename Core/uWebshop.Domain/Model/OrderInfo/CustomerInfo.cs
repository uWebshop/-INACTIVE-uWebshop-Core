using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class CustomerInfo
	{
		[IgnoreDataMember][XmlIgnore][ScriptIgnore]
		internal OrderInfo Order;

		/// <summary>
		///     VAT number of the customer
		/// </summary>
		[DataMember] public string VATNumber;

		private string _shippingCountryCode;
		private string _countryCode;
		[DataMember]
		public string CustomerIPAddress;

		/// <summary>
		///     Umbraco Member Id of the customer
		/// </summary>
		[DataMember]
		public int CustomerId { get; set; }

		/// <summary>
		/// Does this customer accepts marketing?
		/// </summary>
		[DataMember]
		public bool AcceptsMarketing { get; set; }

		/// <summary>
		///     Loginname of the customer (works also when not making use of umbraco members)
		/// </summary>
		[DataMember]
		public string LoginName { get; set; }

		/// <summary>
		///     Countrycode of the customer
		/// </summary>
		[DataMember]
		public string CountryCode
		{
			get { return _countryCode; }
			set
			{
				_countryCode = value;
				if (Order != null)
				{
					Order.ResetCachedValues();
				}
			}
		}

		/// <summary>
		///     CountryName based on countrycode
		/// </summary>
		[DataMember]
		public string CountryName
		{
			get
			{
				if (!string.IsNullOrEmpty(CountryCode))
				{
					Country country = StoreHelper.GetAllCountries().FirstOrDefault(x => x.Code == CountryCode);

					return country != null ? country.Name : string.Empty;
				}

				return string.Empty;
			}
			set { }
		}

		/// <summary>
		///     Countrycode of the country to ship to
		/// </summary>
		[DataMember]
		public string ShippingCountryCode
		{
			get { return _shippingCountryCode ?? CountryCode; }
			set { _shippingCountryCode = value; }
		}

		/// <summary>
		///     Countryname of th ecountry to ship to
		/// </summary>
		[DataMember]
		public string ShippingCountryName
		{
			get
			{
				if (!string.IsNullOrEmpty(ShippingCountryCode))
				{
					Country country = IO.Container.Resolve<ICountryRepository>().GetAllCountries().FirstOrDefault(x => x.Code == ShippingCountryCode);

					return country != null ? country.Name : string.Empty;
				}

				return string.Empty;
			}
			set { }
		}

		/// <summary>
		///     Regioncode of the customer
		/// </summary>
		[DataMember]
		public string RegionCode { get; set; }

		/// <summary>
		///     Regioname of the customer
		/// </summary>
		[DataMember]
		public string RegionName
		{
			get
			{
				if (!string.IsNullOrEmpty(RegionCode))
				{
					Region region = StoreHelper.GetAllRegions().FirstOrDefault(x => x.Code == RegionCode);

					return region != null ? region.Name : string.Empty;
				}

				return string.Empty;
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the customer information.
		/// </summary>
		/// <value>
		/// The customer information.
		/// </value>
		[XmlIgnore]
		public XDocument customerInformation { get; set; }

		/// <summary>
		/// Gets or sets the customer information.
		/// </summary>
		/// <value>
		/// The customer information.
		/// </value>
		[DataMember]
		public XElement CustomerInformation
		{
			get { return customerInformation != null ? customerInformation.Root : null; }
			set
			{
				customerInformation = new XDocument();
				customerInformation.Add(value);
			}
		}

		/// <summary>
		/// Gets or sets the shipping information.
		/// </summary>
		/// <value>
		/// The shipping information.
		/// </value>
		[XmlIgnore]
		public XDocument shippingInformation { get; set; }

		/// <summary>
		/// Gets or sets the shipping information.
		/// </summary>
		/// <value>
		/// The shipping information.
		/// </value>
		[DataMember]
		public XElement ShippingInformation
		{
			get { return shippingInformation != null ? shippingInformation.Root : null; }
			set
			{
				shippingInformation = new XDocument();
				shippingInformation.Add(value);
			}
		}

		/// <summary>
		/// Gets or sets the extra information.
		/// </summary>
		/// <value>
		/// The extra information.
		/// </value>
		[XmlIgnore]
		public XDocument extraInformation { get; set; }

		/// <summary>
		/// Gets or sets the extra information.
		/// </summary>
		/// <value>
		/// The extra information.
		/// </value>
		[DataMember]
		public XElement ExtraInformation
		{
			get { return extraInformation != null ? extraInformation.Root : null; }
			set
			{
				extraInformation = new XDocument();
				extraInformation.Add(value);
			}
		}

		/// <summary>
		///     Are the customer and the shipping fields equal?
		/// </summary>
		[DataMember]
		public bool CustomerIsShipping
		{
			get
			{
				var matchList = new List<bool>();

				if (customerInformation != null && customerInformation.Root != null)
				{
					foreach (var customerNode in customerInformation.Root.Descendants())
					{
						var matchingShippingName = customerNode.Name.ToString().Replace("customer", "shipping");
						if (shippingInformation != null && shippingInformation.Root != null)
						{
							var matchingShippingNode = shippingInformation.Root.Descendants().FirstOrDefault(x => x.Name == matchingShippingName);

							if (matchingShippingNode != null && matchingShippingNode.Value == customerNode.Value)
							{
								matchList.Add(true);
							}
							if (matchingShippingNode == null || matchingShippingNode.Value != customerNode.Value)
							{
								matchList.Add(false);
							}
						}
					}
				}

				return matchList.All(x => x == true);
			}
			set { }
		}
	}
}