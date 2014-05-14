using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class ShippingRequest
	{
		private Dictionary<string, string> _parameters;

		/// <summary>
		/// Gets or sets the parameters.
		/// </summary>
		/// <value>
		/// The parameters.
		/// </value>
		public Dictionary<string, string> Parameters
		{
			get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
			set { _parameters = value; }
		}

		/// <summary>
		/// Gets the parameters in string format.
		/// </summary>
		/// <value>
		/// The parameters in string format.
		/// </value>
		public string ParametersAsString
		{
			get
			{
				string parametersAsString = string.Empty;

				if (Parameters != null && Parameters.Count > 0)
				{
					parametersAsString = Parameters.Aggregate(parametersAsString, (current, kvp) => current + string.Format("{0}={1}&", kvp.Key, kvp.Value));

					parametersAsString = parametersAsString.TrimEnd('&');
				}

				return parametersAsString;
			}
		}

		/// <summary>
		/// Gets or sets the parameter render method.
		/// </summary>
		/// <value>
		/// The parameter render method.
		/// </value>
		public ShippingTransactionMethod ParameterRenderMethod { get; set; }

		/// <summary>
		/// Gets or sets the shipping URL base.
		/// </summary>
		/// <value>
		/// The shipping URL base.
		/// </value>
		public string ShippingUrlBase { get; set; }

		/// <summary>
		/// Gets the shipping URL.
		/// </summary>
		/// <value>
		/// The shipping URL.
		/// </value>
		public string ShippingUrl
		{
			get
			{
				string paymentUrl = string.Empty;

				if (!string.IsNullOrEmpty(ShippingUrlBase))
					paymentUrl = ShippingUrlBase.TrimEnd('/');

				return paymentUrl;
			}
		}
	}
}