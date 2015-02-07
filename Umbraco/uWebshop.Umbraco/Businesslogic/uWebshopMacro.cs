using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.CSharp.RuntimeBinder;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.member;
using umbraco.DataLayer;
using umbraco.IO;
using umbraco.NodeFactory;
using umbraco.presentation.templateControls;
using Macro = umbraco.cms.businesslogic.macro.Macro;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// AV: This copies a LOT from Umbraco Macro, but needed to make razor render properly in code and backend and have full control over input and output
	/// </summary>
	internal class uWebshopMacro
	{
		private readonly StringBuilder _mContent = new StringBuilder();
		private readonly Cache _macroCache = HttpRuntime.Cache;

		private const string MacroRuntimeCacheKey = "UmbracoRuntimeMacroCache";

		private const string MacrosAddedKey = "macrosAdded";
		public IList<Exception> Exceptions = new List<Exception>();

		// Macro-elements
		protected static ISqlHelper SqlHelper
		{
			get { return Application.SqlHelper; }
		}

		#region public properties

		public bool CacheByPersonalization
		{
			get { return Model.CacheByMember; }
		}

		public bool CacheByPage
		{
			get { return Model.CacheByPage; }
		}

		public bool DontRenderInEditor
		{
			get { return !Model.RenderInEditor; }
		}

		public int RefreshRate
		{
			get { return Model.CacheDuration; }
		}

		public String Alias
		{
			get { return Model.Alias; }
		}

		public String Name
		{
			get { return Model.Name; }
		}

		public String XsltFile
		{
			get { return Model.Xslt; }
		}

		public String ScriptFile
		{
			get { return Model.ScriptName; }
		}

		public String ScriptType
		{
			get { return Model.TypeName; }
		}

		public String ScriptAssembly
		{
			get { return Model.TypeName; }
		}

		public int MacroType
		{
			get { return (int) Model.MacroType; }
		}

		public String MacroContent
		{
			set { _mContent.Append(value); }
			get { return _mContent.ToString(); }
		}

		#endregion

		#region REFACTOR

		/// <summary>
		/// Creates a macro object
		/// </summary>
		/// <param name="id">Specify the macro-id which should be loaded (from table macro)</param>
		public uWebshopMacro(int id)
		{
			var m = Macro.GetById(id);
			Model = new MacroModel(m);
		}

		public uWebshopMacro(string alias)
		{
			var m = Macro.GetByAlias(alias);
			Model = new MacroModel(m);
		}

		public MacroModel Model { get; set; }

		public static uWebshopMacro GetMacro(string alias)
		{
			return new uWebshopMacro(alias);
		}

		public static uWebshopMacro GetMacro(int id)
		{
			return new uWebshopMacro(id);
		}

		#endregion

		/// <summary>
		/// Creates an empty macro object.
		/// </summary>
		public uWebshopMacro()
		{
			Model = new MacroModel();
		}


		public override string ToString()
		{
			return Model.Name;
		}

		private static string GetCacheKey(string alias)
		{
			return MacroRuntimeCacheKey + alias;
		}

		/// <summary>
		/// Deletes macro definition from cache.
		/// </summary>
		/// <returns>True if succesfull, false if nothing has been removed</returns>
		public bool RemoveFromCache()
		{
			if (Model.Id > 0)
			{
				umbraco.cms.businesslogic.cache.Cache.ClearCacheItem(GetCacheKey(Model.Alias));
			}
			return false;
		}

		private string GetCacheIdentifier(MacroModel model)
		{
			var id = new StringBuilder();

			var alias = string.IsNullOrEmpty(model.ScriptCode) ? model.Alias : Macro.GenerateCacheKeyFromCode(model.ScriptCode);
			id.AppendFormat("{0}-", alias);


			if (CacheByPersonalization)
			{
				var currentMember = Member.GetCurrentMember();
				id.AppendFormat("m{0}-", currentMember == null ? 0 : currentMember.Id);
			}

			foreach (var prop in model.Properties)
			{
				var propValue = prop.Value;
				id.AppendFormat("{0}-", propValue.Length <= 255 ? propValue : propValue.Substring(0, 255));
			}

			return id.ToString();
		}

		public Control RenderMacro(Hashtable attributes, Hashtable pageElements, int documentId)
		{
			// TODO: Parse attributes
			UpdateMacroModel(attributes);
			return RenderMacro(pageElements, documentId);
		}

		public Control RenderMacro(Hashtable pageElements, int documentId)
		{
			TraceInfo("renderMacro", string.Format("Rendering started (macro: {0}, type: {1}, cacheRate: {2})", Name, MacroType, Model.CacheDuration));
			
			StateHelper.SetContextValue(MacrosAddedKey, StateHelper.GetContextValue<int>(MacrosAddedKey) + 1);

			String macroHtml = null;
			Control macroControl = null;

			// zb-00037 #29875 : parse attributes here (and before anything else)
			foreach (var prop in Model.Properties)
				prop.Value = helper.parseAttribute(pageElements, prop.Value);

			Model.CacheIdentifier = GetCacheIdentifier(Model);


			if (Model.CacheDuration > 0)
			{
				if (CacheMacroAsString(Model))
				{
					macroHtml = _macroCache["macroHtml_" + Model.CacheIdentifier] as String;

					// FlorisRobbemont: 
					// An empty string means: macroHtml has been cached before, but didn't had any output (Macro doesn't need to be rendered again)
					// An empty reference (null) means: macroHtml has NOT been cached before
					if (macroHtml != null)
					{
						if (MacroNeedsToBeClearedFromCache(Model, "macroHtml_DateAdded_" + Model.CacheIdentifier))
						{
							macroHtml = null;
							TraceInfo("renderMacro", string.Format("Macro removed from cache due to file change '{0}'.", Model.CacheIdentifier));
						}
						else
						{
							TraceInfo("renderMacro", string.Format("Macro Content loaded from cache '{0}'.", Model.CacheIdentifier));
						}
					}
				}
				else
				{
					var cacheContent = _macroCache["macroControl_" + Model.CacheIdentifier] as MacroCacheContent;

					if (cacheContent != null)
					{
						macroControl = cacheContent.Content;
						macroControl.ID = cacheContent.ID;

						if (MacroNeedsToBeClearedFromCache(Model, "macroControl_DateAdded_" + Model.CacheIdentifier))
						{
							TraceInfo("renderMacro", string.Format("Macro removed from cache due to file change '{0}'.", Model.CacheIdentifier));
							macroControl = null;
						}
						else
						{
							TraceInfo("renderMacro", string.Format("Macro Control loaded from cache '{0}'.", Model.CacheIdentifier));
						}
					}
				}
			}

			// FlorisRobbemont: Empty macroHtml (not null, but "") doesn't mean a re-render is necessary
			if (macroHtml == null && macroControl == null)
			{
				var renderFailed = false;
				var macroType = Model.MacroType != MacroTypes.Unknown ? (int) Model.MacroType : MacroType;
				switch (macroType)
				{
					case (int) MacroTypes.XSLT:
						macroControl = loadMacroXSLT(this, Model, pageElements);
						break;
					case (int) MacroTypes.Script:
						try
						{
							TraceInfo("umbracoMacro", "MacroEngine script added (" + ScriptFile + ")");

							var result = LoadMacroScript(Model, documentId);
							macroControl = new LiteralControl(result.Result);
							if (result.ResultException != null)
							{
								// we'll throw the error if we run in release mode, show details if we're in release mode!
								renderFailed = true;
								if (HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled)
									throw result.ResultException;
							}
							break;
						}
						catch (Exception e)
						{
							renderFailed = true;
							Exceptions.Add(e);
							Log.Instance.LogError("RenderMacro: " + e);

							var result = new LiteralControl("Error loading MacroEngine script (file: " + ScriptFile + ")");

							macroControl = result;

							break;
						}
					default:
						if (GlobalSettings.DebugMode)
							macroControl = new LiteralControl("&lt;Macro: " + Name + " (" + ScriptAssembly + "," + ScriptType + ")&gt;");
						break;
				}

				// Add result to cache if successful
				if (!renderFailed && Model.CacheDuration > 0)
				{
					// do not add to cache if there's no member and it should cache by personalization
					if (!Model.CacheByMember || (Model.CacheByMember && Member.GetCurrentMember() != null))
					{
						if (macroControl != null)
						{
							// NH: Scripts and XSLT can be generated as strings, but not controls as page events wouldn't be hit (such as Page_Load, etc)
							if (CacheMacroAsString(Model))
							{
								string outputCacheString;

								using (var sw = new StringWriter())
								{
									var hw = new HtmlTextWriter(sw);
									macroControl.RenderControl(hw);

									outputCacheString = sw.ToString();
								}

								_macroCache.Insert("macroHtml_" + Model.CacheIdentifier, outputCacheString, null, DateTime.Now.AddSeconds(Model.CacheDuration), TimeSpan.Zero, CacheItemPriority.NotRemovable, //FlorisRobbemont: issue #27610 -> Macro output cache should not be removable

												   null);


								_macroCache.Insert("macroHtml_DateAdded_" + Model.CacheIdentifier, DateTime.Now, null, DateTime.Now.AddSeconds(Model.CacheDuration), TimeSpan.Zero, CacheItemPriority.NotRemovable, //FlorisRobbemont: issue #27610 -> Macro output cache should not be removable
												   null);

								// zb-00003 #29470 : replace by text if not already text
								// otherwise it is rendered twice
								if (!(macroControl is LiteralControl))
									macroControl = new LiteralControl(outputCacheString);

								TraceInfo("renderMacro", string.Format("Macro Content saved to cache '{0}'.", Model.CacheIdentifier));
							}

							else
							{
								_macroCache.Insert("macroControl_" + Model.CacheIdentifier, new MacroCacheContent(macroControl, macroControl.ID), null, DateTime.Now.AddSeconds(Model.CacheDuration), TimeSpan.Zero, CacheItemPriority.NotRemovable, //FlorisRobbemont: issue #27610 -> Macro output cache should not be removable
												   null);


								_macroCache.Insert("macroControl_DateAdded_" + Model.CacheIdentifier, DateTime.Now, null, DateTime.Now.AddSeconds(Model.CacheDuration), TimeSpan.Zero, CacheItemPriority.NotRemovable, //FlorisRobbemont: issue #27610 -> Macro output cache should not be removable
												   null);

								TraceInfo("renderMacro", string.Format("Macro Control saved to cache '{0}'.", Model.CacheIdentifier));
							}
						}
					}
				}
			}
			else if (macroControl == null)
			{
				macroControl = new LiteralControl(macroHtml);
			}
			
			return macroControl;
		}

		public Control loadMacroXSLT(uWebshopMacro macro, MacroModel model, Hashtable pageElements)
		{
			if (XsltFile.Trim() != string.Empty)
			{
				// Get main XML
				XmlDocument umbracoXML = content.Instance.XmlContent;

				// Create XML document for Macro
				var macroXML = new XmlDocument();
				macroXML.LoadXml("<macro/>");

				foreach (var prop in macro.Model.Properties)
				{
					addMacroXmlNode(umbracoXML, macroXML, prop.Key, prop.Type, prop.Value);
				}

				if (HttpContext.Current.Request.QueryString["umbDebug"] != null && GlobalSettings.DebugMode)
				{
					return new LiteralControl("<div style=\"border: 2px solid green; padding: 5px;\"><b>Debug from " + macro.Name + "</b><br/><p>" + HttpContext.Current.Server.HtmlEncode(macroXML.OuterXml) + "</p></div>");
				}
				try
				{
					XslCompiledTransform xsltFile = umbraco.macro.getXslt(XsltFile);

					try
					{
						Control result = CreateControlsFromText(umbraco.macro.GetXsltTransformResult(macroXML, xsltFile));

						TraceInfo("umbracoMacro", "After performing transformation");

						return result;
					}
					catch (Exception e)
					{
						Exceptions.Add(e);
						// inner exception code by Daniel Lindstr?m from SBBS.se
						Exception ie = e;
						while (ie != null)
						{
							TraceWarn("umbracoMacro InnerException", ie.Message, ie);
							ie = ie.InnerException;
						}
						return new LiteralControl("Error parsing XSLT file: \\xslt\\" + XsltFile);
					}
				}
				catch (Exception e)
				{
					Exceptions.Add(e);
					return new LiteralControl("Error reading XSLT file: \\xslt\\" + XsltFile);
				}
			}
			TraceWarn("macro", "Xslt is empty");
			return new LiteralControl(string.Empty);
		}

		private void addMacroXmlNode(XmlDocument umbracoXML, XmlDocument macroXML, String macroPropertyAlias, String macroPropertyType, String macroPropertyValue)
		{
			XmlNode macroXmlNode = macroXML.CreateNode(XmlNodeType.Element, macroPropertyAlias, string.Empty);
			var x = new XmlDocument();

			int currentID = -1;
			// If no value is passed, then use the current pageID as value
			if (macroPropertyValue == string.Empty)
			{
				var umbPage = (page) HttpContext.Current.Items["umbPageObject"];
				if (umbPage == null)
					return;
				currentID = umbPage.PageID;
			}

			TraceInfo("umbracoMacro", "Xslt node adding search start (" + macroPropertyAlias + ",'" + macroPropertyValue + "')");
			switch (macroPropertyType)
			{
				case "contentTree":
					XmlAttribute nodeID = macroXML.CreateAttribute("nodeID");
					if (macroPropertyValue != string.Empty)
						nodeID.Value = macroPropertyValue;
					else
						nodeID.Value = currentID.ToString();
					macroXmlNode.Attributes.SetNamedItem(nodeID);

					// Get subs
					try
					{
						macroXmlNode.AppendChild(macroXML.ImportNode(umbracoXML.GetElementById(nodeID.Value), true));
					}
					catch
					{
					}
					break;
				case "contentCurrent":
					x.LoadXml("<nodes/>");
					XmlNode currentNode;
					if (macroPropertyValue != string.Empty)
						currentNode = macroXML.ImportNode(umbracoXML.GetElementById(macroPropertyValue), true);
					else
						currentNode = macroXML.ImportNode(umbracoXML.GetElementById(currentID.ToString()), true);

					// remove all sub content nodes
					foreach (XmlNode n in currentNode.SelectNodes("node|*[@isDoc]"))
						currentNode.RemoveChild(n);

					macroXmlNode.AppendChild(currentNode);

					break;
				case "contentSubs":
					x.LoadXml("<nodes/>");
					if (macroPropertyValue != string.Empty)
						x.FirstChild.AppendChild(x.ImportNode(umbracoXML.GetElementById(macroPropertyValue), true));
					else
						x.FirstChild.AppendChild(x.ImportNode(umbracoXML.GetElementById(currentID.ToString()), true));
					macroXmlNode.InnerXml = transformMacroXML(x, "macroGetSubs.xsl");
					break;
				case "contentAll":
					x.ImportNode(umbracoXML.DocumentElement.LastChild, true);
					break;
				case "contentRandom":
					XmlNode source = umbracoXML.GetElementById(macroPropertyValue);
					if (source != null)
					{
						XmlNodeList sourceList = source.SelectNodes("node|*[@isDoc]");
						if (sourceList.Count > 0)
						{
							int rndNumber;
							Random r = library.GetRandom();
							lock (r)
							{
								rndNumber = r.Next(sourceList.Count);
							}
							XmlNode node = macroXML.ImportNode(sourceList[rndNumber], true);
							// remove all sub content nodes
							foreach (XmlNode n in node.SelectNodes("node|*[@isDoc]"))
								node.RemoveChild(n);

							macroXmlNode.AppendChild(node);
							break;
						}
						else
							TraceWarn("umbracoMacro", "Error adding random node - parent (" + macroPropertyValue + ") doesn't have children!");
					}
					else
						TraceWarn("umbracoMacro", "Error adding random node - parent (" + macroPropertyValue + ") doesn't exists!");
					break;
				case "mediaCurrent":
					var c = new umbraco.cms.businesslogic.Content(int.Parse(macroPropertyValue));
					macroXmlNode.AppendChild(macroXML.ImportNode(c.ToXml(content.Instance.XmlContent, false), true));
					break;
				default:
					macroXmlNode.InnerText = HttpContext.Current.Server.HtmlDecode(macroPropertyValue);
					break;
			}
			macroXML.FirstChild.AppendChild(macroXmlNode);
		}
		
		private string transformMacroXML(XmlDocument xmlSource, string xslt_File)
		{
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);

			XslCompiledTransform result = macro.getXslt(xslt_File);

			result.Transform(xmlSource.CreateNavigator(), null, sw);

			return sw.ToString() != string.Empty ? sw.ToString() : string.Empty;
		}

		/// <summary>
		/// check that the file has not recently changed
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="dateAddedKey">The date added key.</param>
		private bool MacroNeedsToBeClearedFromCache(MacroModel model, string dateAddedKey)
		{
			if (MacroIsFileBased(model))
			{
				if (_macroCache[dateAddedKey] != null)
				{
					var dateMacroAdded = DateTime.Parse(_macroCache[dateAddedKey].ToString());

					var macroFile = GetMacroFile(model);
					var fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(macroFile));


					if (fileInfo.LastWriteTime.CompareTo(dateMacroAdded) == 1)
					{
						TraceInfo("renderMacro", string.Format("Macro needs to be removed from cache due to file change '{0}'.", model.CacheIdentifier));
						return true;
					}
				}
			}

			return false;
		}

		private static string GetMacroFile(MacroModel model)
		{
			if (model.ScriptName != string.Empty)
			{
				return string.Concat(string.Format("/macroScripts/{0}", model.ScriptName));
			}

			return "/" + model.TypeName;
		}

		private static bool MacroIsFileBased(MacroModel model)
		{
			return model.MacroType != MacroTypes.CustomControl;
		}

		private static bool CacheMacroAsString(MacroModel model)
		{
			return model.MacroType == MacroTypes.Script;
		}
		
		public void UpdateMacroModel(Hashtable attributes)
		{
			foreach (var mp in Model.Properties)
			{
				mp.Value = attributes.ContainsKey(mp.Key.ToLower()) ? attributes[mp.Key.ToLower()].ToString() : string.Empty;
			}
		}

		public void GenerateMacroModelPropertiesFromAttributes(Hashtable attributes)
		{
			foreach (string key in attributes.Keys)
			{
				Model.Properties.Add(new MacroPropertyModel(key, attributes[key].ToString()));
			}
		}

		/// <summary>
		/// Parses the text for umbraco Item controls that need to be rendered.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <returns>A control containing the parsed text.</returns>
		protected Control CreateControlsFromText(string text)
		{
			// the beginning and end tags
			const string tagStart = "[[[[umbraco:Item";
			const string tagEnd = "]]]]";

			// container that will hold parsed controls
			var container = new PlaceHolder();

			// loop through all text
			var textPos = 0;
			while (textPos < text.Length)
			{
				// try to find an item tag, carefully staying inside the string bounds (- 1)
				var tagStartPos = text.IndexOf(tagStart, textPos);
				var tagEndPos = tagStartPos < 0 ? -1 : text.IndexOf(tagEnd, tagStartPos + tagStart.Length - 1);

				// item tag found?
				if (tagStartPos >= 0 && tagEndPos >= 0)
				{
					// add the preceding text as a literal control
					if (tagStartPos > textPos)
						container.Controls.Add(new LiteralControl(text.Substring(textPos, tagStartPos - textPos)));

					// extract the tag and parse it
					var tag = text.Substring(tagStartPos, (tagEndPos + tagEnd.Length) - tagStartPos);
					var attributes = helper.ReturnAttributes(tag);

					// create item with the parameters specified in the tag
					var item = new Item {NodeId = helper.FindAttribute(attributes, "nodeid"), Field = helper.FindAttribute(attributes, "field"), Xslt = helper.FindAttribute(attributes, "xslt"), XsltDisableEscaping = helper.FindAttribute(attributes, "xsltdisableescaping") == "true"};
					container.Controls.Add(item);

					// advance past the end of the tag
					textPos = tagEndPos + tagEnd.Length;
				}
				else
				{
					// no more tags found, just add the remaning text
					container.Controls.Add(new LiteralControl(text.Substring(textPos)));
					textPos = text.Length;
				}
			}
			return container;
		}

		///// <summary>
		///// Renders a Partial View Macro
		///// </summary>
		///// <param name="macro"></param>
		///// <param name="nodeId"></param>
		///// <returns></returns>
		//internal ScriptingMacroResult LoadPartialViewMacro(MacroModel macro, int nodeId)
		//{
		//	var retVal = new ScriptingMacroResult();
		//	TraceInfo("umbracoMacro", "Rendering Partial View Macro");

		//	var engine = MacroEngineFactory.GetEngine("Partial View Macro Engine");//PartialViewMacroEngine.EngineName);
		//	var ret = engine.Execute(macro, new Node(nodeId));

		//	// if the macro engine supports success reporting and executing failed, then return an empty control so it's not cached
		//	if (engine is IMacroEngineResultStatus)
		//	{
		//		var result = engine as IMacroEngineResultStatus;
		//		if (!result.Success)
		//		{
		//			retVal.ResultException = result.ResultException;
		//		}
		//	}
		//	TraceInfo("umbracoMacro", "Rendering Partial View Macro [done]");
		//	retVal.Result = ret;
		//	return retVal;
		//}

		public ScriptingMacroResult LoadMacroScript(MacroModel macro, int currentPageId)
		{
			Log.Instance.LogDebug("LoadMacroScript macro.Alias: " + macro.Alias);
			Log.Instance.LogDebug("LoadMacroScript macro.ScriptCode: " + macro.ScriptCode);
			Log.Instance.LogDebug("LoadMacroScript macro.ScriptName: " + macro.ScriptName);

			var retVal = new ScriptingMacroResult();
			TraceInfo("umbracoMacro", "Loading IMacroEngine script");
			string ret;
			IMacroEngine engine;
			if (!string.IsNullOrEmpty(macro.ScriptCode))
			{
				Log.Instance.LogDebug("LoadMacroScript engine.ScriptLanguage: " + macro.ScriptLanguage);
				
				engine = MacroEngineFactory.GetByExtension(macro.ScriptLanguage);

				Log.Instance.LogDebug("LoadMacroScript engine.Name: " + engine.Name);

				ret = engine.Execute(macro, new Node(currentPageId));
			}
			else
			{
				var path = macro.ScriptName;

				if (!macro.ScriptName.StartsWith("~"))
				{
					path = SystemDirectories.MacroScripts.TrimEnd('/') + "/" + macro.ScriptName.TrimStart('/');
				}
				
				Log.Instance.LogDebug("LoadMacroScript path: " + path);

				engine = MacroEngineFactory.GetByFilename(path);

				Log.Instance.LogDebug("LoadMacroScript engine.Name: " + engine.Name);

				ret = engine.Execute(macro, new Node(currentPageId));
			}

			// if the macro engine supports success reporting and executing failed, then return an empty control so it's not cached
			if (engine is IMacroEngineResultStatus)
			{
				var result = engine as IMacroEngineResultStatus;
				if (!result.Success)
				{
					retVal.ResultException = result.ResultException;
				}
			}
			TraceInfo("umbracoMacro", "Loading IMacroEngine script [done]");
			retVal.Result = ret;
			return retVal;
		}

		private static void TraceInfo(string category, string message)
		{
			if (HttpContext.Current != null)
				HttpContext.Current.Trace.Write(category, message);
		}

		private static void TraceWarn(string category, string message)
		{
			if (HttpContext.Current != null)
				HttpContext.Current.Trace.Warn(category, message);
		}

		private static void TraceWarn(string category, string message, Exception ex)
		{
			if (HttpContext.Current != null)
				HttpContext.Current.Trace.Warn(category, message, ex);
		}
	}

	internal static class RazorLibraryExtensions
	{
		public static string RenderControlToString(this Control cont)
		{
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);

			var hw = new HtmlTextWriter(sw);
			cont.RenderControl(hw);

			return sb.ToString();
		}

		public static string RenderMacro(string aliasOrPath, int documentId, params object[] properties)
		{
			Log.Instance.LogDebug("uWebshop RenderMacro aliasOrPath: " + aliasOrPath);
			Log.Instance.LogDebug("uWebshop RenderMacro documentId: " + documentId);

			return GetMacroControl(aliasOrPath, documentId, properties).RenderControlToString();
		}

		public static Control GetMacroControl(string aliasOrPath, int documentId, params object[] properties)
		{
			//converting object[] into a single hash table merging common property names,
			//overwriting previously inserted values.

			//stupid macro propertiespropertyAliases are not case sensative !!!
			var attribs = new Hashtable();
			if (properties != null)
			{
				foreach (var prop in properties)
				{
					var dic = prop as IDictionary ?? prop.ToDictionary();

					foreach (var d in dic.Keys)
					{
						var key = d.ToString().ToLower();
						if (attribs.ContainsKey(key))
							attribs[key] = dic[d];
						else
							attribs.Add(key, dic[d]);
					}
				}
			}


			uWebshopMacro macro;
			
			//fit is starts with ~/ then we assume we are trying to render a file and not an aliased macro
			if (aliasOrPath.StartsWith("~/"))
			{
				//this code is adapted from the macro user control
				macro = new uWebshopMacro();
				macro.GenerateMacroModelPropertiesFromAttributes(attribs);
				macro.Model.ScriptName = aliasOrPath;
				macro.Model.MacroType = MacroTypes.Script;
			}
			else
			{
				macro = uWebshopMacro.GetMacro(aliasOrPath);
			}

			if (macro.Model.ScriptName == null)
			{
				macro = new uWebshopMacro();
				macro.GenerateMacroModelPropertiesFromAttributes(attribs);
				macro.Model.ScriptName = aliasOrPath;
				macro.Model.MacroType = MacroTypes.Script;
			}
			
			macro.UpdateMacroModel(attribs);
			
			var result = macro.RenderMacro(attribs, documentId);

			var exceptions = macro.Exceptions;

			if (exceptions.Any())
			{
				Log.Instance.LogError("GetMacroControl Error in Razor: " + aliasOrPath + " Exception: " + exceptions.First().StackTrace);
				throw new Exception(exceptions.First().StackTrace);
			}

			var keyValueDictionary = new Dictionary<string, string>();

			foreach (var key in HttpContext.Current.Request.Form.AllKeys)
			{
				if (key != null && !key.StartsWith("ctl00$") && !key.StartsWith("body_TabView") && !key.StartsWith("__EVENT") && !key.StartsWith("__VIEWSTATE") && !key.StartsWith("__ASYNCPOST"))
				{
					var value = HttpContext.Current.Request.Form[key];

					if (value != null) keyValueDictionary.Add(key, value);

					
				}
			}

			if (HttpContext.Current.Session != null)
			{
				HttpContext.Current.Session.Add("RazorFields", keyValueDictionary);
			}


			return result;
		}

		internal static HttpSessionState Session
		{
			get { return HttpContext.Current.Session; }
		}

		/// <summary>
		///  Convert an object into a dictionary
		/// </summary>
		/// <param name="object">The object</param>
		/// <returns></returns>
		public static Dictionary<string, object> ToDictionary(this object @object)
		{
			if (@object == null)
			{
				return new Dictionary<string, object>();
			}

			var properties = TypeDescriptor.GetProperties(@object);
			var dyn = @object as DynamicObject;

			var count = properties.Count;
			List<string> dynNames = null;
			if (dyn != null)
			{
				dynNames = dyn.GetDynamicMemberNames().ToList();
				count += dynNames.Count;
			}

			var hash = new Dictionary<string, object>(count);

			foreach (PropertyDescriptor descriptor in properties)
			{
				var key = descriptor.Name;
				var value = descriptor.GetValue(@object);

				hash.Add(key, value);
			}

			if (dynNames != null)
			{
				var objType = @object.GetType();

				foreach (var dynName in dynNames)
				{
					var key = dynName;
					var value = GetValue(dynName, dyn, objType);

					hash.Add(key, value);
				}
			}
			return hash;
		}

		public static object GetValue(string name, DynamicObject dyn, Type objType)
		{
			try
			{
				var callSite = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, name, objType, new[] {CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)}));

				return callSite.Target(callSite, dyn);
			}
			catch (RuntimeBinderException)
			{
				return null;
			}
		}
	}
}