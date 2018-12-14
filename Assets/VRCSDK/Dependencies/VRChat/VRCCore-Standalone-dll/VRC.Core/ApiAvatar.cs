using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class ApiAvatar : ApiModel, IUIGroupItemDatasource
	{
		public enum SortHeading
		{
			None,
			Order
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
			All
		}

		public enum Owner
		{
			Public,
			Mine,
			Developer,
			Any
		}

		public const float ListCacheTime = 180f;

		public const float SingleRecordCacheTime = 180f;

		private static AssetVersion _VERSION = null;

		public static AssetVersion MIN_LOADABLE_VERSION = new AssetVersion("5.5.0f1", 0);

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
		public string authorName
		{
			get;
			set;
		}

		[ApiField]
		public string authorId
		{
			get;
			set;
		}

		[ApiField]
		public string assetUrl
		{
			get;
			set;
		}

		[ApiField]
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
		public string unityPackageUrl
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string thumbnailImageUrl
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
		public string releaseStatus
		{
			get;
			set;
		}

		[ApiField(Required = false, IsAdminWritableOnly = true)]
		public bool featured
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public List<string> unityPackages
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public bool unityPackageUpdated
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

		[DefaultValue("standalonewindows")]
		[ApiField(Required = false)]
		public string platform
		{
			get;
			protected set;
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

		public ApiAvatar()
			: base("avatars")
		{
			UpdateVersionAndPlatform();
		}

		public override bool ShouldCache()
		{
			return base.ShouldCache() && !string.IsNullOrEmpty(assetUrl);
		}

		public override float GetLifeSpan()
		{
			return 180f;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "imageUrl":
				if (string.IsNullOrEmpty(imageUrl))
				{
					return false;
				}
				data = imageUrl;
				return true;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		public override void Get(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null, bool disableCache = false)
		{
			Get(compatibleVersionsOnly: true, onSuccess, onFailure, parameters, disableCache);
		}

		public void Get(bool compatibleVersionsOnly, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null, Dictionary<string, object> parameters = null, bool disableCache = false)
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
				parameters["platform"] = API.GetAssetPlatformString();
				base.Get(onSuccess, onFailure, parameters, disableCache);
			}
			else
			{
				base.Get(onSuccess, onFailure, parameters, disableCache);
			}
		}

		public void AssignToThisUser()
		{
			string target = "avatars/" + base.id + "/select";
			API.SendPutRequest(target, new ApiContainer
			{
				OnError = delegate(ApiContainer c)
				{
					Logger.LogError("Could not assign current avatar" + c.Error);
				},
				Model = this
			});
		}

		public static void FetchList(Action<List<ApiAvatar>> successCallback, Action<string> errorCallback, Owner owner, ReleaseStatus relStatus = ReleaseStatus.All, string search = null, int number = 10, int offset = 0, SortHeading heading = SortHeading.None, SortOrder order = SortOrder.Descending, bool compatibleVersionsOnly = true, bool disableCache = false)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (owner == Owner.Mine)
			{
				dictionary.Add("user", "me");
			}
			if (owner == Owner.Public)
			{
				dictionary.Add("featured", "true");
			}
			if (owner == Owner.Developer)
			{
				dictionary.Add("tag", "developer");
			}
			dictionary.Add("releaseStatus", relStatus.ToString().ToLower());
			if (search != null)
			{
				dictionary.Add("search", search);
			}
			dictionary.Add("n", number);
			dictionary.Add("offset", offset);
			if (heading != 0)
			{
				dictionary.Add("sort", heading.ToString().ToLower());
			}
			switch (order)
			{
			case SortOrder.Ascending:
				dictionary.Add("order", "ascending");
				break;
			case SortOrder.Descending:
				dictionary.Add("order", "descending");
				break;
			}
			if (compatibleVersionsOnly)
			{
				dictionary.Add("maxUnityVersion", VERSION.UnityVersion);
				dictionary.Add("minUnityVersion", MIN_LOADABLE_VERSION.UnityVersion);
				dictionary.Add("maxAssetVersion", VERSION.ApiVersion);
				dictionary.Add("minAssetVersion", MIN_LOADABLE_VERSION.ApiVersion);
			}
			dictionary.Add("platform", API.GetAssetPlatformString());
			ApiModelListContainer<ApiAvatar> apiModelListContainer = new ApiModelListContainer<ApiAvatar>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiAvatar>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Logger.Log("Could not fetch avatars with error - " + c.Error);
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiAvatar> responseContainer = apiModelListContainer;
			API.SendRequest("avatars", HTTPMethods.Get, responseContainer, dictionary, needsAPIKey: true, owner != Owner.Public, disableCache, 180f);
		}

		public override void Save(Action<ApiContainer> onSuccess = null, Action<ApiContainer> onFailure = null)
		{
			UpdateVersionAndPlatform();
			base.Save(onSuccess, onFailure);
		}

		public void SaveReleaseStatus(Action<ApiContainer> onSuccess, Action<ApiContainer> onFailure)
		{
			if (string.IsNullOrEmpty(base.Endpoint))
			{
				Debug.LogError((object)"Cannot save to null endpoint");
			}
			else if (string.IsNullOrEmpty(base.id))
			{
				Debug.LogError((object)"Cannot save release status with no id");
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", base.id);
				dictionary.Add("releaseStatus", releaseStatus);
				Dictionary<string, object> parameters = dictionary;
				Action<ApiContainer> onSuccess2 = delegate(ApiContainer c)
				{
					ApiCache.Save(c.Model.id, c.Model, andClone: true);
					if (onSuccess != null)
					{
						onSuccess(c);
					}
				};
				Put(onSuccess2, onFailure, parameters);
			}
		}

		private void UpdateVersionAndPlatform()
		{
			apiVersion = VERSION.ApiVersion;
			unityVersion = VERSION.UnityVersion;
			platform = API.GetAssetPlatformString();
		}
	}
}
