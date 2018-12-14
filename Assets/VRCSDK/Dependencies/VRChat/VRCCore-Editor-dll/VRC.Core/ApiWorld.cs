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

		public class WorldInstance
		{
			public struct Tag
			{
				public string Value;

				public string Data;
			}

			public enum Type
			{
				Public,
				Friends,
				Private,
				PrivatePopCounter
			}

			public string idWithTags;

			public int count;

			public string idOnly
			{
				get
				{
					string[] array = idWithTags.Split('~');
					if (array == null)
					{
						return idWithTags;
					}
					return array[0];
				}
			}

			public bool isPublic => GetWorldType() == Type.Public;

			public WorldInstance(string _idWithTags, int _count)
			{
				idWithTags = _idWithTags;
				count = _count;
			}

			private List<Tag> ParseTags(string id)
			{
				List<Tag> list = null;
				string[] array = idWithTags.Split('~');
				if (array == null || array.Length > 1)
				{
					list = new List<Tag>();
					for (int i = 1; i < array.Length; i++)
					{
						Tag item = default(Tag);
						if (array[i].Contains('('))
						{
							string[] array2 = array[i].Split('(');
							string value = array2[0];
							string data = array2[1].TrimEnd(')');
							item.Value = value;
							item.Data = data;
						}
						else
						{
							item.Value = array[i];
							item.Data = null;
						}
						list.Add(item);
					}
				}
				return list;
			}

			public Type GetWorldType()
			{
				List<Tag> list = ParseTags(idWithTags);
				if (list != null)
				{
					foreach (Tag item in list)
					{
						Tag current = item;
						if (current.Value == "private")
						{
							return Type.Private;
						}
						if (current.Value == "friends")
						{
							return Type.Friends;
						}
						if (current.Value == "privpop")
						{
							return Type.PrivatePopCounter;
						}
					}
				}
				return Type.Public;
			}

			public string GetInstanceCreator()
			{
				List<Tag> list = ParseTags(idWithTags);
				if (list != null)
				{
					foreach (Tag item in list)
					{
						Tag current = item;
						if (current.Value == "private" || current.Value == "friends")
						{
							return current.Data;
						}
					}
				}
				return null;
			}
		}

		public enum WorldData
		{
			Metadata
		}

		private static AssetVersion _VERSION = null;

		public static AssetVersion MIN_LOADABLE_VERSION = new AssetVersion("5.5.0f1", 0);

		private static AssetVersion DefaultAssetVersion = new AssetVersion("5.6.1p4", 0);

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

		private AssetVersion mAssetVersion;

		private string mPlatform;

		private Dictionary<string, int> mInstances;

		private List<WorldInstance> mWorldInstances;

		private string mThumbnailImageUrl;

		public string currentInstanceIdWithTags;

		public string currentInstanceIdOnly;

		public string currentInstanceType;

		public bool isCurated;

		private static Dictionary<string, ApiWorld> localWorlds = new Dictionary<string, ApiWorld>();

		public static AssetVersion VERSION
		{
			get
			{
				if (_VERSION == null)
				{
					_VERSION = new AssetVersion(Application.get_unityVersion(), 1);
				}
				return _VERSION;
			}
		}

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

		public AssetVersion assetVersion
		{
			get
			{
				return mAssetVersion;
			}
			set
			{
				mAssetVersion = value;
			}
		}

		public string platform
		{
			get
			{
				return mPlatform;
			}
			set
			{
				mPlatform = value;
			}
		}

		public Dictionary<string, int> instances => mInstances;

		public List<WorldInstance> worldInstances => mWorldInstances;

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
			mWorldInstances = new List<WorldInstance>();
			UpdateVersionAndPlatform();
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
			mPluginUrl = string.Empty;
			mVersion = -1;
			mReleaseStatus = "private";
			mCapacity = 32;
			mOccupants = 0;
			mWorldInstances = new List<WorldInstance>();
			UpdateVersionAndPlatform();
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
					Debug.LogError((object)("Could not initialize blueprint due to insufficient json parameters, missing: " + string.Join(",", (from s in source
					where !jsonObject.ContainsKey(s)
					select s).ToArray())));
					Init();
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
					if (jsonObject.ContainsKey("unityPackageUrl"))
					{
						mUnityPackageUrl = (jsonObject["unityPackageUrl"] as string);
					}
					if (jsonObject.ContainsKey("instances"))
					{
						mWorldInstances = new List<WorldInstance>();
						mInstances = new Dictionary<string, int>();
						List<object> list2 = jsonObject["instances"] as List<object>;
						foreach (object item in list2)
						{
							List<object> list3 = item as List<object>;
							string text = list3[0].ToString();
							int num = Convert.ToInt32(list3[1].ToString());
							mInstances.Add(text, num);
							mWorldInstances.Add(new WorldInstance(text, num));
						}
					}
					mReleaseStatus = (jsonObject["releaseStatus"] as string);
					mCapacity = Convert.ToInt16(jsonObject["capacity"]);
					if (jsonObject.ContainsKey("occupants"))
					{
						mOccupants = Convert.ToInt16(jsonObject["occupants"]);
					}
					string unityVersion = DefaultAssetVersion.UnityVersion;
					if (jsonObject.ContainsKey("unityVersion"))
					{
						unityVersion = (jsonObject["unityVersion"] as string);
					}
					int result = DefaultAssetVersion.ApiVersion;
					if (jsonObject.ContainsKey("assetVersion"))
					{
						string text2 = jsonObject["assetVersion"] as string;
						if (string.IsNullOrEmpty(text2) || !int.TryParse(text2, out result))
						{
							Debug.LogError((object)("Invalid assetVersion string: " + text2));
						}
					}
					mAssetVersion = new AssetVersion(unityVersion, result);
					mPlatform = "standalonewindows";
					if (jsonObject.ContainsKey("platform"))
					{
						mPlatform = (jsonObject["platform"] as string);
					}
				}
			}
		}

		public void UpdateVersionAndPlatform(string platform = null)
		{
			mAssetVersion = VERSION;
			mPlatform = ((platform != null) ? platform : ApiModel.GetAssetPlatformString());
		}

		protected override Dictionary<string, string> BuildWebParameters()
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
			dictionary["unityVersion"] = mAssetVersion.UnityVersion.ToString();
			dictionary["assetVersion"] = mAssetVersion.ApiVersion.ToString();
			dictionary["platform"] = mPlatform;
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

		public static void FetchData(string id, WorldData data, Action<Dictionary<string, object>> successCallback, Action<string> errorCallback)
		{
			if (id == null)
			{
				errorCallback("APIWorld.FetchData called with null id.");
			}
			else
			{
				string str = null;
				if (data == WorldData.Metadata)
				{
					str = "/metadata";
				}
				Dictionary<string, string> requestParams = new Dictionary<string, string>();
				ApiModel.SendRequest("worlds/" + id + str, HTTPMethods.Get, requestParams, delegate(Dictionary<string, object> obj)
				{
					if (successCallback != null)
					{
						successCallback(obj);
					}
				}, delegate(string message)
				{
					errorCallback(message);
				});
			}
		}

		public static void Fetch(string id, Action<ApiWorld> successCallback, Action<string> errorCallback)
		{
			Fetch(id, compatibleVersionsOnly: true, successCallback, errorCallback);
		}

		public static void Fetch(string id, bool compatibleVersionsOnly, Action<ApiWorld> successCallback, Action<string> errorCallback)
		{
			if (id == null)
			{
				errorCallback("APIWorld.Fetch called with null id.");
			}
			else if (localWorlds.ContainsKey(id))
			{
				successCallback(localWorlds[id]);
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (compatibleVersionsOnly)
				{
					dictionary.Add("maxUnityVersion", VERSION.UnityVersion.ToString());
					dictionary.Add("minUnityVersion", MIN_LOADABLE_VERSION.UnityVersion.ToString());
					dictionary.Add("maxAssetVersion", VERSION.ApiVersion.ToString());
					dictionary.Add("minAssetVersion", MIN_LOADABLE_VERSION.ApiVersion.ToString());
				}
				dictionary.Add("platform", ApiModel.GetAssetPlatformString());
				ApiModel.SendRequest("worlds/" + id, HTTPMethods.Get, dictionary, delegate(Dictionary<string, object> obj)
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

		public static void FetchList(Action<List<ApiWorld>> successCallback, Action<string> errorCallback = null, SortHeading heading = SortHeading.Featured, SortOwnership owner = SortOwnership.Any, SortOrder order = SortOrder.Descending, int offset = 0, int count = 10, string search = "", string[] tags = null, string userId = "", ReleaseStatus releaseStatus = ReleaseStatus.Public, bool compatibleVersionsOnly = true)
		{
			string endpoint = "worlds";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			switch (heading)
			{
			case SortHeading.Featured:
				dictionary.Add("sort", "order");
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
			if (compatibleVersionsOnly)
			{
				dictionary.Add("maxUnityVersion", VERSION.UnityVersion.ToString());
				dictionary.Add("minUnityVersion", MIN_LOADABLE_VERSION.UnityVersion.ToString());
				dictionary.Add("maxAssetVersion", VERSION.ApiVersion.ToString());
				dictionary.Add("minAssetVersion", MIN_LOADABLE_VERSION.ApiVersion.ToString());
			}
			dictionary.Add("platform", ApiModel.GetAssetPlatformString());
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

		public WorldInstance GetBestInstance(List<string> instanceIdsToIgnore = null)
		{
			if (instanceIdsToIgnore == null)
			{
				instanceIdsToIgnore = new List<string>();
			}
			string tags = string.Empty;
			if (mReleaseStatus == "private")
			{
				tags = "~private(" + mAuthorId + ")";
			}
			WorldInstance newInstance = GetNewInstance(tags);
			if (mWorldInstances != null)
			{
				foreach (WorldInstance mWorldInstance in mWorldInstances)
				{
					if (mWorldInstance.count < capacity && !instanceIdsToIgnore.Contains(mWorldInstance.idWithTags))
					{
						return mWorldInstance;
					}
				}
				return newInstance;
			}
			return newInstance;
		}

		public WorldInstance GetNewInstance(string tags = "")
		{
			WorldInstance result = new WorldInstance("1" + tags, 0);
			if (mWorldInstances != null)
			{
				int i;
				for (i = 1; i < 10000; i++)
				{
					if (!mWorldInstances.Any((WorldInstance wi) => wi.idOnly == i.ToString()))
					{
						result = new WorldInstance(i.ToString() + tags, 0);
						break;
					}
				}
			}
			return result;
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
