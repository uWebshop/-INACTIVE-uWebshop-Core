using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.ShippingProviderType
{
    public class ShippingProviderTypeDataType
    {
        public static readonly Guid DefId = new Guid("2cdfc385-15dd-4165-84d9-b5e4b2aca970");
        public static readonly Guid Key = new Guid("c035ada7-5413-48de-8e62-b4b61e2e934f");
        public static readonly string Name = "uWebshop Shipping Provider Type";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}