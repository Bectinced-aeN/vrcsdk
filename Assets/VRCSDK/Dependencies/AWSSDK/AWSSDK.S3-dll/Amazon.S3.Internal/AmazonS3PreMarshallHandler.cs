using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Util;
using System;
using System.IO;
using System.Text;

namespace Amazon.S3.Internal
{
	public class AmazonS3PreMarshallHandler : PipelineHandler
	{
		public override void InvokeSync(IExecutionContext executionContext)
		{
			PreInvoke(executionContext);
			this.InvokeSync(executionContext);
		}

		public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
		{
			PreInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			return this.InvokeAsync(executionContext);
		}

		protected virtual void PreInvoke(IExecutionContext executionContext)
		{
			ProcessPreRequestHandlers(executionContext);
		}

		private static void ProcessPreRequestHandlers(IExecutionContext executionContext)
		{
			AmazonWebServiceRequest originalRequest = executionContext.get_RequestContext().get_OriginalRequest();
			IClientConfig clientConfig = executionContext.get_RequestContext().get_ClientConfig();
			PutObjectRequest putObjectRequest = originalRequest as PutObjectRequest;
			if (putObjectRequest != null)
			{
				if (putObjectRequest.InputStream != null && !string.IsNullOrEmpty(putObjectRequest.FilePath))
				{
					throw new ArgumentException("Please specify one of either an InputStream or a FilePath to be PUT as an S3 object.");
				}
				if (!string.IsNullOrEmpty(putObjectRequest.ContentBody) && !string.IsNullOrEmpty(putObjectRequest.FilePath))
				{
					throw new ArgumentException("Please specify one of either a FilePath or the ContentBody to be PUT as an S3 object.");
				}
				if (putObjectRequest.InputStream != null && !string.IsNullOrEmpty(putObjectRequest.ContentBody))
				{
					throw new ArgumentException("Please specify one of either an InputStream or the ContentBody to be PUT as an S3 object.");
				}
				if (!putObjectRequest.Headers.IsSetContentType())
				{
					string text = null;
					if (!string.IsNullOrEmpty(putObjectRequest.FilePath))
					{
						text = AWSSDKUtils.GetExtension(putObjectRequest.FilePath);
					}
					if (string.IsNullOrEmpty(text) && putObjectRequest.IsSetKey())
					{
						text = AWSSDKUtils.GetExtension(putObjectRequest.Key);
					}
					if (!string.IsNullOrEmpty(text))
					{
						putObjectRequest.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(text);
					}
				}
				if (putObjectRequest.InputStream != null && putObjectRequest.AutoResetStreamPosition && putObjectRequest.InputStream.CanSeek)
				{
					putObjectRequest.InputStream.Seek(0L, SeekOrigin.Begin);
				}
				if (!string.IsNullOrEmpty(putObjectRequest.FilePath))
				{
					putObjectRequest.SetupForFilePath();
				}
				else if (putObjectRequest.InputStream == null)
				{
					if (string.IsNullOrEmpty(putObjectRequest.Headers.ContentType))
					{
						putObjectRequest.Headers.ContentType = "text/plain";
					}
					byte[] bytes = Encoding.UTF8.GetBytes(putObjectRequest.ContentBody ?? "");
					putObjectRequest.InputStream = new MemoryStream(bytes);
				}
			}
			PutBucketRequest putBucketRequest = originalRequest as PutBucketRequest;
			if (putBucketRequest != null && putBucketRequest.UseClientRegion && !putBucketRequest.IsSetBucketRegionName() && !putBucketRequest.IsSetBucketRegion())
			{
				string text2 = DetermineBucketRegionCode(clientConfig);
				if (text2 == "us-east-1")
				{
					text2 = null;
				}
				else if (text2 == "eu-west-1")
				{
					text2 = "EU";
				}
				putBucketRequest.BucketRegion = text2;
			}
			DeleteBucketRequest deleteBucketRequest = originalRequest as DeleteBucketRequest;
			if (deleteBucketRequest != null && deleteBucketRequest.UseClientRegion && !deleteBucketRequest.IsSetBucketRegion())
			{
				string text3 = DetermineBucketRegionCode(clientConfig);
				if (text3 == "us-east-1")
				{
					text3 = null;
				}
				if (text3 != null)
				{
					deleteBucketRequest.BucketRegion = text3;
				}
			}
			UploadPartRequest uploadPartRequest = originalRequest as UploadPartRequest;
			if (uploadPartRequest != null)
			{
				if (uploadPartRequest.InputStream != null && !string.IsNullOrEmpty(uploadPartRequest.FilePath))
				{
					throw new ArgumentException("Please specify one of either a InputStream or a FilePath to be PUT as an S3 object.");
				}
				if (uploadPartRequest.IsSetFilePath())
				{
					uploadPartRequest.SetupForFilePath();
				}
			}
			InitiateMultipartUploadRequest initiateMultipartUploadRequest = originalRequest as InitiateMultipartUploadRequest;
			if (initiateMultipartUploadRequest != null && !initiateMultipartUploadRequest.Headers.IsSetContentType())
			{
				string extension = AWSSDKUtils.GetExtension(initiateMultipartUploadRequest.Key);
				if (!string.IsNullOrEmpty(extension))
				{
					initiateMultipartUploadRequest.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(extension);
				}
			}
		}

		private static string DetermineBucketRegionCode(IClientConfig config)
		{
			if (config.get_RegionEndpoint() != null && string.IsNullOrEmpty(config.get_ServiceURL()))
			{
				return config.get_RegionEndpoint().get_SystemName();
			}
			return AWSSDKUtils.DetermineRegion(config.DetermineServiceURL());
		}

		public AmazonS3PreMarshallHandler()
			: this()
		{
		}
	}
}
