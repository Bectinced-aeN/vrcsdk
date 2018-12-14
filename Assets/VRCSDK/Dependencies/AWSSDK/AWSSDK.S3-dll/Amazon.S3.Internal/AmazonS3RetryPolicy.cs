using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace Amazon.S3.Internal
{
	public class AmazonS3RetryPolicy : DefaultRetryPolicy
	{
		private const string AWS_KMS_Signature_Error = "AWS KMS managed keys require AWS Signature Version 4";

		private static ICollection<Type> RequestsWith200Error = new HashSet<Type>
		{
			typeof(CopyObjectRequest),
			typeof(CopyPartRequest),
			typeof(CompleteMultipartUploadRequest)
		};

		public AmazonS3RetryPolicy(IClientConfig config)
			: this(config)
		{
		}

		public bool? RetryForExceptionSync(IExecutionContext executionContext, Exception exception)
		{
			AmazonServiceException val = exception as AmazonServiceException;
			if (val != null)
			{
				if (val.get_StatusCode() == HttpStatusCode.OK)
				{
					Type type = ((object)executionContext.get_RequestContext().get_OriginalRequest()).GetType();
					if (RequestsWith200Error.Contains(type))
					{
						return true;
					}
				}
				if (val.get_StatusCode() == HttpStatusCode.BadRequest)
				{
					if (new Uri(executionContext.get_RequestContext().get_ClientConfig().DetermineServiceURL()).Host.Equals("s3.amazonaws.com") && (((Exception)val).Message.Contains("AWS4-HMAC-SHA256") || ((Exception)val).Message.Contains("AWS KMS managed keys require AWS Signature Version 4")))
					{
						this.get_Logger().InfoFormat("Request {0}: the bucket you are attempting to access should be addressed using a region-specific endpoint. Additional calls will be made to attempt to determine the correct region to be used. For better performance configure your client to use the correct region.", new object[1]
						{
							executionContext.get_RequestContext().get_RequestName()
						});
						IRequest request = executionContext.get_RequestContext().get_Request();
						AmazonS3Uri amazonS3Uri = new AmazonS3Uri(request.get_Endpoint());
						string uriString = string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}", amazonS3Uri.Bucket, "s3-external-1.amazonaws.com");
						request.set_Endpoint(new Uri(uriString));
						if (((Exception)val).Message.Contains("AWS KMS managed keys require AWS Signature Version 4"))
						{
							request.set_UseSigV4(true);
							request.set_AuthenticationRegion(RegionEndpoint.USEast1.get_SystemName());
							executionContext.get_RequestContext().set_IsSigned(false);
						}
						return true;
					}
					return null;
				}
			}
			return this.RetryForException(executionContext, exception);
		}
	}
}
