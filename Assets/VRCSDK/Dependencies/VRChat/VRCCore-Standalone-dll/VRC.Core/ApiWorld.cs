using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class ApiWorld : ApiModel, IUIGroupItemDatasource
	{
		public enum SortHeading
		{
			Featured,
			Trending,
			Updated,
			Created,
			Active
		}

		public enum SortOwnership
		{
			Any,
			Mine,
			Friend
		}

		public enum SortOrder
		{
			Ascending,
			Descending
		}

		public enum ReleaseStatus
		{
			Public,
			Private,
			All,
			Hidden
		}

		protected string mName;

		protected string mImageUrl;

		protected string mAuthorName;

		protected string mAuthorId;

		protected string mAssetUrl;

		protected string mDescription;

		protected List<string> mTags = new List<string>();

		protected int mVersion;

		protected string mPluginUrl;

		protected string mUnityPackageUrl;

		private string mReleaseStatus;

		private int mCapacity;

		private int mOccupants;

		private Dictionary<string, int> mInstances;

		private string mThumbnailImageUrl;

		public bool isCurated;

		private static Dictionary<string, ApiWorld> localWorlds = new Dictionary<string, ApiWorld>();

		public new string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public string name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}

		public string imageUrl
		{
			get
			{
				return mImageUrl;
			}
			set
			{
				mImageUrl = value;
			}
		}

		public string authorName => mAuthorName;

		public string authorId => mAuthorId;

		public string assetUrl
		{
			get
			{
				return mAssetUrl;
			}
			set
			{
				mAssetUrl = value;
			}
		}

		public string description
		{
			get
			{
				return mDescription;
			}
			set
			{
				mDescription = value;
			}
		}

		public List<string> tags
		{
			get
			{
				return mTags;
			}
			set
			{
				mTags = value;
			}
		}

		public int version
		{
			get
			{
				return mVersion;
			}
			set
			{
				mVersion = value;
			}
		}

		public string pluginUrl
		{
			get
			{
				return mPluginUrl;
			}
			set
			{
				mPluginUrl = value;
			}
		}

		public string unityPackageUrl
		{
			get
			{
				return mUnityPackageUrl;
			}
			set
			{
				mUnityPackageUrl = value;
			}
		}

		public string releaseStatus
		{
			get
			{
				return mReleaseStatus;
			}
			set
			{
				mReleaseStatus = value;
			}
		}

		public int capacity
		{
			get
			{
				return mCapacity;
			}
			set
			{
				mCapacity = value;
			}
		}

		public int occupants
		{
			get
			{
				return mOccupants;
			}
			set
			{
				mOccupants = value;
			}
		}

		public Dictionary<string, int> instances => mInstances;

		public string thumbnailImageUrl
		{
			get
			{
				return mThumbnailImageUrl;
			}
			set
			{
				mThumbnailImageUrl = value;
			}
		}

		public void Init(APIUser author, string name, string imageUrl, string assetUrl, string description, string releaseStatus, int capacity, List<string> tags, int occupants, string pluginUrl = "", string unityPackageUrl = "")
		{
			mName = name;
			mImageUrl = imageUrl;
			mAssetUrl = assetUrl;
			mDescription = description;
			mReleaseStatus = releaseStatus;
			mCapacity = capacity;
			mTags = tags;
			mAuthorName = author.displayName;
			mAuthorId = author.id;
			mPluginUrl = pluginUrl;
			mUnityPackageUrl = unityPackageUrl;
			mOccupants = occupants;
		}

		public void Init()
		{
			mId = string.Empty;
			mName = string.Empty;
			mImageUrl = string.Empty;
			mAssetUrl = string.Empty;
			mDescription = string.Empty;
			mTags = new List<string>();
			mAuthorName = string.Empty;
			mAuthorId = string.Empty;
			mVersion = -1;
			mReleaseStatus = "private";
			mCapacity = 32;
			mOccupants = 0;
		}

		private void InitBrief(Dictionary<string, object> jsonObject)
		{
			Init();
			mId = (jsonObject["id"] as string);
			mName = (jsonObject["name"] as string);
			mImageUrl = (jsonObject["imageUrl"] as string);
			mThumbnailImageUrl = (jsonObject["thumbnailImageUrl"] as string);
			mReleaseStatus = (jsonObject["releaseStatus"] as string);
			mAuthorName = (jsonObject["authorName"] as string);
			mCapacity = Convert.ToInt16(jsonObject["capacity"]);
			mOccupants = Convert.ToInt16(jsonObject["occupants"]);
		}

		public void Init(Dictionary<string, object> jsonObject)
		{
			if (jsonObject == null)
			{
				Init();
				Debug.LogError((object)"Could not Init ApiWorld from jsonObject bc jsonObject is null.");
			}
			else
			{
				List<string> list = new List<string>();
				list.Add("id");
				list.Add("name");
				list.Add("imageUrl");
				list.Add("authorName");
				list.Add("authorId");
				list.Add("assetUrl");
				list.Add("description");
				list.Add("releaseStatus");
				list.Add("capacity");
				list.Add("tags");
				list.Add("pluginUrl");
				List<string> source = list;
				if (source.Any((string s) => !jsonObject.ContainsKey(s)))
				{
					Debug.LogError((object)"Could not initialize blueprint due to insufficient json parameters");
				}
				else
				{
					mId = (jsonObject["id"] as string);
					mName = (jsonObject["name"] as string);
					mImageUrl = (jsonObject["imageUrl"] as string);
					if (jsonObject.ContainsKey("thumbnailImageUrl"))
					{
						mThumbnailImageUrl = (jsonObject["thumbnailImageUrl"] as string);
					}
					mAuthorName = (jsonObject["authorName"] as string);
					mAuthorId = (jsonObject["authorId"] as string);
					mAssetUrl = (jsonObject["assetUrl"] as string);
					mDescription = (jsonObject["description"] as string);
					mTags = Tools.ObjListToStringList((List<object>)jsonObject["tags"]);
					mVersion = (int)(double)jsonObject["version"];
					mPluginUrl = (jsonObject["pluginUrl"] as string);
					if (jsonObject.ContainsKey("instances"))
					{
						mInstances = new Dictionary<string, int>();
						List<object> list2 = jsonObject["instances"] as List<object>;
						foreach (object item in list2)
						{
							List<object> list3 = item as List<object>;
							string key = list3[0].ToString();
							int value = Convert.ToInt32(list3[1].ToString());
							mInstances.Add(key, value);
						}
					}
					mReleaseStatus = (jsonObject["releaseStatus"] as string);
					mCapacity = Convert.ToInt16(jsonObject["capacity"]);
					if (jsonObject.ContainsKey("occupants"))
					{
						mOccupants = Convert.ToInt16(jsonObject["occupants"]);
					}
				}
			}
		}

		protected new Dictionary<string, string> BuildWebParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["userId"] = APIUser.CurrentUser.id;
			dictionary["name"] = name;
			dictionary["imageUrl"] = imageUrl;
			dictionary["authorName"] = authorName;
			dictionary["authorId"] = authorId;
			dictionary["assetUrl"] = assetUrl;
			dictionary["description"] = description;
			dictionary["tags"] = string.Join(", ", tags.ToArray());
			dictionary["pluginUrl"] = pluginUrl;
			if (!string.IsNullOrEmpty(unityPackageUrl))
			{
				dictionary["unityPackageUrl"] = unityPackageUrl;
			}
			dictionary["releaseStatus"] = releaseStatus;
			dictionary["capacity"] = capacity.ToString();
			if (APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
			{
				dictionary["featured"] = isCurated.ToString().ToLower();
			}
			return dictionary;
		}

		public override string ToString()
		{
			return $"[id: {id}; name: {name}; imageUrl: {imageUrl}; authorName: {authorName}; authorId: {authorId}; assetUrl: {assetUrl};description: {description}; releaseStatus: {releaseStatus}; capacity: {capacity}; tags: {Tools.ListToString(tags)}; version {version}; occupants {occupants}; pluginUrl {pluginUrl}]";
		}

		public void Save(Action<ApiModel> onSuccess = null, Action<string> onError = null)
		{
			if (APIUser.IsLoggedInWithCredentials)
			{
				Dictionary<string, string> dictionary = BuildWebParameters();
				if (mId != null)
				{
					if (APIUser.CurrentUser.id == authorId)
					{
						dictionary["id"] = id;
						ApiModel.SendPutRequest("worlds/" + id, dictionary, delegate
						{
							if (onSuccess != null)
							{
								onSuccess(this);
							}
						});
					}
					else
					{
						Logger.LogError("Only the blueprint's author can update this blueprint.");
					}
				}
				else
				{
					ApiModel.SendPostRequest("worlds", dictionary, delegate(Dictionary<string, object> successResponse)
					{
						if (successResponse.ContainsKey("id"))
						{
							mId = (string)successResponse["id"];
						}
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					});
				}
			}
			else
			{
				Logger.Log("Must be logged in with account to create or edit a blueprint.");
			}
		}

		public static void Fetch(string id, Action<ApiWorld> successCallback, Action<string> errorCallback)
		{
			if (localWorlds.ContainsKey(id))
			{
				successCallback(localWorlds[id]);
			}
			else
			{
				ApiModel.SendGetRequest("worlds/" + id, delegate(Dictionary<string, object> obj)
				{
					ApiWorld apiWorld = ScriptableObject.CreateInstance<ApiWorld>();
					apiWorld.Init(obj);
					if (successCallback != null)
					{
						successCallback(apiWorld);
					}
				}, delegate(string message)
				{
					errorCallback(message);
				});
			}
		}

		public static void FetchList(Action<List<ApiWorld>> successCallback, Action<string> errorCallback = null, SortHeading heading = SortHeading.Featured, SortOwnership owner = SortOwnership.Any, SortOrder order = SortOrder.Descending, int offset = 0, int count = 10, string search = "", string[] tags = null, string userId = "", ReleaseStatus releaseStatus = ReleaseStatus.Public)
		{
			string endpoint = "worlds";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			switch (heading)
			{
			case SortHeading.Featured:
				dictionary.Add("sort", "popularity");
				dictionary.Add("featured", "true");
				break;
			case SortHeading.Trending:
				dictionary.Add("sort", "popularity");
				dictionary.Add("featured", "false");
				break;
			case SortHeading.Updated:
				dictionary.Add("sort", "updated");
				break;
			case SortHeading.Created:
				dictionary.Add("sort", "created");
				break;
			case SortHeading.Active:
				endpoint = "worlds/active";
				break;
			}
			switch (owner)
			{
			case SortOwnership.Mine:
				dictionary.Add("user", "me");
				break;
			case SortOwnership.Friend:
				dictionary.Add("userId", userId);
				break;
			}
			dictionary.Add("n", count.ToString());
			switch (order)
			{
			case SortOrder.Ascending:
				dictionary.Add("order", "ascending");
				break;
			case SortOrder.Descending:
				dictionary.Add("order", "descending");
				break;
			}
			if (offset > 0)
			{
				dictionary.Add("offset", offset.ToString());
			}
			if (!string.IsNullOrEmpty(search))
			{
				dictionary.Add("search", search);
			}
			if (tags != null && tags.Length > 0)
			{
				dictionary.Add("tag", string.Join(",", tags));
			}
			dictionary.Add("releaseStatus", releaseStatus.ToString().ToLower());
			ApiModel.SendRequest(endpoint, HTTPMethods.Get, dictionary, delegate(List<object> objs)
			{
				List<ApiWorld> list = new List<ApiWorld>();
				if (objs != null)
				{
					foreach (object obj in objs)
					{
						Dictionary<string, object> jsonObject = obj as Dictionary<string, object>;
						ApiWorld apiWorld = ScriptableObject.CreateInstance<ApiWorld>();
						apiWorld.InitBrief(jsonObject);
						list.Add(apiWorld);
					}
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not fetch worlds with error - " + message);
				errorCallback(message);
			});
		}

		public static void Delete(string id, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("worlds/" + id, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, (Action<string>)delegate(string message)
			{
				errorCallback(message);
			});
		}

		public static void AddLocal(ApiWorld world)
		{
			localWorlds.Add(world.id, world);
		}

		public string GetBestInstance()
		{
			if (mInstances == null)
			{
				return "1";
			}
			KeyValuePair<string, int>[] array = mInstances.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, int> keyValuePair = array[i];
				if (keyValuePair.Value < capacity)
				{
					return keyValuePair.Key;
				}
			}
			int num = 1;
			while (mInstances.ContainsKey(num.ToString()))
			{
				num++;
			}
			return num.ToString();
		}

		public void SaveAndAddToUser(Action<ApiModel> onSuccess = null, Action<string> onError = null)
		{
			if (APIUser.IsLoggedInWithCredentials)
			{
				Dictionary<string, string> dictionary = BuildWebParameters();
				dictionary["shouldAddToAuthor"] = "true";
				if (mId != null)
				{
					if (APIUser.CurrentUser.id == authorId)
					{
						dictionary["id"] = id;
						ApiModel.SendPutRequest("worlds/" + id, dictionary, delegate
						{
							if (onSuccess != null)
							{
								onSuccess(this);
							}
						});
					}
					else
					{
						Logger.LogError("Only the blueprint's author can update this blueprint.");
					}
				}
				else
				{
					ApiModel.SendPostRequest("worlds", dictionary, delegate(Dictionary<string, object> successResponse)
					{
						if (successResponse.ContainsKey("id"))
						{
							mId = (string)successResponse["id"];
						}
						Logger.Log("Sending blueprint post request");
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					});
				}
			}
			else
			{
				Logger.Log("Must be logged in with account to create or edit a blueprint.");
			}
		}
	}
}
