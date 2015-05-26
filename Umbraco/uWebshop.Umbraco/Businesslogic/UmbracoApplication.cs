using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using umbraco;
using uWebshop.Domain.BaseClasses;
using Umbraco.Core;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using umbraco.DataLayer;
using umbraco.cms.businesslogic.member;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;
using Umbraco.Core.Models;
using Umbraco.Web;
using Constants = uWebshop.Common.Constants;
using library = umbraco.library;
using Umbraco.Core.IO;

namespace uWebshop.Umbraco
{
	internal class UmbracoApplication : ICMSApplication
	{	

		private readonly IHttpContextWrapper _httpContextWrapper;

		public UmbracoApplication(IHttpContextWrapper httpContextWrapper)
		{
			_httpContextWrapper = httpContextWrapper;
		}

		public bool RequestIsInCMSBackend(HttpContext context)
		{
			var value = false;
			// return context.Request.Path.ToLower().IndexOf(IOHelper.ResolveUrl(SystemDirectories.Umbraco).ToLower()) > -1;
			var path = context.Request.Path.ToLower();
			// MVC request go through /umbraco/rendermvc, which is not  checked in the RequestIsInUmbracoApplication.
			// if /umbraco/rendermvc is not used, then check for RequestIsInUmbracoApplication (because not in backend), otherwise do the default Umbraco check.
			if (!path.Contains("/umbraco/rendermvc"))
			{
				value = GlobalSettings.RequestIsInUmbracoApplication(context);
			}

			Log.Instance.LogDebug("RequestIsInCMSBackend: " + value);
			return value;
		}

		public bool IsReservedPathOrUrl(string path)
		{
			return GlobalSettings.IsReservedPathOrUrl(path);
		}

		public int VersionMajor
		{
			get { return global::Umbraco.Core.Configuration.UmbracoVersion.Current.Major; }
		}

		public int VersionMinor
		{
			get { return global::Umbraco.Core.Configuration.UmbracoVersion.Current.Minor; }
		}

		public string GetuWebshopCMSNodeUrlName()
		{
			var uWebshopNode = DomainHelper.GetUwebShopNode();
			return uWebshopNode == null ? null : uWebshopNode.UrlName;
		}

		public string GetCatalogRepositoryCMSNodeUrlName()
		{
			var catalogRepositoryNode = DomainHelper.GetObjectsByAlias<uWebshopEntity>(Catalog.NodeAlias, Constants.NonMultiStoreAlias).FirstOrDefault();
			return catalogRepositoryNode == null ? null : catalogRepositoryNode.UrlName;
		}

		public string GetCategoryRepositoryCMSNodeUrlName()
		{
			var categoryRepositoryNode = Catalog.GetCategoryRepositoryNode();
			return categoryRepositoryNode == null ? null : categoryRepositoryNode.UrlName;
		}
		
		public string GetProductRepositoryCMSNodeUrlName()
		{
			var categoryRepositoryNode = Catalog.GetProductRepositoryNode();
			return categoryRepositoryNode == null ? null : categoryRepositoryNode.UrlName;
		}

		public string GetPaymentProviderRepositoryCMSNodeUrlName()
		{
			var paymentProviderRepositoryNode =
				new UmbracoHelper(UmbracoContext.Current).ContentSingleAtXPath(@"//" + PaymentProvider.PaymentProviderRepositoryNodeAlias);
			return paymentProviderRepositoryNode == null ? null : paymentProviderRepositoryNode.UrlName;
		}

		public string GetPaymentProviderSectionCMSNodeUrlName()
		{
			var paymentProviderSectionNode = new UmbracoHelper(UmbracoContext.Current).ContentSingleAtXPath(@"//" + PaymentProvider.PaymentProviderSectionNodeAlias);
			return paymentProviderSectionNode == null ? null : paymentProviderSectionNode.UrlName;
		}

		public string GetDictionaryItem(string key)
		{
			return library.GetDictionaryItem(key);
		}

		public bool MemberLoggedIn()
		{
			return Member.GetCurrentMember() != null;
		}

		public int CurrentMemberId()
		{
			return Member.GetCurrentMember().Id;
		}

		public bool UsesSQLCEDatabase()
		{
			var connectionString = ApplicationContext.Current.DatabaseContext.ConnectionString;
			return DataLayerHelper.IsEmbeddedDatabase(connectionString);
		}

		public bool UsesMySQLDatabase()
		{
			var connectionString = ApplicationContext.Current.DatabaseContext.ConnectionString;
			return connectionString.ToLower().Contains("mysql");
		}

		public bool HideTopLevelNodeFromPath
		{
			get { return GlobalSettings.HideTopLevelNodeFromPath; }
		}

		public bool AddTrailingSlash
		{
			get { return UmbracoSettings.AddTrailingSlash; }
		}

		public bool UsesMvcRendermode
		{
			get { return InternalHelpers.MvcRenderMode; }
		}

		public bool IsBackendUserAuthenticated
		{
			get { return umbraco.BusinessLogic.User.GetCurrent() != null; }
		}

		public IMemberInfo CurrentMemberInfo()
		{
			return new UmbracoMemberInfo();
		}

		public string ParseInternalLinks(string text)
		{
			return Helpers.ParseInternalLinks(text);
		}

		public int CurrentNodeId()
		{
			return UmbracoContext.Current.PageId.GetValueOrDefault();
		}

		// unsure about location! (maybe another service)
		public string GetMultiStoreContentProperty(int contentId, string propertyAlias, ILocalization localization, bool globalOverrulesStore = false)
		{
			var umbHelper = new UmbracoHelper(UmbracoContext.Current);
			var examineNode = Helpers.GetNodeFromExamine(contentId, "GetMultiStoreItem::" + propertyAlias);
			if (localization == null)
			{
				localization = StoreHelper.CurrentLocalization;
			}
			var multiStoreAlias = StoreHelper.CreateMultiStorePropertyAlias(propertyAlias, localization.StoreAlias);
			var multiStoreMultiCurrencyAlias = StoreHelper.CreateFullLocalizedPropertyAlias(propertyAlias, localization);
			if (examineNode != null)
			{
				
				if (multiStoreAlias.StartsWith("description"))
				{
					multiStoreAlias = "RTEItem" + multiStoreAlias;
					propertyAlias = "RTEItem" + propertyAlias;
				}

				if (globalOverrulesStore && examineNode.Fields.ContainsKey(propertyAlias))
				{
					return examineNode.Fields[propertyAlias] ?? string.Empty;
				}

				if (examineNode.Fields.ContainsKey(multiStoreMultiCurrencyAlias))
				{
					return examineNode.Fields[multiStoreMultiCurrencyAlias] ?? string.Empty;
				}

				if (examineNode.Fields.ContainsKey(multiStoreAlias))
				{
					return examineNode.Fields[multiStoreAlias] ?? string.Empty;
				}

				if (examineNode.Fields.ContainsKey(propertyAlias))
				{
					return examineNode.Fields[propertyAlias] ?? string.Empty;
				}

				Log.Instance.LogDebug("GetMultiStoreContentProperty Fallback to node after this");
			}

			var node = umbHelper.Content(contentId);
			if (node.Name != null)
			{
				var property = node.GetProperty(propertyAlias);
				if (globalOverrulesStore && property != null && !string.IsNullOrEmpty(property.Value))
				{
					return property.Value;
				}
				var propertyMultiStoreMultiCurrency = node.GetProperty(multiStoreMultiCurrencyAlias);
				if (propertyMultiStoreMultiCurrency != null && !string.IsNullOrEmpty(propertyMultiStoreMultiCurrency.Value))
				{
					return propertyMultiStoreMultiCurrency.Value;
				}
				var propertyMultistore = node.GetProperty(multiStoreAlias);
				if (propertyMultistore != null && !string.IsNullOrEmpty(propertyMultistore.Value))
				{
					return propertyMultistore.Value;
				}
				if (property != null)
				{
					return property.Value;
				}
			}
			return string.Empty;
		}

		public string RenderMacro(string templateAlias, int contentId, params object[] properties)
		{
			var razorFileLocation = string.Format("{0}/{1}", global::Umbraco.Core.IO.SystemDirectories.MacroScripts.TrimEnd('/'), templateAlias.TrimStart('/'));
			
			Log.Instance.LogDebug("SendOrderEmail razorFileLocation: " + razorFileLocation);

			return RazorLibraryExtensions.RenderMacro(razorFileLocation, contentId, properties);
		}

		public string ApplyUrlFormatRules(string url)
		{
			var replacements = UrlReplacementsHack();

			foreach (var x in replacements.Keys)
				url = url.Replace(x, replacements[x]);

			// check for double dashes
			if (RemoveDoubleDashesFromUrlReplacingHack())
			{
				url = Regex.Replace(url, @"[-]{2,}", "-");
			}
			return url;
		}

		public List<ICustomerType> GetAllMemberTypes()
		{
			return MemberType.GetAll.Select(mt => new UmbracoMemberAdaptor {Alias = mt.Alias}).ToList<ICustomerType>();
		}

	 
		public string GetUrlForContentWithId(int id)
		{
			return library.NiceUrl(id);
		}

		public string RenderXsltMacro(string templateAlias, Dictionary<string, object> xsltParameters, XmlDocument entityXml = null)
		{
			if (entityXml == null) entityXml = content.Instance.XmlContent;
			return macro.GetXsltTransformResult(entityXml, macro.getXslt(templateAlias), xsltParameters);
		}

		private class UmbracoMemberAdaptor : ICustomerType
		{
			public string Alias { get; set; }
		}

		private const string RemoveDoubleDashesFromUrlReplacingHackCacheKey = "RemoveDoubleDashesFromUrlReplacingHackCacheKey";
		private const string UrlReplacementsHackCacheKey = "UrlReplacementsHackCacheKey";

		private static bool RemoveDoubleDashesFromUrlReplacingHack()
		{
			if (HttpContext.Current.Items.Contains(RemoveDoubleDashesFromUrlReplacingHackCacheKey))
			{
				return (bool) HttpContext.Current.Items[RemoveDoubleDashesFromUrlReplacingHackCacheKey];
			}
			bool wtf = UmbracoSettings.RemoveDoubleDashesFromUrlReplacing;
			HttpContext.Current.Items[RemoveDoubleDashesFromUrlReplacingHackCacheKey] = wtf;
			return wtf;
		}

		private static Dictionary<string, string> UrlReplacementsHack()
		{
			if (HttpContext.Current.Items[UrlReplacementsHackCacheKey] != null) return (Dictionary<string, string>) HttpContext.Current.Items[UrlReplacementsHackCacheKey];
			var replacements = new Dictionary<string, string>();
			
			// first check if there are settings (by default empty in v7)
			var replaceChars = UmbracoSettings.UrlReplaceCharacters;
			if (replaceChars != null)
			{
				var xmlNodeList = replaceChars.SelectNodes("char");
				if (xmlNodeList != null)
					foreach (XmlNode n in xmlNodeList)
					{
						if (n.Attributes != null && (n.Attributes.GetNamedItem("org") != null && n.Attributes.GetNamedItem("org").Value != ""))
							replacements.Add(n.Attributes.GetNamedItem("org").Value, XmlHelper.GetNodeValue(n));
					}
			}
			else
			{
				// if nothting defined fallback to defaults as are set in umbraco v7 codebase
				foreach (var replacement in GetDefaultCharReplacements())
				{
					replacements.Add(replacement.Key, replacement.Value);
				}
			}

			HttpContext.Current.Items[UrlReplacementsHackCacheKey] = replacements;

			return replacements;
		}
		
		// code below is COPIED from Umbraco source SINCE it was only internally available..
		internal static Dictionary<string, string> GetDefaultCharReplacements()
		{
			var dictionary = new Dictionary<char, string>()
						{
							{' ',"-"},
							{'\"',""},
							{'\'',""},
							{'%',""},
							{'.',""},
							{';',""},
							{'/',""},
							{'\\',""},
							{':',""},
							{'#',""},
							{'+',"plus"},
							{'*',"star"},
							{'&',""},
							{'?',""},
							{'æ',"ae"},
							{'ø',"oe"},
							{'å',"aa"},
							{'ä',"ae"},
							{'ö',"oe"},
							{'ü',"ue"},
							{'ß',"ss"},
							{'Ä',"ae"},
							{'Ö',"oe"},
							{'|',"-"},
							{'<',""},
							{'>',""}
						};

			//const string chars = @" ,"",',%,.,;,/,\,:,#,+,*,&,?,æ,ø,å,ä,ö,ü,ß,Ä,Ö,|,<,>";

			var collection = new Dictionary<string, string>();
			foreach (var c in dictionary)
			{
				collection.Add(c.Key.ToString(CultureInfo.InvariantCulture),
					c.Value.ToString(CultureInfo.InvariantCulture));
			}

			return collection;
		}


		public IEnumerable<string> GetDomainsForNodeId(int id)
		{
			var domains = umbraco.cms.businesslogic.web.Domain.GetDomainsById(id).Select(d => d.Name);

			return domains.Select(domain =>
				{
					domain = domain.TrimEnd('/');
					if (!domain.StartsWith("http"))
					{
						var connection = _httpContextWrapper.IsSecureConnection ? "https" : "http";
						domain = string.Format("{0}://{1}", connection, domain);
					}
					return domain;
				});
		}

		public string GetDomainForNodeId(int id)
		{
			return GetDomainsForNodeId(id).FirstOrDefault();
		}
	}

	public class UmbracoMemberInfo : IMemberInfo
	{
		public bool VATNumberCheckedAsValid
		{
			get
			{
				var member = Member.GetCurrentMember();

				var vatValid = "0";
				if (member.getProperty("customerValidVAT") != null && member.getProperty("customerValidVAT").Value != null)
				{
					vatValid = member.getProperty("customerValidVAT").Value.ToString();
				}
				return vatValid != "0";
			}
		}
	}


}