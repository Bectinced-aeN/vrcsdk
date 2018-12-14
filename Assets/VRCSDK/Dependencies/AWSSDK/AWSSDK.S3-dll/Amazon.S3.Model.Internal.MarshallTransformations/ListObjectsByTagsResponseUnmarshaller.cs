using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListObjectsByTagsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListObjectsByTagsResponseUnmarshaller _instance;

		public static ListObjectsByTagsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListObjectsByTagsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListObjectsByTagsResponse listObjectsByTagsResponse = new ListObjectsByTagsResponse();
			UnmarshallResult(context, listObjectsByTagsResponse);
			return listObjectsByTagsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListObjectsByTagsResponse response)
		{
			int num = context.get_CurrentDepth();
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Bucket", num))
					{
						response.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ContinuationToken", num))
					{
						response.ContinuationToken = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextContinuationToken", num))
					{
						response.NextContinuationToken = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxKeys", num))
					{
						response.MaxKeys = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("EncodingType", num))
					{
						response.EncodingType = EncodingType.FindValue(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("Contents", num))
					{
						response.Contents.Add(TaggedResourceUnmarshaller.Instance.Unmarshall(context));
					}
				}
			}
		}
	}
}
