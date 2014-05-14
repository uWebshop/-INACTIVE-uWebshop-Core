using System;
using System.Collections.Generic;
using System.Web;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Core;
using uWebshop.Domain.Helpers;
using uWebshop.Umbraco.Interfaces;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Umbraco.Modules
{
	class Module : IUwebshopAddon 
	{
		private const string _Name = "Configurable aliasses";
		private static UwebshopAliassesXMLConfig _aliasses;

		public string Name()
		{
			return _Name;
		}

		public IEnumerable<IDependencyRegistration> GetDependencyRegistrations()
		{
			return new[] {new RegisterTypes(),};
		}

		public IEnumerable<IStateInitialization> GetStateInitializations()
		{
			return new[] {new LoadXmlFile(),};
		}

		class LoadXmlFile : IStateInitialization
		{
			public string Description()
			{
				return _Name + ", load XML file";
			}

			public int Order()
			{
				return InitializationOrder.ContentTypeAliasses;
			}

			public void Initialize(IInitializationControl control)
			{
				if (HttpContext.Current == null)
				{
					control.NotNow();
					return;
				}
				_aliasses = new UwebshopAliassesXMLConfig();
				const string path = "/App_Plugins/uWebshop/config/ContentMapping.config";
				if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(path)))
				{
					control.Debug("No Aliasses.config");
				}
				else
				{
					try
					{
						_aliasses = DomainHelper.DeserializeXmlStringToObject<UwebshopAliassesXMLConfig>(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(path)));
						Log.Instance.LogDebug("ContentMapping xml loaded");
						control.Debug("Done loading Aliasses.config");
					}
					catch (Exception ex)
					{
						Log.Instance.LogError(ex, "ContentMapping xml loading failed");
						control.FatalError("Failure deserializing " + path);
						throw;
					}
				}

				InitNodeAliasses.Initialize(_aliasses);
			}
		}

		class ContentTypeAliassesXmlService : IContentTypeAliassesXmlService
		{
			public UwebshopAliassesXMLConfig Get()
			{
				return _aliasses;
			}
		}

		class RegisterTypes : IDependencyRegistration
		{
			public string Description()
			{
				return _Name;
			}

			public int Order()
			{
				return RegistrationOrder.InternalNoDependencies;
			}

			public void Register(IRegistrationControl control)
			{
				control.RegisterType<IContentTypeAliassesXmlService, ContentTypeAliassesXmlService>();
				ModuleFunctionality.Register(control);
			}
		}
	}
}
