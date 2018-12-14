using System;
using System.Collections.Generic;

namespace VRC.Core
{
	public class ApiContentFeedback : ApiModel
	{
		[ApiField]
		public float positiveScore
		{
			get;
			set;
		}

		[ApiField]
		public float negativeScore
		{
			get;
			set;
		}

		[ApiField]
		public float reportScore
		{
			get;
			set;
		}

		[ApiField]
		public float tagScore
		{
			get;
			set;
		}

		[ApiField]
		public List<string> positiveReasons
		{
			get;
			set;
		}

		[ApiField]
		public List<string> negativeReasons
		{
			get;
			set;
		}

		[ApiField]
		public List<string> reportReasons
		{
			get;
			set;
		}

		[ApiField]
		public List<string> prospectiveTags
		{
			get;
			set;
		}

		public static void FetchFeedback(string worldId, int version, ApiFeedback.ContentType contentType, Action<ApiContentFeedback> successCallback, Action<string> errorCallback)
		{
			ApiModelContainer<ApiContentFeedback> apiModelContainer = new ApiModelContainer<ApiContentFeedback>();
			apiModelContainer.OnSuccess = delegate(ApiContainer c)
			{
				ApiContentFeedback obj = c.Model as ApiContentFeedback;
				if (successCallback != null)
				{
					successCallback(obj);
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
			API.SendGetRequest("/worlds/" + worldId + "/" + version + "/feedback", responseContainer, null, disableCache: true);
		}
	}
}
