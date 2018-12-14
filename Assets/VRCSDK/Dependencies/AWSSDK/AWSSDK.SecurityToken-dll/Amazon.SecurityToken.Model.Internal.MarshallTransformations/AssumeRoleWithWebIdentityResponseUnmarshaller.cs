using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithWebIdentityResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleWithWebIdentityResponseUnmarshaller _instance = new AssumeRoleWithWebIdentityResponseUnmarshaller();

		public static AssumeRoleWithWebIdentityResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleWithWebIdentityResponse assumeRoleWithWebIdentityResponse = new AssumeRoleWithWebIdentityResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("AssumeRoleWithWebIdentityResult", 2))
					{
						UnmarshallResult(context, assumeRoleWithWebIdentityResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleWithWebIdentityResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return assumeRoleWithWebIdentityResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleWithWebIdentityResponse response)
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
					if (context.TestExpression("AssumedRoleUser", num))
					{
						AssumedRoleUserUnmarshaller instance = AssumedRoleUserUnmarshaller.Instance;
						response.AssumedRoleUser = instance.Unmarshall(context);
					}
					else if (context.TestExpression("Audience", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.get_Instance();
						response.Audience = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("Credentials", num))
					{
						CredentialsUnmarshaller instance3 = CredentialsUnmarshaller.Instance;
						response.Credentials = instance3.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance4 = IntUnmarshaller.get_Instance();
						response.PackedPolicySize = instance4.Unmarshall(context);
					}
					else if (context.TestExpression("Provider", num))
					{
						StringUnmarshaller instance5 = StringUnmarshaller.get_Instance();
						response.Provider = instance5.Unmarshall(context);
					}
					else if (context.TestExpression("SubjectFromWebIdentityToken", num))
					{
						StringUnmarshaller instance6 = StringUnmarshaller.get_Instance();
						response.SubjectFromWebIdentityToken = instance6.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (val.get_Code() != null && val.get_Code().Equals("ExpiredTokenException"))
			{
				return new ExpiredTokenException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("IDPCommunicationError"))
			{
				return new IDPCommunicationErrorException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("IDPRejectedClaim"))
			{
				return new IDPRejectedClaimException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("InvalidIdentityToken"))
			{
				return new InvalidIdentityTokenException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("MalformedPolicyDocument"))
			{
				return new MalformedPolicyDocumentException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("PackedPolicyTooLarge"))
			{
				return new PackedPolicyTooLargeException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			if (val.get_Code() != null && val.get_Code().Equals("RegionDisabledException"))
			{
				return new RegionDisabledException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
			}
			return new AmazonSecurityTokenServiceException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
		}

		internal static AssumeRoleWithWebIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public AssumeRoleWithWebIdentityResponseUnmarshaller()
			: this()
		{
		}
	}
}
