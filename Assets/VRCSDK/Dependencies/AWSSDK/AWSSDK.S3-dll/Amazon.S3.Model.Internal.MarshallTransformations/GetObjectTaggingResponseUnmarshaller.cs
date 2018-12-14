using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetObjectTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetObjectTaggingResponseUnmarshaller _instance;

		public static GetObjectTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetObjectTaggingResponse getObjectTaggingResponse = new GetObjectTaggingResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getObjectTaggingResponse);
				}
			}
			return getObjectTaggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetObjectTaggingResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 2;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Tag", num))
					{
						response.Tagging.Add(TagUnmarshaller.Instance.Unmarshall(context));
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
