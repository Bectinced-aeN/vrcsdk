using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class PutBucketNotificationRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		public List<TopicConfiguration> TopicConfigurations
		{
			get;
			set;
		}

		public List<QueueConfiguration> QueueConfigurations
		{
			get;
			set;
		}

		public List<LambdaFunctionConfiguration> LambdaFunctionConfigurations
		{
			get;
			set;
		}

		internal bool IsSetBucketName()
		{
			return BucketName != null;
		}

		internal bool IsSetTopicConfigurations()
		{
			if (TopicConfigurations != null)
			{
				return TopicConfigurations.Count > 0;
			}
			return false;
		}

		internal bool IsSetQueueConfigurations()
		{
			if (QueueConfigurations != null)
			{
				return QueueConfigurations.Count > 0;
			}
			return false;
		}

		internal bool IsSetLambdaFunctionConfigurations()
		{
			if (LambdaFunctionConfigurations != null)
			{
				return LambdaFunctionConfigurations.Count > 0;
			}
			return false;
		}

		public PutBucketNotificationRequest()
			: this()
		{
		}
	}
}
