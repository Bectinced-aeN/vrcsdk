using System;
using System.Collections;
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

		public enum UserStatus
		{
			Active,
			JoinMe,
			Busy,
			Offline
		}

		public enum FriendGroups
		{
			Group_1,
			Group_2,
			Group_3,
			MAX_GROUPS
		}

		public enum WorldPlaylists
		{
			Playlist_1 = 0,
			Playlist_2 = 1,
			Playlist_3 = 2,
			MAX_PLAYLISTS = 3,
			NOT_IN_A_PLAYLIST = 3
		}

		public enum AvatarFavorites
		{
			Avatars_1,
			MAX_AVATAR_FAVORITE_GROUPS
		}

		public const float FriendsListCacheTime = 10f;

		public const float SingleRecordCacheTime = 60f;

		public const float SearchCacheTime = 60f;

		private const int MAX_STATUS_DESCRIPTION_LENGTH = 32;

		private List<string> _playlistNames = new List<string>
		{
			"worlds1",
			"worlds2",
			"worlds3"
		};

		public bool hasFetchedPlaylistNames;

		public bool hasFetchedFavoriteWorlds;

		private List<string> _favoriteWorldIds;

		public bool hasFetchedFavoriteAvatars;

		public List<string> _favoriteAvatarIds;

		public bool[] hasFetchedFavoriteFriendsInGroup = new bool[3];

		public List<string>[] _favoriteFriendIdsInGroup = new List<string>[3];

		public bool[] hasFetchedWorldsInPlaylist = new bool[3];

		public List<string>[] _worldIdsInPlaylist = new List<string>[3];

		private static Hashtable statusDefaultDescriptions = new Hashtable
		{
			{
				UserStatus.Active,
				"Active"
			},
			{
				UserStatus.JoinMe,
				"Join Me"
			},
			{
				UserStatus.Busy,
				"Busy"
			},
			{
				UserStatus.Offline,
				"Offline"
			}
		};

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
		public bool hasPendingEmail
		{
			get;
			protected set;
		}

		[ApiField(Required = false)]
		public string obfuscatedPendingEmail
		{
			get;
			protected set;
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
		[Obsolete("No.")]
		private string networkSessionId
		{
			get;
			set;
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

		[ApiField(Required = false)]
		public string status
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string statusDescription
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool allowAvatarCopying
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public List<string> friendGroupNames
		{
			get;
			protected set;
		}

		public List<string> playlistDisplayNames
		{
			get;
			protected set;
		}

		private List<string> playlistNames
		{
			get
			{
				return _playlistNames;
			}
			set
			{
				_playlistNames = value;
			}
		}

		public List<string> playlistPrivacy
		{
			get;
			protected set;
		}

		public List<string> favoriteWorldIds
		{
			get
			{
				if (_favoriteWorldIds == null)
				{
					_favoriteWorldIds = new List<string>();
				}
				return _favoriteWorldIds;
			}
			set
			{
				_favoriteWorldIds = value;
			}
		}

		public List<string> favoriteAvatarIds
		{
			get
			{
				if (_favoriteAvatarIds == null)
				{
					_favoriteAvatarIds = new List<string>();
				}
				return _favoriteAvatarIds;
			}
			set
			{
				_favoriteAvatarIds = value;
			}
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

		public bool isUntrusted => isAccountVerified && !hasBasicTrustLevel;

		public bool isNewUser => isAccountVerified && HasTag("system_new");

		public bool hasBasicTrustLevel => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_trust_basic"));

		public bool hasKnownTrustLevel => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_trust_known"));

		public bool hasTrustedTrustLevel => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_trust_trusted"));

		public bool hasVeteranTrustLevel => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_trust_veteran"));

		public bool hasLegendTrustLevel => isAccountVerified && (developerType == DeveloperType.Internal || HasTag("system_trust_legend"));

		public bool hasNegativeTrustLevel => isAccountVerified && (HasTag("system_probable_troll") || hasVeryNegativeTrustLevel);

		public bool hasVeryNegativeTrustLevel => isAccountVerified && HasTag("system_troll");

		public bool hasFeedbackAccess => isAccountVerified && HasTag("system_feedback_access");

		public bool showSocialRank => HasTag("show_social_rank");

		public bool showModTag => HasTag("show_mod_tag");

		public bool canSetStatusOffline => isAccountVerified && (developerType == DeveloperType.Moderator || developerType == DeveloperType.Internal);

		public bool statusIsSetToOffline => string.Equals(status, StatusValueToString(UserStatus.Offline));

		public bool statusIsSetToJoinMe => string.Equals(status, StatusValueToString(UserStatus.JoinMe));

		public bool statusIsSetToBusy => string.Equals(status, StatusValueToString(UserStatus.Busy));

		public string statusDefaultDescriptionDisplayString => (string)statusDefaultDescriptions[statusValue];

		public string statusDescriptionDisplayString => (string.IsNullOrEmpty(statusDescription) || !(location != "offline")) ? statusDefaultDescriptionDisplayString : truncatedStatusDescription(statusDescription);

		public UserStatus statusValue
		{
			get
			{
				UserStatus result = (status != null) ? StringToStatusValue(status) : UserStatus.Active;
				if (location == "offline")
				{
					result = UserStatus.Offline;
				}
				return result;
			}
		}

		public bool canSeeAllUsersStatus => isAccountVerified && (developerType == DeveloperType.Moderator || developerType == DeveloperType.Internal);

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

		public void FetchPlaylistNames(Action successCallback = null, Action<string> errorCallback = null)
		{
			if (!hasFetchedPlaylistNames || successCallback != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["type"] = "world";
				dictionary["ownerId"] = base.id;
				hasFetchedPlaylistNames = true;
				ApiModelListContainer<ApiGroup> apiModelListContainer = new ApiModelListContainer<ApiGroup>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					if (c != null)
					{
						List<ApiGroup> list = new List<ApiGroup>();
						foreach (ApiGroup responseModel in (c as ApiModelListContainer<ApiGroup>).ResponseModels)
						{
							list.Add(responseModel);
						}
						for (int i = 0; i < list.Count; i++)
						{
							_playlistNames[i] = list[i].name;
							playlistDisplayNames[i] = list[i].displayName;
						}
					}
					if (successCallback != null)
					{
						successCallback();
					}
				};
				apiModelListContainer.OnError = delegate(ApiContainer c)
				{
					Debug.LogError((object)("Could not fetch users groups - " + c.Error));
					hasFetchedPlaylistNames = false;
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiModelListContainer<ApiGroup> responseContainer = apiModelListContainer;
				API.SendGetRequest("favorite/groups", responseContainer, dictionary, disableCache: true);
			}
		}

		public List<string> GetFavoriteFriendIdsInGroup(FriendGroups group)
		{
			return _favoriteFriendIdsInGroup[(int)group];
		}

		public void SetFavoriteFriendIdsInGroup(List<string> friendIds, FriendGroups group)
		{
			_favoriteFriendIdsInGroup[(int)group] = friendIds;
		}

		public void AddToFavoriteFriendsGroup(string userId, FriendGroups group)
		{
			_favoriteFriendIdsInGroup[(int)group].Add(userId);
		}

		public void RemoveFromFavoriteFriendsGroups(string userId)
		{
			_favoriteFriendIdsInGroup[0].Remove(userId);
			_favoriteFriendIdsInGroup[1].Remove(userId);
			_favoriteFriendIdsInGroup[2].Remove(userId);
		}

		public bool IsFavorite(string userId)
		{
			return (_favoriteFriendIdsInGroup[0] != null && _favoriteFriendIdsInGroup[0].Contains(userId)) || (_favoriteFriendIdsInGroup[1] != null && _favoriteFriendIdsInGroup[1].Contains(userId)) || (_favoriteFriendIdsInGroup[2] != null && _favoriteFriendIdsInGroup[2].Contains(userId));
		}

		public int GetTotalFavoriteFriendsInGroup(FriendGroups group)
		{
			return (_favoriteFriendIdsInGroup[(int)group] != null) ? _favoriteFriendIdsInGroup[(int)group].Count : 0;
		}

		public int GetTotalFavoriteFriendsInAllGroups()
		{
			return ((_favoriteFriendIdsInGroup[0] != null) ? _favoriteFriendIdsInGroup[0].Count : 0) + ((_favoriteFriendIdsInGroup[1] != null) ? _favoriteFriendIdsInGroup[1].Count : 0) + ((_favoriteFriendIdsInGroup[2] != null) ? _favoriteFriendIdsInGroup[2].Count : 0);
		}

		public List<string> GetWorldIdsInPlaylist(WorldPlaylists playlist)
		{
			return _worldIdsInPlaylist[(int)playlist];
		}

		public void SetWorldIdsInPlaylist(List<string> worldIds, WorldPlaylists playlist)
		{
			_worldIdsInPlaylist[(int)playlist] = worldIds;
		}

		public void AddWorldToPlaylist(string worldId, WorldPlaylists playlist)
		{
			if (_worldIdsInPlaylist[(int)playlist] == null)
			{
				_worldIdsInPlaylist[(int)playlist] = new List<string>();
			}
			_worldIdsInPlaylist[(int)playlist].Add(worldId);
		}

		public void RemoveWorldFromPlaylist(string worldId)
		{
			if (_worldIdsInPlaylist[0] != null)
			{
				_worldIdsInPlaylist[0].Remove(worldId);
			}
			if (_worldIdsInPlaylist[1] != null)
			{
				_worldIdsInPlaylist[1].Remove(worldId);
			}
			if (_worldIdsInPlaylist[2] != null)
			{
				_worldIdsInPlaylist[2].Remove(worldId);
			}
		}

		public WorldPlaylists FindPlaylistContainingWorld(string worldId)
		{
			if (_worldIdsInPlaylist[0] != null && _worldIdsInPlaylist[0].Contains(worldId))
			{
				return WorldPlaylists.Playlist_1;
			}
			if (_worldIdsInPlaylist[1] != null && _worldIdsInPlaylist[1].Contains(worldId))
			{
				return WorldPlaylists.Playlist_1;
			}
			if (_worldIdsInPlaylist[2] != null && _worldIdsInPlaylist[2].Contains(worldId))
			{
				return WorldPlaylists.Playlist_1;
			}
			return WorldPlaylists.MAX_PLAYLISTS;
		}

		public bool IsWorldInPlaylist(WorldPlaylists playlist, string worldId)
		{
			return _worldIdsInPlaylist[(int)playlist] != null && _worldIdsInPlaylist[(int)playlist].Contains(worldId);
		}

		public bool IsWorldInPlaylists(string worldId)
		{
			return IsWorldInPlaylist(WorldPlaylists.Playlist_1, worldId) || IsWorldInPlaylist(WorldPlaylists.Playlist_2, worldId) || IsWorldInPlaylist(WorldPlaylists.Playlist_3, worldId);
		}

		public void ClearPlaylist(WorldPlaylists playlist)
		{
			if (_worldIdsInPlaylist[(int)playlist] != null)
			{
				_worldIdsInPlaylist[(int)playlist] = null;
			}
		}

		public int GetTotalWorldsInPlaylist(WorldPlaylists playlist)
		{
			return (_worldIdsInPlaylist[(int)playlist] != null) ? _worldIdsInPlaylist[(int)playlist].Count : 0;
		}

		public int GetTotalWorldsInAllPlaylists()
		{
			return ((_worldIdsInPlaylist[0] != null) ? _worldIdsInPlaylist[0].Count : 0) + ((_worldIdsInPlaylist[1] != null) ? _worldIdsInPlaylist[1].Count : 0) + ((_worldIdsInPlaylist[2] != null) ? _worldIdsInPlaylist[2].Count : 0);
		}

		public static string truncatedStatusDescription(string statusString)
		{
			return string.IsNullOrEmpty(statusString) ? string.Empty : ((statusString.Length <= 32) ? statusString : statusString.Substring(0, 32));
		}

		public static string GetFriendsGroupName(FriendGroups value)
		{
			switch (value)
			{
			case FriendGroups.Group_1:
				return "group_0";
			case FriendGroups.Group_2:
				return "group_1";
			case FriendGroups.Group_3:
				return "group_2";
			case FriendGroups.MAX_GROUPS:
				return null;
			default:
				throw new Exception("Argument not handled in switch: " + value);
			}
		}

		public static FriendGroups StringToFriendsGroup(string groupName)
		{
			switch (groupName)
			{
			case "group_0":
				return FriendGroups.Group_1;
			case "group_1":
				return FriendGroups.Group_2;
			case "group_2":
				return FriendGroups.Group_3;
			default:
				throw new Exception("Argument not handled in switch: " + groupName);
			}
		}

		public string GetFriendsGroupDisplayName(int index)
		{
			if (friendGroupNames != null && index < 3 && index < friendGroupNames.Count && !string.IsNullOrEmpty(friendGroupNames[index]))
			{
				return friendGroupNames[index];
			}
			return "Group " + (index + 1);
		}

		public void SetFriendsGroupDisplayName(int index, string name)
		{
			if (friendGroupNames == null)
			{
				friendGroupNames = new List<string>();
			}
			if (index < friendGroupNames.Count)
			{
				friendGroupNames[index] = name;
			}
			else
			{
				while (friendGroupNames.Count < index)
				{
					friendGroupNames.Add("Group " + (friendGroupNames.Count + 1));
				}
				friendGroupNames.Add(name);
			}
		}

		public static string truncatedPlaylistName(string playlistName)
		{
			return string.IsNullOrEmpty(playlistName) ? string.Empty : ((playlistName.Length <= 24) ? playlistName : playlistName.Substring(0, 24));
		}

		public string GetPlaylistGroupName(WorldPlaylists value)
		{
			switch (value)
			{
			case WorldPlaylists.Playlist_1:
				return playlistNames[0];
			case WorldPlaylists.Playlist_2:
				return playlistNames[1];
			case WorldPlaylists.Playlist_3:
				return playlistNames[2];
			case WorldPlaylists.MAX_PLAYLISTS:
				return null;
			default:
				throw new Exception("Argument not handled in switch: " + value);
			}
		}

		public WorldPlaylists StringToPlaylistGroup(string playlistName)
		{
			if (string.Equals(playlistName, playlistNames[2]))
			{
				return WorldPlaylists.Playlist_3;
			}
			if (string.Equals(playlistName, playlistNames[1]))
			{
				return WorldPlaylists.Playlist_2;
			}
			return WorldPlaylists.Playlist_1;
		}

		public void GetPlaylistDisplayInfoForPlaylist(WorldPlaylists playlistIndex, Action<bool> onFetchedPlaylistInformation = null)
		{
			ApiGroup.GetGroupDisplayNameAndPrivacy(base.id, ApiGroup.GroupType.World, GetPlaylistGroupName(playlistIndex), delegate(ApiGroup c)
			{
				Debug.Log((object)("PLAYLIST GET id=" + base.id + c.name + " " + c.visibility + " " + c.displayName));
				SetPlaylistPrivacyLevel(StringToPlaylistGroup(c.name), ApiGroup.StringToPrivacyValue(c.visibility));
				SetPlaylistDisplayName(StringToPlaylistGroup(c.name), c.displayName);
				if (onFetchedPlaylistInformation != null)
				{
					onFetchedPlaylistInformation(obj: false);
				}
			}, delegate(string obj)
			{
				if (onFetchedPlaylistInformation != null)
				{
					onFetchedPlaylistInformation(obj: true);
				}
				Debug.LogError((object)("Error fetching playlist info: " + obj));
			});
		}

		public string GetPlaylistDisplayName(WorldPlaylists index)
		{
			if (playlistDisplayNames != null && index < WorldPlaylists.MAX_PLAYLISTS && (int)index < playlistDisplayNames.Count && !string.IsNullOrEmpty(playlistDisplayNames[(int)index]))
			{
				return playlistDisplayNames[(int)index];
			}
			return "Playlist " + (int)(index + 1);
		}

		public void SetPlaylistDisplayName(WorldPlaylists index, string name)
		{
			if (playlistDisplayNames == null)
			{
				playlistDisplayNames = new List<string>();
			}
			if ((int)index < playlistDisplayNames.Count)
			{
				playlistDisplayNames[(int)index] = name;
			}
			else
			{
				while (playlistDisplayNames.Count < (int)index)
				{
					playlistDisplayNames.Add("Playlist " + (playlistDisplayNames.Count + 1));
				}
				playlistDisplayNames.Add(name);
			}
		}

		public ApiGroup.Privacy GetPlaylistPrivacyLevel(WorldPlaylists index)
		{
			if (playlistPrivacy != null && index < WorldPlaylists.MAX_PLAYLISTS && (int)index < playlistPrivacy.Count)
			{
				return ApiGroup.StringToPrivacyValue(playlistPrivacy[(int)index]);
			}
			return ApiGroup.Privacy.Private;
		}

		public void SetPlaylistPrivacyLevel(WorldPlaylists index, ApiGroup.Privacy privacyLevel)
		{
			if (playlistPrivacy == null)
			{
				playlistPrivacy = new List<string>();
			}
			if ((int)index < playlistPrivacy.Count)
			{
				playlistPrivacy[(int)index] = ApiGroup.PrivacyValueToString(privacyLevel);
			}
			else
			{
				while (playlistPrivacy.Count < (int)index)
				{
					playlistPrivacy.Add(ApiGroup.PrivacyValueToString(ApiGroup.Privacy.Private));
				}
				playlistPrivacy.Add(ApiGroup.PrivacyValueToString(privacyLevel));
			}
		}

		public static string GetAvatarGroupName(AvatarFavorites value)
		{
			switch (value)
			{
			case AvatarFavorites.Avatars_1:
				return "avatars1";
			case AvatarFavorites.MAX_AVATAR_FAVORITE_GROUPS:
				return null;
			default:
				throw new Exception("Argument not handled in switch: " + value);
			}
		}

		public static AvatarFavorites StringToAvatarGroup(string avatarGroupName)
		{
			switch (avatarGroupName)
			{
			case "avatars1":
				return AvatarFavorites.Avatars_1;
			default:
				throw new Exception("Argument not handled in switch: " + avatarGroupName);
			}
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

		public static void FetchCurrentUser(Action<ApiModelContainer<APIUser>> onSuccess, Action<ApiModelContainer<APIUser>> onError)
		{
			if (!ApiCredentials.Load())
			{
				if (onError != null)
				{
					onError(new ApiModelContainer<APIUser>
					{
						Error = "Not logged in."
					});
				}
			}
			else
			{
				Logger.Log("Fetching user", DebugLevel.API);
				ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
				apiModelContainer.OnSuccess = delegate(ApiContainer c)
				{
					CurrentUser = (c.Model as APIUser);
					if (onSuccess != null)
					{
						onSuccess(c as ApiModelContainer<APIUser>);
					}
				};
				apiModelContainer.OnError = delegate(ApiContainer c)
				{
					Logger.LogWarning("NOT Authenticated: " + c.Error, DebugLevel.API);
					if (onError != null)
					{
						onError(c as ApiModelContainer<APIUser>);
					}
				};
				ApiModelContainer<APIUser> responseContainer = apiModelContainer;
				API.SendGetRequest("auth/user", responseContainer, null, disableCache: true, 0f);
			}
		}

		public static void Register(string username, string email, string password, string birthday, Action<ApiModelContainer<APIUser>> successCallback = null, Action<ApiModelContainer<APIUser>> errorCallback = null)
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
					CurrentUser = (c.Model as APIUser);
					if (successCallback != null)
					{
						successCallback(c as ApiModelContainer<APIUser>);
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c as ApiModelContainer<APIUser>);
					}
				}
			}, requestParams);
		}

		public void UpdateAccountInfo(string email, string birthday, string acceptedTOSVersion, Action<ApiModelContainer<APIUser>> successCallback = null, Action<ApiModelContainer<APIUser>> errorCallback = null)
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
						successCallback(c as ApiModelContainer<APIUser>);
					}
				},
				OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c as ApiModelContainer<APIUser>);
					}
				}
			}, dictionary);
		}

		public static void Login(string usernameOrEmail, string password, Action<ApiModelContainer<APIUser>> successCallback = null, Action<ApiModelContainer<APIUser>> errorCallback = null)
		{
			Logger.Log("Logging in", DebugLevel.API);
			ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				CurrentUser = (c.Model as APIUser);
				if (successCallback != null)
				{
					successCallback(c as ApiModelContainer<APIUser>);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				Logger.LogWarning("NOT Authenticated: " + c.Error, DebugLevel.API);
				if (errorCallback != null)
				{
					errorCallback(c as ApiModelContainer<APIUser>);
				}
			};
			ApiModelContainer<APIUser> responseContainer = apiModelContainer;
			API.SendGetRequest("auth/user", responseContainer, null, disableCache: true, 0f, new API.CredentialsBundle
			{
				Username = usernameOrEmail,
				Password = password
			});
		}

		public static void ThirdPartyLogin(string endpoint, Dictionary<string, object> parameters, Action<string, ApiModelContainer<APIUser>> onFetchSuccess = null, Action<ApiModelContainer<APIUser>> onFetchError = null)
		{
			Logger.Log("Third Party Login: " + endpoint, DebugLevel.API);
			ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				CurrentUser = (c.Model as APIUser);
				if (onFetchSuccess != null)
				{
					onFetchSuccess(CurrentUser.authToken, c as ApiModelContainer<APIUser>);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				Logger.Log("NOT Authenticated: " + c.Error.Replace("{", "{{").Replace("}", "}}"), DebugLevel.API);
				if (onFetchError != null)
				{
					onFetchError(c as ApiModelContainer<APIUser>);
				}
			};
			ApiModelContainer<APIUser> responseContainer = apiModelContainer;
			int retryCount = 0;
			API.SendRequest("auth/" + endpoint, HTTPMethods.Post, responseContainer, parameters, authenticationRequired: false, disableCache: false, 3600f, retryCount);
		}

		public static void Logout()
		{
			if (IsLoggedInWithCredentials)
			{
				API.SendPutRequest("logout");
			}
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

		public static void FetchUser(string userId, Action<APIUser> successCallback, Action<string> errorCallback)
		{
			ApiModelContainer<APIUser> apiModelContainer = new ApiModelContainer<APIUser>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as APIUser);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<APIUser> responseContainer = apiModelContainer;
			API.SendGetRequest("users/" + userId, responseContainer, null, disableCache: true);
		}

		public static void FetchFriends(FriendLocation location, int offset = 0, int count = 100, Action<List<APIUser>> successCallback = null, Action<string> errorCallback = null, bool useCache = true)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("offset", offset);
			dictionary.Add("n", count);
			Dictionary<string, object> requestParams = dictionary;
			if (useCache)
			{
				List<APIUser> list = new List<APIUser>();
				foreach (string friendID in CurrentUser.friendIDs)
				{
					APIUser target = new APIUser();
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
					return;
				}
			}
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
			API.SendGetRequest("auth/user/friends" + ((location != FriendLocation.Offline) ? string.Empty : "?offline=true"), responseContainer, requestParams, disableCache: false, 10f);
		}

		public static void FetchFavoriteFriends(Action<List<APIUser>> successCallback = null, Action<string> errorCallback = null, string tag = null, bool bUseCache = true)
		{
			if (bUseCache && CurrentUser.GetTotalFavoriteFriendsInGroup(StringToFriendsGroup(tag)) > 0)
			{
				List<APIUser> list = new List<APIUser>();
				foreach (string item in CurrentUser.GetFavoriteFriendIdsInGroup(StringToFriendsGroup(tag)))
				{
					APIUser target = new APIUser();
					if (!ApiCache.Fetch(item, ref target))
					{
						break;
					}
					list.Add(target);
				}
				if (list.Count == CurrentUser.GetTotalFavoriteFriendsInGroup(StringToFriendsGroup(tag)))
				{
					if (successCallback != null)
					{
						successCallback(list);
					}
					return;
				}
			}
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
			API.SendGetRequest("auth/user/friends/favorite" + ((tag == null) ? string.Empty : ("?tag=" + tag)), responseContainer, null, disableCache: true);
		}

		public void FetchAllFavoriteFriends()
		{
			int num = 3;
			for (int j = 0; j < num; j++)
			{
				if (!hasFetchedFavoriteFriendsInGroup[j])
				{
					int i = j;
					ApiGroup.FetchGroupMemberIds(CurrentUser.id, ApiGroup.GroupType.Friend, delegate(List<string> userIds)
					{
						SetFavoriteFriendIdsInGroup(userIds, (FriendGroups)i);
						hasFetchedFavoriteFriendsInGroup[i] = true;
					}, delegate(string obj)
					{
						Debug.LogError((object)("Error fetching favorite friends : " + obj));
					}, GetFriendsGroupName((FriendGroups)i));
				}
			}
		}

		public static void FetchFavoriteWorlds(Action successCallback = null, Action<string> errorCallback = null)
		{
			ApiGroup.FetchGroupMemberIds(CurrentUser.id, ApiGroup.GroupType.World, delegate(List<string> worldIds)
			{
				CurrentUser.favoriteWorldIds = worldIds;
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(string obj)
			{
				Debug.LogError((object)("Error fetching favorite worlds: " + obj));
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void FetchFavoriteAvatars(Action successCallback = null, Action<string> errorCallback = null)
		{
			ApiGroup.FetchGroupMemberIds(CurrentUser.id, ApiGroup.GroupType.Avatar, delegate(List<string> avatarIds)
			{
				CurrentUser.favoriteAvatarIds = avatarIds;
				CurrentUser.hasFetchedFavoriteAvatars = true;
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(string obj)
			{
				Debug.LogError((object)("Error fetching favorite avatars: " + obj));
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
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

		public static string StatusValueToString(UserStatus statusValue)
		{
			switch (statusValue)
			{
			case UserStatus.Active:
				return "active";
			case UserStatus.JoinMe:
				return "join me";
			case UserStatus.Busy:
				return "busy";
			case UserStatus.Offline:
				return "offline";
			default:
				throw new Exception("Argument not handled in switch: " + statusValue.ToString());
			}
		}

		public static UserStatus StringToStatusValue(string statusValue)
		{
			switch (statusValue)
			{
			case "active":
				return UserStatus.Active;
			case "join me":
				return UserStatus.JoinMe;
			case "busy":
				return UserStatus.Busy;
			case "offline":
				return UserStatus.Offline;
			default:
				throw new Exception("Argument not handled in switch: " + statusValue);
			}
		}

		public void SetStatus(string status, string statusDescription, Action successCallback, Action<string> errorCallback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["status"] = status;
			dictionary["statusDescription"] = truncatedStatusDescription(statusDescription);
			API.SendPutRequest("users/" + base.id, new ApiContainer
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
			}, dictionary);
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
			if (CurrentUser == null || CurrentUser.friendIDs == null)
			{
				return false;
			}
			for (int i = 0; i < CurrentUser.friendIDs.Count; i++)
			{
				if (CurrentUser.friendIDs[i] == userId)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsFriendsWithAndHasFavorited(string userId)
		{
			return CurrentUser.IsFavorite(userId);
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
			}, dictionary);
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

		public bool AddTag(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				return false;
			}
			if (!tags.Contains(tag))
			{
				tags.Add(tag);
				return true;
			}
			return false;
		}

		public bool RemoveTag(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				return false;
			}
			return tags.RemoveAll((string t) => t == tag) > 0;
		}

		public override string ToString()
		{
			return string.Format("[id: {0}; username: {1}; displayName: {2}; avatarId: {3}; events: {4}; status: {5}; statusDescription: {6};]", base.id, username, displayName, avatarId, events, (!string.IsNullOrEmpty(status)) ? status : "NULL", (!string.IsNullOrEmpty(statusDescription)) ? statusDescription : "NULL");
		}
	}
}
