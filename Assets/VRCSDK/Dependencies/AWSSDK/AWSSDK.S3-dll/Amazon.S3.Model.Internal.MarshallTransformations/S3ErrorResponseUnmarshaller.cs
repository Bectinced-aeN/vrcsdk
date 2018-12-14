using Amazon.Runtime.Internal.Transform;
using System;
using System.Net;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class S3ErrorResponseUnmarshaller : IUnmarshaller<S3ErrorResponse, XmlUnmarshallerContext>
	{
		private const string XML_CONTENT_TYPE = "text/xml";

		private static S3ErrorResponseUnmarshaller _instance;

		public static S3ErrorResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new S3ErrorResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public S3ErrorResponse Unmarshall(XmlUnmarshallerContext context)
		{
			S3ErrorResponse s3ErrorResponse = new S3ErrorResponse();
			HttpStatusCode statusCode = context.get_ResponseData().get_StatusCode();
			s3ErrorResponse.set_Code(statusCode.ToString());
			if (context.get_ResponseData().IsHeaderPresent("x-amz-request-id"))
			{
				s3ErrorResponse.set_RequestId(context.get_ResponseData().GetHeaderValue("x-amz-request-id"));
			}
			if (context.get_ResponseData().IsHeaderPresent("x-amz-id-2"))
			{
				s3ErrorResponse.Id2 = context.get_ResponseData().GetHeaderValue("x-amz-id-2");
			}
			if (context.get_ResponseData().IsHeaderPresent("X-Amz-Cf-Id"))
			{
				s3ErrorResponse.AmzCfId = context.get_ResponseData().GetHeaderValue("X-Amz-Cf-Id");
			}
			if (statusCode >= HttpStatusCode.InternalServerError)
			{
				s3ErrorResponse.set_Type(1);
			}
			else if (statusCode >= HttpStatusCode.BadRequest)
			{
				s3ErrorResponse.set_Type(0);
			}
			else
			{
				s3ErrorResponse.set_Type(2);
			}
			string text = null;
			if (context.get_ResponseData().IsHeaderPresent("Content-Length"))
			{
				text = context.get_ResponseData().GetHeaderValue("Content-Length");
			}
			string text2 = "text/xml";
			if (context.get_ResponseData().IsHeaderPresent("Content-Type"))
			{
				text2 = context.get_ResponseData().GetHeaderValue("Content-Type");
			}
			if (string.IsNullOrEmpty(text) || !long.TryParse(text, out long result))
			{
				result = -1L;
			}
			if (result < 0)
			{
				try
				{
					result = context.get_Stream().Length;
				}
				catch
				{
					result = -1L;
				}
			}
			if (context.get_Stream().CanRead && result != 0L && text2.EndsWith("/xml", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					while (context.Read())
					{
						if (context.get_IsStartElement())
						{
							if (context.TestExpression("Error/Code"))
							{
								s3ErrorResponse.set_Code(StringUnmarshaller.GetInstance().Unmarshall(context));
							}
							else if (context.TestExpression("Error/Message"))
							{
								s3ErrorResponse.set_Message(StringUnmarshaller.GetInstance().Unmarshall(context));
							}
							else if (context.TestExpression("Error/Resource"))
							{
								s3ErrorResponse.Resource = StringUnmarshaller.GetInstance().Unmarshall(context);
							}
							else if (context.TestExpression("Error/RequestId"))
							{
								s3ErrorResponse.set_RequestId(StringUnmarshaller.GetInstance().Unmarshall(context));
							}
							else if (context.TestExpression("Error/HostId"))
							{
								s3ErrorResponse.Id2 = StringUnmarshaller.GetInstance().Unmarshall(context);
							}
							else if (context.TestExpression("Error/Region"))
							{
								s3ErrorResponse.Region = StringUnmarshaller.GetInstance().Unmarshall(context);
							}
						}
					}
					return s3ErrorResponse;
				}
				catch (Exception ex)
				{
					Exception ex3 = s3ErrorResponse.ParsingException = ex;
					return s3ErrorResponse;
				}
			}
			return s3ErrorResponse;
		}
	}
}
