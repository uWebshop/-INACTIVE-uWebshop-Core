using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Examine;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.controls;
using uWebshop.Umbraco.DataTypes.StorePicker;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Constants = uWebshop.Common.Constants;
using DataTypeDefinition = umbraco.cms.businesslogic.datatype.DataTypeDefinition;
using Log = uWebshop.Domain.Log;

namespace uWebshop.Package.Installer
{
	public partial class uWebshopInstaller : UserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		public static IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
		public static IContentService ContentService = ApplicationContext.Current.Services.ContentService;

		protected void Page_Load(object sender, EventArgs e)
		{
			phInstallStorePicker.Controls.Add(NodePickerStore);

			if (IsPostBack)
			{
				return;
			}

			RemoveOldDLLs();

			DoLogged(MoveVersionSpecificFiles, "uWebshop Installer move (umbraco)version specific files");

            System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/bin/uWebshop.REMOVEME.txt"), "This file was created to be removed in a later stadium.");

			DoLogged(Domain.Core.Initialize.Reboot, "uWebshop Installer initialize uWebshop internal state");

			DoLogged(() => IO.Container.Resolve<IInstaller>().Install(), "uWebshop Installer Umbraco installer.Install()");

			DoLogged(MoveXMLFiles, "uWebshop Installer MoveXMLFiles");

			DoLogged(MoveRazorFiles, "uWebshop Installer MoveRazorFiles");

			DoLogged(MoveNEWConfigFiles, "uWebshop Installer MoveNEWConfigFiles");
			
			DoLogged(MoveConfigFiles, "uWebshop Installer MoveConfigFiles");

			DoLogged(MoveXLSTFiles, "uWebshop Installer MoveXLSTFiles");

			DoLogged(AlterWebConfig, "uWebshop Installer AlterWebConfig");

			DoLogged(AlterDashboardConfig, "uWebshop Installer AlterDashboardConfig");

			DoLogged(AddRedirectorModuleToWebConfig, "uWebshop Installer AddRedirectorModuleToWebConfig");
            
			try
			{
				DoLogged(() => ExamineManager.Instance.IndexProviderCollection[UwebshopConfiguration.Current.ExamineIndexer].RebuildIndex(), "uWebshop Installer RebuildIndex");
			}
			catch
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, UwebshopConfiguration.Current.ExamineIndexer + " Error", "Please Republish All uWebshop Nodes");
				Log.Instance.LogError("uWebshop Installer: Could Not Rebuild " + UwebshopConfiguration.Current.ExamineIndexer + "; Publish uWebshop node + children manually");
			}

		    try
		    {
		        System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/bin/uWebshop.REMOVEME.txt"));
		    }
		    catch
		    {
                // Not important
		    }

			Log.Instance.LogDebug("uWebshop installation/update completed");
		}

		private void DoLogged(Action action, string message)
		{	
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, message + " start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			action();
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, message + " end: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}

		private void RemoveOldDLLs()
		{
			var toDelete = new[] { "uWebshop.DataTypes", "uWebshop.ActionHandlers", "uWebshop.Examine", "uWebshop.Web", 
				"uWebshop.DictionaryInstaller", "uWebshop.Package.Installer", "uWebshop.Starterkits.Sandbox", 
				"uWebshop.Starterkits.MVCSandbox", "uWebshop.RazorExtensions", "uWebshop.XSLTExtensions", };
;
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer RemoveOldDLLs start");
			foreach (var filename in toDelete)
			{
				try
				{
					var fileLocation = HttpContext.Current.Server.MapPath("/bin/" + filename + ".dll");

					if (System.IO.File.Exists(fileLocation))
					{
						LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer RemoveOldDLLs, deleting " + fileLocation);
						System.IO.File.Delete(fileLocation);
					}
				}
				catch (Exception ex)
				{
					LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer RemoveOldDLLs, failed " + filename, ex);
				}
			}
		}

		private static void AddRedirectorModuleToWebConfig()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add web.config update Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			try
			{
				var webConfigPath = HttpContext.Current.Server.MapPath("~/web.config");

				var webConfig = XDocument.Load(webConfigPath);

				var webconfigSafe = false;

				if (!webConfig.Descendants("httpModules").Descendants("add").Any(x => x.Attribute("name") != null && x.Attribute("name").Value == "uWebshopRedirectorModule"))
				{
					var uWebshopRedirectorModule = new XElement("add", new XAttribute("name", "uWebshopRedirectorModule"), new XAttribute("type", "uWebshop.ActionHandlers.UrlRewriting"));

					webConfig.Descendants("httpModules").First().AddFirst(uWebshopRedirectorModule);

					webconfigSafe = true;
				}

				if (!webConfig.Descendants("system.webServer").Descendants("add").Any(x => x.Attribute("name") != null && x.Attribute("name").Value == "uWebshopRedirectorModule"))
				{
					var uWebshopRemoveModule = new XElement("remove", new XAttribute("name", "uWebshopRedirectorModule"));
					var uWebshopRedirectorModule = new XElement("add", new XAttribute("name", "uWebshopRedirectorModule"), new XAttribute("type", "uWebshop.ActionHandlers.UrlRewriting"));

					webConfig.Descendants("system.webServer").First().Elements("modules").First().AddFirst(uWebshopRedirectorModule);

					//<remove name="uWebshopRedirectorModule" />
					webConfig.Descendants("system.webServer").First().Elements("modules").First().AddFirst(uWebshopRemoveModule);

					webconfigSafe = true;
				}

				if (webconfigSafe)
				{
					webConfig.Save(webConfigPath);
				}
			}
			catch
			{
				Log.Instance.LogError("httpModules Not Added To Web.Config. Permission issue?");
			}

			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add web.config update End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void AlterDashboardConfig()
		{

			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add dashboardConfig Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var dashboardConfigPath = HttpContext.Current.Server.MapPath("~/config/Dashboard.config");

			var dashboardConfig = XDocument.Load(dashboardConfigPath);

			if (!dashboardConfig.Descendants("section").Any(x => x.Attribute("alias") != null && x.Attribute("alias").Value == "uWebshopDashBoard"))
			{
				var uWebshopDashBoard = new XElement("section", new XAttribute("alias", "uWebshopDashBoard"), new XElement("areas", new XElement("area", "developer")), new XElement("tab", new XAttribute("caption", "uWebshop"), new XElement("control", "/App_Plugins/uWebshop/dashboardcontrols/uWebshopDashBoard.ascx")));

				dashboardConfig.Descendants("dashBoard").First().Add(uWebshopDashBoard);
				dashboardConfig.Save(dashboardConfigPath);
			}

			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add dashboardConfig End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void AlterWebConfig()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add uWebshopDebugMessages Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			try
			{
				if (ConfigurationManager.AppSettings["uWebshopDebugMessages"] == null)
				{
					ConfigurationManager.AppSettings.Add("uWebshopDebugMessages", "true");
				}
			}
			catch
			{
				Log.Instance.LogError("uWebshopDebugMessages=Try Not Added To AppSettings, Web.Config permission issue?");
			}

			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer add uWebshopDebugMessages End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void MoveXLSTFiles()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveXsltItemList Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var moveXsltItemList = new List<string> {"uwbsOrderInfo", "uwbsMailExample", "uwbsAccountMailExample"};

			foreach (var xsltItem in moveXsltItemList)
			{
				var configSource = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/temp/{0}.xslt", xsltItem));
				var configDestination = HttpContext.Current.Server.MapPath(string.Format("/xslt/{0}.xslt", xsltItem));

				if (!System.IO.File.Exists(configDestination))
				{
					System.IO.File.Move(configSource, configDestination);
				}
				else
				{
					System.IO.File.Delete(configSource);
				}
			}
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveXsltItemList End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void MoveConfigFiles()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveConfigItemList Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var moveConfigItemList = new List<string> {"PaymentProviders", "ShippingProviders"};

			foreach (var configItem in moveConfigItemList)
			{
				var oldConfigFile = HttpContext.Current.Server.MapPath(string.Format("/config/uWebshop/{0}.config", configItem));
				var configSource = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/temp/{0}.config", configItem));
				var configDestination = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/config/{0}.config", configItem));

				if (System.IO.File.Exists(oldConfigFile))
				{
					if (System.IO.File.Exists(configDestination))
					{
						System.IO.File.Delete(configDestination);
					}

					System.IO.File.Move(oldConfigFile, configDestination);

					System.IO.File.Delete(configSource);
				}
				else
				{
					if (!System.IO.File.Exists(configDestination))
					{
						System.IO.File.Move(configSource, configDestination);
					}
					else
					{
						System.IO.File.Delete(configSource);
					}
				}
			}
			
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveConfigItemList End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void MoveNEWConfigFiles()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveNEWConfigItemList Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var moveConfigItemList = new List<string> { "CurrencyCultures", "ContentMapping" };

			foreach (var configItem in moveConfigItemList)
			{
				var configSource = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/temp/{0}.config", configItem));
				var configDestination = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/config/{0}.config", configItem));
				var destinationDir = HttpContext.Current.Server.MapPath("/App_Plugins/uWebshop/config/");

				if (!System.IO.File.Exists(configDestination))
				{
					if (!System.IO.Directory.Exists(destinationDir))
					{
						System.IO.Directory.CreateDirectory(destinationDir);
					}

					System.IO.File.Move(configSource, configDestination);
				}
				else
				{
					System.IO.File.Delete(configSource);
				}
			}

			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,  "uWebshop Installer moveNEWConfigItemList End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void MoveXMLFiles()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,"uWebshop Installer moveXmlItemList Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var moveXmlItemList = new List<string> {"countries", "VATcountries", "regions"};

			foreach (var configItem in moveXmlItemList)
			{
				var configSource = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/temp/{0}.xml", configItem));
				var configDestination = HttpContext.Current.Server.MapPath(string.Format("/scripts/uWebshop/{0}.xml", configItem));

				if (!System.IO.File.Exists(configDestination))
				{
					System.IO.File.Move(configSource, configDestination);
				}
				else
				{
					System.IO.File.Delete(configSource);
				}
			}
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer moveXmlItemList End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
		}

		private static void MoveRazorFiles()
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer MoveRazorFiles Start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

			var moveXmlItemList = new List<string> { "uWebshopUmbracoEmailDetails", "uWebshopUmbracoOrderDetails", "uWebshopUmbracoOrderOverview", "uWebshopUmbracoProductOverview" };

			foreach (var configItem in moveXmlItemList)
			{
				var configSource = HttpContext.Current.Server.MapPath(string.Format("/App_Plugins/uWebshop/temp/{0}.cshtml", configItem));

				var dirDestination = HttpContext.Current.Server.MapPath("/MacroScripts/uWebshopBackend/");
				var configDestination = HttpContext.Current.Server.MapPath(string.Format("/MacroScripts/uWebshopBackend/{0}.cshtml", configItem));

				if (!Directory.Exists(dirDestination))
				{
					Directory.CreateDirectory(dirDestination);
				}

				if (!System.IO.File.Exists(configDestination))
				{
					System.IO.File.Move(configSource, configDestination);
				}
				else
				{
					System.IO.File.Delete(configSource);
				}
			}
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "uWebshop Installer MoveRazorFiles End: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));

		}

		private static void MoveVersionSpecificFiles()
		{

		}

		protected ContentPicker NodePickerStore = new ContentPicker {ID = "nodePickerStore", AppAlias = "content", ClientIDMode = ClientIDMode.Static};

		public static List<IContentType> DocumentTypeListWithoutuWebshopDocuments
		{
			get
			{
				var docTypeList = ContentTypeService.GetAllContentTypes();

				return docTypeList.Where(docType => DocumentTypeAliasList.List.Contains(docType.Alias)).ToList();
			}
		}

		protected void BtnInstallStoreClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtStoreAlias.Text))
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "Error", "No Store name entered");
			}
			else
			{
				Umbraco.Helpers.InstallStore(txtStoreAlias.Text, true);

				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Store Installed!", "Your " + txtStoreAlias.Text + " Store is installed!");
			}
		}

		protected void BtnStorePickerCLick(object sender, EventArgs e)
		{
			var contentService = ApplicationContext.Current.Services.ContentService;
			var nodePicker = NodePickerStore;

			if (!string.IsNullOrEmpty(nodePicker.Value))
			{
				var doc = contentService.GetById(int.Parse(nodePicker.Value));
				var selectedDoctype = doc.ContentType;

				var storePickerDataTypeDef = new StorePickerDataType();
				var storePickerDataType = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionById(storePickerDataTypeDef.Id);

				if (storePickerDataType != null)
				{
					if (selectedDoctype.PropertyTypes.All(x => x.Alias != Constants.StorePickerAlias))
					{
						var storePickerPropertyType = new PropertyType(storePickerDataType)
						{
							Alias = Constants.StorePickerAlias,
							Name = "#StorePicker"
						};

						selectedDoctype.AddPropertyType(storePickerPropertyType);

						BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "StorePicker Installed!", "StorePicker installed on " + doc.ContentType.Alias + " document type");
					}
					else
					{
						BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "StorePicker Not Installed!", "StorePicker already on " + doc.ContentType.Alias + " document type");
					}
				}

				else
				{
					BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "StorePicker Install Failed", "StorePicker install failed on " + doc.ContentType.Alias + " document type");
				}
			}
			else
			{
				BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.error, "StorePicker Install Failed", "StorePicker install failed");
			}
		}
	}
}