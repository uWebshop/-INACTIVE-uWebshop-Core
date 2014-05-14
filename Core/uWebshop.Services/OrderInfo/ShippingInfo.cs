using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SuperSimpleWebshop.Domain.BaseClasses;
using SuperSimpleWebshop.Domain.Helpers;
using SuperSimpleWebshop.Domain.Repositories;
using SuperSimpleWebshop.Umbraco;
using umbraco.NodeFactory;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class ShippingInfo
    {
        public string ShippingCountry;

        public string ShippingModule;

        public string ShippingProviderId;
        public string ShippingProviderTitle;

        public string ShippingProviderName;
        public string ShippingProviderNodeName;
        public string ShippingProviderMethodName;

        public ShippingProvider ShippingProvider
        {
            get
            {
                if (!string.IsNullOrEmpty(ShippingProviderId))
                {
                    ShippingProvider shipPro = new ShippingProvider(int.Parse(ShippingProviderId));

                    return shipPro;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
