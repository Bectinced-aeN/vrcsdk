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
			Active,
			None
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
			public enum AccessType
			{
				Public,
				FriendsOfGuests,
				FriendsOnly,
				InviteOnly,
				InvitePlus,
				Counter
			}

			public struct InstanceTag
			{
				public string name;

				public string data;

				public InstanceTag(string n = "", string d = null)
				{
					name = n;
					data = d;
				}
			}

			public class AccessDetail
			{
				public string[] tags
				{
					get;
					private set;
				}

				public string shortName
				{
					get;
					private set;
				}

				public string fullName
				{
					get;
					private set;
				}

				public AccessDetail(string[] inTags, string sName, string fName)
				{
					tags = inTags;
					shortName = sName;
					fullName = fName;
				}
			}

			private static Dictionary<AccessType, AccessDetail> accessDetails;

			public string idWithTags;

			public int count;

			public string idOnly => ParseIDFromIDWithTags(idWithTags);

			public string tagsOnly => ParseTagsFromIDWithTags(idWithTags);

			public bool isPublic => GetAccessType() == AccessType.Public;

			public WorldInstance(string _idWithTags, int _count)
			{
				idWithTags = _idWithTags;
				count = _count;
			}

			static WorldInstance()
			{
				accessDetails = new Dictionary<AccessType, AccessDetail>();
				accessDetails[AccessType.Public] = new AccessDetail(null, "public", "Public");
				accessDetails[AccessType.FriendsOfGuests] = new AccessDetail(new string[1]
				{
					"hidden"
				}, "friends+", "Friends of Guests");
				accessDetails[AccessType.FriendsOnly] = new AccessDetail(new string[1]
				{
					"friends"
				}, "friends", "Friends Only");
				accessDetails[AccessType.InviteOnly] = new AccessDetail(new string[1]
				{
					"private"
				}, "invite", "Invite Only");
				accessDetails[AccessType.InvitePlus] = new AccessDetail(new string[2]
				{
					"private",
					"canRequestInvite"
				}, "invite+", "Invite Plus");
				accessDetails[AccessType.Counter] = new AccessDetail(new string[1]
				{
					"pop"
				}, "popcount", "[Internal Use Only] Population Counter");
			}

			public static string ParseIDFromIDWithTags(string idWithTagsStr)
			{
				if (string.IsNullOrEmpty(idWithTagsStr))
				{
					return null;
				}
				string[] array = idWithTagsStr.Split('~');
				if (array == null || array.Length == 0)
				{
					return idWithTagsStr;
				}
				return array[0];
			}

			public static string ParseTagsFromIDWithTags(string idWithTagsStr)
			{
				if (string.IsNullOrEmpty(idWithTagsStr))
				{
					return null;
				}
				string[] array = idWithTagsStr.Split('~');
				if (array.Length > 1)
				{
					return "~" + array[1].Trim();
				}
				return null;
			}

			private List<InstanceTag> ParseTags(string idWithTags)
			{
				if (string.IsNullOrEmpty(idWithTags))
				{
					return null;
				}
				List<InstanceTag> list = new List<InstanceTag>();
				string[] array = idWithTags.Split('~');
				if (array == null || array.Length > 1)
				{
					for (int i = 1; i < array.Length; i++)
					{
						InstanceTag item;
						if (array[i].Contains('('))
						{
							string[] array2 = array[i].Split('(');
							string n = array2[0];
							string d = array2[1].TrimEnd(')');
							item = new InstanceTag(n, d);
						}
						else
						{
							item = new InstanceTag(array[i]);
						}
						list.Add(item);
					}
				}
				return list;
			}

			public AccessType GetAccessType()
			{
				List<InstanceTag> list = ParseTags(idWithTags);
				if (list != null)
				{
					if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InvitePlus].tags[0]) && list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InvitePlus].tags[1]))
					{
						return AccessType.InvitePlus;
					}
					if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InviteOnly].tags[0]))
					{
						return AccessType.InviteOnly;
					}
					if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.FriendsOnly].tags[0]))
					{
						return AccessType.FriendsOnly;
					}
					if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.FriendsOfGuests].tags[0]))
					{
						return AccessType.FriendsOfGuests;
					}
				}
				return AccessType.Public;
			}

			public static AccessDetail GetAccessDetail(AccessType access)
			{
				return accessDetails[access];
			}

			public static string BuildAccessTags(AccessType access, string userId)
			{
				string result = string.Empty;
				if (access == AccessType.Public)
				{
					return result;
				}
				AccessDetail accessDetail = GetAccessDetail(access);
				switch (access)
				{
				case AccessType.Counter:
					result = "~" + accessDetail.tags[0];
					break;
				case AccessType.FriendsOfGuests:
				case AccessType.FriendsOnly:
				case AccessType.InviteOnly:
					result = string.Format("~{0}({1})~nonce({2})", accessDetail.tags[0], userId, new string((from s in Enumerable.Repeat("ABCDEF0123456789", 64)
					select s[Random.Range(0, s.Length)]).ToArray()));
					break;
				case AccessType.InvitePlus:
					result = string.Format("~{0}({1})~{2}~nonce({3})", accessDetail.tags[0], userId, accessDetail.tags[1], new string((from s in Enumerable.Repeat("ABCDEF0123456789", 64)
					select s[Random.Range(0, s.Length)]).ToArray()));
					break;
				}
				return result;
			}

			public string GetInstanceCreator()
			{
				List<InstanceTag> list = ParseTags(idWithTags);
				if (list != null)
				{
					foreach (InstanceTag item in list)
					{
						InstanceTag current = item;
						if (current.name == accessDetails[AccessType.InviteOnly].tags[0] || current.name == accessDetails[AccessType.InvitePlus].tags[0] || current.name == accessDetails[AccessType.FriendsOnly].tags[0] || current.name == accessDetails[AccessType.FriendsOfGuests].tags[0])
						{
							return current.data;
						}
					}
				}
				return null;
			}

			public override string ToString()
			{
				return "[" + idWithTags + ", " + count + "]";
			}
		}

		public enum WorldData
		{
			Metadata
		}

		private static AssetVersion _VERSION = null;

		public static AssetVersion MIN_LOADABLE_VERSION = new AssetVersion("5.5.0f1", 0);

		private static AssetVersion DefaultAssetVersion = new AssetVersion("5.6.3p1", 0);

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

		public WorldInstance.AccessType currentInstanceAccess;

		public bool isCurated;

		private static Dictionary<string, ApiWorld> localWorlds = new Dictionary<string, ApiWorld>();

		public static AssetVersion VERSION
		{
			get
			{
				if (_VERSION == null)
				{
					_VERSION = new AssetVersion(Application.get_unityVersion(), 2);
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

		public bool isAdminApproved => isCurated || (mTags != null && mTags.Contains("admin_approved"));

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
			if (string.IsNullOrEmpty(mId))
			{
				Debug.LogError((object)("ApiWorld " + mName + " has an invalid ID"));
			}
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
					mWorldInstances = new List<WorldInstance>();
					mInstances = new Dictionary<string, int>();
					if (jsonObject.ContainsKey("instances"))
					{
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
					if (string.IsNullOrEmpty(mId))
					{
						Debug.LogError((object)("ApiWorld " + mName + " has an invalid ID"));
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

		public void Save(bool overwrite, Action<ApiModel> onSuccess = null, Action<string> onError = null)
		{
			if (!APIUser.IsLoggedInWithCredentials)
			{
				Logger.Log("Must be logged in with account to create or edit a blueprint.");
			}
			else if (APIUser.CurrentUser.id != authorId)
			{
				Logger.LogError("Only the blueprint's author can update this blueprint.");
			}
			else
			{
				Dictionary<string, string> dictionary = BuildWebParameters();
				dictionary["id"] = id;
				if (overwrite)
				{
					ApiModel.SendPutRequest("worlds/" + id, dictionary, delegate
					{
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					}, delegate(string error)
					{
						if (onError != null)
						{
							onError(error);
						}
					});
				}
				else
				{
					ApiModel.SendPostRequest("worlds", dictionary, delegate
					{
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					}, delegate(string error)
					{
						if (onError != null)
						{
							onError(error);
						}
					});
				}
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
					ApiWorld apiWorld = new ApiWorld();
					apiWorld.Init(obj);
					if (successCallback != null)
					{
						successCallback(apiWorld);
					}
				}, delegate(string message)
				{
					errorCallback(message);
				}, needsAPIKey: true, Application.get_isEditor());
			}
		}

		public static void FetchList(Action<List<ApiWorld>> successCallback, Action<string> errorCallback = null, SortHeading heading = SortHeading.Featured, SortOwnership owner = SortOwnership.Any, SortOrder order = SortOrder.Descending, int offset = 0, int count = 10, string search = "", string[] tags = null, string[] excludeTags = null, string userId = "", ReleaseStatus releaseStatus = ReleaseStatus.Public, bool compatibleVersionsOnly = true, bool bypassCache = false)
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
			if (excludeTags != null && excludeTags.Length > 0)
			{
				dictionary.Add("notag", string.Join(",", excludeTags));
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
			float cacheLifetime = (heading != SortHeading.Active && heading != SortHeading.Trending) ? 180f : 60f;
			if (bypassCache)
			{
				cacheLifetime = 0f;
			}
			ApiModel.SendRequest(endpoint, HTTPMethods.Get, dictionary, delegate(List<object> objs)
			{
				List<ApiWorld> list = new List<ApiWorld>();
				if (objs != null)
				{
					foreach (object obj in objs)
					{
						Dictionary<string, object> jsonObject = obj as Dictionary<string, object>;
						ApiWorld apiWorld = new ApiWorld();
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
			}, needsAPIKey: true, releaseStatus != ReleaseStatus.Public, cacheLifetime);
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
			}, needsAPIKey: true, authenticationRequired: true, -1f);
		}

		public static void AddLocal(ApiWorld world)
		{
			localWorlds.Add(world.id, world);
		}

		private IEnumerable<WorldInstance> GetViableInstances(string forUserId, List<string> instanceIdsToIgnore = null, bool excludePublicInstances = false, bool includePublicInstancesOnly = false)
		{
			if (instanceIdsToIgnore == null)
			{
				instanceIdsToIgnore = new List<string>();
			}
			if (mWorldInstances != null)
			{
				List<WorldInstance> list = new List<WorldInstance>();
				for (int i = 0; i < mWorldInstances.Count; i++)
				{
					WorldInstance worldInstance = mWorldInstances[i];
					int num = Mathf.Min(capacity, Mathf.Max(6, Mathf.FloorToInt((float)capacity * 0.66f)));
					if (worldInstance.count != 0 && worldInstance.count < num && !instanceIdsToIgnore.Contains(worldInstance.idWithTags) && !instanceIdsToIgnore.Contains(worldInstance.idOnly) && worldInstance.GetAccessType() != WorldInstance.AccessType.InviteOnly && !includePublicInstancesOnly && (worldInstance.GetAccessType() != WorldInstance.AccessType.FriendsOnly || APIUser.IsFriendsWith(worldInstance.GetInstanceCreator())) && !includePublicInstancesOnly && (worldInstance.GetAccessType() != WorldInstance.AccessType.FriendsOfGuests || (worldInstance.GetInstanceCreator() != null && !(worldInstance.GetInstanceCreator() != forUserId))) && (!excludePublicInstances || (worldInstance.GetAccessType() != 0 && worldInstance.GetAccessType() != WorldInstance.AccessType.FriendsOfGuests)))
					{
						list.Add(worldInstance);
					}
				}
				return from instance in list
				orderby instance.count descending
				select instance;
			}
			return new WorldInstance[0];
		}

		private WorldInstance GetBestInstance(WorldInstance[] viableInstances)
		{
			WorldInstance worldInstance = null;
			worldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() == WorldInstance.AccessType.Public && inst.count >= 3
			select inst).ToArray());
			if (worldInstance != null)
			{
				return worldInstance;
			}
			worldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() == WorldInstance.AccessType.Public
			select inst).ToArray());
			if (worldInstance != null)
			{
				return worldInstance;
			}
			worldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() != 0 && inst.count >= 3
			select inst).ToArray());
			if (worldInstance != null)
			{
				return worldInstance;
			}
			worldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() != WorldInstance.AccessType.Public
			select inst).ToArray());
			if (worldInstance != null)
			{
				return worldInstance;
			}
			return null;
		}

		private WorldInstance SelectRandomWorldInstanceWeighted(WorldInstance[] instanceList)
		{
			if (instanceList.Length > 0)
			{
				int num = 0;
				int[] array = new int[instanceList.Length];
				for (int i = 0; i < instanceList.Length; i++)
				{
					int num2 = instanceList[i].count;
					if (num2 < 1)
					{
						num2 = 1;
					}
					array[i] = num2;
					num += num2;
				}
				if (num > 0)
				{
					int num3 = Random.Range(0, num);
					for (int j = 0; j < array.Length; j++)
					{
						num3 -= array[j];
						if (num3 < 0)
						{
							return instanceList[j];
						}
					}
				}
			}
			return null;
		}

		public WorldInstance GetBestInstance(string forUserId, List<string> instanceIdsToIgnore = null, bool excludePublicInstances = false, bool includePublicInstancesOnly = false)
		{
			if (string.IsNullOrEmpty(forUserId))
			{
				forUserId = APIUser.CurrentUser.id;
			}
			WorldInstance[] viableInstances = GetViableInstances(forUserId, instanceIdsToIgnore, excludePublicInstances, includePublicInstancesOnly).ToArray();
			WorldInstance bestInstance = GetBestInstance(viableInstances);
			return (bestInstance != null) ? bestInstance : GetNewInstance(forUserId, excludePublicInstances, includePublicInstancesOnly);
		}

		public WorldInstance GetNewInstance(string forUserId, bool excludePublicInstances, bool includePublicInstancesOnly = false)
		{
			if (string.IsNullOrEmpty(forUserId))
			{
				forUserId = APIUser.CurrentUser.id;
			}
			string tags = string.Empty;
			if (mReleaseStatus == "private")
			{
				tags = WorldInstance.BuildAccessTags(WorldInstance.AccessType.InviteOnly, forUserId);
			}
			else if (excludePublicInstances || !isAdminApproved)
			{
				tags = WorldInstance.BuildAccessTags((!includePublicInstancesOnly) ? WorldInstance.AccessType.FriendsOnly : WorldInstance.AccessType.InviteOnly, forUserId);
			}
			return GetNewInstance(tags);
		}

		public WorldInstance GetNewInstance(string tags = "")
		{
			IL_0000:
			int instanceIndex;
			while (true)
			{
				instanceIndex = Random.Range(1, 99999);
				if (mWorldInstances == null || !mWorldInstances.Any((WorldInstance wi) => wi.idOnly == instanceIndex.ToString()))
				{
					break;
				}
			}
			return new WorldInstance(instanceIndex.ToString() + tags, 0);
			IL_005b:
			goto IL_0000;
		}

		public void SaveAndAddToUser(bool overwrite, Action<ApiModel> onSuccess = null, Action<string> onError = null)
		{
			if (!APIUser.IsLoggedInWithCredentials)
			{
				Logger.Log("Must be logged in with account to create or edit a blueprint.");
			}
			else if (APIUser.CurrentUser.id != authorId)
			{
				Logger.LogError("Only the blueprint's author can update this blueprint.");
			}
			else
			{
				Dictionary<string, string> dictionary = BuildWebParameters();
				dictionary["shouldAddToAuthor"] = "true";
				dictionary["id"] = id;
				if (overwrite)
				{
					ApiModel.SendPutRequest("worlds/" + id, dictionary, delegate
					{
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					}, delegate(string error)
					{
						if (onError != null)
						{
							onError(error);
						}
					});
				}
				else
				{
					ApiModel.SendPostRequest("worlds", dictionary, delegate
					{
						if (onSuccess != null)
						{
							onSuccess(this);
						}
					}, delegate(string error)
					{
						if (onError != null)
						{
							onError(error);
						}
					});
				}
			}
		}
	}
}
