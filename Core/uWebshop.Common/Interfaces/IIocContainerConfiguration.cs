using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uWebshop.Common.Interfaces
{
	public interface IIocContainerConfiguration
	{
		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="T">The interface</typeparam>
		/// <typeparam name="T1">The implementation</typeparam>
		void RegisterType<T, T1>() where T1 : T;

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="T">The interface</typeparam>
		/// <typeparam name="T1">The type of the instance or the interface (unused but required)</typeparam>
		/// <param name="instance">The instance.</param>
		void RegisterInstance<T, T1>(T1 instance) where T1 : T;
	}
}
