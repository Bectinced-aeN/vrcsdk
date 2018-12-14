using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketRequestPaymentResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketRequestPaymentResponseUnmarshaller _instance;

		public static GetBucketRequestPaymentResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketRequestPaymentResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketRequestPaymentResponse getBucketRequestPaymentResponse = new GetBucketRequestPaymentResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketRequestPaymentResponse);
				}
			}
			return getBucketRequestPaymentResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketRequestPaymentResponse response)
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
					if (context.TestExpression("Payer", num))
					{
						response.Payer = StringUnmarshaller.GetInstance().Unmarshall(context);
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
