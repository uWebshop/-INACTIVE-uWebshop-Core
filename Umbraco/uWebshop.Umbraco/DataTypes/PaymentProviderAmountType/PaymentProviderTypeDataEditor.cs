using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderAmountType
{
	public class PaymentProviderAmountTypeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlPaymentProviderTypes;

		public PaymentProviderAmountTypeDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlPaymentProviderTypes.SelectedValue;
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

			_dlPaymentProviderTypes = new DropDownList();

			var paymentProviderAmountTypeAmountText = library.GetDictionaryItem("PaymentProviderAmountTypeAmount");
			if (string.IsNullOrEmpty(paymentProviderAmountTypeAmountText))
			{
				paymentProviderAmountTypeAmountText = "Amount";
			}

			var paymentProviderAmountTypeOrderPercentageText = library.GetDictionaryItem("PaymentProviderAmountTypeOrderPercentage");
			if (string.IsNullOrEmpty(paymentProviderAmountTypeOrderPercentageText))
			{
				paymentProviderAmountTypeOrderPercentageText = "Percentage of order total";
			}

			_dlPaymentProviderTypes.Items.Add(new ListItem(paymentProviderAmountTypeAmountText, Common.PaymentProviderAmountType.Amount.ToString()));
			_dlPaymentProviderTypes.Items.Add(new ListItem(paymentProviderAmountTypeOrderPercentageText, Common.PaymentProviderAmountType.OrderPercentage.ToString()));

			_dlPaymentProviderTypes.SelectedValue = _data.Value.ToString();

			var user = User.GetCurrent();

			if (!user.IsAdmin())
			{
				_dlPaymentProviderTypes.Enabled = false;
			}

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlPaymentProviderTypes);
		}
	}
}