using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.DiscountOrderCondition
{
	public class DiscountOrderConditionDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("2b946d78-256c-48f6-98f5-435ac946a220");
		public static readonly Guid Key = new Guid("e159f935-d7f4-4277-b5d0-adf74b003849");
		public static readonly string Name = "uWebshop Discount Condition Picker";
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
			get { return _editor ?? (_editor = new DiscountOrderConditionDataEditor(Data)); }
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