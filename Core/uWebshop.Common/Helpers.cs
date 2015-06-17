using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace uWebshop.Common
{
	public static class Helpers
	{
		public static int ParseInt(string toParse, int defaultValue = 0)
		{
			if (string.IsNullOrEmpty(toParse) || toParse.Any(char.IsLetter)) return defaultValue;
			int result;
			return int.TryParse(toParse, out result) ? result : defaultValue;
		}

		public static DateTime? DateTimeMultiCultureParse(string value, CultureInfo culture = null)
		{
			DateTime val;
			if (culture == null || !DateTime.TryParse(value, culture.DateTimeFormat, DateTimeStyles.None, out val))
				if (!DateTime.TryParse(value, new CultureInfo("EN-us").DateTimeFormat, DateTimeStyles.None, out val))
					if (!DateTime.TryParse(value, new CultureInfo("NL-nl").DateTimeFormat, DateTimeStyles.None, out val))
						if (!DateTime.TryParse(value, out val)) 
							return null;
			return val;
		}
	}
}