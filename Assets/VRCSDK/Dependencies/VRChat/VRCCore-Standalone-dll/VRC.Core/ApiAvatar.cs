using System;
using System.Collections.Generic;
using System.Linq;
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

		private static AssetVersion _VERSION = null;

		public static AssetVersion MIN_LOADABLE_VERSION = new AssetVersion("5.5.0f1", 0);

		private static AssetVersion DefaultAssetVersion = new AssetVersion("5.6.3p1", 0);

		protected string mName;

		protected string mImageUrl;

		protected string mAuthorName;

		protected string mAuthorId;

		protected string mAssetUrl;

		protected string mDescription;

		protected string mReleaseStatus;

		protected List<string> mTags = new List<string>();

		protected int mVersion;

		protected string mUnityPackageUrl;

		public string thumbnailImageUrl;

		private AssetVersion mAssetVersion;

		private string mPlatform;

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

		public void Init(APIUser author, string name, string imageUrl, string assetUrl, string description, string releaseStatus, List<string> tags, string packageUrl = "")
		{
			mName = name;
			mImageUrl = imageUrl;
			mAssetUrl = assetUrl;
			mDescription = description;
			mReleaseStatus = releaseStatus;
			mTags = tags;
			mAuthorName = author.displayName;
			mAuthorId = author.id;
			mUnityPackageUrl = packageUrl;
			UpdateVersionAndPlatform();
		}

		public void Init()
		{
			mId = string.Empty;
			mName = string.Empty;
			mImageUrl = string.Empty;
			mAssetUrl = string.Empty;
			mDescription = string.Empty;
			mReleaseStatus = string.Empty;
			mTags = new List<string>();
			mAuthorName = string.Empty;
			mAuthorId = string.Empty;
			mVersion = -1;
			UpdateVersionAndPlatform();
		}

		public void Init(Dictionary<string, object> jsonObject)
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
			list.Add("tags");
			list.Add("version");
			List<string> source = list;
			if (source.Any((string s) => !jsonObject.ContainsKey(s)))
			{
				Debug.LogError((object)("Could not initialize Avatar due to insufficient json parameters.\nMissing: " + string.Join(", ", (from s in source
				where !jsonObject.ContainsKey(s)
				select s).ToArray())));
			}
			else
			{
				mId = (jsonObject["id"] as string);
				mName = (jsonObject["name"] as string);
				mImageUrl = (jsonObject["imageUrl"] as string);
				mAuthorName = (jsonObject["authorName"] as string);
				mAuthorId = (jsonObject["authorId"] as string);
				mAssetUrl = (jsonObject["assetUrl"] as string);
				if (jsonObject.ContainsKey("unityPackageUrl"))
				{
					mUnityPackageUrl = (jsonObject["unityPackageUrl"] as string);
				}
				mDescription = (jsonObject["description"] as string);
				mReleaseStatus = (jsonObject["releaseStatus"] as string);
				mTags = Tools.ObjListToStringList((List<object>)jsonObject["tags"]);
				mVersion = (int)(double)jsonObject["version"];
				if (jsonObject.ContainsKey("thumbnailImageUrl"))
				{
					thumbnailImageUrl = (jsonObject["thumbnailImageUrl"] as string);
				}
				string unityVersion = DefaultAssetVersion.UnityVersion;
				if (jsonObject.ContainsKey("unityVersion"))
				{
					unityVersion = (jsonObject["unityVersion"] as string);
				}
				int result = DefaultAssetVersion.ApiVersion;
				if (jsonObject.ContainsKey("assetVersion"))
				{
					string text = jsonObject["assetVersion"] as string;
					if (string.IsNullOrEmpty(text) || !int.TryParse(text, out result))
					{
						Debug.LogError((object)("Invalid assetVersion string: " + text));
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

		public void UpdateVersionAndPlatform()
		{
			mAssetVersion = VERSION;
			mPlatform = ApiModel.GetAssetPlatformString();
		}

		protected override Dictionary<string, string> BuildWebParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["name"] = name;
			dictionary["imageUrl"] = imageUrl;
			dictionary["authorName"] = authorName;
			dictionary["authorId"] = authorId;
			dictionary["assetUrl"] = assetUrl;
			dictionary["description"] = description;
			dictionary["releaseStatus"] = releaseStatus;
			dictionary["tags"] = string.Join(", ", tags.ToArray());
			if (!string.IsNullOrEmpty(unityPackageUrl))
			{
				dictionary["unityPackageUrl"] = unityPackageUrl;
			}
			dictionary["unityVersion"] = mAssetVersion.UnityVersion.ToString();
			dictionary["assetVersion"] = mAssetVersion.ApiVersion.ToString();
			dictionary["platform"] = mPlatform;
			return dictionary;
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
					ApiModel.SendPutRequest("avatars/" + id, dictionary, delegate
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
					ApiModel.SendPostRequest("avatars", dictionary, delegate
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

		public static void Fetch(string id, Action<ApiAvatar> successCallback, Action<string> errorCallback)
		{
			Fetch(id, compatibleVersionsOnly: true, successCallback, errorCallback);
		}

		public static void Fetch(string id, bool compatibleVersionsOnly, Action<ApiAvatar> successCallback, Action<string> errorCallback)
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
			ApiModel.SendRequest("avatars/" + id, HTTPMethods.Get, dictionary, delegate(Dictionary<string, object> obj)
			{
				ApiAvatar apiAvatar = new ApiAvatar();
				apiAvatar.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiAvatar);
				}
			}, delegate(string message)
			{
				errorCallback(message);
			}, needsAPIKey: true, Application.get_isEditor(), 180f);
		}

		public void AssignToThisUser()
		{
			string endpoint = "avatars/" + id + "/select";
			ApiModel.SendPutRequest(endpoint, delegate
			{
			}, delegate(string message)
			{
				Logger.Log("Could not assign current avatar" + message);
			});
		}

		public static void FetchList(Action<List<ApiAvatar>> successCallback, Action<string> errorCallback, Owner owner, ReleaseStatus relStatus = ReleaseStatus.All, string search = null, int number = 10, int offset = 0, SortHeading heading = SortHeading.None, SortOrder order = SortOrder.Descending, bool compatibleVersionsOnly = true, bool bypassCache = false)
		{
			string endpoint = "avatars";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
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
			dictionary.Add("n", number.ToString());
			dictionary.Add("offset", offset.ToString());
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
				dictionary.Add("maxUnityVersion", VERSION.UnityVersion.ToString());
				dictionary.Add("minUnityVersion", MIN_LOADABLE_VERSION.UnityVersion.ToString());
				dictionary.Add("maxAssetVersion", VERSION.ApiVersion.ToString());
				dictionary.Add("minAssetVersion", MIN_LOADABLE_VERSION.ApiVersion.ToString());
			}
			dictionary.Add("platform", ApiModel.GetAssetPlatformString());
			float cacheLifetime = 180f;
			if (bypassCache)
			{
				cacheLifetime = 0f;
			}
			ApiModel.SendRequest(endpoint, HTTPMethods.Get, dictionary, delegate(List<object> objs)
			{
				List<ApiAvatar> list = new List<ApiAvatar>();
				if (objs != null)
				{
					foreach (object obj in objs)
					{
						Dictionary<string, object> jsonObject = obj as Dictionary<string, object>;
						ApiAvatar apiAvatar = new ApiAvatar();
						apiAvatar.Init(jsonObject);
						list.Add(apiAvatar);
					}
				}
				if (successCallback != null)
				{
					successCallback(list);
				}
			}, delegate(string message)
			{
				Logger.Log("Could not fetch avatars with error - " + message);
				errorCallback(message);
			}, needsAPIKey: true, owner != Owner.Public, cacheLifetime);
		}

		public static void Delete(string id, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("avatars/" + id, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
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
	}
}
