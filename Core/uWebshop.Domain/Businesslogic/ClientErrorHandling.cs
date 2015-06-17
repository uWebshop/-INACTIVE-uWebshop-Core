using System;
using System.Collections.Generic;
using System.Web;
using uWebshop.Common;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class ClientErrorHandling
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the name of the field.
		/// </summary>
		/// <value>
		/// The name of the field.
		/// </value>
		public string FieldName { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public string Value { get; set; }

		/// <summary>
		/// Adds the error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static ClientErrorHandling AddErrorMessage(string message, string fieldName = "", string value = "")
		{
			var error = new ClientErrorHandling {Message = message, FieldName = fieldName, Value = value};
			List<ClientErrorHandling> currentErrors = GetErrorMessages();
			currentErrors.Add(error);
			SetErrorMessages(currentErrors);
			return error;
		}

		/// <summary>
		/// Sets the original clear error message.
		/// </summary>
		/// <param name="hasNoError">if set to <c>true</c> [has no error].</param>
		/// <param name="message">The message.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static ClientErrorHandling SetOrClearErrorMessage(bool hasNoError, string message, string fieldName, string value)
		{
			var error = new ClientErrorHandling {Message = message, FieldName = fieldName, Value = value};
			List<ClientErrorHandling> currentErrors = GetErrorMessages();
			currentErrors.RemoveAll(err => err.FieldName == fieldName && err.Message == message);
			if (!hasNoError)
				currentErrors.Add(error);
			SetErrorMessages(currentErrors);
			return hasNoError ? null : error;
		}

		private static void SetErrorMessages(List<ClientErrorHandling> messages)
		{
			if (HttpContext.Current.Session != null)
			{
				HttpContext.Current.Session.Add(Constants.ErrorMessagesSessionKey, messages);
			}
		}

		/// <summary>
		/// Gets the error messages.
		/// </summary>
		/// <returns></returns>
		public static List<ClientErrorHandling> GetErrorMessages()
		{
			if (HttpContext.Current.Session != null && HttpContext.Current.Session[Constants.ErrorMessagesSessionKey] != null)
				return (List<ClientErrorHandling>) HttpContext.Current.Session[Constants.ErrorMessagesSessionKey];
			return new List<ClientErrorHandling>();
		}

		/// <summary>
		/// Authentications the webshop exception.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <exception cref="System.Exception"></exception>
		public static void uWebshopException(string message)
		{
			throw new Exception(message);
			//var response = HttpContext.Current.Response;
			//response.Write(message);
			//response.ContentType = MediaTypeNames.Text.Plain;
			//response.StatusCode = 450;
		}
	}
}