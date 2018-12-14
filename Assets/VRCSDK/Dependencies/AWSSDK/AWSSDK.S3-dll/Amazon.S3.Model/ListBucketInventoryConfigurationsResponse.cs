using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListBucketInventoryConfigurationsResponse : AmazonWebServiceResponse
	{
		private string token;

		private List<InventoryConfiguration> inventoryConfigurationList = new List<InventoryConfiguration>();

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

		public List<InventoryConfiguration> InventoryConfigurationList
		{
			get
			{
				return inventoryConfigurationList;
			}
			set
			{
				inventoryConfigurationList = value;
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

		public bool IsSetInventoryConfigurationList()
		{
			return inventoryConfigurationList.Count > 0;
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		internal bool IsSetNextToken()
		{
			return !string.IsNullOrEmpty(nextToken);
		}

		public ListBucketInventoryConfigurationsResponse()
			: this()
		{
		}
	}
}
