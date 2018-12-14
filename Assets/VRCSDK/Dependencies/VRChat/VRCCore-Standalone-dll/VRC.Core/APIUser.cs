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

		protected string mBlob;

		protected string mDisplayName;

		protected string mUsername;

		protected string mPassword;

		protected string mAvatarId;

		protected bool mVerified;

		protected bool mHasEmail;

		protected bool mHasBirthday;

		protected int mAcceptedTOSVersion;

		protected DeveloperType? mDeveloperType;

		protected List<VRCEvent> mEvents;

		protected string mLocation;

		public List<APIUser> friends;

		public string currentAvatarImageUrl;

		public string currentAvatarThumbnailImageUrl;

		private static Dictionary<string, APIUser> cachedUsers;

		protected static APIUser mCurrentUser;

		public string blob => mBlob;

		public string displayName => mDisplayName;

		public string username => mUsername;

		public string password => mPassword;

		public string avatarId => mAvatarId;

		public bool verified => mVerified;

		public bool hasEmail => mHasEmail;

		public bool hasBirthday => mHasBirthday;

		public int acceptedTOSVersion => mAcceptedTOSVersion;

		public DeveloperType? developerType => mDeveloperType;

		public List<VRCEvent> events => mEvents;

		public string location => mLocation;

		public bool defaultMute
		{
			get;
			set;
		}

		public bool hasScriptingAccess => mDeveloperType == DeveloperType.Trusted || mDeveloperType == DeveloperType.Internal;

		public static bool IsLoggedIn => CurrentUser != null;

		public static bool IsLoggedInWithCredentials => IsLoggedIn && CurrentUser.mId != null;

		public static APIUser CurrentUser => mCurrentUser;

		public static void Register(string username, string email, string password, string birthday, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["username"] = username;
			dictionary["password"] = password;
			dictionary["email"] = email;
			dictionary["birthday"] = birthday;
			dictionary["acceptedTOSVersion"] = "0";
			ApiModel.SendPostRequest("auth/register", dictionary, delegate(Dictionary<string, object> obj)
			{
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.Init(obj);
				if (!aPIUser.developerType.HasValue)
				{
					Debug.LogError((object)"auth/register did not provide a developerType");
				}
				mCurrentUser = aPIUser;
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

		public static void UpdateAccountInfo(string id, string email, string birthday, string acceptedTOSVersion, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			if (IsLoggedInWithCredentials)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(email))
				{
					dictionary["email"] = email;
				}
				if (!string.IsNullOrEmpty(birthday))
				{
					dictionary["birthday"] = birthday;
				}
				if (!string.IsNullOrEmpty(acceptedTOSVersion))
				{
					int result = -1;
					if (int.TryParse(acceptedTOSVersion, out result))
					{
						dictionary["acceptedTOSVersion"] = result.ToString();
					}
					else
					{
						Debug.LogError((object)("UpdateAccountInfo: invalid acceptedTOSVersion string: " + acceptedTOSVersion));
					}
				}
				ApiModel.SendPutRequest("users/" + id, dictionary, delegate(Dictionary<string, object> obj)
				{
					APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
					aPIUser.Init(obj);
					if (!aPIUser.developerType.HasValue)
					{
						Debug.LogError((object)"user/:id did not provide a developerType");
					}
					mCurrentUser = aPIUser;
					if (successCallback != null)
					{
						successCallback(mCurrentUser);
					}
				}, errorCallback);
			}
			else
			{
				Logger.Log("Cannot update anonyomus user account info!");
			}
		}

		public static void Login(Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			Logger.Log("Logging in", DebugLevel.All);
			ApiModel.SendGetRequest("auth/user", delegate(Dictionary<string, object> obj)
			{
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				aPIUser.Init(obj);
				if (!aPIUser.developerType.HasValue)
				{
					Debug.LogError((object)"auth/user did not provide a developerType");
				}
				Logger.Log("Authenticated: " + aPIUser.displayName, DebugLevel.All);
				mCurrentUser = aPIUser;
				if (successCallback != null)
				{
					successCallback(aPIUser);
				}
			}, delegate(string message)
			{
				Logger.Log("NOT Authenticated", DebugLevel.All);
				if (errorCallback != null)
				{
					errorCallback(message);
				}
			});
		}

		public static void ThirdPartyLogin(string endpoint, Dictionary<string, string> parameters, Action<string, APIUser> onFetchSuccess = null, Action<string> onFetchError = null)
		{
			Logger.Log("Third Party Login: " + endpoint, DebugLevel.All);
			ApiModel.SendRequest("auth/" + endpoint, HTTPMethods.Post, parameters, null, delegate(Dictionary<string, object> obj)
			{
				Logger.Log("Authenticated", DebugLevel.All);
				APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
				if (obj.ContainsKey("emailVerified"))
				{
					obj["emailVerified"] = "true";
				}
				else
				{
					obj.Add("emailVerified", "true");
				}
				aPIUser.Init(obj);
				mCurrentUser = aPIUser;
				if (onFetchSuccess != null)
				{
					onFetchSuccess(obj["authToken"].ToString(), aPIUser);
				}
			}, null, delegate(string message)
			{
				Logger.Log("NOT Authenticated", DebugLevel.All);
				if (onFetchError != null)
				{
					onFetchError(message);
				}
			});
		}

		public static void Logout()
		{
			mCurrentUser = null;
		}

		public static void Fetch(string id, Action<APIUser> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("users/" + id, delegate(Dictionary<string, object> obj)
			{
				try
				{
					APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
					aPIUser.InitBrief(obj);
					if (!aPIUser.developerType.HasValue)
					{
						Debug.LogError((object)("users/" + id + " did not provide a developerType"));
					}
					if (successCallback != null)
					{
						successCallback(aPIUser);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)"users/ provided a malformed or incomplete user record");
					Debug.LogException(ex);
				}
			}, delegate(string message)
			{
				errorCallback(message);
			});
		}

		public static void CacheUser(string id, APIUser user)
		{
			if (cachedUsers == null)
			{
				cachedUsers = new Dictionary<string, APIUser>();
			}
			if (user.developerType.HasValue)
			{
				cachedUsers[id] = user;
			}
		}

		public static APIUser GetCachedUser(string id)
		{
			APIUser result = null;
			if (cachedUsers != null && cachedUsers.ContainsKey(id))
			{
				result = cachedUsers[id];
			}
			return result;
		}

		public static void FetchUsersInWorldInstance(string worldId, string instanceId, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("worlds/" + worldId + "/" + instanceId, delegate(Dictionary<string, object> jsonDict)
			{
				List<object> list = jsonDict["users"] as List<object>;
				List<APIUser> list2 = new List<APIUser>();
				if (list != null)
				{
					Debug.LogFormat("Discovered {0} users in world {1}", new object[2]
					{
						list.Count,
						instanceId
					});
					foreach (object item in list)
					{
						if (item != null)
						{
							try
							{
								Dictionary<string, object> jsonObject = item as Dictionary<string, object>;
								APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
								aPIUser.InitBrief(jsonObject);
								if (!aPIUser.developerType.HasValue)
								{
									Debug.LogError((object)("worlds/" + worldId + "/" + instanceId + " did not provide a developerType"));
								}
								list2.Add(aPIUser);
							}
							catch (Exception ex)
							{
								Debug.LogError((object)"worlds/ provided a malformed or incomplete user record");
								Debug.LogException(ex);
							}
						}
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
						if (@object != null)
						{
							try
							{
								Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
								APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
								aPIUser.InitBrief(jsonObject);
								if (!aPIUser.developerType.HasValue)
								{
									Debug.LogError((object)("users?searc=" + searchQuery + " did not provide a developerType"));
								}
								list.Add(aPIUser);
							}
							catch (Exception ex)
							{
								Debug.LogError((object)"users?search provided a malformed or incomplete user record");
								Debug.LogException(ex);
							}
						}
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

		public static void FetchFriends(Action<List<APIUser>> successCallback = null, Action<string> errorCallback = null)
		{
			ApiModel.SendGetRequest("auth/user/friends", delegate(List<object> objects)
			{
				List<APIUser> list = new List<APIUser>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						if (@object != null)
						{
							try
							{
								Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
								APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
								aPIUser.InitBrief(jsonObject);
								if (!aPIUser.developerType.HasValue)
								{
									Debug.LogError((object)"auth/user/friends did not provide a developerType");
								}
								list.Add(aPIUser);
							}
							catch (Exception ex)
							{
								Debug.LogError((object)"auth/user/friends provided a malformed or incomplete user record");
								Debug.LogException(ex);
							}
						}
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
			ApiModel.SendPostRequest("user/" + userId + "/friendRequest", delegate(Dictionary<string, object> obj)
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

		public static void AttemptVerification()
		{
			ApiModel.SendPostRequest("auth/user/resendEmail", (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)null, (Action<string>)null);
		}

		public static void AcceptFriendRequest(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendPutRequest("/auth/user/notifications/" + notificationId + "/accept", delegate
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
			ApiModel.SendPutRequest("/auth/user/notifications/" + notificationId + "/hide", delegate
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
			if (mCurrentUser != null && mCurrentUser.friends != null)
			{
				mCurrentUser.friends.Remove(mCurrentUser.friends.Single((APIUser u) => u.id == userId));
			}
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

		public static void LocalAddFriend(APIUser user)
		{
			if (CurrentUser != null && user != null)
			{
				if (CurrentUser.friends == null)
				{
					CurrentUser.friends = new List<APIUser>();
				}
				if (!CurrentUser.friends.Exists((APIUser u) => u.id == user.id))
				{
					CurrentUser.friends.Add(user);
				}
			}
		}

		public static void LocalRemoveFriend(APIUser user)
		{
			if (CurrentUser != null && user != null && CurrentUser.friends != null)
			{
				CurrentUser.friends.RemoveAll((APIUser u) => u.id == user.id);
			}
		}

		public static bool IsFriendsWith(string userId)
		{
			bool result = false;
			if (CurrentUser != null && CurrentUser.friends != null)
			{
				result = (CurrentUser.friends.Find((APIUser u) => u.id == userId) != null);
			}
			return result;
		}

		public static void FetchOnlineModerators(bool onCallOnly, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["developerType"] = "internal";
			ApiModel.SendRequest("users/active", HTTPMethods.Get, dictionary, delegate(List<object> objects)
			{
				List<APIUser> list = new List<APIUser>();
				if (objects != null)
				{
					foreach (object @object in objects)
					{
						try
						{
							Dictionary<string, object> jsonObject = @object as Dictionary<string, object>;
							APIUser aPIUser = ScriptableObject.CreateInstance<APIUser>();
							aPIUser.InitBrief(jsonObject);
							if (!aPIUser.developerType.HasValue)
							{
								Debug.LogError((object)"users/active did not provide a developerType");
							}
							list.Add(aPIUser);
						}
						catch (Exception ex)
						{
							Debug.LogError((object)"users/active provided a malformed or incomplete user record");
							Debug.LogException(ex);
						}
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

		public static void PostHelpRequest(string fromWorldId, string fromInstanceId, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["worldId"] = fromWorldId;
			dictionary["instanceId"] = fromInstanceId;
			ApiModel.SendPostRequest("halp", dictionary, delegate
			{
				if (successCallback != null)
				{
					successCallback(null);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not post Halp request - " + message);
				errorCallback(message);
			});
		}

		public static bool Exists(APIUser user)
		{
			return user != null && !string.IsNullOrEmpty(user.id);
		}

		public void Init(Dictionary<string, object> jsonObject)
		{
			Init();
			Fill(jsonObject);
			if (mDeveloperType.HasValue)
			{
				CacheUser(mId, this);
			}
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
			if (jsonObject.ContainsKey("developerType"))
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["developerType"] as string).ToLower());
				mDeveloperType = (DeveloperType)(int)Enum.Parse(typeof(DeveloperType), value);
				CacheUser(mId, this);
			}
		}

		public void Init(ApiNotification notification)
		{
			Init();
			mId = notification.senderUserId;
			mDisplayName = notification.senderUsername;
		}

		public void Init()
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
			mVerified = fromUser.verified;
			mHasEmail = fromUser.hasEmail;
			mHasBirthday = fromUser.hasBirthday;
			mAcceptedTOSVersion = fromUser.mAcceptedTOSVersion;
			mDeveloperType = fromUser.developerType;
		}

		protected virtual void Fill(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			mBlob = Json.Encode(jsonObject);
			mUsername = (jsonObject["username"] as string);
			mDisplayName = (jsonObject["displayName"] as string);
			mVerified = Convert.ToBoolean(jsonObject["emailVerified"]);
			if (jsonObject.ContainsKey("hasEmail"))
			{
				mHasEmail = Convert.ToBoolean(jsonObject["hasEmail"]);
			}
			if (jsonObject.ContainsKey("hasBirthday"))
			{
				mHasBirthday = Convert.ToBoolean(jsonObject["hasBirthday"]);
			}
			if (jsonObject.ContainsKey("acceptedTOSVersion"))
			{
				mAcceptedTOSVersion = Convert.ToInt32(jsonObject["acceptedTOSVersion"]);
			}
			if (jsonObject.ContainsKey("currentAvatar"))
			{
				mAvatarId = (jsonObject["currentAvatar"] as string);
			}
			if (jsonObject.ContainsKey("events"))
			{
				mEvents = VRCEvent.VRCEvents(jsonObject["events"] as Dictionary<string, object>);
			}
			if (jsonObject.ContainsKey("developerType"))
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((jsonObject["developerType"] as string).ToLower());
				mDeveloperType = (DeveloperType)(int)Enum.Parse(typeof(DeveloperType), value);
			}
		}

		public virtual void SetCurrentAvatar(ApiAvatar avatar)
		{
			mAvatarId = avatar.id;
		}

		public virtual void SetDisplayName(string name)
		{
			mDisplayName = name;
		}

		public override string ToString()
		{
			return $"[id: {base.id}; username: {username}; displayName: {displayName}; avatarId: {mAvatarId}; events: {events}]";
		}
	}
}
