using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderStatusSection
{
    public class OrderStatusSectionDataType
    {
        public static readonly Guid DefId = new Guid("20cd4651-012b-4761-afb1-f8af120ac74b");
        public static readonly Guid Key = new Guid("c82ca8c9-65e6-47af-8174-1666eb25a26d");
        public static readonly string Name = "uWebshop Order Section";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}