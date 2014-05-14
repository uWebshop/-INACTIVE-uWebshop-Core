using System.Collections.Generic;

namespace uWebshop.Common.Interfaces
{
	public interface IAuthenticationProvider
	{
		IEnumerable<string> RolesForCurrentUser { get; }
		string CurrentLoginName { get; }
	}
}