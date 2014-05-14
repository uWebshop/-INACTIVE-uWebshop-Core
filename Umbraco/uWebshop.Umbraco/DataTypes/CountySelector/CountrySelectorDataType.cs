using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.CountySelector
{
	public class CountrySelectorDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("cdf9f220-ea26-4f37-9505-4b65dfdd59e8");
		public static readonly Guid Key = new Guid("a6d19ee8-ab93-42d6-a61a-8d4aaa759207");
		public static readonly string Name = "uWebshop Country Selector";
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
			get { return _editor ?? (_editor = new CountrySelectorDataEditor(Data)); }
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