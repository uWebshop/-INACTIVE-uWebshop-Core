using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class PaymentRequest
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
//test
				if (Parameters == null || Parameters.Count <= 0) return string.Empty;

				string parametersAsString = Parameters.Aggregate(string.Empty, (current, kvp) => current + string.Format("{0}={1}&", kvp.Key, kvp.Value));

				parametersAsString = parametersAsString.TrimEnd('&');

				return parametersAsString;
			}
		}

		/// <summary>
		/// Gets or sets the parameter render method.
		/// </summary>
		/// <value>
		/// The parameter render method.
		/// </value>
		public PaymentTransactionMethod ParameterRenderMethod { get; set; }

		/// <summary>
		/// Gets or sets the payment URL base.
		/// </summary>
		/// <value>
		/// The payment URL base.
		/// </value>
		public string PaymentUrlBase { get; set; }

		/// <summary>
		/// Gets the payment URL.
		/// </summary>
		/// <value>
		/// The payment URL.
		/// </value>
		public string PaymentUrl
		{
			get
			{
				string paymentUrl = string.Empty;

				if (!string.IsNullOrEmpty(PaymentUrlBase))
					paymentUrl = PaymentUrlBase.TrimEnd('/');

				return paymentUrl;
			}
		}
	}
}