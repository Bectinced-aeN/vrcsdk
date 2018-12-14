using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListBucketsResponseUnmarshaller _instance = new ListBucketsResponseUnmarshaller();

		public static ListBucketsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListBucketsResponse listBucketsResponse = new ListBucketsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listBucketsResponse);
				}
			}
			return listBucketsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListBucketsResponse response)
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
					if (context.TestExpression("Bucket", num + 1))
					{
						response.Buckets.Add(BucketUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Owner", num))
					{
						response.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
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
