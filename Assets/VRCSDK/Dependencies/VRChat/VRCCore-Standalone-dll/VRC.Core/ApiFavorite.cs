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

			private FavoriteType(string val)
			{
				value = val;
			}
		}

		public const int MaxFavoriteWorlds = 32;

		public const int MaxFavoriteFriends = 32;

		public static void AddFavorite(string objectId, FavoriteType favoriteType, Action successCallback, Action<string> errorCallback)
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
			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			API.SendDeleteRequest("favorites/" + objectId, responseContainer, requestParams);
		}

		public static void FetchFavoriteIds(FavoriteType favoriteType, Action<List<string>> successCallback = null, Action<string> errorCallback = null)
		{
			List<string> favoriteWorldIds = new List<string>();
			List<string> favoriteFriendIds = new List<string>();
			if (favoriteType.value == "world")
			{
				ApiModelListContainer<ApiWorld> apiModelListContainer = new ApiModelListContainer<ApiWorld>();
				apiModelListContainer.OnSuccess = delegate(ApiContainer c)
				{
					favoriteWorldIds = new List<string>();
					foreach (ApiWorld responseModel in (c as ApiModelListContainer<ApiWorld>).ResponseModels)
					{
						favoriteWorldIds.Add(responseModel.id);
					}
					if (successCallback != null)
					{
						successCallback(favoriteWorldIds);
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
				API.SendGetRequest("worlds/favorites", responseContainer, null, disableCache: true);
			}
			else if (favoriteType.value == "friend")
			{
				ApiModelListContainer<APIUser> apiModelListContainer2 = new ApiModelListContainer<APIUser>();
				apiModelListContainer2.OnSuccess = delegate(ApiContainer c)
				{
					favoriteFriendIds = new List<string>();
					foreach (APIUser responseModel2 in (c as ApiModelListContainer<APIUser>).ResponseModels)
					{
						favoriteFriendIds.Add(responseModel2.id);
					}
					if (successCallback != null)
					{
						successCallback(favoriteFriendIds);
					}
				};
				apiModelListContainer2.OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiContainer responseContainer2 = apiModelListContainer2;
				API.SendGetRequest("auth/user/friends/favorite", responseContainer2, null, disableCache: true);
			}
			else
			{
				Debug.LogError((object)("Cannot fetch favorite " + favoriteType.value + " b/c it's not implemented yet."));
			}
		}
	}
}
