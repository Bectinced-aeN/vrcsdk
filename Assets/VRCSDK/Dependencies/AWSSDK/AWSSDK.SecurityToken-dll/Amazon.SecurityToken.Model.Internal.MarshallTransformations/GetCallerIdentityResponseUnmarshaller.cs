using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetCallerIdentityResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static GetCallerIdentityResponseUnmarshaller _instance = new GetCallerIdentityResponseUnmarshaller();

		public static GetCallerIdentityResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetCallerIdentityResponse getCallerIdentityResponse = new GetCallerIdentityResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("GetCallerIdentityResult", 2))
					{
						UnmarshallResult(context, getCallerIdentityResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						getCallerIdentityResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return getCallerIdentityResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetCallerIdentityResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Account", num))
					{
						StringUnmarshaller instance = StringUnmarshaller.get_Instance();
						response.Account = instance.Unmarshall(context);
					}
					else if (context.TestExpression("Arn", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.get_Instance();
						response.Arn = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("UserId", num))
					{
						StringUnmarshaller instance3 = StringUnmarshaller.get_Instance();
						response.UserId = instance3.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			return new AmazonSecurityTokenServiceException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
		}

		internal static GetCallerIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public GetCallerIdentityResponseUnmarshaller()
			: this()
		{
		}
	}
}
