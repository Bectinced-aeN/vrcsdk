using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleWithSAMLResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleWithSAMLResponseUnmarshaller _instance = new AssumeRoleWithSAMLResponseUnmarshaller();

		public static AssumeRoleWithSAMLResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleWithSAMLResponse assumeRoleWithSAMLResponse = new AssumeRoleWithSAMLResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("AssumeRoleWithSAMLResult", 2))
					{
						UnmarshallResult(context, assumeRoleWithSAMLResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleWithSAMLResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return assumeRoleWithSAMLResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleWithSAMLResponse response)
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
					else if (context.TestExpression("Issuer", num))
					{
						StringUnmarshaller instance4 = StringUnmarshaller.get_Instance();
						response.Issuer = instance4.Unmarshall(context);
					}
					else if (context.TestExpression("NameQualifier", num))
					{
						StringUnmarshaller instance5 = StringUnmarshaller.get_Instance();
						response.NameQualifier = instance5.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance6 = IntUnmarshaller.get_Instance();
						response.PackedPolicySize = instance6.Unmarshall(context);
					}
					else if (context.TestExpression("Subject", num))
					{
						StringUnmarshaller instance7 = StringUnmarshaller.get_Instance();
						response.Subject = instance7.Unmarshall(context);
					}
					else if (context.TestExpression("SubjectType", num))
					{
						StringUnmarshaller instance8 = StringUnmarshaller.get_Instance();
						response.SubjectType = instance8.Unmarshall(context);
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
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (val.get_Code() != null && val.get_Code().Equals("ExpiredTokenException"))
			{
				return new ExpiredTokenException(val.get_Message(), innerException, val.get_Type(), val.get_Code(), val.get_RequestId(), statusCode);
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

		internal static AssumeRoleWithSAMLResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public AssumeRoleWithSAMLResponseUnmarshaller()
			: this()
		{
		}
	}
}
