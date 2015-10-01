using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.StockUpdate
{
    public class StockUpdateDataType {
        public static readonly Guid DefId = new Guid("8b5da1fc-4c8a-4e8f-b314-ae19a850dae8");
        public static readonly Guid Key = new Guid("5744ead8-977b-44c1-b362-fe8bebca7098");
        public static readonly string Name = "uWebshop Stock";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}