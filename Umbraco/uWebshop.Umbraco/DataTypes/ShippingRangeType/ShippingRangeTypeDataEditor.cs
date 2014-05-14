using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.ShippingRangeType
{
	public class ShippingRangeTypeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlShippingRangeTypes;

		public ShippingRangeTypeDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlShippingRangeTypes.SelectedValue;
		}

		public bool ShowLabel
		{
			get { return true; }
		}

		public bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		public Control Editor
		{
			get { return this; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			_dlShippingRangeTypes = new DropDownList();


			var shippingRangeTypeOrderAmountText = library.GetDictionaryItem("ShippingRangeTypeOrderAmount");
			if (string.IsNullOrEmpty(shippingRangeTypeOrderAmountText))
			{
				shippingRangeTypeOrderAmountText = "Order Amount";
			}

			var shippingRangeTypeQuantityText = library.GetDictionaryItem("ShippingRangeTypeQuantity");
			if (string.IsNullOrEmpty(shippingRangeTypeQuantityText))
			{
				shippingRangeTypeQuantityText = "Order Item Quantity";
			}

			var shippingRangeTypeWeightText = library.GetDictionaryItem("ShippingRangeTypeWeight");
			if (string.IsNullOrEmpty(shippingRangeTypeWeightText))
			{
				shippingRangeTypeWeightText = "Order Weight";
			}

			_dlShippingRangeTypes.Items.Add(new ListItem(shippingRangeTypeOrderAmountText, Common.ShippingRangeType.OrderAmount.ToString()));
			_dlShippingRangeTypes.Items.Add(new ListItem(shippingRangeTypeQuantityText, Common.ShippingRangeType.Quantity.ToString()));
			_dlShippingRangeTypes.Items.Add(new ListItem(shippingRangeTypeWeightText, Common.ShippingRangeType.Weight.ToString()));

			_dlShippingRangeTypes.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlShippingRangeTypes);
		}
	}
}