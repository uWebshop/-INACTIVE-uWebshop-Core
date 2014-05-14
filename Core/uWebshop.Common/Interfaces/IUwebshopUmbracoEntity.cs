using System;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUwebshopRepositoryEntity : IUwebshopEntity
	{
		/// <summary>
		/// Gets a value indicating whether the entity is disabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if disabled; otherwise, <c>false</c>.
		/// </value>
		bool Disabled { get; }

		/// <summary>
		/// Gets a System.DateTime object that is set to the date and time when the node is created
		/// </summary>
		/// <value>
		/// The create date.
		/// </value>
		DateTime CreateDate { get; }

		/// <summary>
		/// Gets a System.DateTime object that is set to the date and time when the node is updated
		/// </summary>
		/// <value>
		/// The update date.
		/// </value>
		DateTime UpdateDate { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IUwebshopSortableEntity : IUwebshopRepositoryEntity
	{
		/// <summary>
		/// SortOrder for the node
		/// </summary>
		/// <value>
		/// The sort order.
		/// </value>
		int SortOrder { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	// todo: what to do with this file? (it should actually move to the Umbraco project, but it has effects on public API in Domain)
	public interface IUwebshopUmbracoEntity : IUwebshopSortableEntity
	{
		/// <summary>
		/// Gets the node type alias.
		/// </summary>
		/// <value>
		/// The node type alias.
		/// </value>
		string NodeTypeAlias { get; }

		/// <summary>
		/// Gets the name of the node
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets the id of the parent of the node
		/// </summary>
		/// <value>
		/// The parent unique identifier.
		/// </value>
		int ParentId { get; }

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>
		/// The path.
		/// </value>
		string Path { get; }
	}
}