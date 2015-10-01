using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.EnableDisable
{
    public class EnableDisableDataType
    {
        public static readonly Guid DefId = new Guid("6e62bca5-8d73-435e-8a26-78ef0e7b6c58");
        public static readonly Guid Key = new Guid("63c6fa9a-975f-4474-9155-62a229bafaef");
        public static readonly string Name = "Enable/Disable";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}