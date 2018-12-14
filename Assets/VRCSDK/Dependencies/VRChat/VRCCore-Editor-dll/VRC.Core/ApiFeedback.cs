using System;
using System.Collections.Generic;

namespace VRC.Core
{
	public class ApiFeedback : ApiModel
	{
		public enum ContentType
		{
			world,
			avatar
		}

		public enum ApprovalType
		{
			positive,
			negative,
			report,
			tag,
			none
		}

		[ApiField]
		public new string id
		{
			get;
			set;
		}

		[ApiField]
		public string type
		{
			get;
			set;
		}

		[ApiField]
		public string reason
		{
			get;
			set;
		}

		[ApiField]
		public string commenterId
		{
			get;
			set;
		}

		[ApiField]
		public string commenterName
		{
			get;
			set;
		}

		[ApiField]
		public string contentId
		{
			get;
			set;
		}

		[ApiField]
		public string contentType
		{
			get;
			set;
		}

		[ApiField]
		public int contentVersion
		{
			get;
			set;
		}

		[ApiField]
		public string contentName
		{
			get;
			set;
		}

		[ApiField]
		public string contentAuthorId
		{
			get;
			set;
		}

		[ApiField]
		public string contentAuthorName
		{
			get;
			set;
		}

		[ApiField]
		public List<string> tags
		{
			get;
			set;
		}

		public static void AddFeedback(string worldId, int version, ContentType contentType, ApprovalType approvalType, string reason, Action<ApiFeedback> successCallback, Action<string> errorCallback)
		{
			ApiModelContainer<ApiFeedback> apiModelContainer = new ApiModelContainer<ApiFeedback>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback(c.Model as ApiFeedback);
				}
			};
			apiModelContainer.OnError = delegate(ApiContainer c)
			{
				if (errorCallback != null)
				{
					errorCallback(c.Error);
				}
			};
			ApiContainer responseContainer = apiModelContainer;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["contentType"] = contentType.ToString();
			dictionary["type"] = approvalType.ToString();
			dictionary["reason"] = reason;
			API.SendPostRequest("/feedback/" + worldId + "/" + version.ToString(), responseContainer, dictionary);
		}

		public static void FetchFeedback(string worldId, int version, ContentType contentType, Action<List<ApiFeedback>> successCallback, Action<string> errorCallback)
		{
			ApiModelListContainer<ApiFeedback> apiModelListContainer = new ApiModelListContainer<ApiFeedback>();
			apiModelListContainer.OnSuccess = delegate(ApiContainer c)
			{
				if (successCallback != null)
				{
					successCallback((c as ApiModelListContainer<ApiFeedback>).ResponseModels);
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
			API.SendGetRequest("/users/" + APIUser.CurrentUser.id + "/feedback?contentType=world&contentId=" + worldId + "&contentVersion=" + version.ToString(), responseContainer, null, disableCache: true);
		}

		public static void DeleteFeedback(string feedbackId, Action successCallback, Action<string> errorCallback)
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
			API.SendDeleteRequest("/feedback/" + feedbackId, responseContainer);
		}
	}
}
