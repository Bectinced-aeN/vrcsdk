using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VRC.Core
{
	public class ApiPlayerModeration : ApiModel
	{
		public enum ModerationType
		{
			None,
			Block,
			Mute,
			Unmute
		}

		public const float ListCacheTime = 120f;

		[ApiField(Required = false, Name = "type")]
		public ModerationType moderationType
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string moderatorUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string moderatorDisplayName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string targetUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string targetDisplayName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string sourceUserId
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string sourceDisplayName
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public DateTime created
		{
			get;
			set;
		}

		[ApiField(Required = false)]
		public string moderated
		{
			get;
			set;
		}

		public ApiPlayerModeration()
			: base("auth/user/playermoderations")
		{
		}

		public override bool ShouldCache()
		{
			return false;
		}

		protected override bool ReadField(string fieldName, ref object data)
		{
			switch (fieldName)
			{
			case "type":
				data = moderationType.ToString().ToLower();
				return true;
			default:
				return base.ReadField(fieldName, ref data);
			}
		}

		protected override bool WriteField(string fieldName, object data)
		{
			switch (fieldName)
			{
			case "type":
			{
				string value = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((data as string).ToLower());
				moderationType = (ModerationType)(int)Enum.Parse(typeof(ModerationType), value);
				return true;
			}
			default:
				return base.WriteField(fieldName, data);
			}
		}

		public static void SendModeration(string targetUserId, ModerationType mType, Action<ApiPlayerModeration> successCallback = null, Action<string> errorCallback = null)
		{
			ApiPlayerModeration apiPlayerModeration = new ApiPlayerModeration();
			apiPlayerModeration.targetUserId = targetUserId;
			apiPlayerModeration.moderationType = mType;
			ApiPlayerModeration apiPlayerModeration2 = apiPlayerModeration;
			apiPlayerModeration2.Save(delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiPlayerModeration);
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void DeleteModeration(string moderationId, Action successCallback, Action<string> errorCallback)
		{
			API.Delete<ApiPlayerModeration>(moderationId, delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			});
		}

		public static void FetchAllAgainstMe(Action<List<ApiPlayerModeration>> successCallback = null, Action<string> errorCallback = null)
		{
			FetchList("auth/user/playermoderated", successCallback, errorCallback);
		}

		public static void FetchAllMine(Action<List<ApiPlayerModeration>> successCallback = null, Action<string> errorCallback = null)
		{
			FetchList("auth/user/playermoderations", successCallback, errorCallback);
		}

		private static void FetchList(string endpoint, Action<List<ApiPlayerModeration>> successCallback, Action<string> errorCallback)
		{
			ApiModelListContainer<ApiPlayerModeration> apiModelListContainer = new ApiModelListContainer<ApiPlayerModeration>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiPlayerModeration>).ResponseModels);
				}
			};
			apiModelListContainer.OnError = delegate(ApiContainer c)
			{
				Debug.LogError((object)("Could not fetch moderations with error - " + c.Error));
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiModelListContainer<ApiPlayerModeration> responseContainer = apiModelListContainer;
			API.SendGetRequest(endpoint, responseContainer, null, disableCache: false, 120f);
		}
	}
}
