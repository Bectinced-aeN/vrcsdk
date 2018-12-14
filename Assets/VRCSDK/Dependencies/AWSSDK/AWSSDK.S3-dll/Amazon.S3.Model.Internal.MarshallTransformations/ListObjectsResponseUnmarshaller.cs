using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListObjectsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListObjectsResponseUnmarshaller _instance;

		public static ListObjectsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListObjectsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListObjectsResponse listObjectsResponse = new ListObjectsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listObjectsResponse);
				}
			}
			return listObjectsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListObjectsResponse response)
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
					if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("NextMarker", num))
					{
						response.NextMarker = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Contents", num))
					{
						S3Object s3Object = ContentsItemUnmarshaller.Instance.Unmarshall(context);
						s3Object.BucketName = response.Name;
						response.S3Objects.Add(s3Object);
					}
					else if (context.TestExpression("Name", num))
					{
						response.Name = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						response.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Delimiter", num))
					{
						response.Delimiter = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MaxKeys", num))
					{
						response.MaxKeys = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("CommonPrefixes", num))
					{
						string text = CommonPrefixesItemUnmarshaller.Instance.Unmarshall(context);
						if (text != null)
						{
							response.CommonPrefixes.Add(text);
						}
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
