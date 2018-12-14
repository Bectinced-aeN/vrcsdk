using System;

namespace Amazon.Runtime.Internal.Transform
{
	public class ErrorResponseUnmarshaller : IUnmarshaller<ErrorResponse, XmlUnmarshallerContext>
	{
		private static ErrorResponseUnmarshaller instance;

		public ErrorResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ErrorResponse errorResponse = new ErrorResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					if (context.TestExpression("Error/Type"))
					{
						try
						{
							errorResponse.Type = (ErrorType)Enum.Parse(typeof(ErrorType), StringUnmarshaller.GetInstance().Unmarshall(context), ignoreCase: true);
						}
						catch (ArgumentException)
						{
							errorResponse.Type = ErrorType.Unknown;
						}
					}
					else if (context.TestExpression("Error/Code"))
					{
						errorResponse.Code = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Error/Message"))
					{
						errorResponse.Message = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("RequestId"))
					{
						errorResponse.RequestId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
			}
			return errorResponse;
		}

		public static ErrorResponseUnmarshaller GetInstance()
		{
			if (instance == null)
			{
				instance = new ErrorResponseUnmarshaller();
			}
			return instance;
		}
	}
}
