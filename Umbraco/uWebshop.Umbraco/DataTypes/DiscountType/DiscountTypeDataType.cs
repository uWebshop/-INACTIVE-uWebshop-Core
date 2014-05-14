using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.DiscountType
{
	public class DiscountTypeDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("c66b5dea-00f7-49df-bc5a-074e5802bce2");
		public static readonly Guid Key = new Guid("2d89188e-33a7-4885-a6d0-1caef40320a7");
		public static readonly string Name = "uWebshop Discount Picker";
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
			get { return _editor ?? (_editor = new DiscountTypeDataEditor(Data)); }
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