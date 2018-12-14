using System;
using System.Collections.Generic;
using System.ComponentModel;
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
			Recent,
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

		public enum WorldData
		{
			Metadata
		}

		public const float ListCacheTime = 180f;

		public const float SingleRecordCacheTime = 15f;

		private static AssetVersion _VERSION = null;

		public static AssetVersion MIN_LOADABLE_VERSION = new AssetVersion("5.5.0f1", 0);

		private static Dictionary<string, ApiWorld> localWorlds = new Dictionary<string, ApiWorld>();

		private List<ApiWorldInstance> mWorldInstances = new List<ApiWorldInstance>();

		public string currentInstanceIdWithTags;

		public string currentInstanceIdOnly;

		public string currentInstanceType;

		public ApiWorldInstance.AccessType currentInstanceAccess;

		public static AssetVersion VERSION
		{
			get
			{
				if (_VERSION == null)
				{
					_VERSION = new AssetVersion(Application.get_unityVersion(), 3);
				}
				return _VERSION;
			}
		}

		[ApiField]
		public string name
		{
			get;
			set;
		}

		[ApiField]
		public string imageUrl
		{
			get;
			set;
		}

		[ApiField]
		public string thumbnailImageUrl
		{
			get;
			set;
		}

		[ApiField]
		public string authorName
		{
			get;
			set;
		}

		[ApiField]
		public string releaseStatus
		{
			get;
			set;
		}

		[ApiField]
		public int capacity
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public int occupants
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string authorId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime createdAt
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string assetUrl
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string description
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public List<string> tags
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string pluginUrl
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string unityPackageUrl
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public int version
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string unityVersion
		{
			get;
			set;
		}

		[ApiField(Required = false, Name = "assetVersion")]
		public int apiVersion
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		[DefaultValue("standalonewindows")]
		public string platform
		{
			get;
			set;
		}

		public List<ApiWorldInstance> worldInstances => mWorldInstances;

		[ApiField(Required = false)]
		public Dictionary<string, int> instances
		{
			get;
			set;
		}

		[ApiField(Required = false, Name = "namespace", IsAdminWritableOnly = true)]
		public string scriptNamespace
		{
			get;
			set;
		}

		[ApiField(Required = false, IsApiWritableOnly = true)]
		public List<string> unityPackages
		{
			get;
			set;
		}

		public bool isAdminApproved => isCurated || (tags != null && tags.Contains("admin_approved"));

		[ApiField(Required = false, IsApiWritableOnly = true)]
		public bool unityPackageUpdated
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public int totalLikes
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public int totalVisits
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool isSecure
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool isLockdown
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string organization
		{
			get;
			set;
		}

		[ApiField(Required = false, IsApiWritableOnly = true)]
		public List<APIUser> users
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool shouldAddToAuthor
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string instanceId
		{
			get;
			set;
		}

		public AssetVersion assetVersion
		{
			get
			{
				return new AssetVersion(unityVersion, apiVersion);
			}
			set
			{
				unityVersion = value.UnityVersion;
				apiVersion = value.ApiVersion;
			}
		}

		[ApiField(Required = false, Name = "featured", IsAdminWritableOnly = true)]
		public bool isCurated
		{
			get;
			set;
		}

		public ApiWorld()
			: base("worlds")
		{
			users = new List<APIUser>();
			tags = new List<string>();
			unityPackages = new List<string>();
			shouldAddToAuthor = false;
			UpdateVersionAndPlatform();
		}

		public override bool ShouldCache()
		{
			return base.ShouldCache() && !string.IsNullOrEmpty(assetUrl);
		}

		public override float GetLifeSpan()
		{
			return 15f;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "shouldAddToAuthor":
				return false;
			case "instances":
				return false;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "instances":
			{
				List<object> source = data as List<object>;
				instances = (from kvp in source
				select new KeyValuePair<string, int>((kvp as List<object>)[0].ToString(), Convert.ToInt32((kvp as List<object>)[1].ToString()))).ToDictionary((KeyValuePair<string, int> pair) => pair.Key, (KeyValuePair<string, int> pair) => pair.Value);
				mWorldInstances = (from kvp in instances
				select new ApiWorldInstance(this, kvp.Key, kvp.Value)).ToList();
				return true;
			}
			default:
				return base.WriteField(fieldName, data);
			}
		}

		public void FetchData(WorldData data, Action<Dictionary<string, object>> successCallback, Action<string> errorCallback)
		{
			if (base.id == null)
			{
				errorCallback("APIWorld.FetchData called with null id.");
			}
			else
			{
				string text = null;
				if (data != 0)
				{
				}
				text = "/metadata";
				ApiDictContainer apiDictContainer = new ApiDictContainer();
				apiDictContainer.OnSuccess = delegate(ApiContainer c)
				{
					if (successCallback != null)
					{
						successCallback((c as ApiDictContainer).ResponseDictionary);
					}
				};
				apiDictContainer.OnError = delegate(ApiContainer c)
				{
					if (errorCallback != null)
					{
						errorCallback(c.Error);
					}
				};
				ApiDictContainer responseContainer = apiDictContainer;
				API.SendGetRequest("worlds/" + base.id + text, responseContainer, null, disableCache: true);
			}
		}

		public override void Save(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			if (base.id != null && APIUser.CurrentUser.id != authorId)
			{
				onFailure(new ApiContainer
				{
					Error = "Only the blueprint's author can update this blueprint."
				});
			}
			else
			{
				UpdateVersionAndPlatform();
				base.Save(onSuccess, onFailure);
			}
		}

		public override void Get(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null, bool disableCache = false)
		{
			Fetch(string.Empty, compatibleVersionsOnly: true, onSuccess, onFailure, parameters);
		}

		public void Fetch(string instanceID = null, bool compatibleVersionsOnly = true, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null)
		{
			if (string.IsNullOrEmpty(base.id))
			{
				onFailure(new ApiContainer
				{
					Error = "APIWorld.Fetch called with null id."
				});
			}
			else if (localWorlds.ContainsKey(base.id))
			{
				ApiModelContainer<ApiWorld> obj = new ApiModelContainer<ApiWorld>(localWorlds[base.id]);
				onSuccess(obj);
			}
			else
			{
				if (parameters == null)
				{
					parameters = new Dictionary<string, object>();
				}
				if (compatibleVersionsOnly)
				{
					parameters["maxUnityVersion"] = VERSION.UnityVersion;
					parameters["minUnityVersion"] = MIN_LOADABLE_VERSION.UnityVersion;
					parameters["maxAssetVersion"] = VERSION.ApiVersion;
					parameters["minAssetVersion"] = MIN_LOADABLE_VERSION.ApiVersion;
				}
				parameters["platform"] = API.GetAssetPlatformString();
				if (!string.IsNullOrEmpty(instanceID))
				{
					ApiDictContainer apiDictContainer = new ApiDictContainer("users");
					apiDictContainer.OnSuccess = delegate(ApiContainer c)
					{
						string error = null;
						List<object> json = (c as ApiDictContainer).ResponseDictionary["users"] as List<object>;
						List<APIUser> list = API.ConvertJsonListToModelList<APIUser>(json, ref error, c.DataTimestamp);
						if (list == null)
						{
							c.Error = error;
							onFailure(c);
						}
						else
						{
							if (instances == null)
							{
								instances = new Dictionary<string, int>();
							}
							if (!mWorldInstances.Any((ApiWorldInstance w) => w.idWithTags == instanceID))
							{
								mWorldInstances.Add(new ApiWorldInstance(this, instanceID, list.Count));
								instances.Add(instanceID, list.Count);
							}
							ApiWorldInstance apiWorldInstance = worldInstances.First((ApiWorldInstance w) => w.idWithTags == instanceID);
							apiWorldInstance.count = list.Count;
							apiWorldInstance.users = list;
							onSuccess(c);
						}
					};
					apiDictContainer.OnError = onFailure;
					ApiDictContainer responseContainer = apiDictContainer;
					API.SendRequest("worlds/" + base.id + "/" + instanceID, HTTPMethods.Get, responseContainer, parameters, needsAPIKey: true, authenticationRequired: true, disableCache: true);
				}
				else
				{
					ApiModelContainer<ApiWorld> apiModelContainer = new ApiModelContainer<ApiWorld>(this);
					apiModelContainer.OnSuccess = onSuccess;
					apiModelContainer.OnError = onFailure;
					ApiModelContainer<ApiWorld> responseContainer2 = apiModelContainer;
					API.SendRequest("worlds/" + base.id, HTTPMethods.Get, responseContainer2, parameters, needsAPIKey: true, authenticationRequired: true, disableCache: true);
				}
			}
		}

		public static void FetchList(Action<List<ApiWorld>> successCallback, Action<string> errorCallback = null, SortHeading heading = SortHeading.Featured, SortOwnership owner = SortOwnership.Any, SortOrder order = SortOrder.Descending, int offset = 0, int count = 10, string search = "", string[] tags = null, string[] excludeTags = null, string userId = "", ReleaseStatus releaseStatus = ReleaseStatus.Public, bool compatibleVersionsOnly = true, bool disableCache = false)
		{
			string endpoint = "worlds";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
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
			case SortHeading.Recent:
				endpoint = "worlds/recent";
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
			dictionary.Add("n", count);
			switch (order)
			{
			case SortOrder.Ascending:
				dictionary.Add("order", "ascending");
				break;
			case SortOrder.Descending:
				dictionary.Add("order", "descending");
				break;
			}
			dictionary.Add("offset", offset);
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
				dictionary.Add("maxUnityVersion", VERSION.UnityVersion);
				dictionary.Add("minUnityVersion", MIN_LOADABLE_VERSION.UnityVersion);
				dictionary.Add("maxAssetVersion", VERSION.ApiVersion);
				dictionary.Add("minAssetVersion", MIN_LOADABLE_VERSION.ApiVersion);
			}
			dictionary.Add("platform", API.GetAssetPlatformString());
			ApiModelListContainer<ApiWorld> apiModelListContainer = new ApiModelListContainer<ApiWorld>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiWorld>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Debug.LogError((object)("Could not fetch worlds, with error - " + c.Error));
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiWorld> responseContainer = apiModelListContainer;
			API.SendRequest(endpoint, HTTPMethods.Get, responseContainer, dictionary, needsAPIKey: true, releaseStatus != ReleaseStatus.Public, disableCache, 180f);
		}

		public static void AddLocal(ApiWorld world)
		{
			localWorlds.Add(world.id, world);
		}

		private IEnumerable<ApiWorldInstance> GetViableInstances(string forUserId, List<string> instanceIdsToIgnore = null, bool excludePublicInstances = false, bool includePublicInstancesOnly = false)
		{
			if (instanceIdsToIgnore == null)
			{
				instanceIdsToIgnore = new List<string>();
			}
			if (worldInstances != null)
			{
				List<ApiWorldInstance> list = new List<ApiWorldInstance>();
				for (int i = 0; i < worldInstances.Count; i++)
				{
					ApiWorldInstance apiWorldInstance = worldInstances[i];
					int num = Mathf.Min(capacity, Mathf.Max(6, Mathf.FloorToInt((float)capacity * 0.66f)));
					ApiWorldInstance.AccessType accessType = apiWorldInstance.GetAccessType();
					string instanceCreator = apiWorldInstance.GetInstanceCreator();
					bool flag = accessType == ApiWorldInstance.AccessType.Public || accessType == ApiWorldInstance.AccessType.FriendsOfGuests;
					if (apiWorldInstance.count < capacity && apiWorldInstance.count < num && !instanceIdsToIgnore.Contains(apiWorldInstance.idWithTags) && !instanceIdsToIgnore.Contains(apiWorldInstance.idOnly) && accessType != ApiWorldInstance.AccessType.InviteOnly && accessType != ApiWorldInstance.AccessType.InvitePlus && (flag || !includePublicInstancesOnly) && (!excludePublicInstances || !flag) && (accessType != ApiWorldInstance.AccessType.FriendsOnly || APIUser.IsFriendsWith(instanceCreator)) && (accessType != ApiWorldInstance.AccessType.FriendsOfGuests || (instanceCreator != null && !(instanceCreator != forUserId))))
					{
						list.Add(apiWorldInstance);
					}
				}
				return from instance in list
				orderby instance.count descending
				select instance;
			}
			return new ApiWorldInstance[0];
		}

		private ApiWorldInstance GetBestInstance(ApiWorldInstance[] viableInstances)
		{
			ApiWorldInstance apiWorldInstance = null;
			apiWorldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() == ApiWorldInstance.AccessType.Public && inst.count >= 3
			select inst).ToArray());
			if (apiWorldInstance != null)
			{
				return apiWorldInstance;
			}
			apiWorldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() == ApiWorldInstance.AccessType.Public
			select inst).ToArray());
			if (apiWorldInstance != null)
			{
				return apiWorldInstance;
			}
			apiWorldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() != 0 && inst.count >= 3
			select inst).ToArray());
			if (apiWorldInstance != null)
			{
				return apiWorldInstance;
			}
			apiWorldInstance = SelectRandomWorldInstanceWeighted((from inst in viableInstances
			where inst.GetAccessType() != ApiWorldInstance.AccessType.Public
			select inst).ToArray());
			if (apiWorldInstance != null)
			{
				return apiWorldInstance;
			}
			return null;
		}

		private ApiWorldInstance SelectRandomWorldInstanceWeighted(ApiWorldInstance[] instanceList)
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

		public ApiWorldInstance GetBestInstance(string forUserId, List<string> instanceIdsToIgnore = null, bool excludePublicInstances = false, bool includePublicInstancesOnly = false)
		{
			if (string.IsNullOrEmpty(forUserId))
			{
				forUserId = APIUser.CurrentUser.id;
			}
			ApiWorldInstance[] viableInstances = GetViableInstances(forUserId, instanceIdsToIgnore, excludePublicInstances, includePublicInstancesOnly).ToArray();
			ApiWorldInstance bestInstance = GetBestInstance(viableInstances);
			return (bestInstance != null) ? bestInstance : GetNewInstance(forUserId, excludePublicInstances, includePublicInstancesOnly);
		}

		public ApiWorldInstance GetNewInstance(string forUserId, bool excludePublicInstances, bool includePublicInstancesOnly = false)
		{
			if (string.IsNullOrEmpty(forUserId))
			{
				forUserId = APIUser.CurrentUser.id;
			}
			string tags = string.Empty;
			if (releaseStatus == "private")
			{
				tags = ApiWorldInstance.BuildAccessTags(ApiWorldInstance.AccessType.InviteOnly, forUserId);
			}
			else if (excludePublicInstances)
			{
				tags = ApiWorldInstance.BuildAccessTags((!includePublicInstancesOnly) ? ApiWorldInstance.AccessType.FriendsOnly : ApiWorldInstance.AccessType.InviteOnly, forUserId);
			}
			return GetNewInstance(tags);
		}

		public ApiWorldInstance GetNewInstance(string tags = "")
		{
			IL_0000:
			int instanceIndex;
			while (true)
			{
				instanceIndex = Random.Range(1, 99999);
				if (worldInstances == null || !worldInstances.Any((ApiWorldInstance wi) => wi.idOnly == instanceIndex.ToString()))
				{
					break;
				}
			}
			ApiWorldInstance apiWorldInstance = new ApiWorldInstance(this, instanceIndex.ToString() + tags, 0);
			worldInstances.Add(apiWorldInstance);
			return apiWorldInstance;
			IL_006a:
			goto IL_0000;
		}

		public void FetchInstance(string instanceId, Action<ApiWorldInstance> success, Action<string> error = null)
		{
			for (int i = 0; i < mWorldInstances.Count; i++)
			{
				if (mWorldInstances[i].idWithTags == instanceId)
				{
					FetchInstance(i, success, error);
					return;
				}
			}
			mWorldInstances.Add(new ApiWorldInstance(this, instanceId, 0));
			FetchInstance(mWorldInstances.Count - 1, success, error);
		}

		public void FetchInstance(int index, Action<ApiWorldInstance> success, Action<string> error = null)
		{
			if (index < 0 || index >= mWorldInstances.Count)
			{
				if (error != null)
				{
					error("Instance index out of range.");
				}
			}
			else
			{
				string idWithTags = mWorldInstances[index].idWithTags;
				ApiDictContainer apiDictContainer = new ApiDictContainer();
				apiDictContainer.OnSuccess = delegate(ApiContainer c)
				{
					Dictionary<string, object> responseDictionary = (c as ApiDictContainer).ResponseDictionary;
					mWorldInstances[index] = new ApiWorldInstance(this, responseDictionary, c.DataTimestamp);
					if (success != null)
					{
						success(mWorldInstances[index]);
					}
				};
				apiDictContainer.OnError = delegate(ApiContainer c)
				{
					if (error != null)
					{
						error(c.Error);
					}
				};
				ApiDictContainer responseContainer = apiDictContainer;
				API.SendGetRequest("worlds/" + base.id + "/" + idWithTags, responseContainer, null, disableCache: true);
			}
		}

		public void PutInstance(ApiWorldInstance inst, Action success, Action<string> error = null)
		{
			string idWithTags = inst.idWithTags;
			int num = -1;
			for (int i = 0; i < mWorldInstances.Count; i++)
			{
				if (mWorldInstances[i].idWithTags == idWithTags)
				{
					mWorldInstances[i] = inst;
				}
			}
			bool newInstance = num == -1;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (newInstance)
			{
				dictionary["id"] = inst.idWithTags;
				ApiWorldInstance.AccessType accessType = inst.GetAccessType();
				if (accessType == ApiWorldInstance.AccessType.InviteOnly)
				{
					dictionary["private"] = inst.instanceCreator;
				}
				else
				{
					dictionary["private"] = "false";
				}
				if (accessType == ApiWorldInstance.AccessType.FriendsOnly)
				{
					dictionary["friends"] = inst.instanceCreator;
				}
				else
				{
					dictionary["friends"] = "false";
				}
			}
			dictionary["name"] = inst.instanceName;
			string tagString = inst.GetTagString();
			if (tagString != null)
			{
				dictionary["tags"] = tagString;
			}
			ApiDictContainer apiDictContainer = new ApiDictContainer();
			apiDictContainer.OnSuccess = delegate(ApiContainer c)
			{
				Dictionary<string, object> responseDictionary = (c as ApiDictContainer).ResponseDictionary;
				if (newInstance && responseDictionary != null)
				{
					mWorldInstances.Add(new ApiWorldInstance(this, responseDictionary, c.DataTimestamp));
				}
				if (success != null)
				{
					success();
				}
			};
			apiDictContainer.OnError = delegate(ApiContainer c)
			{
				if (error != null)
				{
					error(c.Error);
				}
			};
			ApiDictContainer responseContainer = apiDictContainer;
			API.SendPutRequest("worlds/" + base.id + "/" + idWithTags, responseContainer, dictionary);
		}

		private void UpdateVersionAndPlatform()
		{
			apiVersion = VERSION.ApiVersion;
			unityVersion = VERSION.UnityVersion;
			platform = API.GetAssetPlatformString();
		}
	}
}
