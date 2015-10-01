using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.StoreTemplatePicker
{
	public class StoreTemplatePickerDataType
	{
		public static readonly Guid DefId = new Guid("2ad05995-470e-47d9-956d-dd2ec892343d");
		public static readonly Guid Key = new Guid("a20c7c00-09f1-448d-9656-f5cb012107af");
		public static readonly string Name = "uWebshop Template Picker";
		public static readonly DatabaseType DatabaseType = DatabaseType.Integer;

	}
}