using System.Collections.Generic;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Core
{
	interface IUwebshopAddon
	{
		string Name();
		IEnumerable<IDependencyRegistration> GetDependencyRegistrations();
		IEnumerable<IStateInitialization> GetStateInitializations();
	}

	public interface IDependencyRegistration
	{
		string Description();
		int Order();
		void Register(IRegistrationControl control);
	}

	public interface IStateInitialization
	{
		string Description();
		int Order();
		void Initialize(IInitializationControl control);
	}

	public interface IRegistrationControl : IIocContainerConfiguration, IInitializationControl
	{
	}

	public interface IInitializationControl
	{
		void Done();
		void FatalError(string message = null); // when this happens, possibly redo entire initialization without this addon
		void NotNow();
		void Debug(string message);
		IDependencyResolver Resolver { get; }
	}
}