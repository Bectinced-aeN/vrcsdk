using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class GetBucketNotificationResponse : AmazonWebServiceResponse
	{
		private List<TopicConfiguration> _topicConfigurations;

		private List<QueueConfiguration> _queueConfigurations;

		private List<LambdaFunctionConfiguration> _lambdaFunctionConfigurations;

		public List<TopicConfiguration> TopicConfigurations
		{
			get
			{
				if (_topicConfigurations == null)
				{
					_topicConfigurations = new List<TopicConfiguration>();
				}
				return _topicConfigurations;
			}
			set
			{
				_topicConfigurations = value;
			}
		}

		public List<QueueConfiguration> QueueConfigurations
		{
			get
			{
				if (_queueConfigurations == null)
				{
					_queueConfigurations = new List<QueueConfiguration>();
				}
				return _queueConfigurations;
			}
			set
			{
				_queueConfigurations = value;
			}
		}

		public List<LambdaFunctionConfiguration> LambdaFunctionConfigurations
		{
			get
			{
				if (_lambdaFunctionConfigurations == null)
				{
					_lambdaFunctionConfigurations = new List<LambdaFunctionConfiguration>();
				}
				return _lambdaFunctionConfigurations;
			}
			set
			{
				_lambdaFunctionConfigurations = value;
			}
		}

		public GetBucketNotificationResponse()
			: this()
		{
		}
	}
}
