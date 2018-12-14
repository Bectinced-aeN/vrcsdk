namespace Amazon.S3.Model
{
	public class S3Grant
	{
		private S3Grantee grantee;

		private S3Permission permission;

		public S3Grantee Grantee
		{
			get
			{
				return grantee;
			}
			set
			{
				grantee = value;
			}
		}

		public S3Permission Permission
		{
			get
			{
				return permission;
			}
			set
			{
				permission = value;
			}
		}

		internal bool IsSetGrantee()
		{
			return grantee != null;
		}

		internal bool IsSetPermission()
		{
			return permission != null;
		}
	}
}
