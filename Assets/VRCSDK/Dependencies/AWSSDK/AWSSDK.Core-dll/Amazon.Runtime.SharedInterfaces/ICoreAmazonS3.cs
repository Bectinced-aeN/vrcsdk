using System;
using System.Collections.Generic;

namespace Amazon.Runtime.SharedInterfaces
{
	public interface ICoreAmazonS3
	{
		string GeneratePreSignedURL(string bucketName, string objectKey, DateTime expiration, IDictionary<string, object> additionalProperties);
	}
}
