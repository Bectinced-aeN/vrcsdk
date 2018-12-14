using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class DecodeAuthorizationMessageResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static DecodeAuthorizationMessageResponseUnmarshaller _instance = new DecodeAuthorizationMessageResponseUnmarshaller();

		public static DecodeAuthorizationMessageResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			DecodeAuthorizationMessageResponse decodeAuthorizationMessageResponse = new DecodeAuthorizationMessageResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("DecodeAuthorizationMessageResult", 2))
					{
						UnmarshallResult(context, decodeAuthorizationMessageResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						decodeAuthorizationMessageResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return decodeAuthorizationMessageResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, DecodeAuthorizationMessageResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if ((context.get_IsStartElement() || context.get_IsAttribute()) && context.TestExpression("DecodedMessage", num))
				{
					StringUnmarshaller instance = StringUnmarshaller.get_Instance();
					response.DecodedMessage = instance.Unmarshall(context);
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (val.get_Code() != null && val.get_Code().Equals("InvalidAuthorizationMessageException"))
			{
				return new InvalidAuthorizationMessageException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			return new AmazonSecurityTokenServiceException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
		}

		internal static DecodeAuthorizationMessageResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public DecodeAuthorizationMessageResponseUnmarshaller()
			: this()
		{
		}
	}
}
