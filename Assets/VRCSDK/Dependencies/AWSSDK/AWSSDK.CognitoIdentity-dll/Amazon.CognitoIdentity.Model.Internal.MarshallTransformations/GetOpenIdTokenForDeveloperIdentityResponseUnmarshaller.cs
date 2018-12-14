using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller _instance = new GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller();

		public static GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			GetOpenIdTokenForDeveloperIdentityResponse getOpenIdTokenForDeveloperIdentityResponse = new GetOpenIdTokenForDeveloperIdentityResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("IdentityId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					getOpenIdTokenForDeveloperIdentityResponse.IdentityId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("Token", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					getOpenIdTokenForDeveloperIdentityResponse.Token = instance2.Unmarshall(context);
				}
			}
			return getOpenIdTokenForDeveloperIdentityResponse;
		}

		public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
		{
			ErrorResponse errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
			if (errorResponse.Code != null && errorResponse.Code.Equals("DeveloperUserAlreadyRegisteredException"))
			{
				return new DeveloperUserAlreadyRegisteredException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
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
			if (errorResponse.Code != null && errorResponse.Code.Equals("ResourceConflictException"))
			{
				return new ResourceConflictException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
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

		internal static GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
