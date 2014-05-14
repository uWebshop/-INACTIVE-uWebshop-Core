using System.Runtime.Serialization;
using System.Xml.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[DataContract(Name = "Address", Namespace = "")]
	internal class XElementAddress : IAddress
	{
		private readonly XElement _source;
		private readonly string _propertyNameprefix;
		private readonly bool _customerIsShipping;

		public XElementAddress(XElement source, string propertyNamePrefix, bool customerIsShipping = false)
		{
			_source = source;
			_propertyNameprefix = propertyNamePrefix;
			_customerIsShipping = customerIsShipping;
		}

		[DataMember]
		public string FirstName { get { return GetValue(_propertyNameprefix + "FirstName"); } set {}}
		[DataMember]
		public string Name { get { return GetValue(_propertyNameprefix + "Name"); } set { } }
		[DataMember]
		public string LastName { get { return GetValue(_propertyNameprefix + "LastName"); } set { } }
		[DataMember]
		public string Address1 { get { return GetValue(_propertyNameprefix + "Address1"); } set { } }
		[DataMember]
		public string Address2 { get { return GetValue(_propertyNameprefix + "Address2"); } set { } }
		[DataMember]
		public string Street { get { return GetValue(_propertyNameprefix + "Street"); } set { } }
		[DataMember]
		public string StreetNumber { get { return GetValue(_propertyNameprefix + "StreetNumber"); } set { } }
		[DataMember]
		public string StreetNumberAddition { get { return GetValue(_propertyNameprefix + "StreetNumberAddition"); } set { } }
		[DataMember]
		public string City { get { return GetValue(_propertyNameprefix + "City"); } set { } }
		[DataMember]
		public string Country { get { return GetValue(_propertyNameprefix + "Country"); } set { } } // todo?
		[DataMember]
		public string CountryCode { get { return GetValue(_propertyNameprefix + "Country"); } set { } }
		[DataMember]
		public string Region { get { return GetValue(_propertyNameprefix + "Region"); } set { } }
		[DataMember]
		public string ZipCode { get { return GetValue(_propertyNameprefix + "ZipCode"); } set { } }
		[DataMember]
		public string Company { get { return GetValue(_propertyNameprefix + "Company"); } set { } }
		[DataMember]
		public string Phone { get { return GetValue(_propertyNameprefix + "Phone"); } set { } }
		[DataMember]
		public string Email { get { return GetValue(_propertyNameprefix + "Email"); } set { } }

		public T GetValue<T>(string fieldName, bool ignoreCustomerIsShipping = false)
		{
			if (_customerIsShipping && !ignoreCustomerIsShipping)
			{
				return default(T);
			}

			return Helpers.GetValue<T>(fieldName, _source);
		}
		internal string GetValue(string fieldName, bool ignoreCustomerIsShipping = false)
		{
			if (_customerIsShipping && !ignoreCustomerIsShipping)
			{
				return string.Empty;
			}

			return GetCustomerValueFromSessionOrBasketOrProfile(_source, fieldName);
		}

		private static string GetCustomerValueFromSessionOrBasketOrProfile(XContainer xSource, string fieldName)
		{
			// todo: think about the order
			if (fieldName != null)
			{
				var sessionValue = Customers.GetCustomerValueFromSession(fieldName);

				if (!string.IsNullOrEmpty(sessionValue))
				{
					return sessionValue;
				}
			}

			if (xSource != null)
			{
				var element = xSource.Element(fieldName);

				if (element != null && !string.IsNullOrEmpty(element.Value))
				{
					return element.Value;
				}
			}

			var memberValue = Customers.GetCustomerValueFromProfile(fieldName);
			
			if (!string.IsNullOrEmpty(memberValue))
			{
				return memberValue;
			}

			return string.Empty;
		}
	}
}