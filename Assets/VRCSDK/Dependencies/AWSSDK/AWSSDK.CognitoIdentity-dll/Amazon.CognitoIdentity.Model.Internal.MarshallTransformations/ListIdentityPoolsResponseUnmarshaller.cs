using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class ListIdentityPoolsResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static ListIdentityPoolsResponseUnmarshaller _instance = new ListIdentityPoolsResponseUnmarshaller();

		public static ListIdentityPoolsResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			ListIdentityPoolsResponse listIdentityPoolsResponse = new ListIdentityPoolsResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("IdentityPools", currentDepth))
				{
					ListUnmarshaller<IdentityPoolShortDescription, IdentityPoolShortDescriptionUnmarshaller> listUnmarshaller = new ListUnmarshaller<IdentityPoolShortDescription, IdentityPoolShortDescriptionUnmarshaller>(IdentityPoolShortDescriptionUnmarshaller.Instance);
					listIdentityPoolsResponse.IdentityPools = listUnmarshaller.Unmarshall(context);
				}
				else if (context.TestExpression("NextToken", currentDepth))
				{
					StringUnmarshaller instance = StringUnmarshaller.Instance;
					listIdentityPoolsResponse.NextToken = instance.Unmarshall(context);
				}
			}
			return listIdentityPoolsResponse;
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
			if (errorResponse.Code != null && errorResponse.Code.Equals("TooManyRequestsException"))
			{
				return new TooManyRequestsException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonCognitoIdentityException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static ListIdentityPoolsResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
