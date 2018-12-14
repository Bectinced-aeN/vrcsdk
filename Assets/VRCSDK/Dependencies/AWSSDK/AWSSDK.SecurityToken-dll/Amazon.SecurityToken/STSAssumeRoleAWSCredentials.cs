using Amazon.Runtime;
using Amazon.SecurityToken.Model;
using System;

namespace Amazon.SecurityToken
{
	public class STSAssumeRoleAWSCredentials : RefreshingAWSCredentials, IDisposable
	{
		private AmazonSecurityTokenServiceClient _stsClient;

		private AssumeRoleRequest _assumeRequest;

		private AssumeRoleWithSAMLRequest _assumeSamlRequest;

		private bool _isDisposed;

		private static TimeSpan _defaultPreemptExpiryTime = TimeSpan.FromMinutes(5.0);

		public STSAssumeRoleAWSCredentials(IAmazonSecurityTokenService sts, AssumeRoleRequest assumeRoleRequest)
			: this()
		{
			if (sts == null)
			{
				throw new ArgumentNullException("sts");
			}
			if (assumeRoleRequest == null)
			{
				throw new ArgumentNullException("assumeRoleRequest");
			}
			_stsClient = (AmazonSecurityTokenServiceClient)sts;
			_assumeRequest = assumeRoleRequest;
			this.set_PreemptExpiryTime(_defaultPreemptExpiryTime);
		}

		public STSAssumeRoleAWSCredentials(AssumeRoleWithSAMLRequest assumeRoleWithSamlRequest)
			: this()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			if (assumeRoleWithSamlRequest == null)
			{
				throw new ArgumentNullException("assumeRoleWithSamlRequest");
			}
			_stsClient = new AmazonSecurityTokenServiceClient(new AnonymousAWSCredentials());
			_assumeSamlRequest = assumeRoleWithSamlRequest;
			this.set_PreemptExpiryTime(_defaultPreemptExpiryTime);
		}

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			Credentials serviceCredentials = GetServiceCredentials();
			CredentialsRefreshState val = new CredentialsRefreshState();
			val.set_Expiration(serviceCredentials.Expiration);
			val.set_Credentials(serviceCredentials.GetCredentials());
			return val;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing && _stsClient != null)
				{
					_stsClient.Dispose();
					_stsClient = null;
				}
				_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private Credentials GetServiceCredentials()
		{
			if (_assumeRequest != null)
			{
				return _stsClient.AssumeRole(_assumeRequest).Credentials;
			}
			return _stsClient.AssumeRoleWithSAML(_assumeSamlRequest).Credentials;
		}
	}
}
