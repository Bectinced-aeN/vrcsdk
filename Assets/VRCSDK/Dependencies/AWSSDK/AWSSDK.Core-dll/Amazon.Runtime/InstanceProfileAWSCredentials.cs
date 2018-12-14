using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Runtime
{
	public class InstanceProfileAWSCredentials : URIBasedRefreshingCredentialHelper
	{
		private static TimeSpan _preemptExpiryTime = TimeSpan.FromMinutes(15.0);

		private CredentialsRefreshState _currentRefreshState;

		private static TimeSpan _refreshAttemptPeriod = TimeSpan.FromHours(1.0);

		private static string[] AliasSeparators = new string[1]
		{
			"<br/>"
		};

		private static string Server = "http://169.254.169.254";

		private static string RolesPath = "/latest/meta-data/iam/security-credentials/";

		private static string InfoPath = "/latest/meta-data/iam/info";

		public string Role
		{
			get;
			set;
		}

		private static Uri RolesUri => new Uri(Server + RolesPath);

		private Uri CurrentRoleUri => new Uri(Server + RolesPath + Role);

		private static Uri InfoUri => new Uri(Server + InfoPath);

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			CredentialsRefreshState credentialsRefreshState = null;
			try
			{
				credentialsRefreshState = GetRefreshState();
			}
			catch (Exception ex)
			{
				Logger.GetLogger(typeof(InstanceProfileAWSCredentials)).InfoFormat("Error getting credentials from Instance Profile service: {0}", ex);
			}
			if (credentialsRefreshState != null)
			{
				_currentRefreshState = credentialsRefreshState;
			}
			if (_currentRefreshState == null)
			{
				_currentRefreshState = GetRefreshState();
			}
			return GetEarlyRefreshState(_currentRefreshState);
		}

		public InstanceProfileAWSCredentials(string role)
		{
			Role = role;
			base.PreemptExpiryTime = _preemptExpiryTime;
		}

		public InstanceProfileAWSCredentials()
			: this(GetFirstRole())
		{
		}

		public static IEnumerable<string> GetAvailableRoles()
		{
			string contents = URIBasedRefreshingCredentialHelper.GetContents(RolesUri);
			if (!string.IsNullOrEmpty(contents))
			{
				string[] array = contents.Split(AliasSeparators, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Trim();
					if (!string.IsNullOrEmpty(text))
					{
						yield return text;
					}
				}
			}
		}

		private CredentialsRefreshState GetEarlyRefreshState(CredentialsRefreshState state)
		{
			DateTime expiration = DateTime.Now + _refreshAttemptPeriod + base.PreemptExpiryTime;
			if (expiration.ToUniversalTime() > state.Expiration.ToUniversalTime())
			{
				expiration = state.Expiration;
			}
			return new CredentialsRefreshState
			{
				Credentials = state.Credentials.Copy(),
				Expiration = expiration
			};
		}

		private CredentialsRefreshState GetRefreshState()
		{
			SecurityInfo serviceInfo = GetServiceInfo();
			if (!string.IsNullOrEmpty(serviceInfo.Message))
			{
				throw new AmazonServiceException(string.Format(CultureInfo.InvariantCulture, "Unable to retrieve credentials. Message = \"{0}\".", serviceInfo.Message));
			}
			SecurityCredentials roleCredentials = GetRoleCredentials();
			return new CredentialsRefreshState
			{
				Credentials = new ImmutableCredentials(roleCredentials.AccessKeyId, roleCredentials.SecretAccessKey, roleCredentials.Token),
				Expiration = roleCredentials.Expiration
			};
		}

		private static SecurityInfo GetServiceInfo()
		{
			return URIBasedRefreshingCredentialHelper.GetObjectFromResponse<SecurityInfo>(InfoUri);
		}

		private SecurityCredentials GetRoleCredentials()
		{
			return URIBasedRefreshingCredentialHelper.GetObjectFromResponse<SecurityCredentials>(CurrentRoleUri);
		}

		private static string GetFirstRole()
		{
			using (IEnumerator<string> enumerator = GetAvailableRoles().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			throw new InvalidOperationException("No roles found");
		}
	}
}
