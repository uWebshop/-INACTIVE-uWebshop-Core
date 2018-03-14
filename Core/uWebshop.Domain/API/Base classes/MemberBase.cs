using System.Web;
using System.Web.Profile;
using System.Web.Security;

namespace uWebshop.Domain.BaseClasses
{
	/// <summary>
	///     Class based on the Umbraco Document class to inherit from
	/// </summary>
	public class MemberBase
	{
		/// <summary>
		///     Gets the member
		/// </summary>
		//public Member Member { get; protected set; }
		public MembershipUser Member { get; protected set; }

		/// <summary>
		///     Gets the id of the member
		/// </summary>
		public int Id
		{
			get { return (int) Member.ProviderUserKey; }
		}

		/// <summary>
		/// Gets the profile.
		/// </summary>
		/// <value>
		/// The profile.
		/// </value>
		public ProfileBase Profile
		{
			get { return HttpContext.Current.Profile; }
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.MemberBase class
		/// </summary>
		/// <param name="id">NodeId of the member</param>
		public MemberBase(int id)
		{
            const string key = "MemberBaseByIdPerRequest";

            if (HttpContext.Current.Items[key] == null)
            {
                HttpContext.Current.Items[key] = Membership.GetUser(id);
            }
  
            Member = (MembershipUser)HttpContext.Current.Items[key];

        }

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.MemberBase class
		/// </summary>
		/// <param name="email">Email of the member</param>
		public MemberBase(string email)
		{

            const string key = "MemberBaseByEmailPerRequest";
            const string key2 = "MemberBaseGetUserByEmailPerRequest";

            if (HttpContext.Current.Items[key] == null) {
                HttpContext.Current.Items[key] = Membership.GetUserNameByEmail(email);
            }

            if (HttpContext.Current.Items[key2] == null && HttpContext.Current.Items[key] != null)
            {
                HttpContext.Current.Items[key2] = Membership.GetUser((string)HttpContext.Current.Items[key]);
            }

            Member = (MembershipUser)HttpContext.Current.Items[key2];
        }
	}
}