using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderType
{
    public class PaymentProviderTypeDataType
    {
        public static readonly Guid DefId = new Guid("e2e9e8da-f7dc-43f4-806c-8a27fa738971");
        public static readonly Guid Key = new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c");
        public static readonly string Name = "uWebshop Payment Provider Type";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;

    }
}