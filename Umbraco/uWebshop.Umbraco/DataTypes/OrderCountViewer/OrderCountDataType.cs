using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.OrderCountViewer
{
	public class OrderCountDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("1779c16d-1b5d-4312-974b-782590caca3e");
		public static readonly Guid Key = new Guid("2f6e0aa8-8291-4544-8aeb-db78cfb42b07");
		public static readonly string Name = "uWebshop Item Ordered Count";
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
			get { return _editor ?? (_editor = new OrderCountDataEditor(Data)); }
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