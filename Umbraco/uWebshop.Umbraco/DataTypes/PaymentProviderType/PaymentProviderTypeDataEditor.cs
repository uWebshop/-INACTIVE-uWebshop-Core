using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.interfaces;

namespace uWebshop.Umbraco.DataTypes.PaymentProviderType
{
	public class PaymentProviderTypeDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlPaymentProviderTypes;

		public PaymentProviderTypeDataEditor(IData data)
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

			var paymentProviderTypeOnlinePaymentText = library.GetDictionaryItem("PaymentProviderTypeOnlinePayment");
			if (string.IsNullOrEmpty(paymentProviderTypeOnlinePaymentText))
			{
				paymentProviderTypeOnlinePaymentText = "Online Payment Provider";
			}

			var paymentProviderTypeOfflinePaymentInStoreText = library.GetDictionaryItem("PaymentProviderTypeOfflinePaymentInStore");
			if (string.IsNullOrEmpty(paymentProviderTypeOfflinePaymentInStoreText))
			{
				paymentProviderTypeOfflinePaymentInStoreText = "Payment in store";
			}

			var paymentProviderTypeOfflinePaymentAtCustomerText = library.GetDictionaryItem("PaymentProviderTypeOfflinePaymentAtCustomer");
			if (string.IsNullOrEmpty(paymentProviderTypeOfflinePaymentAtCustomerText))
			{
				paymentProviderTypeOfflinePaymentAtCustomerText = "Payment at customers home";
			}

			_dlPaymentProviderTypes.Items.Add(new ListItem(paymentProviderTypeOnlinePaymentText, Common.PaymentProviderType.OnlinePayment.ToString()));
			_dlPaymentProviderTypes.Items.Add(new ListItem(paymentProviderTypeOfflinePaymentInStoreText, Common.PaymentProviderType.OfflinePaymentInStore.ToString()));
			_dlPaymentProviderTypes.Items.Add(new ListItem(paymentProviderTypeOfflinePaymentAtCustomerText, Common.PaymentProviderType.OfflinePaymentAtCustomer.ToString()));

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