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
			lock (_data)
			{
				try
				{
					if (!_data.TryGetValue(key, out value))
					{
						return result;
					}
					result = (T)Convert.ChangeType(value, typeof(T));
					return result;
				}
				catch
				{
					Debug.LogError((object)("AmplitudeAPI: can't cast settings key - " + key + ", val `" + ((value == null) ? "null" : value.ToString()) + "` type " + ((value == null) ? string.Empty : value.GetType().ToString()) + " - to type - " + typeof(T).ToString()));
					return result;
				}
			}
		}

		internal void Save<T>(string key, T value)
		{
			key = _containerName + "_" + key;
			lock (_data)
			{
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
			lock (_data)
			{
				try
				{
					string json = File.ReadAllText(GetSettingsFilePath());
					_data = (Json.Decode(json) as Dictionary<string, object>);
				}
				catch (Exception)
				{
					goto end_IL_002f;
					IL_0035:
					end_IL_002f:;
				}
			}
		}

		private string GetSettingsFilePath()
		{
			return Path.Combine(_tempCachePath, "settings_" + _containerName);
		}

		internal void WriteToStorage()
		{
			lock (_data)
			{
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(GetSettingsFilePath()));
					File.WriteAllText(GetSettingsFilePath(), Json.Encode(_data));
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("AmplitudeAPI: Couldn't save settings: " + GetSettingsFilePath() + "\n" + ex.Message));
					goto end_IL_0039;
					IL_005f:
					end_IL_0039:;
				}
			}
		}
	}
}
