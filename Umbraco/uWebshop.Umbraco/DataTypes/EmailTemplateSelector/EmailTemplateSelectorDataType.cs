using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.EmailTemplateSelector
{
	public class EmailTemplateSelectorDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("20e19862-f52d-4c56-88c3-244238eafffc");
		public static readonly Guid Key = new Guid("0a10d9dd-ebbc-48f5-be93-9fac239ac876");
		public static readonly string Name = "uWebshop Email Template Picker";
		public static readonly DatabaseType DatabaseType = DatabaseType.Nvarchar;

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
			get { return _editor ?? (_editor = new EmailTemplateSelectorDataEditor(Data)); }
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