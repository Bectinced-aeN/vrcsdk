using System.Linq;
using System.Net;

namespace Amazon.S3.Util
{
	public class S3PostUploadResponse
	{
		public HttpStatusCode StatusCode
		{
			get;
			set;
		}

		public string RequestId
		{
			get;
			set;
		}

		public string HostId
		{
			get;
			set;
		}

		public string AmzCfId
		{
			get;
			set;
		}

		internal static S3PostUploadResponse FromWebResponse(HttpWebResponse response)
		{
			S3PostUploadResponse s3PostUploadResponse = new S3PostUploadResponse
			{
				StatusCode = response.StatusCode
			};
			if (response.Headers.AllKeys.Contains("x-amz-request-id"))
			{
				s3PostUploadResponse.RequestId = response.Headers["x-amz-request-id"];
			}
			if (response.Headers.AllKeys.Contains("x-amz-id-2"))
			{
				s3PostUploadResponse.HostId = response.Headers["x-amz-id-2"];
			}
			if (response.Headers.AllKeys.Contains("X-Amz-Cf-Id"))
			{
				s3PostUploadResponse.AmzCfId = response.Headers["X-Amz-Cf-Id"];
			}
			return s3PostUploadResponse;
		}
	}
}
