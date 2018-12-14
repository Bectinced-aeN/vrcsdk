using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketTaggingResponseUnmarshaller _instance;

		public static GetBucketTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketTaggingResponse getBucketTaggingResponse = new GetBucketTaggingResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketTaggingResponse);
				}
			}
			return getBucketTaggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketTaggingResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Tag", num + 1))
					{
						response.TagSet.Add(TagUnmarshaller.Instance.Unmarshall(context));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					break;
				}
			}
		}
	}
}
