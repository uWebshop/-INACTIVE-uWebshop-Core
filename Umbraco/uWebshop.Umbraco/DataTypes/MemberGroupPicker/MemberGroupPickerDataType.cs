using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.MemberGroupPicker
{
	public class MemberGroupPickerDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("bd6054bc-86ec-4f2c-bcc8-447766f1d569");
		public static readonly Guid Key = new Guid("a99d5614-8b33-4a63-891a-a254c87af481");
		public static readonly string Name = "uWebshop MemberGroup Picker";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;

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
			get { return _editor ?? (_editor = new MemberGroupPickerDataEditor(Data)); }
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