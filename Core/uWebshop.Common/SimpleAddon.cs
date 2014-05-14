using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Core
{
	public abstract class SimpleAddon : IUwebshopAddon
	{
		public abstract string Name();
		public virtual void DependencyRegistration(IRegistrationControl control)
		{

		}
		public virtual int DependencyRegistrationOrder()
		{
			return RegistrationOrder.InternalNoDependencies;
		}
		public virtual void StateInitialization(IInitializationControl control)
		{
			control.Done();
		}
		public virtual int StateInitializationOrder()
		{
			return InitializationOrder.InternalNoDependencies;
		}

		class TypeR : IDependencyRegistration
		{
			private readonly SimpleAddon _addon;

			public TypeR(SimpleAddon addon)
			{
				_addon = addon;
			}

			public string Description()
			{
				return _addon.Name();
			}

			public int Order()
			{
				return _addon.DependencyRegistrationOrder();
			}

			public void Register(IRegistrationControl control)
			{
				_addon.DependencyRegistration(control);
			}
		}

		class StateI : IStateInitialization
		{
			private readonly SimpleAddon _addon;

			public StateI(SimpleAddon addon)
			{
				_addon = addon;
			}

			public string Description()
			{
				return _addon.Name();
			}

			public int Order()
			{
				return _addon.StateInitializationOrder();
			}

			public void Initialize(IInitializationControl control)
			{
				_addon.StateInitialization(control);
			}
		}

		public IEnumerable<IDependencyRegistration> GetDependencyRegistrations()
		{
			return new[] { new TypeR(this), };
		}

		public IEnumerable<IStateInitialization> GetStateInitializations()
		{
			return new[] { new StateI(this), };
		}
	}
}