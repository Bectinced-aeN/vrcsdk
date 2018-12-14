using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class CreateIdentityPoolResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static CreateIdentityPoolResponseUnmarshaller _instance = new CreateIdentityPoolResponseUnmarshaller();

		public static CreateIdentityPoolResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			CreateIdentityPoolResponse createIdentityPoolResponse = new CreateIdentityPoolResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("AllowUnauthenticatedIdentities", currentDepth))
				{
					BoolUnmarshaller instance = BoolUnmarshaller.Instance;
					createIdentityPoolResponse.AllowUnauthenticatedIdentities = instance.Unmarshall(context);
				}
				else if (context.TestExpression("CognitoIdentityProviders", currentDepth))
				{
					ListUnmarshaller<CognitoIdentityProviderInfo, CognitoIdentityProviderInfoUnmarshaller> listUnmarshaller = new ListUnmarshaller<CognitoIdentityProviderInfo, CognitoIdentityProviderInfoUnmarshaller>(CognitoIdentityProviderInfoUnmarshaller.Instance);
					createIdentityPoolResponse.CognitoIdentityProviders = listUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("DeveloperProviderName", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					createIdentityPoolResponse.DeveloperProviderName = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolId", currentDepth))
				{
					StringUnmarshaller instance3 = StringUnmarshaller.Instance;
					createIdentityPoolResponse.IdentityPoolId = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolName", currentDepth))
				{
					StringUnmarshaller instance4 = StringUnmarshaller.Instance;
					createIdentityPoolResponse.IdentityPoolName = instance4.Unmarshall(context);
				}
				else if (context.TestExpression("OpenIdConnectProviderARNs", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller2 = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					createIdentityPoolResponse.OpenIdConnectProviderARNs = listUnmarshaller2.Unmarshall(context);
				}
				else if (context.TestExpression("SamlProviderARNs", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller3 = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					createIdentityPoolResponse.SamlProviderARNs = listUnmarshaller3.Unmarshall(context);
				}
				else if (context.TestExpression("SupportedLoginProviders", currentDepth))
				{
					DictionaryUnmarshaller<string, string, StringUnmarshaller, StringUnmarshaller> dictionaryUnmarshaller = new DictionaryUnmarshaller<string, string, StringUnmarshaller, StringUnmarshaller>(StringUnmarshaller.Instance, StringUnmarshaller.Instance);
					createIdentityPoolResponse.SupportedLoginProviders = dictionaryUnmarshaller.Unmarshall(context);
				}
			}
			return createIdentityPoolResponse;
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
			if (errorResponse.Code != null && errorResponse.Code.Equals("LimitExceededException"))
			{
				return new LimitExceededException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("NotAuthorizedException"))
			{
				return new NotAuthorizedException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("ResourceConflictException"))
			{
				return new ResourceConflictException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			if (errorResponse.Code != null && errorResponse.Code.Equals("TooManyRequestsException"))
			{
				return new TooManyRequestsException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonCognitoIdentityException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static CreateIdentityPoolResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
