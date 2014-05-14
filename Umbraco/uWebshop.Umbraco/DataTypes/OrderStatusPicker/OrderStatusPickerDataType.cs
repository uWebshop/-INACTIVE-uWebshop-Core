using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderStatusPicker
{
	public class OrderStatusPickerDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("f1ad9252-933b-4c44-8496-b24318dac2ed");
		public static readonly Guid Key = new Guid("8fa38a1b-7854-43ce-b03e-c7cc2ffd3d20");
		public static readonly string Name = "uWebshop Order Status Picker";
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
			get { return _editor ?? (_editor = new OrderStatusPickerDataEditor(Data)); }
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