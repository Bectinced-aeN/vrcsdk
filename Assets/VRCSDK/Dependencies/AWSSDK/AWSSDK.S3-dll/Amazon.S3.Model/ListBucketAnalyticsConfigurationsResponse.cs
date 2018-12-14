using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListBucketAnalyticsConfigurationsResponse : AmazonWebServiceResponse
	{
		private string token;

		private List<AnalyticsConfiguration> analyticsConfigurationList = new List<AnalyticsConfiguration>();

		private bool? isTruncated;

		private string nextToken;

		public string ContinuationToken
		{
			get
			{
				return token;
			}
			set
			{
				token = value;
			}
		}

		public List<AnalyticsConfiguration> AnalyticsConfigurationList
		{
			get
			{
				return analyticsConfigurationList;
			}
			set
			{
				analyticsConfigurationList = value;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return isTruncated ?? false;
			}
			set
			{
				isTruncated = value;
			}
		}

		public string NextContinuationToken
		{
			get
			{
				return nextToken;
			}
			set
			{
				nextToken = value;
			}
		}

		internal bool IsSetToken()
		{
			return !string.IsNullOrEmpty(token);
		}

		public bool IsSetAnalyticsConfigurationList()
		{
			return analyticsConfigurationList.Count > 0;
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetNextToken()
		{
			return !string.IsNullOrEmpty(nextToken);
		}

		public ListBucketAnalyticsConfigurationsResponse()
			: this()
		{
		}
	}
}
