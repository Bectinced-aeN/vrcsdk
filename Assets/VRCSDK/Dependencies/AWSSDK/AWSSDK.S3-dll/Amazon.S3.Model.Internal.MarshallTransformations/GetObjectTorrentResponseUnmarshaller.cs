using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetObjectTorrentResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetObjectTorrentResponseUnmarshaller _instance;

		public override bool HasStreamingProperty => true;

		public static GetObjectTorrentResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectTorrentResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetObjectTorrentResponse getObjectTorrentResponse = new GetObjectTorrentResponse();
			UnmarshallResult(context, getObjectTorrentResponse);
			return getObjectTorrentResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetObjectTorrentResponse response)
		{
			response.ResponseStream = context.get_Stream();
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
		}
	}
}
