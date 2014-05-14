using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.EmailPreview
{
	public class EmailPreviewDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("77345447-e3b4-4891-907f-bc0e0d5831a7");
		public static readonly Guid Key = new Guid("e8dc0a53-d659-4d37-a1ca-b339c070054e");
		public static readonly string Name = "uWebshop Email Preview";

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
			get { return _editor ?? (_editor = new EmailPreviewDataEditor(Data)); }
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