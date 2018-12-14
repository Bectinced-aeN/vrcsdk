using Amazon.Runtime.Internal;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public static class HeaderACLRequestMarshaller
	{
		public static void Marshall(IRequest request, PutWithACLRequest aclRequest)
		{
			Dictionary<S3Permission, string> dictionary = new Dictionary<S3Permission, string>();
			foreach (S3Grant grant in aclRequest.Grants)
			{
				string text = null;
				if (grant.Grantee.CanonicalUser != null && !string.IsNullOrEmpty(grant.Grantee.CanonicalUser))
				{
					text = string.Format(CultureInfo.InvariantCulture, "id=\"{0}\"", grant.Grantee.CanonicalUser);
				}
				else if (grant.Grantee.IsSetEmailAddress())
				{
					text = string.Format(CultureInfo.InvariantCulture, "emailAddress=\"{0}\"", grant.Grantee.EmailAddress);
				}
				else
				{
					if (!grant.Grantee.IsSetURI())
					{
						continue;
					}
					text = string.Format(CultureInfo.InvariantCulture, "uri=\"{0}\"", grant.Grantee.URI);
				}
				string value = null;
				if (dictionary.TryGetValue(grant.Permission, out value))
				{
					dictionary[grant.Permission] = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", value, text);
				}
				else
				{
					dictionary.Add(grant.Permission, text);
				}
			}
			foreach (S3Permission key in dictionary.Keys)
			{
				if (S3Permission.READ == key)
				{
					request.get_Headers()["x-amz-grant-read"] = dictionary[key];
				}
				else if (S3Permission.WRITE == key)
				{
					request.get_Headers()["x-amz-grant-write"] = dictionary[key];
				}
				else if (S3Permission.READ_ACP == key)
				{
					request.get_Headers()["x-amz-grant-read-acp"] = dictionary[key];
				}
				else if (S3Permission.WRITE_ACP == key)
				{
					request.get_Headers()["x-amz-grant-write-acp"] = dictionary[key];
				}
				else if (S3Permission.RESTORE_OBJECT == key)
				{
					request.get_Headers()["x-amz-grant-restore-object"] = dictionary[key];
				}
				else if (S3Permission.FULL_CONTROL == key)
				{
					request.get_Headers()["x-amz-grant-full-control"] = dictionary[key];
				}
			}
		}
	}
}
