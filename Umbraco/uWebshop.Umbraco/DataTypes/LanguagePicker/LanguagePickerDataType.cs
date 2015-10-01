using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.LanguagePicker
{
    public class LanguagePickerDataType
    {
        public static readonly Guid DefId = new Guid("24a53e42-4bb4-4c9a-a16c-3651e3083f30");
        public static readonly Guid Key = new Guid("4235f880-64cc-4d78-8fc2-6e6e5ee72010");
        public static readonly string Name = "uWebshop Language Picker";
        public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;
    }

}