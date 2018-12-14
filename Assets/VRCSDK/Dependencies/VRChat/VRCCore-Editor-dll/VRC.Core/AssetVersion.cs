using UnityEngine;

namespace VRC.Core
{
	public class AssetVersion
	{
		private string _unityVersion;

		private int _apiVersion;

		public string UnityVersion
		{
			get
			{
				if (string.IsNullOrEmpty(_unityVersion))
				{
					_unityVersion = Application.get_unityVersion();
				}
				return _unityVersion;
			}
			set
			{
				_unityVersion = value;
			}
		}

		public int ApiVersion
		{
			get
			{
				return _apiVersion;
			}
			set
			{
				_apiVersion = value;
			}
		}

		public AssetVersion()
		{
			_unityVersion = Application.get_unityVersion();
			_apiVersion = 0;
		}

		public AssetVersion(int apiVersion)
		{
			_unityVersion = string.Empty;
			_apiVersion = apiVersion;
		}

		public AssetVersion(string unityVersion, int apiVersion)
		{
			_unityVersion = unityVersion;
			_apiVersion = apiVersion;
		}

		public override string ToString()
		{
			return _unityVersion + "," + _apiVersion.ToString();
		}
	}
}
