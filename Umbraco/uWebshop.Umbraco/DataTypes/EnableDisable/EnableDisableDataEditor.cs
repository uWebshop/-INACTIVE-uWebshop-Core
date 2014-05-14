using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.EnableDisable
{
	public class EnableDisableDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlEnableDisable;

		public EnableDisableDataEditor(IData data)
		{
			_data = data;
		}

		public void Save()
		{
			if (_data != null) _data.Value = _dlEnableDisable.SelectedValue;
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

			_dlEnableDisable = new DropDownList();

			var nodeIdstring = HttpContext.Current.Request["id"];

			int nodeId;
			int.TryParse(nodeIdstring, out nodeId);

			if (nodeId > 0)
			{
				var doc = new Document(nodeId);

				if (!doc.ContentType.Alias.StartsWith(Store.NodeAlias))
				{
					var storeDefaultText = library.GetDictionaryItem("StoreDefault");
					if (string.IsNullOrEmpty(storeDefaultText))
					{
						storeDefaultText = "Store Default";
					}

					_dlEnableDisable.Items.Add(new ListItem(storeDefaultText, "default"));
				}
			}

			var enableText = library.GetDictionaryItem("Enable");
			if (string.IsNullOrEmpty(enableText))
			{
				enableText = "Enable";
			}
			_dlEnableDisable.Items.Add(new ListItem(enableText, "enable"));

			var disableText = library.GetDictionaryItem("Disable");
			if (string.IsNullOrEmpty(disableText))
			{
				disableText = "Disable";
			}
			_dlEnableDisable.Items.Add(new ListItem(disableText, "disable"));

			_dlEnableDisable.SelectedValue = _data.Value.ToString();

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlEnableDisable);
		}
	}
}