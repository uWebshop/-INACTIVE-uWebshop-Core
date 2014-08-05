using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// Class representing a range
	/// </summary>
	[DataContract(Namespace = "")]
	public class Range
	{
		/// <summary>
		/// Gets the from value of the range
		/// </summary>
		/// <value>
		/// The from value.
		/// </value>
		[DataMember]
		public int From { get; set; }

		/// <summary>
		/// Gets the to value of the range
		/// </summary>
		/// <value>
		/// The to value.
		/// </value>
		[DataMember]
		public int To { get; set; }

		/// <summary>
		/// Gets the amount of the range
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[DataMember]
		public int PriceInCents { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}|{1}|{2}", From, To, PriceInCents);
		}

		/// <summary>
		/// Creates from string.
		/// </summary>
		/// <param name="rangesData">The ranges data.</param>
		/// <returns></returns>
		public static List<Range> CreateFromString(string rangesData)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(rangesData) || rangesData.Contains("Range")) return new List<Range>();
				return rangesData.Split('#').Select(rangeCode => rangeCode.Split('|')).Where(splitStrings => splitStrings.Length > 2 && !(int.Parse(splitStrings[0]) == 0 && int.Parse(splitStrings[1]) == 0 && splitStrings[2] == "0")).Select(
					splitStrings => 
						new Range
						{
							From = int.Parse(splitStrings[0]), 
							PriceInCents = int.Parse(splitStrings[2]),
							To = splitStrings[1] == "*" || splitStrings[1] == "0" || splitStrings[1] == "" ? int.MaxValue : int.Parse(splitStrings[1]),
						}).ToList();
			}
			catch (Exception ex)
			{
				Log.Instance.LogDebug("Exception in parsing ranges data: " + ex);
				return new List<Range>();
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class RangesListExtensions
	{
		/// <summary>
		/// Automatics the ranges string.
		/// </summary>
		/// <param name="ranges">The ranges.</param>
		/// <returns></returns>
		public static string ToRangesString(this IEnumerable<Range> ranges)
		{
			return string.Join("#", ranges.Select(range => range.ToString()));
		}

		/// <summary>
		/// Finds the range for value.
		/// </summary>
		/// <param name="ranges">The ranges.</param>
		/// <param name="itemCount">The item count.</param>
		/// <returns></returns>
		public static Range FindRangeForValue(this IEnumerable<Range> ranges, int itemCount)
		{
			return ranges.FirstOrDefault(x => itemCount >= x.From && itemCount < x.To);
		}

		/// <summary>
		/// Gets the range amount for value.
		/// </summary>
		/// <param name="ranges">The ranges.</param>
		/// <param name="itemCount">The item count.</param>
		/// <returns></returns>
		public static int? GetRangeAmountForValue(this IEnumerable<Range> ranges, int itemCount)
		{
			var range = FindRangeForValue(ranges, itemCount);
			if (range == null) return null;
			return range.PriceInCents;
		}
	}
}