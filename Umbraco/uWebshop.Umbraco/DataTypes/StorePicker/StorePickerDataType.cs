using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.StorePicker
{
	public class StorePickerDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("5fa345e3-9352-45d6-adaa-2da6cdc9aca3");
		public static readonly Guid Key = new Guid("1e8cdc0b-436e-46f5-bfec-57be45745771");
		public static readonly string Name = "uWebshop Store Picker";
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
			get { return _editor ?? (_editor = new StorePickerDataEditor(Data)); }
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