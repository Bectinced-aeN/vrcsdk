using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Amazon.S3.Internal
{
	public class AmazonS3PostMarshallHandler : PipelineHandler
	{
		private static HashSet<Type> UnsupportedAccelerateRequestTypes = new HashSet<Type>
		{
			typeof(ListBucketsRequest),
			typeof(PutBucketRequest),
			typeof(DeleteBucketRequest),
			typeof(CopyObjectRequest),
			typeof(CopyPartRequest)
		};

		private static HashSet<string> sseKeyHeaders = new HashSet<string>
		{
			"x-amz-server-side-encryption-customer-key",
			"x-amz-server-side-encryption-aws-kms-key-id"
		};

		private static char[] separators = new char[2]
		{
			'/',
			'?'
		};

		private static Regex bucketValidationRegex = new Regex("^[A-Za-z0-9._\\-]+$");

		private static Regex dnsValidationRegex1 = new Regex("^[a-z0-9][a-z0-9.-]+[a-z0-9]$");

		private static Regex dnsValidationRegex2 = new Regex("(\\d+\\.){3}\\d+");

		private static string[] invalidPatterns = new string[3]
		{
			"..",
			"-.",
			".-"
		};

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
			ProcessRequestHandlers(executionContext);
		}

		public static void ProcessRequestHandlers(IExecutionContext executionContext)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			IRequest request = executionContext.get_RequestContext().get_Request();
			IClientConfig clientConfig = executionContext.get_RequestContext().get_ClientConfig();
			if (request.get_Headers().TryGetValue("x-amz-server-side-encryption", out string value) && string.Equals(value, ServerSideEncryptionMethod.AWSKMS.get_Value(), StringComparison.Ordinal))
			{
				request.set_UseSigV4(true);
			}
			string bucketName = GetBucketName(request.get_ResourcePath());
			if (!string.IsNullOrEmpty(bucketName))
			{
				AmazonS3Config amazonS3Config = clientConfig as AmazonS3Config;
				if (amazonS3Config == null)
				{
					throw new AmazonClientException("Current config object is not of type AmazonS3Config");
				}
				bool flag = IsDnsCompatibleBucketName(bucketName);
				UriBuilder uriBuilder = new UriBuilder(EndpointResolver.DetermineEndpoint(amazonS3Config, request));
				bool flag2 = string.Equals(uriBuilder.Scheme, "http", StringComparison.OrdinalIgnoreCase);
				if (!amazonS3Config.ForcePathStyle && flag && (flag2 || bucketName.IndexOf('.') < 0))
				{
					uriBuilder.Host = bucketName + "." + uriBuilder.Host;
					request.set_Endpoint(uriBuilder.Uri);
					string text = request.get_ResourcePath();
					string text2 = "/" + bucketName;
					if (text.IndexOf(text2, StringComparison.Ordinal) == 0)
					{
						text = text.Substring(text2.Length);
					}
					request.set_ResourcePath(text);
					request.set_CanonicalResourcePrefix(text2);
				}
				if (amazonS3Config.UseAccelerateEndpoint)
				{
					if (!flag || BucketNameContainsPeriod(bucketName))
					{
						throw new AmazonClientException("S3 accelerate is enabled for this request but the bucket name is not accelerate compatible. The bucket name must be DNS compatible (http://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html) and must not contain any period (.) characters to be accelerate compatible.");
					}
					AmazonWebServiceRequest originalRequest = request.get_OriginalRequest();
					if (!UnsupportedAccelerateRequestTypes.Contains(((object)originalRequest).GetType()))
					{
						request.set_Endpoint(GetAccelerateEndpoint(bucketName, amazonS3Config));
						if (request.get_UseSigV4() && amazonS3Config.get_RegionEndpoint() != null)
						{
							request.set_AlternateEndpoint(amazonS3Config.get_RegionEndpoint());
						}
					}
				}
				if (flag2)
				{
					ValidateHttpsOnlyHeaders(request);
				}
			}
		}

		private static Uri GetAccelerateEndpoint(string bucketName, AmazonS3Config config)
		{
			return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}", config.get_UseHttp() ? "http://" : "https://", bucketName, config.AccelerateEndpoint));
		}

		private static void ValidateHttpsOnlyHeaders(IRequest request)
		{
			ValidateSseKeyHeaders(request);
			ValidateSseHeaderValue(request);
		}

		private static void ValidateSseHeaderValue(IRequest request)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (request.get_Headers().TryGetValue("x-amz-server-side-encryption", out string value) && string.Equals(value, ConstantClass.op_Implicit(ServerSideEncryptionMethod.AWSKMS)))
			{
				throw new AmazonClientException("Request specifying Server Side Encryption with AWS KMS managed keys can only be transmitted over HTTPS");
			}
		}

		private static void ValidateSseKeyHeaders(IRequest request)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			string[] array = (from kvp in request.get_Headers().Where(delegate(KeyValuePair<string, string> kvp)
			{
				if (!string.IsNullOrEmpty(kvp.Value))
				{
					return sseKeyHeaders.Contains(kvp.Key);
				}
				return false;
			})
			select kvp.Key).ToArray();
			if (array.Length != 0)
			{
				throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture, "Request contains headers which can only be transmitted over HTTPS: {0}", string.Join(", ", array)));
			}
		}

		internal static string GetBucketName(string resourcePath)
		{
			resourcePath = resourcePath.Trim().Trim(separators);
			return resourcePath.Split(separators, 2)[0];
		}

		public static bool IsValidBucketName(string bucketName)
		{
			if (string.IsNullOrEmpty(bucketName))
			{
				return false;
			}
			if (bucketName.Length < 3 || bucketName.Length > 255)
			{
				return false;
			}
			if (bucketName.IndexOf('\n') >= 0)
			{
				return false;
			}
			if (!bucketValidationRegex.IsMatch(bucketName))
			{
				return false;
			}
			return true;
		}

		public static bool IsDnsCompatibleBucketName(string bucketName)
		{
			if (!IsValidBucketName(bucketName))
			{
				return false;
			}
			if (bucketName.Length > 63)
			{
				return false;
			}
			if (!dnsValidationRegex1.IsMatch(bucketName))
			{
				return false;
			}
			if (dnsValidationRegex2.IsMatch(bucketName))
			{
				return false;
			}
			if (StringContainsAny(bucketName, invalidPatterns, StringComparison.Ordinal))
			{
				return false;
			}
			return true;
		}

		public static bool BucketNameContainsPeriod(string bucketName)
		{
			return bucketName.IndexOf(".", StringComparison.Ordinal) >= 0;
		}

		private static bool StringContainsAny(string toCheck, string[] values, StringComparison stringComparison)
		{
			foreach (string value in values)
			{
				if (toCheck.IndexOf(value, stringComparison) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public AmazonS3PostMarshallHandler()
			: this()
		{
		}
	}
}
