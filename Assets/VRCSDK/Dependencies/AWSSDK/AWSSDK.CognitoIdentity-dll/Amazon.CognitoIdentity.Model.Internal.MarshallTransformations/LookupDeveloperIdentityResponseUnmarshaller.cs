using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class LookupDeveloperIdentityResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static LookupDeveloperIdentityResponseUnmarshaller _instance = new LookupDeveloperIdentityResponseUnmarshaller();

		public static LookupDeveloperIdentityResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			LookupDeveloperIdentityResponse lookupDeveloperIdentityResponse = new LookupDeveloperIdentityResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("DeveloperUserIdentifierList", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					lookupDeveloperIdentityResponse.DeveloperUserIdentifierList = listUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					lookupDeveloperIdentityResponse.IdentityId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("NextToken", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					lookupDeveloperIdentityResponse.NextToken = instance2.Unmarshall(context);
				}
			}
			return lookupDeveloperIdentityResponse;
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

		internal static LookupDeveloperIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
