using System;

namespace SuperSimpleWebshop.Domain
{
    [Serializable]
    public class StaticShippingInfo
    {
        public string ShippingCountry;

        /// <summary>
        /// Gets the shipping provider of the order
        /// </summary>
        //public XmlCDataSection ShippingProviderAsXml
        //{
        //    get
        //    {
        //        XmlDocument doc = new XmlDocument();

        //        return doc.CreateCDataSection(ShippingProvider);
        //    }
        //    set
        //    {
        //        ShippingProvider = value.Value;
        //    }
        //}

        //[XmlIgnore]
        public string ShippingProvider;
    }
}
