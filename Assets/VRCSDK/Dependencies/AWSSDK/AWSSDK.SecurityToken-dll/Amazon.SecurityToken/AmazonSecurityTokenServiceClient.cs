using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SecurityToken.Model;
using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using System;

namespace Amazon.SecurityToken
{
	public class AmazonSecurityTokenServiceClient : AmazonServiceClient, IAmazonSecurityTokenService, IDisposable, ICoreAmazonSTS, IAmazonService
	{
		public AmazonSecurityTokenServiceClient(AWSCredentials credentials)
			: this(credentials, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(AWSCredentials credentials, RegionEndpoint region)
		{
			AmazonSecurityTokenServiceConfig amazonSecurityTokenServiceConfig = new AmazonSecurityTokenServiceConfig();
			amazonSecurityTokenServiceConfig.set_RegionEndpoint(region);
			this._002Ector(credentials, amazonSecurityTokenServiceConfig);
		}

		public AmazonSecurityTokenServiceClient(AWSCredentials credentials, AmazonSecurityTokenServiceConfig clientConfig)
			: this(credentials, clientConfig)
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
		{
			AmazonSecurityTokenServiceConfig amazonSecurityTokenServiceConfig = new AmazonSecurityTokenServiceConfig();
			amazonSecurityTokenServiceConfig.set_RegionEndpoint(region);
			this._002Ector(awsAccessKeyId, awsSecretAccessKey, amazonSecurityTokenServiceConfig);
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonSecurityTokenServiceConfig clientConfig)
			: this(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
		{
			AmazonSecurityTokenServiceConfig amazonSecurityTokenServiceConfig = new AmazonSecurityTokenServiceConfig();
			amazonSecurityTokenServiceConfig.set_RegionEndpoint(region);
			this._002Ector(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, amazonSecurityTokenServiceConfig);
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonSecurityTokenServiceConfig clientConfig)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
		{
		}

		protected override AbstractAWSSigner CreateSigner()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new AWS4Signer();
		}

		protected override void Dispose(bool disposing)
		{
			this.Dispose(disposing);
		}

		internal AssumeRoleResponse AssumeRole(AssumeRoleRequest request)
		{
			AssumeRoleRequestMarshaller assumeRoleRequestMarshaller = new AssumeRoleRequestMarshaller();
			AssumeRoleResponseUnmarshaller instance = AssumeRoleResponseUnmarshaller.Instance;
			return this.Invoke<AssumeRoleRequest, AssumeRoleResponse>(request, assumeRoleRequestMarshaller, instance);
		}

		public void AssumeRoleAsync(AssumeRoleRequest request, AmazonServiceCallback<AssumeRoleRequest, AssumeRoleResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			AssumeRoleRequestMarshaller assumeRoleRequestMarshaller = new AssumeRoleRequestMarshaller();
			AssumeRoleResponseUnmarshaller instance = AssumeRoleResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleRequest, AssumeRoleResponse> val = new AmazonServiceResult<AssumeRoleRequest, AssumeRoleResponse>((AssumeRoleRequest)req, (AssumeRoleResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<AssumeRoleRequest>(request, assumeRoleRequestMarshaller, instance, options, action);
		}

		internal AssumeRoleWithSAMLResponse AssumeRoleWithSAML(AssumeRoleWithSAMLRequest request)
		{
			AssumeRoleWithSAMLRequestMarshaller assumeRoleWithSAMLRequestMarshaller = new AssumeRoleWithSAMLRequestMarshaller();
			AssumeRoleWithSAMLResponseUnmarshaller instance = AssumeRoleWithSAMLResponseUnmarshaller.Instance;
			return this.Invoke<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse>(request, assumeRoleWithSAMLRequestMarshaller, instance);
		}

		public void AssumeRoleWithSAMLAsync(AssumeRoleWithSAMLRequest request, AmazonServiceCallback<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			AssumeRoleWithSAMLRequestMarshaller assumeRoleWithSAMLRequestMarshaller = new AssumeRoleWithSAMLRequestMarshaller();
			AssumeRoleWithSAMLResponseUnmarshaller instance = AssumeRoleWithSAMLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse> val = new AmazonServiceResult<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse>((AssumeRoleWithSAMLRequest)req, (AssumeRoleWithSAMLResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<AssumeRoleWithSAMLRequest>(request, assumeRoleWithSAMLRequestMarshaller, instance, options, action);
		}

		internal AssumeRoleWithWebIdentityResponse AssumeRoleWithWebIdentity(AssumeRoleWithWebIdentityRequest request)
		{
			AssumeRoleWithWebIdentityRequestMarshaller assumeRoleWithWebIdentityRequestMarshaller = new AssumeRoleWithWebIdentityRequestMarshaller();
			AssumeRoleWithWebIdentityResponseUnmarshaller instance = AssumeRoleWithWebIdentityResponseUnmarshaller.Instance;
			return this.Invoke<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse>(request, assumeRoleWithWebIdentityRequestMarshaller, instance);
		}

		public void AssumeRoleWithWebIdentityAsync(AssumeRoleWithWebIdentityRequest request, AmazonServiceCallback<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			AssumeRoleWithWebIdentityRequestMarshaller assumeRoleWithWebIdentityRequestMarshaller = new AssumeRoleWithWebIdentityRequestMarshaller();
			AssumeRoleWithWebIdentityResponseUnmarshaller instance = AssumeRoleWithWebIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> val = new AmazonServiceResult<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse>((AssumeRoleWithWebIdentityRequest)req, (AssumeRoleWithWebIdentityResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<AssumeRoleWithWebIdentityRequest>(request, assumeRoleWithWebIdentityRequestMarshaller, instance, options, action);
		}

		internal DecodeAuthorizationMessageResponse DecodeAuthorizationMessage(DecodeAuthorizationMessageRequest request)
		{
			DecodeAuthorizationMessageRequestMarshaller decodeAuthorizationMessageRequestMarshaller = new DecodeAuthorizationMessageRequestMarshaller();
			DecodeAuthorizationMessageResponseUnmarshaller instance = DecodeAuthorizationMessageResponseUnmarshaller.Instance;
			return this.Invoke<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse>(request, decodeAuthorizationMessageRequestMarshaller, instance);
		}

		public void DecodeAuthorizationMessageAsync(DecodeAuthorizationMessageRequest request, AmazonServiceCallback<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DecodeAuthorizationMessageRequestMarshaller decodeAuthorizationMessageRequestMarshaller = new DecodeAuthorizationMessageRequestMarshaller();
			DecodeAuthorizationMessageResponseUnmarshaller instance = DecodeAuthorizationMessageResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse> val = new AmazonServiceResult<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse>((DecodeAuthorizationMessageRequest)req, (DecodeAuthorizationMessageResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DecodeAuthorizationMessageRequest>(request, decodeAuthorizationMessageRequestMarshaller, instance, options, action);
		}

		internal GetCallerIdentityResponse GetCallerIdentity(GetCallerIdentityRequest request)
		{
			GetCallerIdentityRequestMarshaller getCallerIdentityRequestMarshaller = new GetCallerIdentityRequestMarshaller();
			GetCallerIdentityResponseUnmarshaller instance = GetCallerIdentityResponseUnmarshaller.Instance;
			return this.Invoke<GetCallerIdentityRequest, GetCallerIdentityResponse>(request, getCallerIdentityRequestMarshaller, instance);
		}

		public void GetCallerIdentityAsync(GetCallerIdentityRequest request, AmazonServiceCallback<GetCallerIdentityRequest, GetCallerIdentityResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetCallerIdentityRequestMarshaller getCallerIdentityRequestMarshaller = new GetCallerIdentityRequestMarshaller();
			GetCallerIdentityResponseUnmarshaller instance = GetCallerIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCallerIdentityRequest, GetCallerIdentityResponse> val = new AmazonServiceResult<GetCallerIdentityRequest, GetCallerIdentityResponse>((GetCallerIdentityRequest)req, (GetCallerIdentityResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetCallerIdentityRequest>(request, getCallerIdentityRequestMarshaller, instance, options, action);
		}

		internal GetFederationTokenResponse GetFederationToken(GetFederationTokenRequest request)
		{
			GetFederationTokenRequestMarshaller getFederationTokenRequestMarshaller = new GetFederationTokenRequestMarshaller();
			GetFederationTokenResponseUnmarshaller instance = GetFederationTokenResponseUnmarshaller.Instance;
			return this.Invoke<GetFederationTokenRequest, GetFederationTokenResponse>(request, getFederationTokenRequestMarshaller, instance);
		}

		public void GetFederationTokenAsync(GetFederationTokenRequest request, AmazonServiceCallback<GetFederationTokenRequest, GetFederationTokenResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetFederationTokenRequestMarshaller getFederationTokenRequestMarshaller = new GetFederationTokenRequestMarshaller();
			GetFederationTokenResponseUnmarshaller instance = GetFederationTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetFederationTokenRequest, GetFederationTokenResponse> val = new AmazonServiceResult<GetFederationTokenRequest, GetFederationTokenResponse>((GetFederationTokenRequest)req, (GetFederationTokenResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetFederationTokenRequest>(request, getFederationTokenRequestMarshaller, instance, options, action);
		}

		internal GetSessionTokenResponse GetSessionToken()
		{
			return GetSessionToken(new GetSessionTokenRequest());
		}

		internal GetSessionTokenResponse GetSessionToken(GetSessionTokenRequest request)
		{
			GetSessionTokenRequestMarshaller getSessionTokenRequestMarshaller = new GetSessionTokenRequestMarshaller();
			GetSessionTokenResponseUnmarshaller instance = GetSessionTokenResponseUnmarshaller.Instance;
			return this.Invoke<GetSessionTokenRequest, GetSessionTokenResponse>(request, getSessionTokenRequestMarshaller, instance);
		}

		public void GetSessionTokenAsync(AmazonServiceCallback<GetSessionTokenRequest, GetSessionTokenResponse> callback, AsyncOptions options = null)
		{
			GetSessionTokenAsync(new GetSessionTokenRequest(), callback, options);
		}

		public void GetSessionTokenAsync(GetSessionTokenRequest request, AmazonServiceCallback<GetSessionTokenRequest, GetSessionTokenResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetSessionTokenRequestMarshaller getSessionTokenRequestMarshaller = new GetSessionTokenRequestMarshaller();
			GetSessionTokenResponseUnmarshaller instance = GetSessionTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetSessionTokenRequest, GetSessionTokenResponse> val = new AmazonServiceResult<GetSessionTokenRequest, GetSessionTokenResponse>((GetSessionTokenRequest)req, (GetSessionTokenResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetSessionTokenRequest>(request, getSessionTokenRequestMarshaller, instance, options, action);
		}

		IClientConfig IAmazonService.get_Config()
		{
			return this.get_Config();
		}
	}
}
