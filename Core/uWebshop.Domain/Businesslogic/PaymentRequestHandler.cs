using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// 
	/// </summary>
	internal class PaymentRequestHandler
	{
		/// <summary>
		/// Handles the uwebshop payment request.
		/// </summary>
		/// <param name="paymentProviderNodeId">The payment provider node unique identifier.</param>
		/// <exception cref="System.Exception">HandleuWebshopPaymentRequest paymentProvider.Name == null
		/// or
		/// HandleuWebshopPaymentRequest responsehandler == null:  + paymentProvider.Name</exception>
		public void HandleuWebshopPaymentRequest(PaymentProvider paymentProvider)
		{
			if (paymentProvider.Name == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest paymentProvider.Name == null paymentProviderNodeId: " + paymentProvider.Id);
				throw new Exception("HandleuWebshopPaymentRequest paymentProvider.Name == null");
			}

			var responsehandler = PaymentProviderHelper.GetAllPaymentResponseHandlers().FirstOrDefault(paymentResponseHandler => paymentResponseHandler.GetName().ToLower() == paymentProvider.Name.ToLower());

			if (responsehandler == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest responsehandler == null paymentProvider.Name: " + paymentProvider.Name);
				
				throw new Exception("HandleuWebshopPaymentRequest responsehandler == null: " + paymentProvider.Name);
			}

			responsehandler.HandlePaymentResponse(paymentProvider, null);
		}

		/// <summary>
		/// HandleuWebshopPaymentResponse Used for Inline payment provider
		/// </summary>
		/// <param name="paymentProvider"></param>
		/// <returns></returns>
		public string HandleuWebshopPaymentResponse(PaymentProvider paymentProvider)
		{
			if (paymentProvider.Name == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest paymentProvider.Name == null paymentProviderNodeId: " + paymentProvider.Id);
				throw new Exception("HandleuWebshopPaymentRequest paymentProvider.Name == null");
			}

			var responsehandler = PaymentProviderHelper.GetAllPaymentResponseHandlers().FirstOrDefault(paymentResponseHandler => paymentResponseHandler.GetName().ToLower() == paymentProvider.Name.ToLower());

			if (responsehandler == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest responsehandler == null paymentProvider.Name: " + paymentProvider.Name);

				throw new Exception("HandleuWebshopPaymentRequest responsehandler == null: " + paymentProvider.Name);
			}

			return responsehandler.HandlePaymentResponse(paymentProvider, null).OrderNumber;
		}

		public OrderInfo HandleuWebshopPaymentResponse(PaymentProvider paymentProvider, OrderInfo order)
		{
			if (paymentProvider.Name == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest paymentProvider.Name == null paymentProviderNodeId: " + paymentProvider.Id);
				throw new Exception("HandleuWebshopPaymentRequest paymentProvider.Name == null");
			}

			var responsehandler = PaymentProviderHelper.GetAllPaymentResponseHandlers().FirstOrDefault(paymentResponseHandler => paymentResponseHandler.GetName().ToLower() == paymentProvider.Name.ToLower());

			if (responsehandler == null)
			{
				Log.Instance.LogError("HandleuWebshopPaymentRequest responsehandler == null paymentProvider.Name: " + paymentProvider.Name);

				throw new Exception("HandleuWebshopPaymentRequest responsehandler == null: " + paymentProvider.Name);
			}

			return responsehandler.HandlePaymentResponse(paymentProvider, order);
		}
	}
}