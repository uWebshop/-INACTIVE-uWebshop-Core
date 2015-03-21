using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Xml.XPath;
using Examine;
using Examine.Providers;
using Examine.SearchCriteria;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using umbraco;
using umbraco.NodeFactory;
using umbraco.interfaces;
using umbraco.presentation;
using uWebshop.Umbraco.Businesslogic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Language = umbraco.cms.businesslogic.language.Language;
using Log = uWebshop.Domain.Log;
using Template = umbraco.cms.businesslogic.template.Template;

namespace uWebshop.Umbraco
{
	// part of public API
	public class Helpers
	{
		public static readonly IContentService ContentService = ApplicationContext.Current.Services.ContentService;
		public static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;

		internal static void LoadUwebshopEntityPropertiesFromNode(uWebshopEntity entity, Node node)
		{
			if (entity.Id == 0) entity.Id = node.Id;
			entity.ParentId = node.Parent != null ? node.Parent.Id : -1;
			entity.SortOrder = node.SortOrder;
			entity.NodeTypeAlias = node.NodeTypeAlias;
			entity.Name = node.Name;
			entity.Path = node.Path;
			entity.CreateDate = node.CreateDate;
			entity.UpdateDate = node.UpdateDate;
			entity.UrlName = node.UrlName;
		}

		public static SearchResult GetNodeFromExamine(int documentId, string debugInfo = "")
		{
			var examineprovider = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher];
			if (documentId < 0 || examineprovider == null)
				return null;
			var searcher = examineprovider.CreateSearchCriteria();
			if (searcher == null)
				return null;
			var filter = searcher.RawQuery("__NodeId:" + documentId);
			var searchResults = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher].Search(filter);
            
			return searchResults.FirstOrDefault();
		}

		/// <summary>
		/// Gets the image by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public static UwbsImage GetImageById(int id)
		{
			// todo: nodetypealias image werkt nog niet
			return DomainHelper.GetObjectsByAlias<UwbsImage>("image").FirstOrDefault(img => img.Id == id);
		}

		public static string GetMultiStoreItemExamine(string propertyAlias, SearchResult examineNode, string storeAlias = null, string currencyCode = null)
		{
			if (examineNode != null)
			{
				propertyAlias = StoreHelper.MakeRTEItemPropertyAliasIfApplicable(propertyAlias);
				return StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(propertyAlias, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode), new DictionaryPropertyProvider(examineNode));
			}

			return string.Empty;
		}

		/// <summary>
		/// Parses the string looking for the {localLink} syntax and updates them to their correct links.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ParseInternalLinks(string text)
		{
			//don't attempt to proceed without a context as we cannot lookup urls without one
			if (global::Umbraco.Web.UmbracoContext.Current == null)
			{
				return text;
			}

			//var niceUrlsProvider = UmbracoContext.Current.NiceUrlProvider;

			// Parse internal links
			var tags = Regex.Matches(text, @"href=""[/]?(?:\{|\%7B)localLink:([0-9]+)(?:\}|\%7D)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (Match tag in tags)
				if (tag.Groups.Count > 0)
				{
					var id = tag.Groups[1].Value; //.Remove(tag.Groups[1].Value.Length - 1, 1);
					var newLink = library.NiceUrlWithDomain(int.Parse(id));
					text = text.Replace(tag.Value, "href=\"" + newLink);
				}
			return text;
		}

		public static List<UmbracoNode> GetNodesWithAliasUsingExamine(string nodeTypeAlias)
		{
			return GetExamineResultsForNodeTypeAlias(nodeTypeAlias).Select(searchResult => new UmbracoNode(searchResult)).ToList();
		}

		public static IEnumerable<SearchResult> GetExamineResultsForNodeTypeAlias(string nodeTypeAlias)
		{
			try
			{
				var examineprovider = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher];
				var searcher = examineprovider != null ? examineprovider.CreateSearchCriteria() : null;
				if (searcher != null)
				{
					var query = searcher.Field("__NodeTypeAlias", nodeTypeAlias.ToLower());
					var searchResults = examineprovider.Search(query.Compile());
					return searchResults.Where(examineNode => examineNode.Fields["__NodeId"] != null);
				}
			}
			catch (Exception ex)
			{
				Log.Instance.LogError(ex, "Error while searching Examine for type with alias " + nodeTypeAlias);
			}

			return Enumerable.Empty<SearchResult>();
		}

		internal static int uWebshopInstalleruWebshopPaymentsHandlerTemplate()
		{
			// first install payment handler template
			var template = Template.GetByAlias("uWebshopPaymentsHandler");
			if (template == null)
			{
				var paymentProviderTemplate = Template.MakeNew("uWebshop Payments Handler", new User(0));
				paymentProviderTemplate.Alias = "uWebshopPaymentsHandler";
				paymentProviderTemplate.SaveMasterPageFile("<%@ Master Language=\"C#\" MasterPageFile=\"~/umbraco/masterpages/default.master\" AutoEventWireup=\"true\" %><asp:Content ID=\"Content1\" ContentPlaceHolderID=\"ContentPlaceHolderDefault\" runat=\"server\"><!-- THIS TEMPLATE HANDLERS THE COMMUNICATION BETWEEN PAYMENT PROVIDERS AND THE PAYMENT PROVIDER API --><!-- ADD THIS TEMPLATE TO THE PAYMENT PROVIDER DOCUMENT TYPE IF YOU WANT TO USE PAYMENT PROVIDER APIs--><umbraco:Macro Alias=\"uWebshopPaymentProviderHandler\" runat=\"server\"/></asp:Content>");
				paymentProviderTemplate.Save();

				return paymentProviderTemplate.Id;
			}

			return template.Id;
		}

		/// <summary>
		///     Install a new uWebshop store using the first language installed in umbraco as the culture
		/// </summary>
		/// <param name="storeAlias">the store alias to use</param>
		public static void InstallStore(string storeAlias)
		{
			// vanuit sandbox
			InstallStore(storeAlias, null);
		}

		/// <summary>
		///     Install a new uWebshop store using the first language installed in umbraco as the culture
		/// </summary>
		/// <param name="storeAlias">the store alias to use</param>
		/// <param name="preFillRequiredItems"></param>
		public static void InstallStore(string storeAlias, bool preFillRequiredItems)
		{
			// vanuit installer knop
			InstallStore(storeAlias, null, null, preFillRequiredItems);
		}

		/// <summary>
		///     Install a new uWebshop store
		/// </summary>
		/// <param name="storeAlias">the store alias to use</param>
		/// <param name="storeDocument">the document of the store</param>
		/// <param name="cultureCode"> </param>
		/// <param name="preFillRequiredItems"></param>
		internal static void InstallStore(string storeAlias, IContent storeDocument, string cultureCode = null, bool preFillRequiredItems = false)
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var reg = new Regex(@"\s*");
			storeAlias = reg.Replace(storeAlias, "");

			if (cultureCode == null)
			{
				var languages = Language.GetAllAsList();

				var firstOrDefaultlanguage = languages.FirstOrDefault();
				if (firstOrDefaultlanguage == null)
					return;
				cultureCode = firstOrDefaultlanguage.CultureAlias;
			}

			var installStoreSpecificPropertiesOnDocumentTypes = WebConfigurationManager.AppSettings["InstallStoreDocumentTypes"];

			if (installStoreSpecificPropertiesOnDocumentTypes == null || installStoreSpecificPropertiesOnDocumentTypes != "false")
			{
				if (contentTypeService.GetAllContentTypes().Where(x => x.Alias.StartsWith(Store.NodeAlias)).All(x => x.Name.ToLower() != storeAlias.ToLower()))
				{
					IO.Container.Resolve<IUmbracoDocumentTypeInstaller>().InstallStore(storeAlias);
				}
				else
				{
					// todo: return message that store already existed?
					return;
				}
			}

			var uwbsStoreDt = ContentTypeService.GetContentType(Store.NodeAlias);
			var uwbsStoreRepositoryDt = ContentTypeService.GetContentType(Store.StoreRepositoryNodeAlias);
			var uwbsStoreRepository = ContentService.GetContentOfContentType(uwbsStoreRepositoryDt.Id).FirstOrDefault(x => !x.Trashed);

			if (storeDocument == null)
			{
				if (uwbsStoreRepository != null)
					if (uwbsStoreDt != null)
					{
						storeDocument = ContentService.CreateContent(storeAlias, uwbsStoreRepository, uwbsStoreDt.Alias);
						if (storeDocument != null && preFillRequiredItems)
						{
							storeDocument.SetValue("orderNumberPrefix", storeAlias);
							storeDocument.SetValue("globalVat", "0");
							storeDocument.SetValue("countryCode", "DK");
							storeDocument.SetValue("storeEmailFrom", string.Format("info@{0}.com", storeAlias));
							storeDocument.SetValue("storeEmailTo", string.Format("info@{0}.com", storeAlias));
							storeDocument.SetValue("storeEmailFromName", storeAlias);

							ContentService.Publish(storeDocument);
						}
					}
			}

			var language = Language.GetByCultureCode(cultureCode);

			if (storeDocument == null)
			{
				return;
			}
			if (language != null) storeDocument.SetValue("currencyCulture", language.id.ToString());
			ContentService.Save(storeDocument);

			//InstallProductUrlRewritingRules(storeAlias);
		}

		/// <summary>
		///     Get node ID based on document
		/// </summary>
		/// <param name="documentTypeAlias"></param>
		/// <param name="documentName"></param>
		/// <returns></returns>
		public static int GetNodeIdForDocument(string documentTypeAlias, string documentName)
		{
			try
			{
				if (documentTypeAlias != null && documentName != null)
				{
					var it = library.GetXmlNodeByXPath(string.Format("//{0}[@nodeName = '{1}']", documentTypeAlias, documentName));

					// todo: all uwebshop doctypes are now customizable, not only category/product/variant
					//parent[starts-with(@name,'data')]
					if (Category.IsAlias(documentTypeAlias) || Product.IsAlias(documentTypeAlias) || ProductVariant.IsAlias(documentTypeAlias))
					{
						// todo: IMPORTANT, incorrect logic (startwith needs to be on the first part)
						it = library.GetXmlNodeByXPath(string.Format("//{0}[starts-with(@nodeName,'{1}') and not(starts-with(name(),'{1}Repository'))]", documentTypeAlias, documentName));
					}

					while (it.MoveNext())
					{
						if (it.Current == null) continue;
						{
							var nodeId = Convert.ToInt32(it.Current.GetAttribute("id", it.Current.NamespaceURI));

							return nodeId;
						}
					}
				}


				return -1;
			}
			catch
			{
				return -1;
			}
		}

		/// <summary>
		/// Get node IDs based on document alias
		/// </summary>
		/// <param name="documentTypeAlias">The document type alias.</param>
		/// <returns></returns>
		public static List<int> GetNodeIdForDocumentAlias(string documentTypeAlias)
		{
			var result = new List<int>();
			try
			{
				if (documentTypeAlias != null)
				{
					//var it = library.GetXmlNodeByXPath(string.Format("//{0}", documentTypeAlias));
					XPathNodeIterator it = library.GetXmlNodeByXPath(string.Format("//*[starts-with(name(), '{0}')]", documentTypeAlias));

					while (it.MoveNext())
					{
						if (it.Current == null) continue;
						string id = it.Current.GetAttribute("id", it.Current.NamespaceURI);
						if (string.IsNullOrWhiteSpace(id)) continue;
						result.Add(Convert.ToInt32(id));
					}
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		/// <summary>
		/// Gets the umbraco media.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public static MediaValues GetUmbracoMedia(int id)
		{
			//first check in Examine as this is WAY faster
			ISearchCriteria criteria = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher].CreateSearchCriteria("media");
			IBooleanOperation filter = criteria.Id(id);
			ISearchResults results = ExamineManager.Instance.SearchProviderCollection[UwebshopConfiguration.Current.ExamineSearcher].Search(filter.Compile());
			if (results.Any())
			{
				return new MediaValues(results.First());
			}

			XPathNodeIterator media = library.GetMedia(id, false);
			if (media != null && media.Current != null)
			{
				media.MoveNext();
				return new MediaValues(media.Current);
			}

			return null;
		}
	}


	public class UmbracoNode : uWebshopEntity
	{
		private readonly IPropertyProvider propertyProvider;

		internal UmbracoNode(SearchResult searchResult)
		{
			propertyProvider = new DictionaryPropertyProvider(searchResult);
			LoadFieldsFromExamine(propertyProvider);
		}

		public string GetPropertyString(string propertyAlias)
		{
			string result = null;
			propertyProvider.UpdateValueIfPropertyPresent(propertyAlias, ref result);
			return result;
		}

		public IProperty GetProperty(string propertyAlias)
		{
			string result = null;
			propertyProvider.UpdateValueIfPropertyPresent(propertyAlias, ref result);
			return new Property {Alias = propertyAlias, Value = result};
		}

		public string Url
		{
			get
			{
				if (!propertyProvider.ContainsKey("url"))
					return Node.Url;
				return propertyProvider.GetStringValue("url");
			}
		}
	}

	public class Property : IProperty
	{
		public string Alias { get; set; }
		public string Value { get; set; }

		public Guid Version
		{
			get { throw new NotSupportedException("Version information not present in examine data"); }
			set { throw new NotSupportedException("Version information not present in examine data"); }
		}
	}

	public static class Extensions
	{
	}
}