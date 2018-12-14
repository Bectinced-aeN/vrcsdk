using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListBucketMetricsConfigurationsResponse : AmazonWebServiceResponse
	{
		private string token;

		private List<MetricsConfiguration> metricsConfigurationList = new List<MetricsConfiguration>();

		private bool? isTruncated;

		private string nextToken;

		public string Token
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

		public List<MetricsConfiguration> MetricsConfigurationList
		{
			get
			{
				return metricsConfigurationList;
			}
			set
			{
				metricsConfigurationList = value;
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

		public string NextToken
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

		public bool IsSetMetricsConfigurationList()
		{
			return metricsConfigurationList.Count > 0;
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetNextToken()
		{
			return !string.IsNullOrEmpty(nextToken);
		}

		public ListBucketMetricsConfigurationsResponse()
			: this()
		{
		}
	}
}
