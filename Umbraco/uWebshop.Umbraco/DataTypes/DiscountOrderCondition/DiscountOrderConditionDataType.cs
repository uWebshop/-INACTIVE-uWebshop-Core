using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.DiscountOrderCondition
{
    public class DiscountOrderConditionDataType
    {
        public static readonly Guid DefId = new Guid("2b946d78-256c-48f6-98f5-435ac946a220");
        public static readonly Guid Key = new Guid("e159f935-d7f4-4277-b5d0-adf74b003849");
        public static readonly string Name = "uWebshop Discount Condition Picker";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}