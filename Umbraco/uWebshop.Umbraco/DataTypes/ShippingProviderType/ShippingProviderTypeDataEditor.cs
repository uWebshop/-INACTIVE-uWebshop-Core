using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.ShippingProviderType
{
	public class ShippingProviderTypeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlShippingProviderTypes;

		public ShippingProviderTypeDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlShippingProviderTypes.SelectedValue;
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

			_dlShippingProviderTypes = new DropDownList();

			var shippingProviderTypePickupText = library.GetDictionaryItem("ShippingProviderTypePickup");
			if (string.IsNullOrEmpty(shippingProviderTypePickupText))
			{
				shippingProviderTypePickupText = "Pickup";
			}

			var shippingProviderTypeShippingText = library.GetDictionaryItem("ShippingProviderTypeShipping");
			if (string.IsNullOrEmpty(shippingProviderTypeShippingText))
			{
				shippingProviderTypeShippingText = "Shipping";
			}

			_dlShippingProviderTypes.Items.Add(new ListItem(shippingProviderTypePickupText, Common.ShippingProviderType.Pickup.ToString()));
			_dlShippingProviderTypes.Items.Add(new ListItem(shippingProviderTypeShippingText, Common.ShippingProviderType.Shipping.ToString()));

			_dlShippingProviderTypes.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlShippingProviderTypes);
		}
	}
}