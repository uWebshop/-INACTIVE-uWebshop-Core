using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using umbraco.interfaces;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco.DataTypes.RazorWrapper
{
	public class RazorWrapperDataEditor : UpdatePanel, IDataEditor
	{
		private Literal _lblRenderRazorContent;
		
		private readonly IData _data;
		private readonly bool _showLabel;
		private readonly string _razorFile;

		private readonly RazorWrapperDataEditorSetting _dataSetting;

		public RazorWrapperDataEditor(IData data, RazorWrapperDataEditorSetting setting)
		{
			_data = data;
			_dataSetting = setting;

			var configSettings = _dataSetting.Configuration.Split('|');

			if (configSettings.Length > 0 && configSettings[0] != "")
				_razorFile = configSettings[0];

			if (configSettings.Length > 1 && configSettings[1] != "")
				_showLabel = bool.Parse(configSettings[1]);
		}

		public void Save()
		{
			var saveValue = (Dictionary<string, string>) HttpContext.Current.Session["RazorFields"];

			if (saveValue != null && saveValue.Any())
			{
				var el = new XElement("values", saveValue.Select(kv => new XElement(kv.Key, kv.Value)));

				if (_data != null) _data.Value = el.ToString();

				HttpContext.Current.Session.Remove("RazorFields");
			}
		}

		public virtual bool ShowLabel
		{
			get { return _showLabel; }
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

			if (!(Page.Request.CurrentExecutionFilePath ?? string.Empty).Contains("editContent.aspx"))
				return;

			try
			{
				var documentId = HttpContext.Current.Request["id"];

				int docId;
				int.TryParse(documentId, out docId);

				if (docId != 0)
				{
					_lblRenderRazorContent = new Literal { Text = RazorLibraryExtensions.RenderMacro(_razorFile, docId) };


					if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblRenderRazorContent);
				}
			}
			catch
			{
				_lblRenderRazorContent = new Literal {Text = "Error rendering data"};

				if (ContentTemplateContainer != null) ContentTemplateContainer.Controls.Add(_lblRenderRazorContent);
			}
		}
	}
}