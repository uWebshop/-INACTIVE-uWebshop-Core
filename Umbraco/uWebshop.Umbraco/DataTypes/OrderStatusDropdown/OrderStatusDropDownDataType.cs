using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderStatusDropdown
{
    public class OrderStatusDrowpDownDataType
    {
        public static readonly Guid DefId = new Guid("1a2124b7-729a-4253-81e2-fe4542841976");
        public static readonly Guid Key = new Guid("2e04185a-8b4c-48b2-a363-05af42819f6f");
        public static readonly string Name = "uWebshop Order Status Dropdown";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}