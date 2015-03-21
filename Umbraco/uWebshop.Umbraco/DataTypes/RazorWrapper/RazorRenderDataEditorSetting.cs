using System;
using System.Collections;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace uWebshop.Umbraco.DataTypes.RazorWrapper
{
	public class RazorWrapperDataEditorSetting : PlaceHolder, IDataPrevalue
	{
		private readonly BaseDataType _datatype;
		private CheckBox _showLabel = new CheckBox();

		private DropDownList _ddlRazorScripts = new DropDownList();

		public static ISqlHelper SqlHelper
		{
			get { return Application.SqlHelper; }
		}

		public RazorWrapperDataEditorSetting(BaseDataType datatype)
		{
			_datatype = datatype;
			SetupChildControls();
		}

		private void SetupChildControls()
		{
			_ddlRazorScripts = new DropDownList {ID = "ddlRazorScripts"};
			Controls.Add(_ddlRazorScripts);

			_showLabel = new CheckBox {ID = "cbLabel"};
			Controls.Add(_showLabel);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table cellpadding=\"10\">");
			writer.Write("<tr><th>Razor Script:</th><td>");
			_ddlRazorScripts.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Show Label?</th><td>");
			_showLabel.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("</table>");
		}

		public string Configuration
		{
			get
			{
				var conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));

				return conf != null ? conf.ToString() : "";
			}
		}


		public void Save()
		{
			_datatype.DBType = (DBTypes) Enum.Parse(typeof (DBTypes), DBTypes.Ntext.ToString(), true);

			var data = _ddlRazorScripts.SelectedValue + "|";

			data += _showLabel.Checked.ToString();

			SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
			SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", data));
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			//if (!Page.IsPostBack)
			//{
			_ddlRazorScripts.Items.Clear();
			_ddlRazorScripts.Items.Insert(0, new ListItem(string.Empty, string.Empty));

			var razorScripts = new ArrayList();
			var razorDir = IOHelper.MapPath(SystemDirectories.MacroScripts + "/");
			GetRazorFilesFromDir(razorDir, razorDir, razorScripts);
			_ddlRazorScripts.DataSource = razorScripts;
			_ddlRazorScripts.DataBind();

			var configSettings = Configuration.Split('|');


			try
			{
				if (configSettings.Length > 0 && configSettings[0] != "")
					_ddlRazorScripts.SelectedValue = configSettings[0];
			}
			catch
			{
			}


			try
			{
				if (configSettings.Length > 0 && configSettings[1] != "")
					_showLabel.Checked = bool.Parse(configSettings[1]);
			}
			catch
			{
			}


			_ddlRazorScripts.Items.Insert(0, new ListItem("Browse scripting files on server...", string.Empty));
			//}
		}


		public Control Editor
		{
			get { return this; }
		}

		private static void GetRazorFilesFromDir(string orgPath, string path, IList files)
		{
			var dirInfo = new DirectoryInfo(path);
			if (!dirInfo.Exists)
				return;

			var fileInfo = dirInfo.GetFiles("*.*");
			foreach (var file in fileInfo)
				files.Add(path.Replace(orgPath, string.Empty) + file.Name);

			// Populate subdirectories
			var dirInfos = dirInfo.GetDirectories();
			foreach (var dir in dirInfos)
				GetRazorFilesFromDir(orgPath, path + "/" + dir.Name + "/", files);
		}
	}
}