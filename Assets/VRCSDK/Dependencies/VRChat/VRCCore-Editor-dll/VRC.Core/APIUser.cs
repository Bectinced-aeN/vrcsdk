using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core
{
	public class APIUser : ApiModel
	{
		public enum DeveloperType
		{
			None,
			Trusted,
			Internal
		}

		protected const string SECURE_PLAYER_PREFS_PW = "vl9u1grTnvXA";

		protected bool mIsFresh;

		protected string mBlob;

		protected string mDisplayName;

		protected string mUsername;

		protected string mPassword;

		protected string mAvatarId;

		protected DeveloperType mDeveloperType;

		protected List<VRCEvent> mEvents;

		protected string mLocation;

		public List<APIUser> friends;

		public string currentAvatarImageUrl;

		public string currentAvatarThumbnailImageUrl;

		protected static APIUser mCurrentUser;

		public bool isFresh
		{
			get
			{
				return mIsFresh;
			}
			set
			{
				mIsFresh = value;
			}
		}

		public string blob => mBlob;

		public string displayName => mDisplayName;

		public string username => mUsername;

		public string password => mPassword;

		public string avatarId => mAvatarId;

		public DeveloperType developerType => mDeveloperType;

		public List<VRCEvent> events => mEvents;

		public string location => mLocation;

		public bool hasScriptingAccess => mDeveloperType == DeveloperType.Trusted || mDeveloperType == DeveloperType.Internal;

		public static bool IsCached => SecurePlayerPrefs.HasKey("userId");

		public static bool IsLoggedIn => CurrentUser != null;

		public static bool IsLoggedInWithCredentials => IsLoggedIn && CurrentUser.mId != null;

		public static bool IsAnonymous => IsLoggedIn && CurrentUser.mId == null;

		public static APIUser CurrentUser => mCurrentUser;

		public static void Register(string username, string email, string password, Action<APIUser> successCallback = null, Action<string> errorCallback = null, bool cacheUser = true)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["username"] = username;
			dictionary["password"] = password;
			dictionary["email"] = email;
			ApiModel.SendPostRequest("auth/register", dictionary, delegate(Dictionary<string, object> obj)
			{
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.Init(obj);
				aPIUser.mIsFresh = true;
				aPIUser.mPassword = password;
				mCurrentUser = aPIUser;
				if (cacheUser)
				{
					CacheUser(aPIUser);
				}
				if (successCallback != null)
				{
					successCallback(aPIUser);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void Login(string usernameOrEmail, string password, Action<APIUser> successCallback = null, Action<string> errorCallback = null, bool cacheUser = true)
		{
			Logger.Log("Logging in with " + usernameOrEmail + " and password: " + password, DebugLevel.All);
			FetchLocal(usernameOrEmail, password, delegate(APIUser user)
			{
				user.mPassword = password;
				mCurrentUser = user;
				if (cacheUser)
				{
					CacheUser(user);
				}
				if (successCallback != null)
				{
					successCallback(user);
				}
			}, errorCallback);
		}

		public static APIUser CachedLogin(Action<APIUser> onFetchSuccess = null, Action<string> onFetchError = null, bool shouldFetch = true)
		{
			Logger.Log("Cached Login", DebugLevel.All);
			APIUser user = GetCachedUser();
			mCurrentUser = user;
			if (user != null && shouldFetch)
			{
				user.FetchCachedLocal(delegate
				{
					if (onFetchSuccess != null)
					{
						mCurrentUser = user;
						onFetchSuccess(user);
					}
				}, delegate(string obj)
				{
					if (onFetchError != null)
					{
						onFetchError(obj);
					}
				});
			}
			return user;
		}

		public static void AnonymousLogin()
		{
			mCurrentUser = ScriptableObject.CreateInstance<APIUser>();
			mCurrentUser.Init();
		}

		public static void Logout()
		{
			mCurrentUser = null;
			ClearCachedUser();
		}

		private static void FetchLocal(string username, string password, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			ApiModel.SendRequest(username, password, "auth/user", HTTPMethods.Get, null, null, delegate(Dictionary<string, object> obj)
			{
				Logger.Log("Authenticated", DebugLevel.All);
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.Init(obj);
				aPIUser.mIsFresh = true;
				aPIUser.mPassword = password;
				CacheUser(aPIUser);
				if (successCallback != null)
				{
					successCallback(aPIUser);
				}
			}, null, delegate(string message)
			{
				Logger.Log("NOT Authenticated", DebugLevel.All);
				if (errorCallback != null)
				{
					errorCallback(message);
				}
			});
		}

		public void FetchCachedLocal(Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			FetchLocal(username, password, delegate(APIUser user)
			{
				Fill(user);
				if (successCallback != null)
				{
					successCallback(user);
				}
			}, errorCallback);
		}

		public static void Fetch(string id, Action<APIUser> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("users/" + id, delegate(Dictionary<string, object> obj)
			{
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.InitBrief(obj);
				if (successCallback != null)
				{
					successCallback(aPIUser);
				}
			}, delegate(string message)
			{
				errorCallback(message);
			});
		}

		public static void FetchUsersInWorldInstance(string worldId, string instanceId, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("worlds/" + worldId + "/" + instanceId, delegate(Dictionary<string, object> jsonDict)
			{
				List<object> list = jsonDict["users"] as List<object>;
				Debug.Log((object)("jsonObjects: " + list));
				List<APIUser> list2 = new List<APIUser>();
				if (list != null)
				{
					foreach (object item in list)
					{
						Dictionary<string, object> jsonObject = item as Dictionary<string, object>;
						APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
						aPIUser.InitBrief(jsonObject);
						list2.Add(aPIUser);
					}
				}
				if (successCallback != null)
				{
					successCallback(list2);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not fetch users with error - " + message);
				errorCallback(message);
			});
		}

		public static void FetchUsers(string searchQuery, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("users?search=" + searchQuery, delegate(List<object> objects)
			{
				List<APIUser> list = new List<APIUser>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
						aPIUser.InitBrief(jsonObject);
						list.Add(aPIUser);
					}
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not fetch users with error - " + message);
				errorCallback(message);
			});
		}

		public static void FetchFriends(Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("auth/user/friends", delegate(List<object> objects)
			{
				List<APIUser> list = new List<APIUser>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
						APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
						aPIUser.InitBrief(jsonObject);
						list.Add(aPIUser);
					}
				}
				if (mCurrentUser != null)
				{
					mCurrentUser.friends = list;
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not fetch friends with error - " + message);
				errorCallback(message);
			});
		}

		public static void SendFriendRequest(string userId, Action<ApiNotification> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPostRequest("user/" + userId + "/friendRequest", null, delegate(Dictionary<string, object> obj)
			{
				ApiNotification apiNotification = ScriptableObject.CreateInstance<ApiNotification>();
				apiNotification.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiNotification);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void AcceptFriendRequest(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPutRequest("/auth/user/notifications/" + notificationId + "/accept", null, delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void DeclineFriendRequest(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPutRequest("/auth/user/notifications/" + notificationId + "/hide", null, delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void UnfriendUser(string userId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("/auth/user/friends/" + userId, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, (Action<string>)delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static bool IsFriendsWith(string userId)
		{
			bool result = false;
			if (CurrentUser.friends != null)
			{
				result = (CurrentUser.friends.Find((APIUser u) => u.id == userId) != null);
			}
			return result;
		}

		private static void CacheUser(APIUser user)
		{
			SecurePlayerPrefs.SetString("username", user.username, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("password", user.password, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("userBlob", user.mBlob, "vl9u1grTnvXA");
			SecurePlayerPrefs.SetString("userId", user.id, "vl9u1grTnvXA");
			PlayerPrefs.Save();
		}

		public void CacheUser()
		{
			CacheUser(this);
		}

		private static APIUser GetCachedUser()
		{
			string @string = SecurePlayerPrefs.GetString("username", "vl9u1grTnvXA");
			string string2 = SecurePlayerPrefs.GetString("password", "vl9u1grTnvXA");
			string string3 = SecurePlayerPrefs.GetString("userBlob", "vl9u1grTnvXA");
			APIUser aPIUser = null;
			try
			{
				Dictionary<string, object> jsonObject = Json.Decode(string3) as Dictionary<string, object>;
				aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.Init(jsonObject);
				aPIUser.mPassword = string2;
				aPIUser.isFresh = false;
				return aPIUser;
			}
			catch (Exception)
			{
				Debug.Log((object)("GetCachedUser Exception due to corrupted player pref user data. Wiping cached user info. User " + @string + " needs to relogin"));
				ClearCachedUser();
				return null;
			}
		}

		public static void ClearCachedUser()
		{
			SecurePlayerPrefs.DeleteKey("username");
			SecurePlayerPrefs.DeleteKey("password");
			SecurePlayerPrefs.DeleteKey("userBlob");
			SecurePlayerPrefs.DeleteKey("userId");
			PlayerPrefs.Save();
		}

		public static bool Exists(APIUser user)
		{
			return user != null && !string.IsNullOrEmpty(user.id);
		}

		public void Init(Dictionary<string, object> jsonObject)
		{
			Fill(jsonObject);
		}

		public void InitBrief(Dictionary<string, object> jsonObject)
		{
			Init();
			mId = (jsonObject["id"] as string);
			mUsername = (jsonObject["username"] as string);
			mDisplayName = (jsonObject["displayName"] as string);
			if (jsonObject.ContainsKey("location"))
			{
				mLocation = (jsonObject["location"] as string);
			}
			if (jsonObject.ContainsKey("currentAvatarThumbnailImageUrl"))
			{
				currentAvatarThumbnailImageUrl = (jsonObject["currentAvatarThumbnailImageUrl"] as string);
			}
			if (jsonObject.ContainsKey("currentAvatarImageUrl"))
			{
				currentAvatarImageUrl = (jsonObject["currentAvatarImageUrl"] as string);
			}
		}

		public void Init(ApiNotification notification)
		{
			Init();
			mId = notification.senderUserId;
			mDisplayName = notification.senderUsername;
		}

		protected void Init()
		{
			mId = null;
			mBlob = null;
			mUsername = "guest";
			mDisplayName = "guest";
			mAvatarId = null;
			mEvents = null;
		}

		protected virtual void Fill(APIUser fromUser)
		{
			mId = fromUser.id;
			mBlob = fromUser.blob;
			mUsername = fromUser.username;
			mPassword = fromUser.mPassword;
			mDisplayName = fromUser.displayName;
			mAvatarId = fromUser.mAvatarId;
			mEvents = fromUser.events;
			mIsFresh = fromUser.isFresh;
			mDeveloperType = fromUser.developerType;
		}

		protected virtual void Fill(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			mBlob = Json.Encode(jsonObject);
			mUsername = (jsonObject["username"] as string);
			mDisplayName = (jsonObject["displayName"] as string);
			mAvatarId = (jsonObject["currentAvatar"] as string);
			mEvents = VRCEvent.VRCEvents(jsonObject["events"] as Dictionary<string, object>);
			string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["developerType"] as string).ToLower());
			mDeveloperType = (DeveloperType)(int)Enum.Parse(typeof(DeveloperType), value);
		}

		public void Save(Action<ApiModel> onSuccess = null, Action<string> onError = null)
		{
			if (IsLoggedInWithCredentials)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["userId"] = CurrentUser.id;
				dictionary["username"] = username;
				dictionary["displayName"] = displayName;
				dictionary["currentAvatarBlueprintId"] = mAvatarId;
				dictionary["eventIds"] = string.Join(",", ApiModel.GetIds(mEvents.Cast<ApiModel>()).ToArray());
				ApiModel.SendPutRequest("user/" + mId, dictionary, delegate
				{
					CacheUser(mCurrentUser);
					if (onSuccess != null)
					{
						onSuccess(mCurrentUser);
					}
				}, onError);
			}
			else
			{
				Logger.Log("Cannot save anonyomus user!");
			}
		}

		public virtual void SetCurrentAvatar(ApiAvatar avatar)
		{
			mAvatarId = avatar.id;
		}

		public virtual void SetDisplayName(string name)
		{
			mDisplayName = name;
			Save();
		}

		public override string ToString()
		{
			return $"[id: {base.id}; username: {username}; displayName: {displayName}; avatarId: {mAvatarId}; events: {events}]";
		}
	}
}
