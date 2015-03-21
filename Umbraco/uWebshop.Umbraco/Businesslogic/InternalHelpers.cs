using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using umbraco;
using umbraco.BusinessLogic;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Umbraco6;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using File = uWebshop.Domain.File;
using Log = umbraco.BusinessLogic.Log;

namespace uWebshop.Umbraco.Businesslogic
{
	internal class InternalHelpers
	{
		public static IMediaService MediaService = ApplicationContext.Current.Services.MediaService;

		internal static string[] ParseTagsString(string property)
		{
			var valueList = new List<string>();

			if (!string.IsNullOrWhiteSpace(property))
			{
				if (property.Contains(","))
				{
					valueList.AddRange(property.Split(',').ToList());
				}
                else if (property.Contains(" ") && !property.Contains(","))
				{
					valueList.AddRange(property.Split(' ').ToList());
				}
				else
				{
					valueList.Add(property);
				}
			}

			return valueList.ToArray();
		}

		public static string GetFileNameForIcon(ContentIcon icon)
		{
			// to build from package xml:
			// find with regex: (<file.*\r?\n.*<guid?)(.*)(</guid>.*\r?\n.*\r?\n.*\r?\n.*</file>)
			// replace with: if(icon == ContentIcon.BaggageCartBoxLabel) \r\n{\r\nreturn "$2";\r\n}\r\n
			if (icon == ContentIcon.BaggageCartBoxLabel)
			{
				return "baggage-cart-box-label.png";
			}

			if (icon == ContentIcon.BaggageCart)
			{
				return "baggage-cart.png";
			}

			if (icon == ContentIcon.Bank)
			{
				return "bank.png";
			}

			if (icon == ContentIcon.BoxLabel)
			{
				return "box-label.png";
			}

			if (icon == ContentIcon.BoxSearchResults)
			{
				return "box-search-result.png";
			}

			if (icon == ContentIcon.CalendarMonth)
			{
				return "calendar-month.png";
			}

			if (icon == ContentIcon.ClipboardInvoice)
			{
				return "clipboard-invoice.png";
			}

			if (icon == ContentIcon.ClipboardList)
			{
				return "clipboard-list.png";
			}

			if (icon == ContentIcon.CreditCard)
			{
				return "credit-card.png";
			}

			if (icon == ContentIcon.Drawer)
			{
				return "drawer.png";
			}

			if (icon == ContentIcon.FolderOpenTable)
			{
				return "folder-open-table.png";
			}

			if (icon == ContentIcon.FolderSearchResults)
			{
				return "folder-search-result.png";
			}

			if (icon == ContentIcon.Folder)
			{
				return "folder.png";
			}

			if (icon == ContentIcon.GlobeGreen)
			{
				return "globe-green.png";
			}

			if (icon == ContentIcon.InboxDocument)
			{
				return "inbox-document.png";
			}

			if (icon == ContentIcon.MagnetSmall)
			{
				return "magnet-small.png";
			}

			if (icon == ContentIcon.MapPin)
			{
				return "map-pin.png";
			}

			if (icon == ContentIcon.MailAir)
			{
				return "mail-air.png";
			}

			if (icon == ContentIcon.MailOpenDocumentText)
			{
				return "mail-open-document-text.png";
			}

			if (icon == ContentIcon.MailsStack)
			{
				return "mails-stack.png";
			}

			if (icon == ContentIcon.MegaPhone)
			{
				return "megaphone.png";
			}

			if (icon == ContentIcon.PresentLabel)
			{
				return "present-label.png";
			}

			if (icon == ContentIcon.PriceTagMinus)
			{
				return "price-tag--minus.png";
			}

			if (icon == ContentIcon.PriceTagLabel)
			{
				return "price-tag-label.png";
			}

			if (icon == ContentIcon.Scissors)
			{
				return "scissors.png";
			}

			if (icon == ContentIcon.StoreNetwork)
			{
				return "store-network.png";
			}

			if (icon == ContentIcon.Store)
			{
				return "store.png";
			}

			if (icon == ContentIcon.Toolbox)
			{
				return "toolbox.png";
			}

			if (icon == ContentIcon.TruckBoxLabel)
			{
				return "truck-box-label.png";
			}

			if (icon == ContentIcon.Uwebshop)
			{
				return "uwebshop16x16.png";
			}

			if (icon == ContentIcon.Wallet)
			{
				return "wallet.png";
			}

			return "folder.png";
		}

		private static void LoadMediaBase(MediaBase entity, IMedia media)
		{
			entity.Id = media.Id;
			entity.ParentId = media.ParentId;
			entity.CreateDateTime = media.CreateDate;
			entity.IsTrashed = media.Trashed;

			entity.FileExtension = media.GetValue<string>("umbracoExtension");
			entity.RelativePathToFile = media.HasProperty("umbracoFile") ? media.GetValue<string>("umbracoFile") : string.Empty;
			entity.FileSize = media.GetValue<int>("umbracoBytes");
		}

		internal static File LoadFileWithId(int id)
		{
			try
			{
				var media = MediaService.GetById(id);
				var file = new File();
				LoadMediaBase(file, media);
				file.FileName = media.Name;
				file.MultilanguageFileName = media.HasProperty("title") ? media.GetValue<string>("title") : media.Name;
				return file;
			}
			catch (Exception)
			{
				return null;
			}
		}

		internal static Image LoadImageWithId(int id)
		{
			try
			{
				var media = MediaService.GetById(id);
				var image = new Image();
				LoadMediaBase(image, media);
				image.Width = media.GetValue<int>("umbracoWidth");
				image.Height = media.GetValue<int>("umbracoHeight");
				return image;
			}
			catch (Exception)
			{
				return null;
			}
		}

		internal static IEnumerable<Document> GetSearchResults(string luceneQuery)
		{
			var examineIndexesCollection = ExamineManager.Instance.IndexProviderCollection;

			var indexer = (LuceneIndexer) examineIndexesCollection.FirstOrDefault(x => x.Name == UwebshopConfiguration.Current.ExamineIndexer);

			if (indexer == null)
			{
				return Enumerable.Empty<Document>();
			}

			var path = indexer.GetLuceneDirectory();

			var queryParser = new QueryParser(string.Empty, new WhitespaceAnalyzer());
			var q = queryParser.Parse(luceneQuery);
			var parsedQuery = q.ToString();
			var searcher = new IndexSearcher(path, true);

			var hits = searcher.Search(q, null, 1000000);

			var list = new List<Document>();
			for (var i = 0; i < hits.TotalHits; i++)
			{
				var doc = searcher.Doc(hits.ScoreDocs[i].doc);
				if (doc != null)
				{
					list.Add(doc);
				}
			}
			searcher.Dispose();

			//list.AddRange(GetSearchResultsRaw(luceneQuery));

			return list; //.Distinct();
		}

		#region Mvc Rendermode determination

		private static bool? _mvcRenderMode;

		public static bool MvcRenderMode
		{
			get
			{
				if (_mvcRenderMode == null)
				{
					try
					{
						var value = GetKey("/settings/templates/defaultRenderingEngine");
						_mvcRenderMode = value.ToLowerInvariant() != "webforms";
					}
					catch (Exception ex)
					{
						LogHelper.Error<UmbracoLoggingService>("Could not load /settings/templates/defaultRenderingEngine from umbracosettings.config",ex);
						_mvcRenderMode = false;
					}
				}
				return _mvcRenderMode.Value;
			}
		}

		public static string GetKey(string key)
		{
			EnsureSettingsDocument();

			string attrName = null;
			var pos = key.IndexOf('@');
			if (pos > 0)
			{
				attrName = key.Substring(pos + 1);
				key = key.Substring(0, pos - 1);
			}

			var node = UmbracoSettingsXmlDoc.DocumentElement.SelectSingleNode(key);
			if (node == null)
				return string.Empty;

			if (pos < 0)
			{
				if (node.FirstChild == null || node.FirstChild.Value == null)
					return string.Empty;
				return node.FirstChild.Value;
			}
			else
			{
				var attr = node.Attributes[attrName];
				if (attr == null)
					return string.Empty;
				return attr.Value;
			}
		}

		internal static XmlDocument UmbracoSettingsXmlDoc
		{
			get { return (XmlDocument) HttpRuntime.Cache["umbracoSettingsFile"] ?? EnsureSettingsDocument(); }
		}

		internal static string SettingsFilePath
		{
			get { return GlobalSettings.FullpathToRoot + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar; }
		}

		internal static XmlDocument EnsureSettingsDocument()
		{
			var Filename = "umbracoSettings.config";
			var settingsFile = HttpRuntime.Cache["umbracoSettingsFile"];

			// Check for language file in cache
			if (settingsFile == null)
			{
				var temp = new XmlDocument();
				var settingsReader = new XmlTextReader(SettingsFilePath + Filename);
				try
				{
					temp.Load(settingsReader);
					HttpRuntime.Cache.Insert("umbracoSettingsFile", temp, new CacheDependency(SettingsFilePath + Filename));
				}
				catch (XmlException e)
				{
					throw new XmlException("Your umbracoSettings.config file fails to pass as valid XML. Refer to the InnerException for more information", e);
				}
				catch (Exception e)
				{
					LogHelper.Error<UmbracoSettings>("Error reading umbracoSettings file: " + e, e);
				}
				settingsReader.Close();
				return temp;
			}
			return (XmlDocument) settingsFile;
		}

		#endregion
	}
}