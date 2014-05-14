using System.Collections.Generic;
using System.Web.Security;
using uWebshop.Common.Interfaces;
using umbraco.cms.businesslogic.member;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDotnetMembershipAuthenticationProvider : IAuthenticationProvider
	{
		public IEnumerable<string> RolesForCurrentUser
		{
			get
			{
				var umbracoMember = Member.GetCurrentMember();
				if (umbracoMember != null)
				{
					var user = Membership.GetUser(umbracoMember.LoginName);
					if (user != null)
					{
						return Roles.GetRolesForUser(user.UserName);
					}
				}
				return new string[] {};
			}
		}

		public string CurrentLoginName
		{
			get
			{
				var umbracoMember = Member.GetCurrentMember();

				return umbracoMember.LoginName;
			}
		}
	}
}