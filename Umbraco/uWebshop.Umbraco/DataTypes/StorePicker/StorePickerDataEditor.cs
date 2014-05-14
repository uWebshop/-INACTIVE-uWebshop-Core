using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.StorePicker
{
	public class StorePickerDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlInstalledStores;

		public StorePickerDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			if (_data != null) _data.Value = _dlInstalledStores.SelectedValue;
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

			//if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
			//    return;

			Licensing.uWebshopTrialMessage();

			_dlInstalledStores = new DropDownList();

			var chooseText = library.GetDictionaryItem("Choose");
			if (string.IsNullOrEmpty(chooseText))
			{
				chooseText = "Choose...";
			}

			_dlInstalledStores.Items.Add(new ListItem(chooseText, "0"));

			foreach (var store in StoreHelper.GetAllStores())
			{
				_dlInstalledStores.Items.Add(new ListItem(store.Alias, store.Id.ToString()));
			}

			if (_data.Value != null)
			{
				try
				{
					_dlInstalledStores.SelectedValue = _data.Value.ToString();
				}
				catch
				{
				}
			}

			//var user = User.GetCurrent();
			//var user = Membership.GetUser();

			//if(!user.IsAdmin())
			//{
			//    _dlInstalledStores.Enabled = false;
			//}

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlInstalledStores);
		}
	}
}