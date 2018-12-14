using Amazon.Runtime.Internal.Transform;
using System.IO;
using System.Net;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class S3UnmarshallerContext : XmlUnmarshallerContext
	{
		private bool _checkedForErrorResponse;

		public S3UnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData responseData)
			: this(responseStream, maintainResponseBody, responseData)
		{
		}

		public override bool Read()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			bool result = this.Read();
			if (this.get_ResponseData().get_StatusCode() == HttpStatusCode.OK && !_checkedForErrorResponse && this.get_IsStartElement())
			{
				if (this.TestExpression("Error", 1))
				{
					S3ErrorResponse s3ErrorResponse = new S3ErrorResponseUnmarshaller().Unmarshall(this);
					throw new AmazonS3Exception(s3ErrorResponse.get_Message(), null, s3ErrorResponse.get_Type(), s3ErrorResponse.get_Code(), s3ErrorResponse.get_RequestId(), this.get_ResponseData().get_StatusCode(), s3ErrorResponse.Id2, s3ErrorResponse.AmzCfId)
					{
						Region = s3ErrorResponse.Region
					};
				}
				_checkedForErrorResponse = true;
			}
			return result;
		}
	}
}
