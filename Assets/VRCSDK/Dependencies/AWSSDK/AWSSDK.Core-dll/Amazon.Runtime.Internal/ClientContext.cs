using Amazon.Util.Internal.PlatformServices;
using System;
using System.Collections;
using System.Collections.Generic;
using ThirdParty.Json.LitJson;

namespace Amazon.Runtime.Internal
{
	public class ClientContext
	{
		private const string CLIENT_KEY = "client";

		private const string CLIENT_ID_KEY = "client_id";

		private const string CLIENT_APP_TITLE_KEY = "app_title";

		private const string CLIENT_APP_VERSION_NAME_KEY = "app_version_name";

		private const string CLIENT_APP_VERSION_CODE_KEY = "app_version_code";

		private const string CLIENT_APP_PACKAGE_NAME_KEY = "app_package_name";

		private const string CUSTOM_KEY = "custom";

		private const string ENV_KEY = "env";

		private const string ENV_PLATFORM_KEY = "platform";

		private const string ENV_MODEL_KEY = "model";

		private const string ENV_MAKE_KEY = "make";

		private const string ENV_PLATFORM_VERSION_KEY = "platform_version";

		private const string ENV_LOCALE_KEY = "locale";

		private const string SERVICES_KEY = "services";

		private const string SERVICE_MOBILE_ANALYTICS_KEY = "mobile_analytics";

		private const string SERVICE_MOBILE_ANALYTICS_APP_ID_KEY = "app_id";

		private IDictionary<string, string> _client;

		private IDictionary<string, string> _custom;

		private IDictionary<string, string> _env;

		private IDictionary<string, IDictionary> _services;

		private IDictionary _clientContext;

		private static object _lock = new object();

		private static string _clientID = null;

		private const string APP_ID_KEY = "APP_ID_KEY";

		private const string CLIENT_ID_CACHE_FILENAME = "client-ID-cache";

		private const string APP_CLIENT_ID_KEY = "mobile_analytics_client_id";

		private static IApplicationSettings _appSetting = ServiceFactory.Instance.GetService<IApplicationSettings>();

		private static IApplicationInfo _appInfo = ServiceFactory.Instance.GetService<IApplicationInfo>();

		private static IEnvironmentInfo _envInfo = ServiceFactory.Instance.GetService<IEnvironmentInfo>();

		public string AppID
		{
			get;
			set;
		}

		public void AddCustomAttributes(string key, string value)
		{
			lock (_lock)
			{
				if (_custom == null)
				{
					_custom = new Dictionary<string, string>();
				}
				_custom.Add(key, value);
			}
		}

		public string ToJsonString()
		{
			lock (_lock)
			{
				_client = new Dictionary<string, string>();
				_env = new Dictionary<string, string>();
				_services = new Dictionary<string, IDictionary>();
				_client.Add("client_id", _clientID);
				_client.Add("app_title", _appInfo.AppTitle);
				_client.Add("app_version_name", _appInfo.AppVersionName);
				_client.Add("app_version_code", _appInfo.AppVersionCode);
				_client.Add("app_package_name", _appInfo.PackageName);
				_env.Add("platform", _envInfo.Platform);
				_env.Add("platform_version", _envInfo.PlatformVersion);
				_env.Add("locale", _envInfo.Locale);
				_env.Add("make", _envInfo.Make);
				_env.Add("model", _envInfo.Model);
				if (!string.IsNullOrEmpty(AppID))
				{
					IDictionary dictionary = new Dictionary<string, string>();
					dictionary.Add("app_id", AppID);
					_services.Add("mobile_analytics", dictionary);
				}
				_clientContext = new Dictionary<string, IDictionary>();
				_clientContext.Add("client", _client);
				_clientContext.Add("env", _env);
				_clientContext.Add("custom", _custom);
				_clientContext.Add("services", _services);
				return JsonMapper.ToJson(_clientContext);
			}
		}

		public ClientContext(string appId)
		{
			AppID = appId;
			_custom = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(_clientID))
			{
				_clientID = _appSetting.GetValue("mobile_analytics_client_id", ApplicationSettingsMode.Local);
				if (string.IsNullOrEmpty(_clientID))
				{
					_clientID = Guid.NewGuid().ToString();
					_appSetting.SetValue("mobile_analytics_client_id", _clientID, ApplicationSettingsMode.Local);
				}
			}
		}
	}
}
