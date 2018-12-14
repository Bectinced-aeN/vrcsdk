using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteObjectsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteObjectsResponseUnmarshaller _instance;

		public static DeleteObjectsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			DeleteObjectsResponse deleteObjectsResponse = new DeleteObjectsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, deleteObjectsResponse);
				}
			}
			return deleteObjectsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, DeleteObjectsResponse response)
		{
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
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
					if (context.TestExpression("Deleted", num))
					{
						response.DeletedObjects.Add(DeletedObjectUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Error", num))
					{
						response.DeleteErrors.Add(ErrorsItemUnmarshaller.Instance.Unmarshall(context));
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
