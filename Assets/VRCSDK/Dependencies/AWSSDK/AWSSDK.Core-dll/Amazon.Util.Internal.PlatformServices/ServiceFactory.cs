using System;
using System.Collections.Generic;

namespace Amazon.Util.Internal.PlatformServices
{
	public class ServiceFactory
	{
		private enum InstantiationModel
		{
			Singleton,
			InstancePerCall
		}

		internal const string NotImplementedErrorMessage = "This functionality is not implemented in the portable version of this assembly. You should reference the AWSSDK.Core NuGet package from your main application project in order to reference the platform-specific implementation.";

		private static readonly object _lock = new object();

		private static bool _factoryInitialized = false;

		private static IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>
		{
			{
				typeof(IApplicationSettings),
				typeof(ApplicationSettings)
			},
			{
				typeof(INetworkReachability),
				typeof(NetworkReachability)
			},
			{
				typeof(IApplicationInfo),
				typeof(ApplicationInfo)
			},
			{
				typeof(IEnvironmentInfo),
				typeof(EnvironmentInfo)
			}
		};

		private IDictionary<Type, InstantiationModel> _instantationMappings = new Dictionary<Type, InstantiationModel>
		{
			{
				typeof(IApplicationSettings),
				InstantiationModel.InstancePerCall
			},
			{
				typeof(INetworkReachability),
				InstantiationModel.Singleton
			},
			{
				typeof(IApplicationInfo),
				InstantiationModel.Singleton
			},
			{
				typeof(IEnvironmentInfo),
				InstantiationModel.Singleton
			}
		};

		private IDictionary<Type, object> _singletonServices = new Dictionary<Type, object>();

		public static ServiceFactory Instance = new ServiceFactory();

		private ServiceFactory()
		{
			foreach (KeyValuePair<Type, InstantiationModel> instantationMapping in _instantationMappings)
			{
				Type key = instantationMapping.Key;
				if (instantationMapping.Value == InstantiationModel.Singleton)
				{
					object value = Activator.CreateInstance(_mappings[key]);
					_singletonServices.Add(key, value);
				}
			}
			_factoryInitialized = true;
		}

		public static void RegisterService<T>(Type serviceType)
		{
			if (_factoryInitialized)
			{
				throw new InvalidOperationException("New services can only be registered before ServiceFactory is accessed by calling ServiceFactory.Instance .");
			}
			lock (_lock)
			{
				_mappings[typeof(T)] = serviceType;
			}
		}

		public T GetService<T>()
		{
			Type typeFromHandle = typeof(T);
			if (_instantationMappings[typeFromHandle] == InstantiationModel.Singleton)
			{
				return (T)_singletonServices[typeFromHandle];
			}
			return (T)Activator.CreateInstance(GetServiceType<T>());
		}

		private static Type GetServiceType<T>()
		{
			lock (_lock)
			{
				return _mappings[typeof(T)];
			}
		}
	}
}
