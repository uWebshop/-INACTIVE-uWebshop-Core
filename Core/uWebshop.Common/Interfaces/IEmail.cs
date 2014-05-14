namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IEmail : IUwebshopRepositoryEntity 
	{
		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description { get; }

		/// <summary>
		/// Gets the template.
		/// </summary>
		/// <value>
		/// The template.
		/// </value>
		string Template { get; }
	}
}