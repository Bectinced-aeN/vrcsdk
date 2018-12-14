using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class ListIdentitiesResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static ListIdentitiesResponseUnmarshaller _instance = new ListIdentitiesResponseUnmarshaller();

		public static ListIdentitiesResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			ListIdentitiesResponse listIdentitiesResponse = new ListIdentitiesResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("Identities", currentDepth))
				{
					ListUnmarshaller<IdentityDescription, IdentityDescriptionUnmarshaller> listUnmarshaller = new ListUnmarshaller<IdentityDescription, IdentityDescriptionUnmarshaller>(IdentityDescriptionUnmarshaller.Instance);
					listIdentitiesResponse.Identities = listUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityPoolId", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					listIdentitiesResponse.IdentityPoolId = instance.Unmarshall(context);
				}
				else if (context.TestExpression("NextToken", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					listIdentitiesResponse.NextToken = instance2.Unmarshall(context);
				}
			}
			return listIdentitiesResponse;
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

		internal static ListIdentitiesResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
