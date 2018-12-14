using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumeRoleResponseUnmarshaller : XmlResponseUnmarshaller
	{
		private static AssumeRoleResponseUnmarshaller _instance = new AssumeRoleResponseUnmarshaller();

		public static AssumeRoleResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AssumeRoleResponse assumeRoleResponse = new AssumeRoleResponse();
			context.Read();
			int currentDepth = context.get_CurrentDepth();
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.get_IsStartElement())
				{
					if (context.TestExpression("AssumeRoleResult", 2))
					{
						UnmarshallResult(context, assumeRoleResponse);
					}
					else if (context.TestExpression("ResponseMetadata", 2))
					{
						assumeRoleResponse.set_ResponseMetadata(ResponseMetadataUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
			}
			return assumeRoleResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, AssumeRoleResponse response)
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
					else if (context.TestExpression("Credentials", num))
					{
						CredentialsUnmarshaller instance2 = CredentialsUnmarshaller.Instance;
						response.Credentials = instance2.Unmarshall(context);
					}
					else if (context.TestExpression("PackedPolicySize", num))
					{
						IntUnmarshaller instance3 = IntUnmarshaller.get_Instance();
						response.PackedPolicySize = instance3.Unmarshall(context);
					}
				}
			}
		}

		public override AmazonServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			ErrorResponse val = ErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
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

		internal static AssumeRoleResponseUnmarshaller GetInstance()
		{
			return _instance;
		}

		public AssumeRoleResponseUnmarshaller()
			: this()
		{
		}
	}
}
