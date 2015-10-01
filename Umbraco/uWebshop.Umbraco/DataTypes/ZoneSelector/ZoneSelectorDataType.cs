using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.ZoneSelector
{
    public class ZoneSelectorDataType
    {
        public static readonly Guid DefId = new Guid("4ee1243a-889b-4ffa-8f44-59cc1fc73be2");
        public static readonly Guid Key = new Guid("8bc628dd-fe95-4a73-bdde-a7f4b620c170");
        public static readonly string Name = "uWebshop Zone Selector";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}