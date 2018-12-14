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

		public enum Owner
		{
			Public,
			Mine,
			Developer,
			Any
		}

		protected string mName;

		protected string mImageUrl;

		protected string mAuthorName;

		protected string mAuthorId;

		protected string mAssetUrl;

		protected string mDescription;

		protected List<string> mTags = new List<string>();

		protected int mVersion;

		protected string mUnityPackageUrl;

		public string thumbnailImageUrl;

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

		public void Init(APIUser author, string name, string imageUrl, string assetUrl, string description, List<string> tags, string packageUrl = "")
		{
			mName = name;
			mImageUrl = imageUrl;
			mAssetUrl = assetUrl;
			mDescription = description;
			mTags = tags;
			mAuthorName = author.displayName;
			mAuthorId = author.id;
			mUnityPackageUrl = packageUrl;
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
			list.Add("tags");
			list.Add("version");
			List<string> source = list;
			if (source.Any((string s) => !jsonObject.ContainsKey(s)))
			{
				Debug.LogError((object)"Could not initialize Avatar due to insufficient json parameters");
			}
			else
			{
				mId = (jsonObject["id"] as string);
				mName = (jsonObject["name"] as string);
				mImageUrl = (jsonObject["imageUrl"] as string);
				mAuthorName = (jsonObject["authorName"] as string);
				mAuthorId = (jsonObject["authorId"] as string);
				mAssetUrl = (jsonObject["assetUrl"] as string);
				mDescription = (jsonObject["description"] as string);
				mTags = Tools.ObjListToStringList((List<object>)jsonObject["tags"]);
				mVersion = (int)(double)jsonObject["version"];
				if (jsonObject.ContainsKey("thumbnailImageUrl"))
				{
					thumbnailImageUrl = (jsonObject["thumbnailImageUrl"] as string);
				}
			}
		}

		protected new virtual Dictionary<string, string> BuildWebParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["name"] = name;
			dictionary["imageUrl"] = imageUrl;
			dictionary["authorName"] = authorName;
			dictionary["authorId"] = authorId;
			dictionary["assetUrl"] = assetUrl;
			dictionary["description"] = description;
			dictionary["tags"] = string.Join(", ", tags.ToArray());
			if (!string.IsNullOrEmpty(unityPackageUrl))
			{
				dictionary["unityPackageUrl"] = unityPackageUrl;
			}
			return dictionary;
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
						ApiModel.SendPutRequest("avatars/" + id, dictionary, delegate
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
					ApiModel.SendPostRequest("avatars", dictionary, delegate(Dictionary<string, object> successResponse)
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

		public static void Fetch(string id, Action<ApiAvatar> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("avatars/" + id, delegate(Dictionary<string, object> obj)
			{
				ApiAvatar apiAvatar = ScriptableObject.CreateInstance<ApiAvatar>();
				apiAvatar.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiAvatar);
				}
			}, delegate(string message)
			{
				errorCallback(message);
			});
		}

		public void AssignToThisUser()
		{
			string endpoint = "avatars/" + id + "/select";
			ApiModel.SendPutRequest(endpoint, null, delegate
			{
			}, delegate(string message)
			{
				Logger.Log("Could not assign current avatar" + message);
			});
		}

		public static void FetchList(Action<List<ApiAvatar>> successCallback, Action<string> errorCallback, Owner owner, string search = null, int number = 10, int offset = 0, SortHeading heading = SortHeading.None, SortOrder order = SortOrder.Descending)
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
			ApiModel.SendRequest(endpoint, HTTPMethods.Get, dictionary, delegate(List<object> objs)
			{
				List<ApiAvatar> list = new List<ApiAvatar>();
				if (objs != null)
				{
					foreach (object obj in objs)
					{
						Dictionary<string, object> jsonObject = obj as Dictionary<string, object>;
						ApiAvatar apiAvatar = ScriptableObject.CreateInstance<ApiAvatar>();
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
			});
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
			});
		}
	}
}
