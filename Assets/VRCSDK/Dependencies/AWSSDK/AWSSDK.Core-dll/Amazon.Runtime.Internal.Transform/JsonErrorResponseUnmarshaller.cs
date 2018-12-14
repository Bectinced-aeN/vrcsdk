using System;

namespace Amazon.Runtime.Internal.Transform
{
	public class JsonErrorResponseUnmarshaller : IUnmarshaller<ErrorResponse, JsonUnmarshallerContext>
	{
		private static JsonErrorResponseUnmarshaller instance;

		public ErrorResponse Unmarshall(JsonUnmarshallerContext context)
		{
			ErrorResponse errorResponse = new ErrorResponse();
			if (context.Peek() == 60)
			{
				ErrorResponseUnmarshaller errorResponseUnmarshaller = new ErrorResponseUnmarshaller();
				XmlUnmarshallerContext context2 = new XmlUnmarshallerContext(context.Stream, maintainResponseBody: false, null);
				errorResponse = errorResponseUnmarshaller.Unmarshall(context2);
			}
			else
			{
				while (context.Read())
				{
					if (context.TestExpression("__type"))
					{
						string text = StringUnmarshaller.GetInstance().Unmarshall(context);
						errorResponse.Code = text.Substring(text.LastIndexOf("#", StringComparison.Ordinal) + 1);
						if (Enum.IsDefined(typeof(ErrorType), text))
						{
							errorResponse.Type = (ErrorType)Enum.Parse(typeof(ErrorType), text, ignoreCase: true);
						}
						else
						{
							errorResponse.Type = ErrorType.Unknown;
						}
					}
					else if (context.TestExpression("code"))
					{
						errorResponse.Code = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("message"))
					{
						errorResponse.Message = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				if (string.IsNullOrEmpty(errorResponse.Code) && context.ResponseData.IsHeaderPresent("x-amzn-ErrorType"))
				{
					string text2 = context.ResponseData.GetHeaderValue("x-amzn-ErrorType");
					if (!string.IsNullOrEmpty(text2))
					{
						int num = text2.IndexOf(":", StringComparison.Ordinal);
						if (num != -1)
						{
							text2 = text2.Substring(0, num);
						}
						errorResponse.Code = text2;
					}
				}
			}
			return errorResponse;
		}

		public static JsonErrorResponseUnmarshaller GetInstance()
		{
			if (instance == null)
			{
				instance = new JsonErrorResponseUnmarshaller();
			}
			return instance;
		}
	}
}
