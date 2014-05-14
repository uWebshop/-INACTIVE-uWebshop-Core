using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	internal class IO
	{
		public static IDependencyResolver Container { get; internal set; }
	}

	// very simple handmade IoC Container, only serving singletons
	internal class IoCContainer : IIocContainer
	{
		private readonly ConcurrentDictionary<Type, object> _instanceMap = new ConcurrentDictionary<Type, object>();
		private readonly ConcurrentDictionary<Type, Type> _typeMap = new ConcurrentDictionary<Type, Type>();

		private IServiceFactory _defaultServiceFactory;

		public void RegisterType<T, T1>() where T1 : T
		{
			_typeMap.AddOrUpdate(typeof (T), typeof (T1), (t1, t2) => typeof (T1));
			object outArg;
			_instanceMap.TryRemove(typeof (T), out outArg);
		}

		public void RegisterInstance<T, T1>(T1 instance) where T1 : T
		{
			_instanceMap.AddOrUpdate(typeof (T), instance, (t, o) => instance);
		}

		public void SetDefaultServiceFactory(IServiceFactory serviceFactory)
		{
			_defaultServiceFactory = serviceFactory;
		}

		public T Resolve<T>() where T : class
		{
			object resolvedInstance = _instanceMap.GetOrAdd(typeof (T), InstanceFactory);
			if (resolvedInstance != null)
			{
				return (T) resolvedInstance;
			}
			if (_defaultServiceFactory == null) throw new Exception("IoC container could not resolve type " + typeof (T).Name);
			return _defaultServiceFactory.Build<T>();
		}

		private object InstanceFactory(Type type)
		{
			Type resolvedType = _typeMap.ContainsKey(type) ? _typeMap[type] : null;
			if (resolvedType == null)
			{
				if (_defaultServiceFactory != null)
					return null;
				throw new Exception("IoC container could not resolve type " + type.Name);
			}

			ConstructorInfo constructor = resolvedType.GetConstructors().First();
			var parameters = constructor.GetParameters().Select(p => new Tuple<Type, object>(p.ParameterType, _instanceMap.GetOrAdd(p.ParameterType, InstanceFactory))).ToArray();

			parameters.Where(p => p.Item2 == null).ToList().ForEach(p => { throw new Exception("IoC container could not resolve type " + type.Name + " because of required service " + p.Item1.Name); });
			
			var result = parameters.Any() ? constructor.Invoke(parameters.Select(p => p.Item2).ToArray()) : Activator.CreateInstance(resolvedType);
			if (result == null) throw new Exception("IoC container could not resolve type " + type.Name);
			return result;
		}
	}
}