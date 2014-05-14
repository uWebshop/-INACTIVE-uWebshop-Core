using System;
using System.Linq;
using SuperSimpleWebshop.Domain.Helpers;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class StaticCustomerInfo
    {
        #region customer
        public string EmailAddress;
        public string FirstName;
        public string LastName;

        public string FullName
        {
            get
            {
                try
                {
                    return string.Format("{0} {1}", FirstName, LastName);
                    
                }
                catch
                {
                    return null;
                }
            }
        }

        public string CustomerVATNumber;

        public string CustomerCountryCode;

        public string CustomerCountry
        {
            get
            {
                var country = ShippingHelper.GetAllCountries().Where(x => x.Code == CustomerCountryCode).FirstOrDefault();

                if (country != null)
                {
                    return country.Name;
                }
                else
                {
                    return CustomerCountryCode;
                }
            }
        }

        public string CustomerEmail;
        #endregion
    }
}
