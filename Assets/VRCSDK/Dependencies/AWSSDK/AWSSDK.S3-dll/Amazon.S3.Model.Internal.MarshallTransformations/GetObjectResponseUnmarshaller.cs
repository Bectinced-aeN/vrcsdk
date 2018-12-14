using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using System;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetObjectResponseUnmarshaller _instance;

		public override bool HasStreamingProperty => true;

		public static GetObjectResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetObjectResponse getObjectResponse = new GetObjectResponse();
			UnmarshallResult(context, getObjectResponse);
			return getObjectResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetObjectResponse response)
		{
			response.ResponseStream = context.get_Stream();
			IWebResponseData responseData = context.get_ResponseData();
			if (responseData.IsHeaderPresent("x-amz-delete-marker"))
			{
				response.DeleteMarker = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-delete-marker"));
			}
			if (responseData.IsHeaderPresent("accept-ranges"))
			{
				response.AcceptRanges = S3Transforms.ToString(responseData.GetHeaderValue("accept-ranges"));
			}
			if (responseData.IsHeaderPresent("x-amz-expiration"))
			{
				response.Expiration = new Expiration(responseData.GetHeaderValue("x-amz-expiration"));
			}
			if (responseData.IsHeaderPresent("x-amz-restore"))
			{
				AmazonS3Util.ParseAmzRestoreHeader(responseData.GetHeaderValue("x-amz-restore"), out bool restoreInProgress, out DateTime? restoreExpiration);
				response.RestoreInProgress = restoreInProgress;
				response.RestoreExpiration = restoreExpiration;
			}
			if (responseData.IsHeaderPresent("Last-Modified"))
			{
				response.LastModified = S3Transforms.ToDateTime(responseData.GetHeaderValue("Last-Modified"));
			}
			if (responseData.IsHeaderPresent("ETag"))
			{
				response.ETag = S3Transforms.ToString(responseData.GetHeaderValue("ETag"));
			}
			if (responseData.IsHeaderPresent("x-amz-missing-meta"))
			{
				response.MissingMeta = S3Transforms.ToInt(responseData.GetHeaderValue("x-amz-missing-meta"));
			}
			if (responseData.IsHeaderPresent("x-amz-version-id"))
			{
				response.VersionId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-version-id"));
			}
			if (responseData.IsHeaderPresent("Cache-Control"))
			{
				response.Headers.CacheControl = S3Transforms.ToString(responseData.GetHeaderValue("Cache-Control"));
			}
			if (responseData.IsHeaderPresent("Content-Disposition"))
			{
				response.Headers.ContentDisposition = S3Transforms.ToString(responseData.GetHeaderValue("Content-Disposition"));
			}
			if (responseData.IsHeaderPresent("Content-Encoding"))
			{
				response.Headers.ContentEncoding = S3Transforms.ToString(responseData.GetHeaderValue("Content-Encoding"));
			}
			if (responseData.IsHeaderPresent("Content-Length"))
			{
				response.Headers.ContentLength = long.Parse(responseData.GetHeaderValue("Content-Length"), CultureInfo.InvariantCulture);
			}
			if (responseData.IsHeaderPresent("Content-Type"))
			{
				response.Headers.ContentType = S3Transforms.ToString(responseData.GetHeaderValue("Content-Type"));
			}
			if (responseData.IsHeaderPresent("Expires"))
			{
				response.RawExpires = S3Transforms.ToString(responseData.GetHeaderValue("Expires"));
			}
			if (responseData.IsHeaderPresent("x-amz-website-redirect-location"))
			{
				response.WebsiteRedirectLocation = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-website-redirect-location"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption"))
			{
				response.ServerSideEncryptionMethod = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption-customer-algorithm"))
			{
				response.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.FindValue(responseData.GetHeaderValue("x-amz-server-side-encryption-customer-algorithm"));
			}
			if (responseData.IsHeaderPresent("x-amz-server-side-encryption-aws-kms-key-id"))
			{
				response.ServerSideEncryptionKeyManagementServiceKeyId = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-server-side-encryption-aws-kms-key-id"));
			}
			if (responseData.IsHeaderPresent("x-amz-replication-status"))
			{
				response.ReplicationStatus = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-replication-status"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderMultipartPartsCount))
			{
				response.PartsCount = S3Transforms.ToInt(responseData.GetHeaderValue(S3Constants.AmzHeaderMultipartPartsCount));
			}
			if (responseData.IsHeaderPresent("x-amz-storage-class"))
			{
				response.StorageClass = S3Transforms.ToString(responseData.GetHeaderValue("x-amz-storage-class"));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderRequestCharged))
			{
				response.RequestCharged = RequestCharged.FindValue(responseData.GetHeaderValue(S3Constants.AmzHeaderRequestCharged));
			}
			if (responseData.IsHeaderPresent(S3Constants.AmzHeaderTaggingCount))
			{
				response.TagCount = S3Transforms.ToInt(responseData.GetHeaderValue(S3Constants.AmzHeaderTaggingCount));
			}
			string[] headerNames = responseData.GetHeaderNames();
			foreach (string text in headerNames)
			{
				if (text.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
				{
					response.Metadata[text] = responseData.GetHeaderValue(text);
				}
			}
		}
	}
}
