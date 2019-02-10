using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class ApiGroup : ApiModel
	{
		public class GroupType
		{
			public string value
			{
				get;
				set;
			}

			public static GroupType World => new GroupType("world");

			public static GroupType Friend => new GroupType("friend");

			public static GroupType Avatar => new GroupType("avatar");

			private GroupType(string val)
			{
				value = val;
			}
		}

		public enum Privacy
		{
			Public,
			Friends,
			Private
		}

		public const int MAX_GROUP_NAME_LENGTH = 24;

		public const int MAX_WORLDS_IN_GROUP = 32;

		public const int MAX_FRIENDS_IN_GROUP = 32;

		public const int MAX_AVATARS_IN_GROUP = 16;

		public const int MAX_FAVORITE_FRIEND_GROUPS = 3;

		public const int MAX_WORLD_PLAYLIST_GROUPS = 3;

		public const int MAX_FAVORITE_AVATAR_GROUPS = 1;

		[ApiField]
		public new string id
		{
			get;
			set;
		}

		[ApiField]
		public string ownerId
		{
			get;
			set;
		}

		[ApiField]
		public string ownerDisplayName
		{
			get;
			set;
		}

		[ApiField]
		public string name
		{
			get;
			set;
		}

		[ApiField]
		public string displayName
		{
			get;
			set;
		}

		[ApiField]
		public string type
		{
			get;
			set;
		}

		[ApiField]
		public string visibility
		{
			get;
			set;
		}

		[ApiField]
		public List<string> tags
		{
			get;
			set;
		}

		public static string PrivacyValueToString(Privacy privacyValue)
		{
			switch (privacyValue)
			{
			case Privacy.Friends:
				return "friends";
			case Privacy.Private:
				return "private";
			case Privacy.Public:
				return "public";
			default:
				throw new Exception("Argument not handled in switch: " + privacyValue.ToString());
			}
		}

		public static Privacy StringToPrivacyValue(string privacyString)
		{
			switch (privacyString)
			{
			case "friends":
				return Privacy.Friends;
			case "private":
				return Privacy.Private;
			case "public":
				return Privacy.Public;
			default:
				throw new Exception("Argument not handled in switch: " + privacyString);
			}
		}

		public static void AddToGroup(string objectId, GroupType groupType, Action<ApiModelContainer<ApiFavorite>> successCallback, Action<string> errorCallback, List<string> tags = null)
		{
			ApiModelContainer<ApiFavorite> apiModelContainer = new ApiModelContainer<ApiFavorite>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c as ApiModelContainer<ApiFavorite>);
				}
			};
			apiModelContainer.OnError = delegate
			{
				if (errorCallback != null)
				{
					errorCallback("Error");
				}
			};
			ApiModelContainer<ApiFavorite> responseContainer = apiModelContainer;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["type"] = groupType.value;
			dictionary["favoriteId"] = objectId;
			if (tags != null)
			{
				dictionary["tags"] = tags;
			}
			API.SendPostRequest("favorites", responseContainer, dictionary);
		}

		public static void RemoveFromGroup(string objectId, Action successCallback, Action<string> errorCallback)
		{
			ApiDictContainer apiDictContainer = new ApiDictContainer();
			apiDictContainer.OnSuccess = delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			};
			apiDictContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiContainer responseContainer = apiDictContainer;
			API.SendDeleteRequest("favorites/" + objectId, responseContainer);
		}

		public static void FetchGroupNames(string userId, string groupType, Action<List<string>> successCallback = null, Action<string> errorCallback = null)
		{
			ApiModelListContainer<ApiGroup> apiModelListContainer = new ApiModelListContainer<ApiGroup>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				List<string> list = new List<string>();
				foreach (ApiGroup responseModel in (c as ApiModelListContainer<ApiGroup>).ResponseModels)
				{
					list.Add(responseModel.name);
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiGroup> responseContainer = apiModelListContainer;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ownerId"] = userId;
			if (groupType != null)
			{
				dictionary["type"] = groupType;
			}
			API.SendGetRequest("favorite/groups", responseContainer, dictionary, disableCache: true);
		}

		public static void SetGroupDisplayNameAndPrivacy(GroupType groupType, string group, string displayName, string privacyLevel, Action<ApiGroup> successCallback = null, Action<string> errorCallback = null)
		{
			ApiModelContainer<ApiGroup> apiModelContainer = new ApiModelContainer<ApiGroup>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiGroup);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<ApiGroup> responseContainer = apiModelContainer;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (displayName != null)
			{
				dictionary["displayName"] = displayName;
			}
			if (privacyLevel != null)
			{
				dictionary["visibility"] = privacyLevel;
			}
			API.SendPutRequest("favorite/group/" + groupType.value + "/" + group + "/" + APIUser.CurrentUser.id, responseContainer, dictionary);
		}

		public static void GetGroupDisplayNameAndPrivacy(string userId, GroupType groupType, string group, Action<ApiGroup> successCallback = null, Action<string> errorCallback = null)
		{
			ApiModelContainer<ApiGroup> apiModelContainer = new ApiModelContainer<ApiGroup>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiGroup);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelContainer<ApiGroup> responseContainer = apiModelContainer;
			API.SendGetRequest("favorite/group/" + groupType.value + "/" + group + "/" + userId, responseContainer, null, disableCache: true);
		}

		public static void ClearGroup(string userId, GroupType groupType, string group, Action successCallback = null, Action<string> errorCallback = null)
		{
			ApiContainer apiContainer = new ApiContainer();
			apiContainer.OnSuccess = delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			};
			apiContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiContainer responseContainer = apiContainer;
			API.SendDeleteRequest("favorite/group/" + groupType.value + "/" + group + "/" + userId, responseContainer);
		}

		public static void FetchGroupMembers(string userId, GroupType groupType, Action<List<ApiFavorite>> successCallback = null, Action<string> errorCallback = null, string tag = null)
		{
			if (groupType.value == "world" || groupType.value == "avatar" || groupType.value == "friend")
			{
				ApiModelListContainer<ApiFavorite> apiModelListContainer = new ApiModelListContainer<ApiFavorite>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					List<ApiFavorite> responseModels = (c as ApiModelListContainer<ApiFavorite>).ResponseModels;
					if (successCallback != null)
					{
						successCallback((c as ApiModelListContainer<ApiFavorite>).ResponseModels);
					}
				};
				apiModelListContainer.OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiModelListContainer<ApiFavorite> responseContainer = apiModelListContainer;
				string target = "favorites";
				Dictionary<string, object> dictionary = null;
				dictionary = new Dictionary<string, object>();
				dictionary["ownerId"] = userId;
				if (tag != null)
				{
					dictionary["tag"] = tag;
				}
				dictionary["type"] = groupType.value;
				dictionary["n"] = 96;
				API.SendGetRequest(target, responseContainer, dictionary, disableCache: true);
			}
			else
			{
				Debug.LogError((object)("Cannot fetch group members " + groupType.value + " b/c it's not implemented yet."));
			}
		}

		public static void FetchGroupMemberIds(string userId, GroupType groupType, Action<List<string>> successCallback = null, Action<string> errorCallback = null, string tag = null)
		{
			if (groupType.value == "world" || groupType.value == "avatar" || groupType.value == "friend")
			{
				ApiModelListContainer<ApiFavorite> apiModelListContainer = new ApiModelListContainer<ApiFavorite>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					List<string> list = new List<string>();
					foreach (ApiFavorite responseModel in (c as ApiModelListContainer<ApiFavorite>).ResponseModels)
					{
						list.Add(responseModel.favoriteId);
					}
					if (successCallback != null)
					{
						successCallback(list);
					}
				};
				apiModelListContainer.OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiModelListContainer<ApiFavorite> responseContainer = apiModelListContainer;
				string target = "favorites";
				Dictionary<string, object> dictionary = null;
				dictionary = new Dictionary<string, object>();
				dictionary["ownerId"] = userId;
				if (tag != null)
				{
					dictionary["tag"] = tag;
				}
				dictionary["type"] = groupType.value;
				dictionary["n"] = 96;
				API.SendGetRequest(target, responseContainer, dictionary, disableCache: true);
			}
			else
			{
				Debug.LogError((object)("Cannot fetch group member ids " + groupType.value + " b/c it's not implemented yet."));
			}
		}
	}
}
