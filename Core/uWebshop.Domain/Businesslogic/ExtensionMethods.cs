using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using uWebshop.Common;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		///   <para>Creates a log-string from the Exception.</para>
		///   <para>
		/// The result includes the stacktrace, innerexception et cetera, separated by
		///   <seealso cref="Environment.NewLine" />.
		///   </para>
		/// </summary>
		/// <param name="ex">The exception to create the string from.</param>
		/// <param name="printTrace">if set to <c>true</c> [print trace].</param>
		/// <param name="additionalMessage">Additional message to place at the top of the string, maybe be empty or null.</param>
		/// <returns></returns>
		public static string ToLogString(this Exception ex, bool printTrace = true, string additionalMessage = "")
		{
			var msg = new StringBuilder();

			if (!string.IsNullOrEmpty(additionalMessage))
			{
				msg.Append(additionalMessage);
				msg.Append(Environment.NewLine);
			}

			if (ex == null) return msg.ToString();

			AddExceptionMessagesRecursive(ex, msg);

			if (!HttpContext.Current.IsDebuggingEnabled) return msg.ToString();

			foreach (var i in ex.Data)
			{
				msg.Append("Data :");
				msg.Append(i);
				msg.Append(Environment.NewLine);
			}

			if (printTrace && ex.StackTrace != null)
			{
				msg.Append("StackTrace:");
				msg.Append(Environment.NewLine);
				msg.Append(ex.StackTrace);
				msg.Append(Environment.NewLine);
			}

			if (ex.InnerException != null)
			{
				msg.Append("Inner:");
				msg.Append(Environment.NewLine);
				ToLogString(ex.InnerException, msg);
			}
			return msg.ToString();
		}

		private static void AddExceptionMessagesRecursive(Exception ex, StringBuilder msg)
		{
			Exception orgEx = ex;

			msg.Append("Ex: ");
			msg.Append(Environment.NewLine);
			while (orgEx != null)
			{
				msg.Append(orgEx.Message);
				msg.Append(Environment.NewLine);
				orgEx = orgEx.InnerException;
			}
		}

		/// <summary>
		/// Creates the log string.
		/// </summary>
		/// <param name="ex">The executable.</param>
		/// <param name="msg">The MSG.</param>
		public static void ToLogString(this Exception ex, StringBuilder msg)
		{
			AddExceptionMessagesRecursive(ex, msg);

			if (!HttpContext.Current.IsDebuggingEnabled) return;

			foreach (var i in ex.Data)
			{
				msg.Append("Data :");
				msg.Append(i);
				msg.Append(Environment.NewLine);
			}

			if (ex.InnerException != null)
			{
				msg.Append("Inner:");
				msg.Append(Environment.NewLine);
				ToLogString(ex.InnerException, msg);
			}
		}

		/// <summary>
		/// Gets the adjusted price.
		/// </summary>
		/// <param name="discount">The discount.</param>
		/// <param name="priceBeforeDiscount">The price before discount.</param>
		/// <returns></returns>
		public static int GetAdjustedPrice(this IProductDiscount discount, int priceBeforeDiscount, int orderTotalItemCount = 0)
		{
			if (discount == null || discount.Type == DiscountType.FreeShipping) return priceBeforeDiscount;

			var discountValue = discount.DiscountValue;// discount.RangedDiscountValue(orderTotalItemCount);
			if (discount.Type == DiscountType.Percentage)
			{
				return priceBeforeDiscount - DiscountHelper.PercentageCalculation(priceBeforeDiscount, discountValue);
			}
			if (discount.Type == DiscountType.Amount)
			{
				return priceBeforeDiscount - discountValue;
			}
			if (discount.Type == DiscountType.NewPrice)
			{
				return discountValue;
			}
			return priceBeforeDiscount;
		}

		internal static int ValueInCents(this IVatPrice price)
		{
			if (price is CombiPrice) return (price as CombiPrice).ValueInCents;
			if (price is IPrice) return (price as IPrice).ValueInCents;
			var source = (Price)price;
			return source.ValueInCents;
		}
	}
}