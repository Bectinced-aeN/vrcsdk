using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VRC.Core;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Forms;
using VRC.Core.BestHTTP.JSON;

namespace AmplitudeSDKWrapper
{
	public sealed class AmplitudeWrapper
	{
		private enum ErrorCode
		{
			None,
			GeneralError,
			BadRequest,
			TooManyEventsInRequest,
			TooManyRequestsForDevice,
			TimedOut,
			ServerError,
			Exception
		}

		private const string EVENT_LOG_URL = "https://api.amplitude.com/httpapi";

		private const int EVENT_UPLOAD_THRESHOLD = 30;

		private const int EVENT_UPLOAD_MAX_BATCH_SIZE = 10;

		private const int EVENT_MAX_COUNT = 1000;

		private const int EVENT_REMOVE_BATCH_SIZE = 20;

		private const int EVENT_UPLOAD_PERIOD_MILLISECONDS = 30000;

		private const long MIN_TIME_BETWEEN_SESSIONS_MILLIS = 1800000L;

		private const long SESSION_TIMEOUT_MILLIS = 1800000L;

		private const string SETTINGS_CONTAINER = "com.amplitude";

		private const string SETTINGS_KEY_DEVICE_ID = "deviceId";

		private const string SETTINGS_KEY_USER_ID = "userId";

		private const string SETTINGS_KEY_END_SESSION_EVENT_ID = "endSessionEventId";

		private const string SETTINGS_KEY_LAST_EVENT_TIME = "lastEventTime";

		private const string SETTINGS_KEY_PREVIOUS_SESSION_ID = "previousSessionId";

		private const string START_SESSION_EVENT = "session_start";

		private const string END_SESSION_EVENT = "session_end";

		private const int UPLOAD_RETRY_DELAY_MS = 1000;

		private static AmplitudeWrapper instance;

		private string apiKey;

		private string userId;

		private string deviceId;

		private DeviceInfo deviceInfo;

		private string _buildVersionString = string.Empty;

		private long sessionId;

		private Dictionary<string, object> userProperties;

		private int isUploading;

		private int isUpdateScheduled;

		private DatabaseHelper dbHelper;

		private Settings settings;

		private bool sessionOpen;

		private LimitedConcurrencyLevelTaskScheduler httpQueue;

		private LimitedConcurrencyLevelTaskScheduler logQueue;

		private object _serverUpdateLock = new object();

		private int _serverUpdateScheduledTime;

		private int _serverUpdateDelayMs = -1;

		private int _serverUpdateBatchSize;

		private bool _isAppExiting;

		public static AmplitudeWrapper Instance => instance;

		public AmplitudeWrapper(string api_key)
		{
			Init(api_key, null);
		}

		public AmplitudeWrapper(string api_key, string userId)
		{
			Init(api_key, userId);
		}

		public static AmplitudeWrapper Initialize(string api_key)
		{
			return Initialize(api_key, null);
		}

		public static AmplitudeWrapper Initialize(string api_key, string userId)
		{
			if (instance == null)
			{
				instance = new AmplitudeWrapper(api_key, userId);
			}
			return instance;
		}

		private void Init(string api_key, string userId)
		{
			if (string.IsNullOrEmpty(api_key))
			{
				throw new ArgumentException("apiKey must not be null or empty");
			}
			apiKey = api_key;
			httpQueue = new LimitedConcurrencyLevelTaskScheduler(1);
			logQueue = new LimitedConcurrencyLevelTaskScheduler(1);
			dbHelper = new DatabaseHelper();
			settings = new Settings("com.amplitude");
			if (userId != null)
			{
				SetUserId(userId);
			}
			else
			{
				this.userId = InitializeUserId();
			}
			deviceInfo = new DeviceInfo();
			deviceId = InitializeDeviceId();
			sessionId = -1L;
			userProperties = null;
			StartSession();
		}

		public void OnApplicationFocus(bool isFocused)
		{
			if (isFocused)
			{
				StartSession();
			}
			else
			{
				EndSession();
			}
		}

		public void OnApplicationQuit()
		{
			_isAppExiting = true;
			EndSession();
			if (settings != null)
			{
				settings.WriteToStorage();
			}
		}

		public void SetUserId(string userId)
		{
			this.userId = userId;
			settings.Save("userId", userId);
		}

		public void SetBuildVersion(string buildVersion)
		{
			_buildVersionString = buildVersion;
		}

		public string InitializeUserId()
		{
			return settings.Get<string>("userId");
		}

		public string InitializeDeviceId()
		{
			return SystemInfo.get_deviceUniqueIdentifier();
		}

		public string GetDeviceId(string deviceId)
		{
			return deviceId;
		}

		public void SetUserProperties(Dictionary<string, object> userProperties)
		{
			SetUserProperties(userProperties, replace: false);
		}

		public void SetUserProperties(Dictionary<string, object> userProperties, bool replace)
		{
			if (replace)
			{
				this.userProperties = userProperties;
			}
			else
			{
				this.userProperties = Merge(this.userProperties, userProperties);
			}
		}

		public void LogEvent(string eventType)
		{
			CheckedLogEvent(eventType, null, CurrentTimeMillis(), AnalyticsEventOptions.None);
		}

		public void LogEvent(string eventType, IDictionary<string, object> eventProperties)
		{
			CheckedLogEvent(eventType, eventProperties, CurrentTimeMillis(), AnalyticsEventOptions.None);
		}

		public void LogEvent(string eventType, IDictionary<string, object> eventProperties, AnalyticsEventOptions options)
		{
			CheckedLogEvent(eventType, eventProperties, CurrentTimeMillis(), options);
		}

		private void CheckedLogEvent(string eventType, IDictionary<string, object> eventProperties, long timestamp, AnalyticsEventOptions options)
		{
			if (!string.IsNullOrEmpty(eventType))
			{
				logQueue.QueueTask(delegate
				{
					LogEvent(eventType, eventProperties, timestamp, options);
				});
			}
		}

		private int LogEvent(string eventType, IDictionary<string, object> eventProperties, long timestamp, AnalyticsEventOptions options)
		{
			if (timestamp <= 0)
			{
				timestamp = CurrentTimeMillis();
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("user_id", userId);
			dictionary.Add("device_id", deviceId);
			dictionary.Add("event_type", eventType);
			dictionary.Add("session_id", ((options & AnalyticsEventOptions.MarkOutOfSession) == AnalyticsEventOptions.None) ? sessionId : (-1));
			dictionary.Add("insert_id", Hash(userId + deviceId + eventType + sessionId + timestamp));
			dictionary.Add("time", timestamp);
			dictionary.Add("event_properties", eventProperties);
			dictionary.Add("user_properties", userProperties);
			dictionary.Add("app_version", _buildVersionString);
			dictionary.Add("platform", deviceInfo.GetPlatform());
			dictionary.Add("os_name", deviceInfo.GetOsName());
			dictionary.Add("os_version", deviceInfo.GetOsVersion());
			dictionary.Add("device_model", deviceInfo.GetModel());
			dictionary.Add("device_name", deviceInfo.GetDeviceName());
			dictionary.Add("language", deviceInfo.GetLanguage());
			dictionary.Add("ip", "$remote");
			settings.Save("lastEventTime", timestamp);
			string evt = Json.Encode(dictionary);
			int result = dbHelper.AddEvent(evt);
			if (dbHelper.GetEventCount() >= 1000)
			{
				dbHelper.RemoveEvents(dbHelper.GetNthEventId(20));
			}
			if (dbHelper.GetEventCount() >= 30)
			{
				UpdateServer();
			}
			else
			{
				UpdateServerDelayed(30000);
			}
			return result;
		}

		private IEnumerable<Dictionary<string, object>> GetLastEvents(int maxEventId, int batchSize)
		{
			if (batchSize > 10)
			{
				batchSize = 10;
			}
			if (batchSize < 1)
			{
				batchSize = 1;
			}
			try
			{
				KeyValuePair<int, IEnumerable<Event>> events = dbHelper.GetEvents(maxEventId, batchSize);
				int key = events.Key;
				IEnumerable<Event> value = events.Value;
				return value.Select(delegate(Event e)
				{
					Dictionary<string, object> dictionary = Json.Decode(e.Text) as Dictionary<string, object>;
					if (dictionary == null)
					{
						throw new Exception("failed to decode json event object: " + e.Text);
					}
					dictionary.Add("event_id", e.Id);
					return dictionary;
				});
				IL_005e:;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return new List<Dictionary<string, object>>();
		}

		private void UpdateServer(int batchSize = 10)
		{
			if (Interlocked.Exchange(ref isUploading, 1) == 0)
			{
				IEnumerable<Dictionary<string, object>> lastEvents = GetLastEvents(-1, batchSize);
				int num = lastEvents.Count();
				if (num > 0)
				{
					httpQueue.QueueTask(delegate
					{
						PostEvents(lastEvents, delegate
						{
							logQueue.QueueTask(delegate
							{
								int maxId = (int)lastEvents.Last()["event_id"];
								dbHelper.RemoveEvents(maxId);
								isUploading = 0;
								if (dbHelper.GetEventCount() > 30)
								{
									logQueue.QueueTask(delegate
									{
										UpdateServer();
									});
								}
							});
						}, delegate(ErrorCode errorCode)
						{
							isUploading = 0;
							switch (errorCode)
							{
							case ErrorCode.TimedOut:
							case ErrorCode.ServerError:
								UpdateServerDelayed(1000);
								break;
							case ErrorCode.TooManyRequestsForDevice:
								UpdateServerDelayed(30000);
								break;
							case ErrorCode.TooManyEventsInRequest:
								UpdateServerDelayed(1000, Mathf.Max(batchSize / 2, 1));
								break;
							}
						});
					});
				}
				else
				{
					isUploading = 0;
				}
			}
		}

		private void UpdateServerDelayed(int delayMs, int batchSize = 10)
		{
			int num = -1;
			lock (_serverUpdateLock)
			{
				num = Mathf.Max(_serverUpdateDelayMs - (Environment.TickCount - _serverUpdateScheduledTime), 0);
				if (_serverUpdateDelayMs < 0 || delayMs < num)
				{
					_serverUpdateScheduledTime = Environment.TickCount;
					_serverUpdateDelayMs = delayMs;
				}
				if (_serverUpdateBatchSize <= 0 || batchSize < _serverUpdateBatchSize)
				{
					_serverUpdateBatchSize = batchSize;
				}
			}
			if (Interlocked.Exchange(ref isUpdateScheduled, 1) == 0)
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					AmplitudeWrapper amplitudeWrapper = this;
					int batch = 1;
					while (true)
					{
						bool flag = false;
						lock (_serverUpdateLock)
						{
							flag = (Mathf.Max(_serverUpdateDelayMs - (Environment.TickCount - _serverUpdateScheduledTime), 0) == 0);
							batch = _serverUpdateBatchSize;
						}
						if (flag || _isAppExiting)
						{
							break;
						}
						Thread.Sleep(100);
					}
					if (!_isAppExiting)
					{
						lock (_serverUpdateLock)
						{
							_serverUpdateScheduledTime = 0;
							_serverUpdateDelayMs = -1;
							_serverUpdateBatchSize = -1;
							isUpdateScheduled = 0;
						}
						logQueue.QueueTask(delegate
						{
							amplitudeWrapper.UpdateServer(batch);
						});
					}
				});
			}
		}

		private void PostEvents(IEnumerable<Dictionary<string, object>> events, Action onSuccess, Action<ErrorCode> onError)
		{
			int eventCount = events.Count();
			if (eventCount <= 0)
			{
				onSuccess();
			}
			else
			{
				string eventsJson = Json.Encode(events.ToList());
				if (eventsJson == null)
				{
					Debug.LogError((object)"AmplitudeAPI: PostEvents: failed to serialize events to JSON");
					onError(ErrorCode.GeneralError);
				}
				else if (string.IsNullOrEmpty(apiKey))
				{
					Debug.LogError((object)"AmplitudeAPI: PostEvents: apiKey is missing!");
					onError(ErrorCode.GeneralError);
				}
				else
				{
					UpdateDelegator.Dispatch(delegate
					{
						HTTPRequest hTTPRequest = new HTTPRequest(new Uri("https://api.amplitude.com/httpapi"), HTTPMethods.Post, delegate(HTTPRequest req, HTTPResponse resp)
						{
							switch (req.State)
							{
							case HTTPRequestStates.Finished:
								if (resp.IsSuccess)
								{
									Debug.Log((object)("AmplitudeAPI: upload finished, successfully posted " + eventCount + " events"));
									onSuccess();
								}
								else
								{
									Debug.LogError((object)("AmplitudeAPI: upload failed - status code " + resp.StatusCode + " " + resp.Message + ", response `" + resp.DataAsText + "`"));
									if (resp.StatusCode == 400)
									{
										onError(ErrorCode.BadRequest);
									}
									else if (resp.StatusCode == 413)
									{
										onError(ErrorCode.TooManyEventsInRequest);
									}
									else if (resp.StatusCode == 429)
									{
										onError(ErrorCode.TooManyRequestsForDevice);
									}
									else if (resp.StatusCode >= 500 && resp.StatusCode < 600)
									{
										onError(ErrorCode.ServerError);
									}
									else
									{
										onError(ErrorCode.GeneralError);
									}
								}
								break;
							case HTTPRequestStates.Error:
							{
								string str = (req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace);
								Debug.LogError((object)("AmplitudeAPI: upload failed with exception - " + str));
								onError(ErrorCode.Exception);
								break;
							}
							case HTTPRequestStates.Aborted:
								Debug.LogError((object)"AmplitudeAPI: upload failed - request aborted");
								onError(ErrorCode.GeneralError);
								break;
							case HTTPRequestStates.ConnectionTimedOut:
							case HTTPRequestStates.TimedOut:
								Debug.LogError((object)("AmplitudeAPI: upload failed - " + req.State));
								onError(ErrorCode.TimedOut);
								break;
							default:
								Debug.LogError((object)("AmplitudeAPI: upload failed - " + req.State));
								onError(ErrorCode.GeneralError);
								break;
							}
						});
						hTTPRequest.ConnectTimeout = TimeSpan.FromSeconds(20.0);
						hTTPRequest.Timeout = TimeSpan.FromSeconds(60.0);
						hTTPRequest.FormUsage = HTTPFormUsage.UrlEncoded;
						hTTPRequest.AddField("api_key", apiKey);
						hTTPRequest.AddField("event", eventsJson);
						hTTPRequest.Send();
					});
				}
			}
		}

		private void StartNewSession(long timestamp)
		{
			sessionOpen = true;
			sessionId = timestamp;
			settings.Save("previousSessionId", sessionId);
		}

		public void StartSession()
		{
			long timestamp = CurrentTimeMillis();
			logQueue.QueueTask(delegate
			{
				if (!sessionOpen)
				{
					long num = settings.Get<long>("lastEventTime");
					if (timestamp - num < 1800000)
					{
						long num2 = settings.Get<long>("previousSessionId");
						if (num2 <= 0)
						{
							StartNewSession(timestamp);
						}
						else
						{
							sessionId = num2;
						}
					}
					else
					{
						StartNewSession(timestamp);
					}
				}
				else
				{
					long num3 = settings.Get<long>("lastEventTime");
					if (timestamp - num3 > 1800000 || sessionId <= 0)
					{
						StartNewSession(timestamp);
					}
				}
				sessionOpen = true;
			});
		}

		private void EndSession()
		{
			long num = CurrentTimeMillis();
			logQueue.QueueTask(delegate
			{
				sessionOpen = false;
				UpdateServer();
			});
		}

		private long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		private int Hash(string data)
		{
			return data.GetHashCode();
		}

		private Dictionary<K, V> Merge<K, V>(Dictionary<K, V> a, Dictionary<K, V> b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return b.Concat(from kvp in a
			where !b.ContainsKey(kvp.Key)
			select kvp).ToDictionary((KeyValuePair<K, V> kvp) => kvp.Key, (KeyValuePair<K, V> kvp) => kvp.Value);
		}
	}
}
