using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Examine;

namespace uWebshop.Umbraco.Businesslogic
{
	/// <summary>
	/// 
	/// </summary>
	public class MediaValues
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MediaValues"/> class.
		/// </summary>
		/// <param name="xpath">The xpath.</param>
		/// <exception cref="System.ArgumentNullException">xpath</exception>
		public MediaValues(XPathNavigator xpath)
		{
			if (xpath == null) throw new ArgumentNullException("xpath");
			Name = xpath.GetAttribute("nodeName", "");
			Values = new Dictionary<string, string>();
			XPathNodeIterator result = xpath.SelectChildren(XPathNodeType.Element);
			while (result.MoveNext())
			{
				if (result.Current != null && !result.Current.HasAttributes)
				{
					Values.Add(result.Current.Name, result.Current.Value);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaValues"/> class.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <exception cref="System.ArgumentNullException">result</exception>
		public MediaValues(SearchResult result)
		{
			if (result == null) throw new ArgumentNullException("result");
			Name = result.Fields["nodeName"];
			Values = result.Fields;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>
		/// The values.
		/// </value>
		public IDictionary<string, string> Values { get; private set; }
	}
}