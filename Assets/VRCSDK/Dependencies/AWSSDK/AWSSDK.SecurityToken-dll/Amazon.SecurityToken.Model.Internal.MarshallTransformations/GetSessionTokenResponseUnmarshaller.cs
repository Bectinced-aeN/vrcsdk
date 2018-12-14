using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class GetSessionTokenResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static GetSessionTokenResponseUnmarshaller _instance = new GetSessionTokenResponseUnmarshaller();

		public static GetSessionTokenResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetSessionTokenResponse getSessionTokenResponse = new GetSessionTokenResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("GetSessionTokenResult", 2))
					{
						UnmarshallResult(context, getSessionTokenResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						getSessionTokenResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return getSessionTokenResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetSessionTokenResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if ((context.get_IsStartElement() || context.get_IsAttribute()) && context.TestExpression("Credentials", num))
				{
					CredentialsUnmarshaller instance = CredentialsUnmarshaller.Instance;
					response.Credentials = instance.Unmarshall(context);
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (val.get_Code() != null && val.get_Code().Equals("RegionDisabledException"))
			{
				return new RegionDisabledException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			return new AmazonSecurityTokenServiceException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
		}

		internal static GetSessionTokenResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public GetSessionTokenResponseUnmarshaller()
			: this()
		{
		}
	}
}
