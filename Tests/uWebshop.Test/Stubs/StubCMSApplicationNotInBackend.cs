using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	public class StubCMSApplicationNotInBackend : ICMSApplication
	{
		public bool RequestIsInCMSBackend(HttpContext context)
		{
			return false;
		}

		public bool IsReservedPathOrUrl(string path)
		{
			return false;
		}

		public int VersionMajor
		{
			get { return 4; }
		}

		public int VersionMinor
		{
			get { return 11; }
		}

		private string _uWebshopCMSNodeUrlName;

		public string uWebshopRepositoryCMSNodeUrlName
		{
			set { _uWebshopCMSNodeUrlName = value; }
		}

		public string GetuWebshopCMSNodeUrlName()
		{
			return _uWebshopCMSNodeUrlName;
		}

		private string _catalogRepositoryCMSNodeUrlName;

		public string catalogRepositoryCMSNodeUrlName
		{
			set { _catalogRepositoryCMSNodeUrlName = value; }
		}

		public string GetCatalogRepositoryCMSNodeUrlName()
		{
			return _catalogRepositoryCMSNodeUrlName;
		}

		private string _categoryRepositoryCMSNodeUrlName;

		public string categoryRepositoryCMSNodeUrlName
		{
			set { _categoryRepositoryCMSNodeUrlName = value; }
		}

		public string GetCategoryRepositoryCMSNodeUrlName()
		{
			return _categoryRepositoryCMSNodeUrlName;
		}
		public string GetProductRepositoryCMSNodeUrlName()
		{
			return "products";
		}

		private string _paymentProviderRepositoryCMSNodeName;

		public string PaymentProviderRepositoryCMSNodeName
		{
			set { _paymentProviderRepositoryCMSNodeName = value; }
		}

		public string GetPaymentProviderRepositoryCMSNodeUrlName()
		{
			return _paymentProviderRepositoryCMSNodeName;
		}

		private string _paymentProviderSectionCMSNodeName;

		public string PaymentProviderSectionCMSNodeName
		{
			set { _paymentProviderSectionCMSNodeName = value; }
		}

		public string GetPaymentProviderSectionCMSNodeUrlName()
		{
			return _paymentProviderSectionCMSNodeName;
		}

		public string GetDictionaryItem(string key)
		{
			return "bladiebla";
		}

		public bool MemberLoggedIn()
		{
			return false;
		}

		public int CurrentMemberId()
		{
			return -1;
		}

		public bool UsesSQLCEDatabase()
		{
			return false;
		}

		public bool UsesMySQLDatabase()
		{
			return false;
		}

		public bool HideTopLevelNodeFromPath
		{
			get { return true; }
		}

		public bool AddTrailingSlash
		{
			get { return false; }
		}

		public bool UsesMvcRendermode
		{
			get { return false; }
		}

		public bool IsBackendUserAuthenticated { get; private set; }

		public IMemberInfo CurrentMemberInfo()
		{
			throw new System.NotImplementedException();
		}

		public string ParseInternalLinks(string text)
		{
			return text;
		}

		public string GetDomainForNodeId(int id)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<string> GetDomainsForNodeId(int id)
		{
			return Enumerable.Empty<string>();
		}

		public int CurrentNodeId()
		{
			return 0;
		}

		public bool HasValidLicense()
		{
			return true;
		}

		public string GetMultiStoreContentProperty(int contentId, string propertyAlias, ILocalization localization, bool globalOverrulesStore = false)
		{
			return string.Empty;
		}

		public string RenderMacro(string aliasOrPath, int contentId, params object[] properties)
		{
			return string.Empty;
		}

		public string ApplyUrlFormatRules(string url)
		{
			return url;
		}

		public List<ICustomerType> GetAllMemberTypes()
		{
			return new List<ICustomerType>();
		}

		public string GetUrlForContentWithId(int id)
		{
			throw new System.NotImplementedException();
		}

		public string RenderXsltMacro(string templateAlias, Dictionary<string, object> xsltParameters, XmlDocument entityXml = null)
		{
			throw new System.NotImplementedException();
		}


	}
}