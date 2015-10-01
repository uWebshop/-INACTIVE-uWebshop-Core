using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.ShippingRangeType
{
    public class ShippingRangeTypeDataType
    {
        public static readonly Guid DefId = new Guid("c0737154-5d14-4820-bcb1-98ed82363f7c");
        public static readonly Guid Key = new Guid("34c70cbf-ee1b-465e-941f-ddcd097f7912");
        public static readonly string Name = "uWebshop Shipping Range Type";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}