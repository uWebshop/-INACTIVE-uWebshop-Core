using System;
using System.Collections.Generic;
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
	}
}