using Amazon.Runtime;

namespace Amazon.SecurityToken.Model
{
	public class AssumeRoleResponse : AmazonWebServiceResponse
	{
		private AssumedRoleUser _assumedRoleUser;

		private Credentials _credentials;

		private int? _packedPolicySize;

		public AssumedRoleUser AssumedRoleUser
		{
			get
			{
				return _assumedRoleUser;
			}
			set
			{
				_assumedRoleUser = value;
			}
		}

		public Credentials Credentials
		{
			get
			{
				return _credentials;
			}
			set
			{
				_credentials = value;
			}
		}

		public int PackedPolicySize
		{
			get
			{
				return _packedPolicySize.GetValueOrDefault();
			}
			set
			{
				_packedPolicySize = value;
			}
		}

		internal bool IsSetAssumedRoleUser()
		{
			return _assumedRoleUser != null;
		}

		internal bool IsSetCredentials()
		{
			return _credentials != null;
		}

		internal bool IsSetPackedPolicySize()
		{
			return _packedPolicySize.HasValue;
		}

		public AssumeRoleResponse()
			: this()
		{
		}
	}
}
