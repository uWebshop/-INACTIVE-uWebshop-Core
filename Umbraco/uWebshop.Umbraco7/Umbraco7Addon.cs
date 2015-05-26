using System;
using Umbraco.Core;
using Umbraco.Core.Services;
using uWebshop.Common;
using uWebshop.Domain.Core;
using uWebshop.Umbraco;
using uWebshop.Umbraco.Interfaces;

namespace uWebshop.Umbraco7
{
	class Umbraco7Addon : SimpleAddon
	{
		public override string Name()
		{
			return "Umbraco v7 coupling";
		}

		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<IUmbracoVersion, UmbracoVersion>();

			if (ApplicationContext.Current == null || ApplicationContext.Current.Services == null)
			{
				control.NotNow();
				return;
			}
			try
			{
				control.RegisterInstance<IContentService, IContentService>(ApplicationContext.Current.Services.ContentService);
				control.RegisterInstance<IContentTypeService, IContentTypeService>(ApplicationContext.Current.Services.ContentTypeService);
			}
			catch (Exception)
			{
				control.NotNow();
				return;
			}

			UmbracoAddon.VersionSpecificTypesConfiguredInIOCContainer = true;
		}

		public override int DependencyRegistrationOrder()
		{
			return RegistrationOrder.Logging;
		}
		public override int StateInitializationOrder()
		{
			return InitializationOrder.InternalNoDependencies + 7;
		}
	}
}