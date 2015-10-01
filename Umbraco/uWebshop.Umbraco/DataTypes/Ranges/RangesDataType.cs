using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.Ranges
{
    public class RangesDataType
    {
        public static readonly Guid DefId = new Guid("d5065baa-bb03-437f-9b25-46aa363e1b1c");
        public static readonly Guid Key = new Guid("09dfbeec-2681-42af-b07c-0ed56a575d48");
        public static readonly string Name = "uWebshop Range Selector";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}