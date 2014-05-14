using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Core;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

[assembly: PreApplicationStartMethod(typeof(Initialize), "Reboot")]

namespace uWebshop.Domain.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class Initialize
	{
		public static int FinishedRegistrationLevel { get; private set; }
		public static int FinishedInitializationLevel { get; private set; }
		public static bool CoreServiceLocatorsInitialized { get; private set; }

		private static Lazy<IEnumerable<IUwebshopAddon>> _addons;
		private static IoCContainer _iocContainer;

		private static int _firstTimeBooted;
		private static int _currentlyInitializing;
		private static readonly object _initializingLockObject = new object();

		/// <summary>
		/// (Re)boots uWebshop.
		/// </summary>
		public static void Reboot()
		{
			lock (_initializingLockObject)
			{
				FinishedRegistrationLevel = 0;
				FinishedInitializationLevel = 0;
				CoreServiceLocatorsInitialized = false;
				_iocContainer = new IoCContainer();
				_addons = new Lazy<IEnumerable<IUwebshopAddon>>(LoadAddons);
			}
			InitializeAsFarAsPossible();
		}

		internal static void ContinueInitialization()
		{
			if (Interlocked.CompareExchange(ref _firstTimeBooted, 1, 0) == 0)
			{
				Reboot();
			}
			else
			{
				InitializeAsFarAsPossible();
			}
		}
		
		private static void InitializeAsFarAsPossible()
		{
			if (Interlocked.CompareExchange(ref _currentlyInitializing, 1, 0) > 0)
			{
				return; // was already triggered, don't trigger again at the same time
			}
			lock (_initializingLockObject)
			{
				if (!CoreServiceLocatorsInitialized)
				{
					if (StartOrContinuRegisteringDependencies())
					{
						_currentlyInitializing = 0;
						return;
					}

					InitializeServiceLocators(_iocContainer);
				}

				StartOrContinuInitializingModulesState();
				_currentlyInitializing = 0;
			}
		}

		private static void StartOrContinuInitializingModulesState()
		{
			// todo: don't retry indefinately
			// todo: don't wait indefinately for external code (use ask based implementation)
			// todo: make sure no initialization has the same order ( * 1000 + counter)
			// these points will weigh more heavily once external plugins are made and used
			var failedInitializations = new List<Tuple<IUwebshopAddon, IStateInitialization, Exception>>();
			foreach (var registration in _addons.Value.SelectMany(a => a.GetStateInitializations().Select(dr => new {Addon = a, Initialization = dr})).Where(d => d.Initialization.Order() > FinishedInitializationLevel).OrderBy(d => d.Initialization.Order()))
			{
				var initializationControl = new InitializationControl();
				try
				{
					registration.Initialization.Initialize(initializationControl); // never trust external code
				}
				catch (Exception e)
				{
					failedInitializations.Add(new Tuple<IUwebshopAddon, IStateInitialization, Exception>(registration.Addon, registration.Initialization, e));
				}
				if (initializationControl.NotNowCalled)
				{
					break;
				}
				FinishedInitializationLevel = registration.Initialization.Order();
			}
			foreach (var initialization in failedInitializations)
			{
				// possible todo: redo everything without failed addon (at least don't call initialization on that addon)

				Log.Instance.LogError(initialization.Item3, "Failed registering addon " + initialization.Item1.Name() + " on part " + initialization.Item2.Description());
			}
		}

		internal static void InitializeServiceLocators(IDependencyResolver container)
		{
			IO.Container = container;
			UwebshopConfiguration.Current = container.Resolve<IUwebshopConfiguration>();
			UwebshopRequest.Service = container.Resolve<IUwebshopRequestService>();
			StoreHelper.StoreService = container.Resolve<IStoreService>();
			CoreServiceLocatorsInitialized = true;
		}

		private static bool StartOrContinuRegisteringDependencies()
		{
			// todo: don't retry indefinately
			// todo: don't wait indefinately for external code (use ask based implementation)
			// todo: make sure no registration has the same order ( * 1000 + counter)
			// these points will weigh more heavily once external plugins are made and used
			var failedRegistrations = new List<Tuple<IUwebshopAddon, IDependencyRegistration, Exception>>();
			foreach (var registration in _addons.Value.SelectMany(a => a.GetDependencyRegistrations().Select(dr => new {Addon = a, DependencyRegistration = dr})).Where(d => d.DependencyRegistration.Order() > FinishedRegistrationLevel).OrderBy(d => d.DependencyRegistration.Order()))
			{
				var control = new RegistrationControl(_iocContainer);
				try
				{
					registration.DependencyRegistration.Register(control); // never trust external code
				}
				catch (Exception e)
				{
					failedRegistrations.Add(new Tuple<IUwebshopAddon, IDependencyRegistration, Exception>(registration.Addon, registration.DependencyRegistration, e));
					throw;
				}
				if (control.NotNowCalled)
				{
					return true;
				}
				FinishedRegistrationLevel = registration.DependencyRegistration.Order();
			}
			foreach (var registration in failedRegistrations)
			{
				// possible todo: redo registration without failed addon (at least don't call initialization on that addon)

				Log.Instance.LogError(registration.Item3, "Failed registering addon " +registration.Item1.Name() + " on part " + registration.Item2.Description());
			}
			return false;
		}

		private static IEnumerable<IUwebshopAddon> LoadAddons()
		{
			var targetType = typeof(IUwebshopAddon);
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			path = Path.GetDirectoryName(path);
			//path = AppDomain.CurrentDomain.BaseDirectory + "/bin"; // eventueel als fallback
				
			var files = Directory.GetFiles(path);
			var dlls = files.Select(filepath => new FileInfo(filepath)).Where(fileInfo => fileInfo.Name.ToLowerInvariant().StartsWith("uwebshop.") && fileInfo.Name.ToLowerInvariant().EndsWith(".dll")).ToList();
			if (dlls.Count == 0) throw new Exception("uWebshop dlls not found on location: " + path);

			var addons = new List<IUwebshopAddon>();
			foreach (var dll in dlls)
			{
				try
				{
					var assembly = Assembly.LoadFrom(dll.FullName);

					addons.AddRange(GetMatchingTypesInAssembly(assembly, type => targetType.IsAssignableFrom(type) && targetType != type && !type.IsAbstract).Select(type =>
							                {
								                try
								                {
									                return (IUwebshopAddon) Activator.CreateInstance(type);
								                }
								                catch (Exception)
								                {
									                return null;
								                }
							                }));
				}
				catch (Exception ex) { }
			}
			return addons.Where(a => a != null);
		}
		private static IEnumerable<Type> GetMatchingTypesInAssembly(Assembly assembly, Predicate<Type> predicate)
		{
			try
			{
				return assembly.GetTypes().Where(i => i != null && predicate(i) && i.Assembly == assembly).ToList();
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types.Where(theType =>
					{
						try { return (theType != null && predicate(theType) && theType.Assembly == assembly); }
						catch (BadImageFormatException) { // Type not in this assembly - reference to elsewhere ignored
						}
						return false;
					});
			}
		}
	public class InitializationControl : IInitializationControl
		{
			internal bool NotNowCalled = false;
			public void Done()
			{

			}

			public void FatalError(string message = null)
			{

			}

			public void NotNow()
			{
				NotNowCalled = true;
			}

			public void Debug(string message)
			{

			}
		}
	public class RegistrationControl : InitializationControl, IRegistrationControl
		{
			private readonly IIocContainerConfiguration _container;

			public RegistrationControl(IIocContainerConfiguration container)
			{
				_container = container;
			}

			public void RegisterType<T, T1>() where T1 : T
			{
				_container.RegisterType<T, T1>();
			}

			public void RegisterInstance<T, T1>(T1 instance) where T1 : T
			{
				_container.RegisterInstance<T, T1>(instance);
			}
		}
	}
}
