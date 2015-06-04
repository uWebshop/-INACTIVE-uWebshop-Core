using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common.ServiceInterfaces;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;

namespace uWebshop.DataAccess
{
	sealed class LegacyDataAccessModule : SimpleAddon
	{
		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 14;
		} 
		public override string Name()
		{
			return "PlainSQLOnLegacyUmbracoDatahelper";
		}

		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<ICouponCodeService, CouponCodeService>();
		}

		public override int StateInitializationOrder()
		{
			return 1;
		}

		public override void StateInitialization(IInitializationControl control)
		{
			uWebshopOrders.ConnectionString = control.Resolver.Resolve<IUwebshopConfiguration>().ConnectionString;
			base.StateInitialization(control);
		}
	}
}
