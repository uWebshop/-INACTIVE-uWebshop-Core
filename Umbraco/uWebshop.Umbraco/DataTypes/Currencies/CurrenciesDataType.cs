using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.Currencies
{
	public class CurrenciesDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("9dbebe55-72b2-4573-b16c-9bfe3c4d24f6");
		public static readonly Guid Key = new Guid("7e6cab81-528d-4b00-8321-72c36f131eea");
		public static readonly string Name = "uWebshop Currencies";
		public static readonly DatabaseType DatabaseType = DatabaseType.Ntext;

		private IDataEditor _editor;
		private IData _baseData;
		private PrevalueEditor _prevalueeditor;

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
			get { return _editor ?? (_editor = new CurrenciesDataEditor(Data)); }
		}

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