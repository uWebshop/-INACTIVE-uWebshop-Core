using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.StoreTemplatePicker
{
	public class StoreTemplatePickerDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("2ad05995-470e-47d9-956d-dd2ec892343d");
		public static readonly Guid Key = new Guid("a20c7c00-09f1-448d-9656-f5cb012107af");
		public static readonly string Name = "uWebshop Template Picker";
		public static readonly DatabaseType DatabaseType = DatabaseType.Integer;

		private IDataEditor _editor;
		private IData _baseData;

		public override Guid Id
		{
			get { return DefId; }
		}

		public override string DataTypeName
		{
			get { return Name; }
		}

		public override IDataEditor DataEditor
		{
			get { return _editor ?? (_editor = new StoreTemplatePickerDataEditor(Data)); }
		}

		private PrevalueEditor _prevalueeditor;

		public override IDataPrevalue PrevalueEditor
		{
			get { return _prevalueeditor ?? (_prevalueeditor = new PrevalueEditor()); }
		}

		public override IData Data
		{
			get { return _baseData ?? (_baseData = new DefaultData(this)); }
		}
	}
}