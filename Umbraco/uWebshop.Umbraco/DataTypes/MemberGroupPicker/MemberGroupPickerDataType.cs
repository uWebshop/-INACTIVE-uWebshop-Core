using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.MemberGroupPicker
{
    public class MemberGroupPickerDataType
    {
        public static readonly Guid DefId = new Guid("bd6054bc-86ec-4f2c-bcc8-447766f1d569");
        public static readonly Guid Key = new Guid("a99d5614-8b33-4a63-891a-a254c87af481");
        public static readonly string Name = "uWebshop MemberGroup Picker";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }
}