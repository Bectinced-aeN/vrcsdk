using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRC.Core.BestHTTP.JSON;

namespace AmplitudeSDKWrapper
{
	internal class Settings
	{
		private const int WRITE_TO_STORAGE_DELAY_MS = 20000;

		private string _containerName = string.Empty;

		private Dictionary<string, object> _data = new Dictionary<string, object>();

		private readonly object _dataLock = new object();

		private bool _hasWrittenToStorage;

		private int _lastTimeWrittenToStorage;

		private string _tempCachePath;

		internal Settings(string containerName)
		{
			_containerName = containerName;
			_tempCachePath = Application.get_temporaryCachePath();
			LoadFromStorage();
		}

		internal T Get<T>(string key)
		{
			key = _containerName + "_" + key;
			object value = null;
			T result = default(T);
			lock (_dataLock)
			{
				if (_data == null)
				{
					Debug.LogError((object)("AmplitudeAPI: Settings.Get " + key + ": _data wasn't initialized"));
					_data = new Dictionary<string, object>();
				}
				try
				{
					if (!_data.TryGetValue(key, out value))
					{
						return result;
					}
					result = (T)Convert.ChangeType(value, typeof(T));
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("AmplitudeAPI: exception reading <" + typeof(T).GetType().Name + "> setting - " + key + ", val `" + ((value == null) ? "null" : value.ToString()) + "`: "));
					Debug.LogException(ex);
					return result;
				}
			}
		}

		internal void Save<T>(string key, T value)
		{
			key = _containerName + "_" + key;
			lock (_dataLock)
			{
				if (_data == null)
				{
					Debug.LogError((object)("AmplitudeAPI: Settings.Save " + key + ": _data wasn't initialized"));
					_data = new Dictionary<string, object>();
				}
				_data[key] = value;
			}
			if (!_hasWrittenToStorage || Mathf.Max(20000 - (Environment.TickCount - _lastTimeWrittenToStorage), 0) == 0)
			{
				_hasWrittenToStorage = true;
				_lastTimeWrittenToStorage = Environment.TickCount;
				WriteToStorage();
			}
		}

		private void LoadFromStorage()
		{
			lock (_dataLock)
			{
				try
				{
					string json = File.ReadAllText(GetSettingsFilePath());
					Dictionary<string, object> dictionary = Json.Decode(json) as Dictionary<string, object>;
					if (dictionary == null)
					{
						throw new Exception("Couldn't decode JSON");
					}
					_data = dictionary;
				}
				catch (FileNotFoundException)
				{
				}
				catch (DirectoryNotFoundException)
				{
				}
				catch (Exception ex3)
				{
					Debug.LogError((object)"AmplitudeAPI: Settings.LoadFromStorage: exception loading settings:");
					Debug.LogException(ex3);
				}
				if (_data == null)
				{
					_data = new Dictionary<string, object>();
				}
			}
		}

		private string GetSettingsFilePath()
		{
			return Path.Combine(_tempCachePath, "settings_" + _containerName);
		}

		internal void WriteToStorage()
		{
			lock (_dataLock)
			{
				if (_data == null)
				{
					Debug.LogError((object)"AmplitudeAPI: Settings.WriteToStorage: _data wasn't initialized");
					_data = new Dictionary<string, object>();
				}
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(GetSettingsFilePath()));
					File.WriteAllText(GetSettingsFilePath(), Json.Encode(_data));
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("AmplitudeAPI: Couldn't save settings: " + GetSettingsFilePath() + "\n" + ex.Message));
				}
			}
		}
	}
}
