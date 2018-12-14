using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;
using Amazon.Util;
using System;
using System.IO;
using System.Linq;

namespace Amazon.S3.Internal
{
	public class AmazonS3ResponseHandler : PipelineHandler
	{
		private static char[] etagTrimChars = new char[1]
		{
			'"'
		};

		public override void InvokeSync(IExecutionContext executionContext)
		{
			this.InvokeSync(executionContext);
			PostInvoke(executionContext);
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (executionContext.get_ResponseContext().get_AsyncResult().get_Exception() == null)
			{
				PostInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			}
			this.InvokeAsyncCallback(executionContext);
		}

		protected virtual void PostInvoke(IExecutionContext executionContext)
		{
			ProcessResponseHandlers(executionContext);
		}

		private static void ProcessResponseHandlers(IExecutionContext executionContext)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			AmazonWebServiceResponse response = executionContext.get_ResponseContext().get_Response();
			IRequest request = executionContext.get_RequestContext().get_Request();
			bool flag = HasSSEHeaders(executionContext.get_ResponseContext().get_HttpResponse());
			GetObjectResponse getObjectResponse = response as GetObjectResponse;
			if (getObjectResponse != null)
			{
				GetObjectRequest getObjectRequest = request.get_OriginalRequest() as GetObjectRequest;
				getObjectResponse.BucketName = getObjectRequest.BucketName;
				getObjectResponse.Key = getObjectRequest.Key;
				if (!string.IsNullOrEmpty(getObjectResponse.ETag) && !getObjectResponse.ETag.Contains("-") && !flag && getObjectRequest.ByteRange == null)
				{
					byte[] array = AWSSDKUtils.HexStringToBytes(getObjectResponse.ETag.Trim(etagTrimChars));
					HashStream val = getObjectResponse.ResponseStream = (Stream)new MD5Stream(getObjectResponse.ResponseStream, array, getObjectResponse.get_ContentLength());
				}
			}
			DeleteObjectsResponse deleteObjectsResponse = response as DeleteObjectsResponse;
			if (deleteObjectsResponse != null && deleteObjectsResponse.DeleteErrors != null && deleteObjectsResponse.DeleteErrors.Count > 0)
			{
				throw new DeleteObjectsException(deleteObjectsResponse);
			}
			PutObjectResponse putObjectResponse = response as PutObjectResponse;
			PutObjectRequest putObjectRequest = request.get_OriginalRequest() as PutObjectRequest;
			if (putObjectRequest != null)
			{
				HashStream val2 = putObjectRequest.InputStream as HashStream;
				if (val2 != null)
				{
					if (putObjectResponse != null && !flag)
					{
						val2.CalculateHash();
						CompareHashes(putObjectResponse.ETag, val2.get_CalculatedHash());
					}
					putObjectRequest.InputStream = val2.GetNonWrapperBaseStream();
				}
			}
			ListObjectsResponse listObjectsResponse = response as ListObjectsResponse;
			if (listObjectsResponse != null && listObjectsResponse.IsTruncated && string.IsNullOrEmpty(listObjectsResponse.NextMarker) && listObjectsResponse.S3Objects.Count > 0)
			{
				listObjectsResponse.NextMarker = listObjectsResponse.S3Objects.Last().Key;
			}
			UploadPartRequest uploadPartRequest = request.get_OriginalRequest() as UploadPartRequest;
			UploadPartResponse uploadPartResponse = response as UploadPartResponse;
			if (uploadPartRequest != null)
			{
				if (uploadPartResponse != null)
				{
					uploadPartResponse.PartNumber = uploadPartRequest.PartNumber;
				}
				HashStream val3 = uploadPartRequest.InputStream as HashStream;
				if (val3 != null)
				{
					if (uploadPartResponse != null && !flag)
					{
						val3.CalculateHash();
						CompareHashes(uploadPartResponse.ETag, val3.get_CalculatedHash());
					}
					uploadPartRequest.InputStream = val3.GetNonWrapperBaseStream();
				}
			}
			CopyPartResponse copyPartResponse = response as CopyPartResponse;
			if (copyPartResponse != null)
			{
				copyPartResponse.PartNumber = ((CopyPartRequest)request.get_OriginalRequest()).PartNumber;
			}
			AmazonS3Client.CleanupRequest(request);
		}

		private static bool HasSSEHeaders(IWebResponseData webResponseData)
		{
			bool num = !string.IsNullOrEmpty(webResponseData.GetHeaderValue("x-amz-server-side-encryption-customer-algorithm"));
			bool flag = !string.IsNullOrEmpty(webResponseData.GetHeaderValue("x-amz-server-side-encryption-aws-kms-key-id"));
			return num | flag;
		}

		private static void CompareHashes(string etag, byte[] hash)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(etag) && !etag.Contains("-"))
			{
				etag = etag.Trim(etagTrimChars);
				string b = AWSSDKUtils.BytesToHexString(hash);
				if (!string.Equals(etag, b, StringComparison.OrdinalIgnoreCase))
				{
					throw new AmazonClientException("Expected hash not equal to calculated hash");
				}
			}
		}

		public AmazonS3ResponseHandler()
			: this()
		{
		}
	}
}
