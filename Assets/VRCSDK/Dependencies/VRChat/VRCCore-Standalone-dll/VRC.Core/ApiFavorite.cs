using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core
{
	public class ApiFavorite : ApiModel
	{
		public class FavoriteType
		{
			public string value
			{
				get;
				set;
			}

			public static FavoriteType World => new FavoriteType("world");

			public static FavoriteType Friend => new FavoriteType("friend");

			public static FavoriteType Avatar => new FavoriteType("avatar");

			private FavoriteType(string val)
			{
				value = val;
			}
		}

		public const int MaxFavoriteWorlds = 32;

		public const int MaxFavoriteFriends = 32;

		public const int MaxFavoriteAvatars = 3;

		public const int MaxFavoriteFriendGroups = 3;

		public static void AddFavorite(string objectId, FavoriteType favoriteType, Action successCallback, Action<string> errorCallback, List<string> tags = null)
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
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["type"] = favoriteType.value;
			dictionary["favoriteId"] = objectId;
			if (tags != null)
			{
				dictionary["tags"] = tags;
			}
			API.SendPostRequest("favorites", responseContainer, dictionary);
		}

		public static void RemoveFavorite(string objectId, Action successCallback, Action<string> errorCallback)
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

		public static void FetchFavoriteIds(FavoriteType favoriteType, Action<List<string>> successCallback = null, Action<string> errorCallback = null, string tag = null)
		{
			if (favoriteType.value == "world" || favoriteType.value == "avatar" || favoriteType.value == "friend")
			{
				ApiModelListContainer<ApiModel> apiModelListContainer = new ApiModelListContainer<ApiModel>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					List<string> list = new List<string>();
					foreach (ApiModel responseModel in (c as ApiModelListContainer<ApiModel>).ResponseModels)
					{
						list.Add(responseModel.id);
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
				ApiContainer responseContainer = apiModelListContainer;
				string target = string.Empty;
				Dictionary<string, object> dictionary = null;
				if (favoriteType.value == "world")
				{
					target = "worlds/favorites";
				}
				else if (favoriteType.value == "avatar")
				{
					target = "avatars/favorites";
				}
				else if (favoriteType.value == "friend")
				{
					target = "auth/user/friends/favorite";
					if (tag != null)
					{
						dictionary = new Dictionary<string, object>();
						dictionary["tag"] = tag;
					}
				}
				API.SendGetRequest(target, responseContainer, dictionary, disableCache: true);
			}
			else
			{
				Debug.LogError((object)("Cannot fetch favorite " + favoriteType.value + " b/c it's not implemented yet."));
			}
		}
	}
}
