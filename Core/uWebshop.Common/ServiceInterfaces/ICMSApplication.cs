using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICMSApplication
	{
		/// <summary>
		/// Requests the is information CMS backend.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		bool RequestIsInCMSBackend(HttpContext context);

		/// <summary>
		/// Determines whether [is reserved path original URL] [the specified path].
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		bool IsReservedPathOrUrl(string path);

		/// <summary>
		/// Gets the version major.
		/// </summary>
		/// <value>
		/// The version major.
		/// </value>
		int VersionMajor { get; }

		/// <summary>
		/// Gets the version minor.
		/// </summary>
		/// <value>
		/// The version minor.
		/// </value>
		int VersionMinor { get; }

		/// <summary>
		/// Getus the name of the webshop CMS node URL.
		/// </summary>
		/// <returns></returns>
		string GetuWebshopCMSNodeUrlName();

		/// <summary>
		/// Gets the name of the catalog repository CMS node URL.
		/// </summary>
		/// <returns></returns>
		string GetCatalogRepositoryCMSNodeUrlName();

		/// <summary>
		/// Gets the name of the category repository CMS node URL.
		/// </summary>
		/// <returns></returns>
		string GetCategoryRepositoryCMSNodeUrlName();

		/// <summary>
		/// Gets the name of the payment provider repository CMS node URL.
		/// </summary>
		/// <returns></returns>
		string GetPaymentProviderRepositoryCMSNodeUrlName();

		/// <summary>
		/// Gets the name of the payment provider section CMS node URL.
		/// </summary>
		/// <returns></returns>
		string GetPaymentProviderSectionCMSNodeUrlName();

		/// <summary>
		/// Gets the dictionary item.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		string GetDictionaryItem(string key);

		/// <summary>
		/// Members the logged information.
		/// </summary>
		/// <returns></returns>
		bool MemberLoggedIn();

		/// <summary>
		/// Currents the member unique identifier.
		/// </summary>
		/// <returns></returns>
		int CurrentMemberId();

		/// <summary>
		/// Useses the sqlce database.
		/// </summary>
		/// <returns></returns>
		bool UsesSQLCEDatabase();

		/// <summary>
		/// Useses my SQL database.
		/// </summary>
		/// <returns></returns>
		bool UsesMySQLDatabase();

		/// <summary>
		/// Gets a value indicating whether [hide top level node from path].
		/// </summary>
		/// <value>
		/// <c>true</c> if [hide top level node from path]; otherwise, <c>false</c>.
		/// </value>
		bool HideTopLevelNodeFromPath { get; }

		/// <summary>
		/// Gets a value indicating whether [add trailing slash].
		/// </summary>
		/// <value>
		///   <c>true</c> if [add trailing slash]; otherwise, <c>false</c>.
		/// </value>
		bool AddTrailingSlash { get; }

		/// <summary>
		/// Gets a value indicating whether [uses MVC rendermode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [uses MVC rendermode]; otherwise, <c>false</c>.
		/// </value>
		bool UsesMvcRendermode { get; }

		bool IsBackendUserAuthenticated { get;}

		/// <summary>
		/// Currents the member information.
		/// </summary>
		/// <returns></returns>
		IMemberInfo CurrentMemberInfo();

		/// <summary>
		/// Parses the internal links.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		string ParseInternalLinks(string text);

		/// <summary>
		/// Gets the domain for node unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		string GetDomainForNodeId(int id);

		IEnumerable<string> GetDomainsForNodeId(int id);

		/// <summary>
		/// Currents the node unique identifier.
		/// </summary>
		/// <returns></returns>
		int CurrentNodeId();

		/// <summary>
		/// Determines whether [has valid license].
		/// </summary>
		/// <returns></returns>
		bool HasValidLicense();

		/// <summary>
		/// Gets the multi store content property.
		/// </summary>
		/// <param name="contentId">The content unique identifier.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="localization">The localization.</param>
		/// <param name="globalOverrulesStore">if set to <c>true</c> [global overrules store].</param>
		/// <returns></returns>
		string GetMultiStoreContentProperty(int contentId, string propertyAlias, ILocalization localization, bool globalOverrulesStore = false);

		/// <summary>
		/// Renders the macro.
		/// </summary>
		/// <param name="aliasOrPath">The alias original path.</param>
		/// <param name="contentId">The content unique identifier.</param>
		/// <param name="properties">The properties.</param>
		/// <returns></returns>
		string RenderMacro(string aliasOrPath, int contentId, params object[] properties);

		// unsure about location
		/// <summary>
		/// Applies the URL format rules.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		string ApplyUrlFormatRules(string url);

		/// <summary>
		/// Gets all member types.
		/// </summary>
		/// <returns></returns>
		List<ICustomerType> GetAllMemberTypes();

		/// <summary>
		/// Gets the URL for content with unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		string GetUrlForContentWithId(int id);

		/// <summary>
		/// Renders the XSLT macro.
		/// </summary>
		/// <param name="templateAlias">The template alias.</param>
		/// <param name="xsltParameters">The XSLT parameters.</param>
		/// <param name="entityXml">The entity XML.</param>
		/// <returns></returns>
		string RenderXsltMacro(string templateAlias, Dictionary<string, object> xsltParameters, XmlDocument entityXml = null);

		string GetProductRepositoryCMSNodeUrlName();
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IMemberInfo
	{
		/// <summary>
		/// Gets a value indicating whether [vat number checked asynchronous valid].
		/// </summary>
		/// <value>
		/// <c>true</c> if [vat number checked asynchronous valid]; otherwise, <c>false</c>.
		/// </value>
		bool VATNumberCheckedAsValid { get; }
	}
}