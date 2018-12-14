using Amazon.CognitoIdentity.Model;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Util.Internal.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Amazon.CognitoIdentity
{
	public class CognitoAWSCredentials : RefreshingAWSCredentials
	{
		[Flags]
		private enum RefreshIdentityOptions
		{
			None = 0x0,
			Refresh = 0x1
		}

		public class IdentityChangedArgs : EventArgs
		{
			public string OldIdentityId
			{
				get;
				private set;
			}

			public string NewIdentityId
			{
				get;
				private set;
			}

			internal IdentityChangedArgs(string oldIdentityId, string newIdentityId)
			{
				OldIdentityId = oldIdentityId;
				NewIdentityId = newIdentityId;
			}
		}

		public class IdentityState
		{
			public string IdentityId
			{
				get;
				private set;
			}

			public string LoginProvider
			{
				get;
				private set;
			}

			public string LoginToken
			{
				get;
				private set;
			}

			public bool FromCache
			{
				get;
				private set;
			}

			public bool LoginSpecified
			{
				get
				{
					if (!string.IsNullOrEmpty(LoginProvider))
					{
						return string.IsNullOrEmpty(LoginToken);
					}
					return false;
				}
			}

			public IdentityState(string identityId, string provider, string token, bool fromCache)
			{
				IdentityId = identityId;
				LoginProvider = provider;
				LoginToken = token;
				FromCache = fromCache;
			}

			public IdentityState(string identityId, bool fromCache)
			{
				IdentityId = identityId;
				FromCache = fromCache;
			}
		}

		private static object refreshIdLock = new object();

		private string identityId;

		private static int DefaultDurationSeconds = (int)TimeSpan.FromHours(1.0).TotalSeconds;

		private AmazonCognitoIdentityClient cib;

		private AmazonSecurityTokenServiceClient sts;

		private IdentityState _identityState;

		private EventHandler<IdentityChangedArgs> mIdentityChangedEvent;

		private static readonly string IDENTITY_ID_CACHE_KEY = "CognitoIdentity:IdentityId";

		private static object _lock = new object();

		private bool IsIdentitySet
		{
			get
			{
				if (string.IsNullOrEmpty(identityId))
				{
					identityId = GetCachedIdentityId();
				}
				return !string.IsNullOrEmpty(identityId);
			}
		}

		protected Dictionary<string, string> CloneLogins
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(Logins.Count, Logins.Comparer);
				foreach (KeyValuePair<string, string> login in Logins)
				{
					dictionary.Add(login.Key, login.Value);
				}
				return dictionary;
			}
		}

		public string AccountId
		{
			get;
			private set;
		}

		public string IdentityPoolId
		{
			get;
			private set;
		}

		public string UnAuthRoleArn
		{
			get;
			private set;
		}

		public string AuthRoleArn
		{
			get;
			private set;
		}

		private Dictionary<string, string> Logins
		{
			get;
			set;
		}

		public string[] CurrentLoginProviders => Logins.Keys.ToArray();

		public int LoginsCount => Logins.Count;

		public event EventHandler<IdentityChangedArgs> IdentityChangedEvent
		{
			add
			{
				lock (this)
				{
					mIdentityChangedEvent = (EventHandler<IdentityChangedArgs>)Delegate.Combine(mIdentityChangedEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mIdentityChangedEvent = (EventHandler<IdentityChangedArgs>)Delegate.Remove(mIdentityChangedEvent, value);
				}
			}
		}

		private void UpdateIdentity(string newIdentityId)
		{
			if (!string.Equals(identityId, newIdentityId, StringComparison.Ordinal))
			{
				CacheIdentityId(newIdentityId);
				ClearCredentials();
				string oldIdentityId = identityId;
				identityId = newIdentityId;
				EventHandler<IdentityChangedArgs> eventHandler = mIdentityChangedEvent;
				if (eventHandler != null)
				{
					IdentityChangedArgs e = new IdentityChangedArgs(oldIdentityId, newIdentityId);
					eventHandler(this, e);
				}
			}
		}

		protected string GetNamespacedKey(string key)
		{
			return key + ":" + IdentityPoolId;
		}

		public void Clear()
		{
			identityId = null;
			ClearCredentials();
			ClearIdentityCache();
			Logins.Clear();
		}

		public bool ContainsProvider(string providerName)
		{
			return Logins.ContainsKey(providerName);
		}

		public void RemoveLogin(string providerName)
		{
			Logins.Remove(providerName);
			ClearCredentials();
		}

		public void AddLogin(string providerName, string token)
		{
			Logins[providerName] = token;
			ClearCredentials();
		}

		public string GetIdentityId()
		{
			return GetIdentityId(RefreshIdentityOptions.None);
		}

		private string GetIdentityId(RefreshIdentityOptions options)
		{
			lock (refreshIdLock)
			{
				if (!IsIdentitySet || options == RefreshIdentityOptions.Refresh)
				{
					_identityState = RefreshIdentity();
					if (!string.IsNullOrEmpty(_identityState.LoginProvider))
					{
						Logins[_identityState.LoginProvider] = _identityState.LoginToken;
					}
					UpdateIdentity(_identityState.IdentityId);
				}
			}
			return identityId;
		}

		protected virtual IdentityState RefreshIdentity()
		{
			bool fromCache = true;
			if (!IsIdentitySet)
			{
				GetIdRequest request = new GetIdRequest
				{
					AccountId = AccountId,
					IdentityPoolId = IdentityPoolId,
					Logins = Logins
				};
				GetIdResponse id = cib.GetId(request);
				fromCache = false;
				UpdateIdentity(id.IdentityId);
			}
			return new IdentityState(identityId, fromCache);
		}

		private bool ShouldRetry(AmazonCognitoIdentityException e)
		{
			if (_identityState.LoginSpecified && ((e is NotAuthorizedException && e.Message.StartsWith("Access to Identity", StringComparison.OrdinalIgnoreCase)) || e is ResourceNotFoundException))
			{
				identityId = null;
				ClearIdentityCache();
				return true;
			}
			return false;
		}

		public CognitoAWSCredentials(string identityPoolId, RegionEndpoint region)
			: this(null, identityPoolId, null, null, region)
		{
		}

		public CognitoAWSCredentials(string accountId, string identityPoolId, string unAuthRoleArn, string authRoleArn, RegionEndpoint region)
			: this(accountId, identityPoolId, unAuthRoleArn, authRoleArn, new AmazonCognitoIdentityClient(new AnonymousAWSCredentials(), region), new AmazonSecurityTokenServiceClient(new AnonymousAWSCredentials(), region))
		{
		}

		public CognitoAWSCredentials(string accountId, string identityPoolId, string unAuthRoleArn, string authRoleArn, IAmazonCognitoIdentity cibClient, IAmazonSecurityTokenService stsClient)
		{
			if (string.IsNullOrEmpty(identityPoolId))
			{
				throw new ArgumentNullException("identityPoolId");
			}
			if (cibClient == null)
			{
				throw new ArgumentNullException("cibClient");
			}
			if ((unAuthRoleArn != null || authRoleArn != null) && stsClient == null)
			{
				throw new ArgumentNullException("stsClient");
			}
			AccountId = accountId;
			IdentityPoolId = identityPoolId;
			UnAuthRoleArn = unAuthRoleArn;
			AuthRoleArn = authRoleArn;
			Logins = new Dictionary<string, string>(StringComparer.Ordinal);
			cib = (AmazonCognitoIdentityClient)cibClient;
			sts = (AmazonSecurityTokenServiceClient)stsClient;
			string cachedIdentityId = GetCachedIdentityId();
			if (!string.IsNullOrEmpty(cachedIdentityId))
			{
				UpdateIdentity(cachedIdentityId);
				currentState = GetCachedCredentials();
			}
		}

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			string text = UnAuthRoleArn;
			if (Logins.Count > 0)
			{
				text = AuthRoleArn;
			}
			CredentialsRefreshState credentialsRefreshState = string.IsNullOrEmpty(text) ? GetPoolCredentials() : GetCredentialsForRole(text);
			CacheCredentials(credentialsRefreshState);
			return credentialsRefreshState;
		}

		private CredentialsRefreshState GetPoolCredentials()
		{
			string text = GetIdentityId(RefreshIdentityOptions.Refresh);
			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = new GetCredentialsForIdentityRequest
			{
				IdentityId = text
			};
			if (Logins.Count > 0)
			{
				getCredentialsForIdentityRequest.Logins = Logins;
			}
			if (_identityState != null && !string.IsNullOrEmpty(_identityState.LoginToken))
			{
				getCredentialsForIdentityRequest.Logins = new Dictionary<string, string>();
				getCredentialsForIdentityRequest.Logins["cognito-identity.amazonaws.com"] = _identityState.LoginToken;
			}
			bool flag = false;
			GetCredentialsForIdentityResponse getCredentialsForIdentityResponse = null;
			try
			{
				getCredentialsForIdentityResponse = GetCredentialsForIdentity(getCredentialsForIdentityRequest);
			}
			catch (AmazonCognitoIdentityException e)
			{
				if (!ShouldRetry(e))
				{
					throw;
				}
				flag = true;
			}
			if (flag)
			{
				return GetPoolCredentials();
			}
			UpdateIdentity(getCredentialsForIdentityResponse.IdentityId);
			Amazon.CognitoIdentity.Model.Credentials credentials = getCredentialsForIdentityResponse.Credentials;
			return new CredentialsRefreshState(credentials.GetCredentials(), credentials.Expiration);
		}

		private CredentialsRefreshState GetCredentialsForRole(string roleArn)
		{
			string text = GetIdentityId(RefreshIdentityOptions.Refresh);
			GetOpenIdTokenRequest getOpenIdTokenRequest = new GetOpenIdTokenRequest
			{
				IdentityId = text
			};
			if (Logins.Count > 0)
			{
				getOpenIdTokenRequest.Logins = Logins;
			}
			bool flag = false;
			GetOpenIdTokenResponse getOpenIdTokenResponse = null;
			try
			{
				getOpenIdTokenResponse = GetOpenId(getOpenIdTokenRequest);
			}
			catch (AmazonCognitoIdentityException e)
			{
				if (!ShouldRetry(e))
				{
					throw;
				}
				flag = true;
			}
			if (flag)
			{
				return GetCredentialsForRole(roleArn);
			}
			string token = getOpenIdTokenResponse.Token;
			UpdateIdentity(getOpenIdTokenResponse.IdentityId);
			AssumeRoleWithWebIdentityRequest assumeRequest = new AssumeRoleWithWebIdentityRequest
			{
				WebIdentityToken = token,
				RoleArn = roleArn,
				RoleSessionName = "NetProviderSession",
				DurationSeconds = DefaultDurationSeconds
			};
			Amazon.SecurityToken.Model.Credentials stsCredentials = GetStsCredentials(assumeRequest);
			return new CredentialsRefreshState(stsCredentials.GetCredentials(), stsCredentials.Expiration);
		}

		private Amazon.SecurityToken.Model.Credentials GetStsCredentials(AssumeRoleWithWebIdentityRequest assumeRequest)
		{
			AutoResetEvent ars = new AutoResetEvent(initialState: false);
			Amazon.SecurityToken.Model.Credentials credentials = null;
			Exception exception = null;
			sts.AssumeRoleWithWebIdentityAsync(assumeRequest, delegate(AmazonServiceResult<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> assumeResult)
			{
				if (assumeResult.Exception != null)
				{
					exception = assumeResult.Exception;
				}
				else
				{
					credentials = assumeResult.Response.Credentials;
				}
				ars.Set();
			});
			ars.WaitOne();
			if (exception != null)
			{
				throw exception;
			}
			return credentials;
		}

		private GetCredentialsForIdentityResponse GetCredentialsForIdentity(GetCredentialsForIdentityRequest getCredentialsRequest)
		{
			return cib.GetCredentialsForIdentity(getCredentialsRequest);
		}

		private GetOpenIdTokenResponse GetOpenId(GetOpenIdTokenRequest getTokenRequest)
		{
			return cib.GetOpenIdToken(getTokenRequest);
		}

		public virtual string GetCachedIdentityId()
		{
			return ServiceFactory.Instance.GetService<IApplicationSettings>().GetValue(GetNamespacedKey(IDENTITY_ID_CACHE_KEY), ApplicationSettingsMode.Local);
		}

		public virtual void CacheIdentityId(string identityId)
		{
			ServiceFactory.Instance.GetService<IApplicationSettings>().SetValue(GetNamespacedKey(IDENTITY_ID_CACHE_KEY), identityId, ApplicationSettingsMode.Local);
		}

		public virtual void ClearIdentityCache()
		{
			ServiceFactory.Instance.GetService<IApplicationSettings>().RemoveValue(GetNamespacedKey(IDENTITY_ID_CACHE_KEY), ApplicationSettingsMode.Local);
		}

		internal void CacheCredentials(CredentialsRefreshState credentialsState)
		{
		}

		internal CredentialsRefreshState GetCachedCredentials()
		{
			return null;
		}

		public void GetIdentityIdAsync(AmazonCognitoIdentityCallback<string> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			CognitoIdentityAsyncExecutor.ExecuteAsync(() => GetIdentityId(), options, callback);
		}

		public void GetCredentialsAsync(AmazonCognitoIdentityCallback<ImmutableCredentials> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			CognitoIdentityAsyncExecutor.ExecuteAsync(() => GetCredentials(), options, callback);
		}
	}
}
