using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.Price
{
    public class PriceDataType
    {
        public static readonly Guid DefId = new Guid("efe95454-4982-4bd8-a5dc-3fdfa24e5b5b");
        public static readonly Guid Key = new Guid("40e2736a-8ecd-41b3-bb27-7d11909f0a21");
        public static readonly string Name = "uWebshop Price Editor";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}