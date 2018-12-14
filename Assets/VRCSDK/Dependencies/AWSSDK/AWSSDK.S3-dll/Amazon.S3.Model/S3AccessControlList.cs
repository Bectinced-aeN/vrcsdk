using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class S3AccessControlList
	{
		private List<S3Grant> grantList;

		public Owner Owner
		{
			get;
			set;
		}

		public List<S3Grant> Grants
		{
			get
			{
				if (grantList == null)
				{
					grantList = new List<S3Grant>();
				}
				return grantList;
			}
			set
			{
				grantList = value;
			}
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

		internal bool IsSetOwner()
		{
			return Owner != null;
		}

		internal bool IsSetGrants()
		{
			return Grants.Count > 0;
		}
	}
}
