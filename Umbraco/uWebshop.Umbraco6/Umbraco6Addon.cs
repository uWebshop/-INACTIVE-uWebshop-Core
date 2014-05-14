using System.Collections.Generic;
using System.ComponentModel;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco;
using uWebshop.Umbraco.Interfaces;

namespace uWebshop.Umbraco6
{
	class Umbraco6Addon : SimpleAddon
	{
		public override string Name()
		{
			return "Umbraco v6 coupling";
		}

		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<IUmbracoVersion, UmbracoVersion>();

			if (ApplicationContext.Current == null || ApplicationContext.Current.Services == null)
			{
				control.NotNow();
				return;
			}
			control.RegisterInstance<IContentService, IContentService>(ApplicationContext.Current.Services.ContentService);
			control.RegisterInstance<IContentTypeService, IContentTypeService>(ApplicationContext.Current.Services.ContentTypeService);

			UmbracoAddon.VersionSpecificTypesConfiguredInIOCContainer = true;
		}

		public override int DependencyRegistrationOrder()
		{
			return RegistrationOrder.Logging;
		}
		public override int StateInitializationOrder()
		{
			return InitializationOrder.InternalNoDependencies;
		}
	}
}