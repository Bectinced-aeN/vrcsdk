using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class DescribeIdentityPoolResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static DescribeIdentityPoolResponseUnmarshaller _instance = new DescribeIdentityPoolResponseUnmarshaller();

		public static DescribeIdentityPoolResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			DescribeIdentityPoolResponse describeIdentityPoolResponse = new DescribeIdentityPoolResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("AllowUnauthenticatedIdentities", currentDepth))
				{
					BoolUnmarshaller instance = BoolUnmarshaller.Instance;
					describeIdentityPoolResponse.AllowUnauthenticatedIdentities = instance.Unmarshall(context);
				}
				else if (context.TestExpression("CognitoIdentityProviders", currentDepth))
				{
					ListUnmarshaller<CognitoIdentityProviderInfo, CognitoIdentityProviderInfoUnmarshaller> listUnmarshaller = new ListUnmarshaller<CognitoIdentityProviderInfo, CognitoIdentityProviderInfoUnmarshaller>(CognitoIdentityProviderInfoUnmarshaller.Instance);
					describeIdentityPoolResponse.CognitoIdentityProviders = listUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("DeveloperProviderName", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					describeIdentityPoolResponse.DeveloperProviderName = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolId", currentDepth))
				{
					StringUnmarshaller instance3 = StringUnmarshaller.Instance;
					describeIdentityPoolResponse.IdentityPoolId = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolName", currentDepth))
				{
					StringUnmarshaller instance4 = StringUnmarshaller.Instance;
					describeIdentityPoolResponse.IdentityPoolName = instance4.Unmarshall(context);
				}
				else if (context.TestExpression("OpenIdConnectProviderARNs", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller2 = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					describeIdentityPoolResponse.OpenIdConnectProviderARNs = listUnmarshaller2.Unmarshall(context);
				}
				else if (context.TestExpression("SamlProviderARNs", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller3 = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					describeIdentityPoolResponse.SamlProviderARNs = listUnmarshaller3.Unmarshall(context);
				}
				else if (context.TestExpression("SupportedLoginProviders", currentDepth))
				{
					DictionaryUnmarshaller<string, string, StringUnmarshaller, StringUnmarshaller> dictionaryUnmarshaller = new DictionaryUnmarshaller<string, string, StringUnmarshaller, StringUnmarshaller>(StringUnmarshaller.Instance, StringUnmarshaller.Instance);
					describeIdentityPoolResponse.SupportedLoginProviders = dictionaryUnmarshaller.Unmarshall(context);
				}
			}
			return describeIdentityPoolResponse;
		}

		public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("InternalErrorException"))
			{
				return new InternalErrorException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("InvalidParameterException"))
			{
				return new InvalidParameterException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("NotAuthorizedException"))
			{
				return new NotAuthorizedException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("ResourceNotFoundException"))
			{
				return new ResourceNotFoundException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("TooManyRequestsException"))
			{
				return new TooManyRequestsException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonCognitoIdentityException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static DescribeIdentityPoolResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
