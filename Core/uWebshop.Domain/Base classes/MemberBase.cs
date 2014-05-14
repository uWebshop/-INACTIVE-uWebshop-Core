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
			Member = Membership.GetUser(id);
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.MemberBase class
		/// </summary>
		/// <param name="email">Email of the member</param>
		public MemberBase(string email)
		{
			string userName = Membership.GetUserNameByEmail(email);
			Member = Membership.GetUser(userName);
		}
	}
}