using System;
using System.Collections.Generic;

namespace Amazon.S3.Util
{
	internal static class S3Constants
	{
		internal const int PutObjectDefaultTimeout = 1200000;

		internal static readonly long MinPartSize = 5 * (long)Math.Pow(2.0, 20.0);

		internal const int MaxNumberOfParts = 10000;

		internal const int DefaultBufferSize = 8192;

		internal const string S3DefaultEndpoint = "s3.amazonaws.com";

		internal const string S3AlternateDefaultEndpoint = "s3-external-1.amazonaws.com";

		internal const int MinBucketLength = 3;

		internal const int MaxBucketLength = 63;

		internal const int MULTIPLE_OBJECT_DELETE_LIMIT = 1000;

		internal const string AmzGrantHeaderRead = "x-amz-grant-read";

		internal const string AmzGrantHeaderWrite = "x-amz-grant-write";

		internal const string AmzGrantHeaderReadAcp = "x-amz-grant-read-acp";

		internal const string AmzGrantHeaderWriteAcp = "x-amz-grant-write-acp";

		internal const string AmzGrantHeaderRestoreObject = "x-amz-grant-restore-object";

		internal const string AmzGrantHeaderFullControl = "x-amz-grant-full-control";

		internal static string PostFormDataObjectKey = "key";

		internal static string PostFormDataAcl = "acl";

		internal static string PostFormDataRedirect = "success_action_redirect";

		internal static string PostFormDataStatus = "success_action_status";

		internal static string PostFormDataContentType = "Content-Type";

		internal static string PostFormDataMetaPrefix = "x-amz-meta-";

		internal static string PostFormDataXAmzPrefix = "x-amz-";

		internal static string PostFormDataAccessKeyId = "AWSAccessKeyId";

		internal static string PostFormDataPolicy = "Policy";

		internal static string PostFormDataSignature = "Signature";

		internal static string PostFormDataXAmzSignature = "x-amz-signature";

		internal static string PostFormDataXAmzAlgorithm = "x-amz-algorithm";

		internal static string PostFormDataXAmzCredential = "x-amz-credential";

		internal static string PostFormDataXAmzDate = "x-amz-date";

		internal static string PostFormDataSecurityToken = "x-amz-security-token";

		internal static string AmzHeaderMultipartPartsCount = "x-amz-mp-parts-count";

		internal static string AmzHeaderRequestPayer = "x-amz-request-payer";

		internal static string AmzHeaderRequestCharged = "x-amz-request-charged";

		internal static string AmzHeaderTagging = "x-amz-tagging";

		internal static string AmzHeaderTaggingDirective = "x-amz-tagging-directive";

		internal static string AmzHeaderTaggingCount = "x-amz-tagging-count";

		internal static readonly string[] BucketVersions = new string[3]
		{
			"",
			"V1",
			"V2"
		};

		internal const string REGION_US_EAST_1 = "us-east-1";

		internal const string REGION_EU_WEST_1 = "eu-west-1";

		internal static readonly string[] MetadataDirectives = new string[2]
		{
			"COPY",
			"REPLACE"
		};

		internal const string VersioningOff = "Off";

		internal const string VersioningSuspended = "Suspended";

		internal const string VersioningEnabled = "Enabled";

		internal const string NoSuchBucketPolicy = "NoSuchBucketPolicy";

		internal const string NoSuchWebsiteConfiguration = "NoSuchWebsiteConfiguration";

		internal const string NoSuchLifecycleConfiguration = "NoSuchLifecycleConfiguration";

		internal const string NoSuchCORSConfiguration = "NoSuchCORSConfiguration";

		internal static HashSet<string> GetObjectExtraSubResources = new HashSet<string>
		{
			"response-cache-control",
			"response-content-disposition",
			"response-content-encoding",
			"response-content-language",
			"response-content-type",
			"response-expires"
		};
	}
}
