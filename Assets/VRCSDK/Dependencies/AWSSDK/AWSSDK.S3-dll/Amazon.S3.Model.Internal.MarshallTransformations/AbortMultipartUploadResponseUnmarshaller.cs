using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AbortMultipartUploadResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static AbortMultipartUploadResponseUnmarshaller _instance;

		public static AbortMultipartUploadResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AbortMultipartUploadResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			AbortMultipartUploadResponse abortMultipartUploadResponse = new AbortMultipartUploadResponse();
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				abortMultipartUploadResponse.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
			return abortMultipartUploadResponse;
		}
	}
}
