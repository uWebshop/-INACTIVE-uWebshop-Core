using System;

namespace uWebshop.Common.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IApplicationCacheTokenService
	{
		/// <summary>
		/// Get the application cache token
		/// </summary>
		/// <returns>The token.</returns>
		Guid GetToken();
		/// <summary>
		/// Sets the application cache token.
		/// </summary>
		/// <param name="token">The token.</param>
		void SetToken(Guid token);
	}
}