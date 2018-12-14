using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class S3BucketLoggingConfig
	{
		private List<S3Grant> targetGrants = new List<S3Grant>();

		public string TargetBucketName
		{
			get;
			set;
		}

		public List<S3Grant> Grants
		{
			get
			{
				return targetGrants;
			}
			set
			{
				targetGrants = value;
			}
		}

		public string TargetPrefix
		{
			get;
			set;
		}

		internal bool IsSetTargetBucket()
		{
			return TargetBucketName != null;
		}

		internal bool IsSetGrants()
		{
			return targetGrants.Count > 0;
		}

		internal bool IsSetTargetPrefix()
		{
			return TargetPrefix != null;
		}

		public void AddGrant(S3Grantee grantee, S3Permission permission)
		{
			S3Grant item = new S3Grant
			{
				Grantee = grantee,
				Permission = permission
			};
			Grants.Add(item);
		}

		public void RemoveGrant(S3Grantee grantee, S3Permission permission)
		{
			foreach (S3Grant grant in Grants)
			{
				if (grant.Grantee.Equals(grantee) && grant.Permission == permission)
				{
					Grants.Remove(grant);
					break;
				}
			}
		}

		public void RemoveGrant(S3Grantee grantee)
		{
			List<S3Grant> list = new List<S3Grant>();
			foreach (S3Grant grant in Grants)
			{
				if (grant.Grantee.Equals(grantee))
				{
					list.Add(grant);
				}
			}
			foreach (S3Grant item in list)
			{
				Grants.Remove(item);
			}
		}
	}
}
