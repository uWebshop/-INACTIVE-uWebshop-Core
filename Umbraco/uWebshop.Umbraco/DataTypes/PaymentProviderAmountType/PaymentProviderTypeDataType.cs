using System;
using uWebshop.Domain.ContentTypes;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;
using uWebshop.Umbraco.DataTypes.CountySelector;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderAmountType
{
	public class PaymentProviderAmountTypeDataType : BaseDataType, IDataType
	{
		public static readonly Guid DefId = new Guid("85c8f557-cf82-4450-856f-79492a02f55d");
		public static readonly Guid Key = new Guid("fb79c76e-8ccc-406d-a3ad-39c1f939a38d");
		public static readonly string Name = "uWebshop Payment Provider Amount Type";
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
			get { return _editor ?? (_editor = new PaymentProviderAmountTypeDataEditor(Data)); }
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