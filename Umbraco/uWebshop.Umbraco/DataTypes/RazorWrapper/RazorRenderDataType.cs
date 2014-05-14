using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.RazorWrapper
{
	public class RazorWrapperDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("3c148cf9-de63-4bc6-860e-81fe9546fa61");
		public static readonly Guid Key = new Guid("e2e02d79-c7d1-4806-9aa9-1a6353e18952");
		public static readonly string Name = "Razor Wrapper";

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
			get { return _editor ?? (_editor = new RazorWrapperDataEditor(Data, new RazorWrapperDataEditorSetting(this))); }
		}

		private RazorWrapperDataEditorSetting _prevalueeditor;

		public override IDataPrevalue PrevalueEditor
		{
			get { return _prevalueeditor ?? (_prevalueeditor = new RazorWrapperDataEditorSetting(this)); }
		}

		public override IData Data
		{
			get { return _baseData ?? (_baseData = new DefaultData(this)); }
		}
	}
}