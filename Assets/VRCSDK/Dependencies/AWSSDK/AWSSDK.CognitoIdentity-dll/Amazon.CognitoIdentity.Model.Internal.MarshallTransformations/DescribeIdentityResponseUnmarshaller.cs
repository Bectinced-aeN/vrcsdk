using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.CognitoIdentity.Model.Internal.MarshallTransformations
{
	public class DescribeIdentityResponseUnmarshaller : JsonResponseUnmarshaller
	{
		private static DescribeIdentityResponseUnmarshaller _instance = new DescribeIdentityResponseUnmarshaller();

		public static DescribeIdentityResponseUnmarshaller Instance => _instance;

		public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
		{
			DescribeIdentityResponse describeIdentityResponse = new DescribeIdentityResponse();
			context.Read();
			int currentDepth = context.CurrentDepth;
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.TestExpression("CreationDate", currentDepth))
				{
					DateTimeUnmarshaller instance = DateTimeUnmarshaller.Instance;
					describeIdentityResponse.CreationDate = instance.Unmarshall(context);
				}
				else if (context.TestExpression("IdentityId", currentDepth))
				{
					StringUnmarshaller instance2 = StringUnmarshaller.Instance;
					describeIdentityResponse.IdentityId = instance2.Unmarshall(context);
				}
				else if (context.TestExpression("LastModifiedDate", currentDepth))
				{
					DateTimeUnmarshaller instance3 = DateTimeUnmarshaller.Instance;
					describeIdentityResponse.LastModifiedDate = instance3.Unmarshall(context);
				}
				else if (context.TestExpression("Logins", currentDepth))
				{
					ListUnmarshaller<string, StringUnmarshaller> listUnmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
					describeIdentityResponse.Logins = listUnmarshaller.Unmarshall(context);
				}
			}
			return describeIdentityResponse;
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

		internal static DescribeIdentityResponseUnmarshaller GetInstance()
		{
			return _instance;
		}
	}
}
