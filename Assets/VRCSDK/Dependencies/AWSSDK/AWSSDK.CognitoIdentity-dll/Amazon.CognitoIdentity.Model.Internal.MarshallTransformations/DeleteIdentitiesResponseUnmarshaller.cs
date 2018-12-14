using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class DeleteIdentitiesResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static DeleteIdentitiesResponseUnmarshaller _instance = new DeleteIdentitiesResponseUnmarshaller();

		public static DeleteIdentitiesResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			DeleteIdentitiesResponse deleteIdentitiesResponse = new DeleteIdentitiesResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("UnprocessedIdentityIds", currentDepth))
				{
					ListUnmarshaller<UnprocessedIdentityId, UnprocessedIdentityIdUnmarshaller> listUnmarshaller = new ListUnmarshaller<UnprocessedIdentityId, UnprocessedIdentityIdUnmarshaller>(UnprocessedIdentityIdUnmarshaller.Instance);
					deleteIdentitiesResponse.UnprocessedIdentityIds = listUnmarshaller.Unmarshall(context);
				}
			}
			return deleteIdentitiesResponse;
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
			if (errorResponse.Code != null && errorResponse.Code.Equals("TooManyRequestsException"))
			{
				return new TooManyRequestsException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
			}
			return new AmazonCognitoIdentityException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
		}

		internal static DeleteIdentitiesResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
