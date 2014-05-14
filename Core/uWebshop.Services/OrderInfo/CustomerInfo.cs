using System;
using System.Linq;
using System.Globalization;
using SuperSimpleWebshop.Domain.Helpers;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic;
using System.Xml;
using System.Xml.XPath;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class CustomerInfo
    {
        #region customer
        public string VATNumber;
        
        public string CountryCode;
        
        public string CountryName
        {
            get
            {
                var country = ShippingHelper.GetAllCountries().Where(x => x.Code == CountryCode).FirstOrDefault();

                if (country != null)
                {
                    return country.Name;
                }
                else
                {
                    return CountryCode;
                }
            }
        }

        public string RegionCode;

        public string RegionName
        {
            get
            {
                var country = ShippingHelper.GetAllRegions().Where(x => x.Code == RegionCode).FirstOrDefault();

                if (country != null)
                {
                    return country.Name;
                }
                else
                {
                    return RegionCode;
                }
            }
        }
           
        #endregion
    }

   
}
