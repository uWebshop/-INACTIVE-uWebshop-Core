using System.Collections.Generic;
using System.Web.Security;
using uWebshop.Common.Interfaces;
using umbraco.cms.businesslogic.member;
using System.Web;
using Umbraco.Core;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDotnetMembershipAuthenticationProvider : IAuthenticationProvider
	{
		public IEnumerable<string> RolesForCurrentUser
		{
			get
			{

				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					return Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name);
				}
				
				return new string[] {};
			}
		}

		public string CurrentLoginName
		{
			get
			{
                var username = HttpContext.Current.User.Identity.Name;

                return !string.IsNullOrEmpty(username) ? username : null;
			}
		}
	}
}