using uWebshop.Common.Interfaces;
using uWebshop.Domain.Core;

namespace uWebshop.Azure
{
	internal class Module : SimpleAddon
	{
		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 16; // todo: requires thought => should overrule SQL or be configurable
		}
		public override string Name()
		{
			return "uWebshop Azure Cache Module";
		}
		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<IApplicationCacheTokenService, AzureAppCacheTokenService>();
		}
	}
}