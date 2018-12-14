using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class APIUser : ApiModel
	{
		public enum FriendLocation
		{
			Online,
			Offline
		}

		public enum DeveloperType
		{
			None,
			Trusted,
			Internal,
			Moderator
		}

		public const float FriendsListCacheTime = 60f;

		public const float SingleRecordCacheTime = 60f;

		public const float SearchCacheTime = 60f;

		[ApiField(Required = false)]
		public string blob
		{
			get;
			protected set;
		}

		[ApiField]
		public string displayName
		{
			get;
			set;
		}

		[ApiField]
		public string username
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string location
		{
			get;
			protected set;
		}

		[ApiField(Required = false, Name = "currentAvatar")]
		public string avatarId
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool hasEmail
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool hasBirthday
		{
			get;
			protected set;
		}

		[ApiField]
		[DefaultValue(DeveloperType.None)]
		public DeveloperType developerType
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public List<VRCEvent> events
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public int acceptedTOSVersion
		{
			get;
			private set;
		}

		[ApiField(Required = false)]
		public string currentAvatarImageUrl
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string currentAvatarThumbnailImageUrl
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string authToken
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool emailVerified
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool defaultMute
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string birthday
		{
			get;
			protected set;
		}

		[ApiField(Required = false, Name = "friends")]
		public List<string> friendIDs
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public Dictionary<string, object> blueprints
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public Dictionary<string, object> currentAvatarBlueprint
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string currentAvatarAssetUrl
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public Dictionary<string, object> steamDetails
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string worldId
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string instanceId
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string obfuscatedEmail
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool unsubscribe
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public bool hasLoggedInFromClient
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double nuisanceFactor
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double socialLevel
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double creatorLevel
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double timeEquity
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double level
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public List<string> pastDisplayNames
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public double actorNumber
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string networkSessionId
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string homeLocation
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public List<string> tags
		{
			get;
			protected set;
		}

		public bool isAccountVerified => true;

		public bool hasNoPowers => !isAccountVerified || developerType == DeveloperType.None;

		public bool hasScriptingAccess => isAccountVerified && (developerType == DeveloperType.Trusted || developerType == DeveloperType.Internal || HasTag("admin_scripting_access") || HasTag("system_scripting_access"));

		public bool hasModerationPowers => isAccountVerified && (developerType == DeveloperType.Moderator || developerType == DeveloperType.Internal);

		public bool hasVIPAccess => isAccountVerified && developerType == DeveloperType.Internal;

		public bool hasSuperPowers => isAccountVerified && developerType == DeveloperType.Internal;

		public bool canPublishAllContent => canPublishWorlds && canPublishAvatars;

		public bool canPublishAvatars => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_avatar_access") || RemoteConfig.GetBool("disableAvatarGating"));

		public bool canPublishWorlds => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_world_access") || RemoteConfig.GetBool("disableAvatarGating"));

		public static bool IsLoggedIn => CurrentUser != null;

		public static bool IsLoggedInWithCredentials => IsLoggedIn && CurrentUser.id != null;

		public static APIUser CurrentUser
		{
			get;
			protected set;
		}

		public APIUser()
			: base("users")
		{
		}

		public override bool ShouldCache()
		{
			return base.ShouldCache() && !string.IsNullOrEmpty(location);
		}

		public override float GetLifeSpan()
		{
			return 60f;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "events":
				return false;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "events":
				events = VRCEvent.MakeEvents(data as Dictionary<string, object>);
				return true;
			default:
				return base.WriteField(fieldName, data);
			}
		}

		public static void FetchCurrentUser(Action<APIUser> onSuccess, Action<string> onError)
		{
			if (!IsLoggedIn)
			{
				onError?.Invoke("Not logged in");
			}
			else
			{
				Login(onSuccess, onError);
			}
		}

		public static void Register(string username, string email, string password, string birthday, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("username", username);
			dictionary.Add("password", password);
			dictionary.Add("email", email);
			dictionary.Add("birthday", birthday);
			dictionary.Add("acceptedTOSVersion", "0");
			Dictionary<string, object> requestParams = dictionary;
			API.SendRequest("auth/register", HTTPMethods.Post, new ApiModelContainer<APIUser>
			{
				OnSuccess = delegate(ApiContainer c)
				{
					if (successCallback != null)
					{
						successCallback(c.Model as APIUser);
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			}, requestParams);
		}

		public void UpdateAccountInfo(string email, string birthday, string acceptedTOSVersion, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
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
			API.SendPutRequest("users/" + base.id, new ApiModelContainer<APIUser>(this)
			{
				OnSuccess = delegate(ApiContainer c)
				{
					if (successCallback != null)
					{
						successCallback(c.Model as APIUser);
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			}, dictionary);
		}

		public void SetSessionId(string sessionId, Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			if (IsLoggedInWithCredentials)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("networkSessionId", sessionId);
				Dictionary<string, object> requestParams = dictionary;
				API.SendPutRequest("users/" + base.id, new ApiModelContainer<APIUser>(this)
				{
					OnSuccess = delegate(ApiContainer c)
					{
						if (successCallback != null)
						{
							successCallback(c.Model as APIUser);
						}
					},
					OnError = delegate(ApiContainer c)
					{
						if (errorCallback != null)
						{
							errorCallback(c.Error);
						}
					}
				}, requestParams);
			}
		}

		public static void Login(Action<APIUser> successCallback = null, Action<string> errorCallback = null)
		{
			Logger.Log("Logging in", DebugLevel.All);
			ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				CurrentUser = (c.Model as APIUser);
				if (successCallback != null)
				{
					successCallback(CurrentUser);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				Logger.LogError("NOT Authenticated: " + c.Error, DebugLevel.All);
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<APIUser> responseContainer = apiModelContainer;
			API.SendGetRequest("auth/user", responseContainer, null, disableCache: true);
		}

		public static void ThirdPartyLogin(string endpoint, Dictionary<string, object> parameters, Action<string, APIUser> onFetchSuccess = null, Action<string> onFetchError = null)
		{
			Logger.Log("Third Party Login: " + endpoint, DebugLevel.All);
			ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				CurrentUser = (c.Model as APIUser);
				if (onFetchSuccess != null)
				{
					onFetchSuccess(CurrentUser.authToken, CurrentUser);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				Logger.Log("NOT Authenticated", DebugLevel.All);
				if (onFetchError != null)
				{
					onFetchError(c.Error);
				}
			};
			ApiModelContainer<APIUser> responseContainer = apiModelContainer;
			API.SendPostRequest("auth/" + endpoint, responseContainer, parameters);
		}

		public static void Logout()
		{
			CurrentUser = null;
		}

		public static void FetchUsersInWorldInstance(string worldId, string instanceId, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiWorld w = API.FromCacheOrNew<ApiWorld>(worldId);
			w.Fetch(instanceId, compatibleVersionsOnly: false, delegate
			{
				if (successCallback != null)
				{
					ApiWorldInstance apiWorldInstance = w.worldInstances.FirstOrDefault((ApiWorldInstance world) => world != null && world.idWithTags == instanceId);
					if (apiWorldInstance != null)
					{
						successCallback(apiWorldInstance.users);
					}
					else if (errorCallback != null)
					{
						errorCallback("Could not locate appropriate instance.");
					}
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void FetchUsers(string searchQuery, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			ApiModelListContainer<APIUser> apiModelListContainer = new ApiModelListContainer<APIUser>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<APIUser>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<APIUser> responseContainer = apiModelListContainer;
			API.SendGetRequest("users?search=" + searchQuery, responseContainer, null, disableCache: false, 60f);
		}

		public static void FetchFriends(FriendLocation location, int offset = 0, int count = 100, Action<List<APIUser>> successCallback = null, Action<string> errorCallback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("offset", offset);
			dictionary.Add("n", count);
			Dictionary<string, object> requestParams = dictionary;
			List<APIUser> list = new List<APIUser>();
			foreach (string friendID in CurrentUser.friendIDs)
			{
				APIUser target = null;
				if (!ApiCache.Fetch(friendID, ref target))
				{
					break;
				}
				list.Add(target);
			}
			if (list.Count == CurrentUser.friendIDs.Count)
			{
				if (successCallback != null)
				{
					successCallback(list);
				}
			}
			else
			{
				ApiModelListContainer<APIUser> apiModelListContainer = new ApiModelListContainer<APIUser>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					if (successCallback != null)
					{
						successCallback((c as ApiModelListContainer<APIUser>).ResponseModels);
					}
				};
				apiModelListContainer.OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiModelListContainer<APIUser> responseContainer = apiModelListContainer;
				API.SendGetRequest("auth/user/friends" + ((location != FriendLocation.Offline) ? string.Empty : "?offline=true"), responseContainer, requestParams, disableCache: false, 60f);
			}
		}

		public static void SendFriendRequest(string userId, Action<ApiNotification> successCallback, Action<string> errorCallback)
		{
			ApiModelContainer<ApiNotification> apiModelContainer = new ApiModelContainer<ApiNotification>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiNotification);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<ApiNotification> responseContainer = apiModelContainer;
			API.SendPostRequest("user/" + userId + "/friendRequest", responseContainer);
		}

		public static void AttemptVerification()
		{
			API.SendPostRequest("auth/user/resendEmail", new ApiContainer());
		}

		public static void AcceptFriendRequest(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			API.SendPutRequest("/auth/user/notifications/" + notificationId + "/accept", new ApiContainer
			{
				OnSuccess = delegate
				{
					if (successCallback != null)
					{
						successCallback();
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			});
		}

		public static void DeclineFriendRequest(string notificationId, Action successCallback, Action<string> errorCallback)
		{
			API.SendPutRequest("/auth/user/notifications/" + notificationId + "/hide", new ApiContainer
			{
				OnSuccess = delegate
				{
					if (successCallback != null)
					{
						successCallback();
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			});
		}

		public static void UnfriendUser(string userId, Action successCallback, Action<string> errorCallback)
		{
			if (CurrentUser != null && CurrentUser.friendIDs != null)
			{
				CurrentUser.friendIDs.RemoveAll((string id) => id == userId);
			}
			API.SendDeleteRequest("/auth/user/friends/" + userId, new ApiContainer
			{
				OnSuccess = delegate
				{
					if (successCallback != null)
					{
						successCallback();
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			});
		}

		public static void LocalAddFriend(APIUser user)
		{
			if (CurrentUser != null && user != null && !CurrentUser.friendIDs.Exists((string id) => id == user.id))
			{
				CurrentUser.friendIDs.Add(user.id);
			}
		}

		public static void LocalRemoveFriend(APIUser user)
		{
			if (CurrentUser != null && CurrentUser.friendIDs != null)
			{
				CurrentUser.friendIDs.RemoveAll((string id) => id == user.id);
			}
		}

		public static bool IsFriendsWith(string userId)
		{
			if (CurrentUser != null && CurrentUser.friendIDs != null)
			{
				return CurrentUser.friendIDs.Any((string u) => u == userId);
			}
			return false;
		}

		public static void FetchOnlineModerators(bool onCallOnly, Action<List<APIUser>> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("developerType", "internal");
			Dictionary<string, object> requestParams = dictionary;
			ApiModelListContainer<APIUser> apiModelListContainer = new ApiModelListContainer<APIUser>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<APIUser>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Logger.Log("Could not fetch users with error - " + c.Error);
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<APIUser> responseContainer = apiModelListContainer;
			API.SendGetRequest("users/active", responseContainer, requestParams, disableCache: true);
		}

		public static void PostHelpRequest(string fromWorldId, string fromInstanceId, Action successCallback, Action<string> errorCallback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["worldId"] = fromWorldId;
			dictionary["instanceId"] = fromInstanceId;
			API.SendPostRequest("halp", new ApiContainer
			{
				OnSuccess = delegate
				{
					if (successCallback != null)
					{
						successCallback();
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				}
			});
		}

		public static bool Exists(APIUser user)
		{
			return user != null && !string.IsNullOrEmpty(user.id);
		}

		public bool HasTag(string tag)
		{
			if (string.IsNullOrEmpty(tag) || tags == null)
			{
				return false;
			}
			return tags.Contains(tag);
		}

		public override string ToString()
		{
			return $"[id: {base.id}; username: {username}; displayName: {displayName}; avatarId: {avatarId}; events: {events}]";
		}
	}
}
