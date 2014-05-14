using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderType
{
	public class PaymentProviderTypeDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("e2e9e8da-f7dc-43f4-806c-8a27fa738971");
		public static readonly Guid Key = new Guid("bbaf3c0e-fc22-4d33-b884-7b87d8dc3c8c");
		public static readonly string Name = "uWebshop Payment Provider Type";
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
			get { return _editor ?? (_editor = new PaymentProviderTypeDataEditor(Data)); }
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