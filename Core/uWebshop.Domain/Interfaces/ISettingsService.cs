using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISettingsService : ISettings
	{
		/// <summary>
		/// Registers the settings changed event.
		/// </summary>
		/// <param name="e">The decimal.</param>
		void RegisterSettingsChangedEvent(Action<ISettings> e);

		/// <summary>
		/// Triggers the store changed event.
		/// </summary>
		/// <param name="settings">The settings.</param>
		void TriggerSettingsChangedEvent(ISettings settings);
	}
	
	/// <summary>
	/// 
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// Are all prices in the store including VAT?
		/// </summary>
		/// <value>
		///   <c>true</c> if [including vat]; otherwise, <c>false</c>.
		/// </value>
		bool IncludingVat { get; }

		/// <summary>
		/// Render all url's lowercase instead of Case Sensitive
		/// </summary>
		/// <value>
		///   <c>true</c> if [use lowercase urls]; otherwise, <c>false</c>.
		/// </value>
		bool UseLowercaseUrls { get; }

		/// <summary>
		/// Lifetime of an incomplete order in minutes
		/// </summary>
		/// <value>
		/// The incomple order lifetime.
		/// </value>
		int IncompleteOrderLifetime { get; }
	}
}