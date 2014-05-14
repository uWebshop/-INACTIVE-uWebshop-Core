using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.interfaces;
using umbraco.IO;
using uWebshop.Domain;

namespace uWebshop.Umbraco.DataTypes.EmailTemplateSelector
{
	public class EmailTemplateSelectorDataEditor : UpdatePanel, IDataEditor
	{
		private readonly IData _data;

		private DropDownList _dlTemplates;

		public void Save()
		{
			if (_data != null)
			{
				_data.Value = _dlTemplates.SelectedValue;
			}
		}

		public bool ShowLabel
		{
			get { return true; }
		}

		public bool TreatAsRichTextEditor
		{
			get { return false; }
		}

		public EmailTemplateSelectorDataEditor(IData data)
		{
			_data = data;
		}

		public Control Editor
		{
			get { return this; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Licensing.uWebshopTrialMessage();


			_dlTemplates = new DropDownList();

			var filePaths = Directory.GetFiles(IOHelper.MapPath(SystemDirectories.Xslt), "*.xslt", SearchOption.AllDirectories);

			var razorfiles = Directory.GetFiles(IOHelper.MapPath(SystemDirectories.MacroScripts), "*.cshtml", SearchOption.AllDirectories);

			var chooseText = library.GetDictionaryItem("Choose");
			if (string.IsNullOrEmpty(chooseText))
			{
				chooseText = "Choose...";
			}

			_dlTemplates.Items.Add(new ListItem(chooseText, "0"));

			foreach (var file in filePaths)
			{
				var fileOutput = file.Replace(IOHelper.MapPath(SystemDirectories.Xslt) + @"\", string.Empty);
				_dlTemplates.Items.Add(new ListItem(fileOutput, fileOutput));
			}

			foreach (var file in razorfiles)
			{
				var fileOutput = file.Replace(IOHelper.MapPath(SystemDirectories.MacroScripts) + @"\", string.Empty);
				_dlTemplates.Items.Add(new ListItem(fileOutput, fileOutput));
			}

			if (_data != null)
			{
				_dlTemplates.SelectedValue = _data.Value.ToString();
			}

			var user = User.GetCurrent();

			if (!user.IsAdmin())
			{
				_dlTemplates.Enabled = false;
			}

			if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_dlTemplates);
		}
	}
}