using System;

namespace uWebshop.Common.Interfaces
{
	public interface ILoggingService
	{
		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="message">The message.</param>
		void LogError(Exception exception, string message = null);

		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="message">The message.</param>
		void LogError(string message);

		/// <summary>
		/// Logs an warning.
		/// </summary>
		/// <param name="message">The message.</param>
		void LogWarning(string message);

        /// <summary>
        /// Logs an info.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogInfo(string message);

        /// <summary>
        /// Logs the message when debug logging enabled.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogDebug(string message);

		// voor dev gebruik
	}
}