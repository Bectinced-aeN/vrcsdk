using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentity.Model.Internal.MarshallTransformations;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using System;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity
{
	public class AmazonCognitoIdentityClient : AmazonServiceClient, IAmazonCognitoIdentity, IAmazonService, IDisposable
	{
		public AmazonCognitoIdentityClient(AWSCredentials credentials)
			: this(credentials, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(AWSCredentials credentials, RegionEndpoint region)
			: this(credentials, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(AWSCredentials credentials, AmazonCognitoIdentityConfig clientConfig)
			: base(credentials, clientConfig)
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonCognitoIdentityConfig clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonCognitoIdentityConfig clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
		{
		}

		protected override AbstractAWSSigner CreateSigner()
		{
			return new AWS4Signer();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		internal CreateIdentityPoolResponse CreateIdentityPool(CreateIdentityPoolRequest request)
		{
			CreateIdentityPoolRequestMarshaller marshaller = new CreateIdentityPoolRequestMarshaller();
			CreateIdentityPoolResponseUnmarshaller instance = CreateIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<CreateIdentityPoolRequest, CreateIdentityPoolResponse>(request, marshaller, instance);
		}

		public void CreateIdentityPoolAsync(CreateIdentityPoolRequest request, AmazonServiceCallback<CreateIdentityPoolRequest, CreateIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			CreateIdentityPoolRequestMarshaller marshaller = new CreateIdentityPoolRequestMarshaller();
			CreateIdentityPoolResponseUnmarshaller instance = CreateIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CreateIdentityPoolRequest, CreateIdentityPoolResponse> responseObject = new AmazonServiceResult<CreateIdentityPoolRequest, CreateIdentityPoolResponse>((CreateIdentityPoolRequest)req, (CreateIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal DeleteIdentitiesResponse DeleteIdentities(DeleteIdentitiesRequest request)
		{
			DeleteIdentitiesRequestMarshaller marshaller = new DeleteIdentitiesRequestMarshaller();
			DeleteIdentitiesResponseUnmarshaller instance = DeleteIdentitiesResponseUnmarshaller.Instance;
			return Invoke<DeleteIdentitiesRequest, DeleteIdentitiesResponse>(request, marshaller, instance);
		}

		public void DeleteIdentitiesAsync(DeleteIdentitiesRequest request, AmazonServiceCallback<DeleteIdentitiesRequest, DeleteIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteIdentitiesRequestMarshaller marshaller = new DeleteIdentitiesRequestMarshaller();
			DeleteIdentitiesResponseUnmarshaller instance = DeleteIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteIdentitiesRequest, DeleteIdentitiesResponse> responseObject = new AmazonServiceResult<DeleteIdentitiesRequest, DeleteIdentitiesResponse>((DeleteIdentitiesRequest)req, (DeleteIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal DeleteIdentityPoolResponse DeleteIdentityPool(DeleteIdentityPoolRequest request)
		{
			DeleteIdentityPoolRequestMarshaller marshaller = new DeleteIdentityPoolRequestMarshaller();
			DeleteIdentityPoolResponseUnmarshaller instance = DeleteIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse>(request, marshaller, instance);
		}

		public void DeleteIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			DeleteIdentityPoolRequest deleteIdentityPoolRequest = new DeleteIdentityPoolRequest();
			deleteIdentityPoolRequest.IdentityPoolId = identityPoolId;
			DeleteIdentityPoolAsync(deleteIdentityPoolRequest, callback, options);
		}

		public void DeleteIdentityPoolAsync(DeleteIdentityPoolRequest request, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteIdentityPoolRequestMarshaller marshaller = new DeleteIdentityPoolRequestMarshaller();
			DeleteIdentityPoolResponseUnmarshaller instance = DeleteIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> responseObject = new AmazonServiceResult<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse>((DeleteIdentityPoolRequest)req, (DeleteIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal DescribeIdentityResponse DescribeIdentity(DescribeIdentityRequest request)
		{
			DescribeIdentityRequestMarshaller marshaller = new DescribeIdentityRequestMarshaller();
			DescribeIdentityResponseUnmarshaller instance = DescribeIdentityResponseUnmarshaller.Instance;
			return Invoke<DescribeIdentityRequest, DescribeIdentityResponse>(request, marshaller, instance);
		}

		public void DescribeIdentityAsync(string identityId, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null)
		{
			DescribeIdentityRequest describeIdentityRequest = new DescribeIdentityRequest();
			describeIdentityRequest.IdentityId = identityId;
			DescribeIdentityAsync(describeIdentityRequest, callback, options);
		}

		public void DescribeIdentityAsync(DescribeIdentityRequest request, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DescribeIdentityRequestMarshaller marshaller = new DescribeIdentityRequestMarshaller();
			DescribeIdentityResponseUnmarshaller instance = DescribeIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DescribeIdentityRequest, DescribeIdentityResponse> responseObject = new AmazonServiceResult<DescribeIdentityRequest, DescribeIdentityResponse>((DescribeIdentityRequest)req, (DescribeIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal DescribeIdentityPoolResponse DescribeIdentityPool(DescribeIdentityPoolRequest request)
		{
			DescribeIdentityPoolRequestMarshaller marshaller = new DescribeIdentityPoolRequestMarshaller();
			DescribeIdentityPoolResponseUnmarshaller instance = DescribeIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse>(request, marshaller, instance);
		}

		public void DescribeIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			DescribeIdentityPoolRequest describeIdentityPoolRequest = new DescribeIdentityPoolRequest();
			describeIdentityPoolRequest.IdentityPoolId = identityPoolId;
			DescribeIdentityPoolAsync(describeIdentityPoolRequest, callback, options);
		}

		public void DescribeIdentityPoolAsync(DescribeIdentityPoolRequest request, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DescribeIdentityPoolRequestMarshaller marshaller = new DescribeIdentityPoolRequestMarshaller();
			DescribeIdentityPoolResponseUnmarshaller instance = DescribeIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> responseObject = new AmazonServiceResult<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse>((DescribeIdentityPoolRequest)req, (DescribeIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal GetCredentialsForIdentityResponse GetCredentialsForIdentity(GetCredentialsForIdentityRequest request)
		{
			GetCredentialsForIdentityRequestMarshaller marshaller = new GetCredentialsForIdentityRequestMarshaller();
			GetCredentialsForIdentityResponseUnmarshaller instance = GetCredentialsForIdentityResponseUnmarshaller.Instance;
			return Invoke<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse>(request, marshaller, instance);
		}

		public void GetCredentialsForIdentityAsync(string identityId, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = new GetCredentialsForIdentityRequest();
			getCredentialsForIdentityRequest.IdentityId = identityId;
			GetCredentialsForIdentityAsync(getCredentialsForIdentityRequest, callback, options);
		}

		public void GetCredentialsForIdentityAsync(string identityId, Dictionary<string, string> logins, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = new GetCredentialsForIdentityRequest();
			getCredentialsForIdentityRequest.IdentityId = identityId;
			getCredentialsForIdentityRequest.Logins = logins;
			GetCredentialsForIdentityAsync(getCredentialsForIdentityRequest, callback, options);
		}

		public void GetCredentialsForIdentityAsync(GetCredentialsForIdentityRequest request, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetCredentialsForIdentityRequestMarshaller marshaller = new GetCredentialsForIdentityRequestMarshaller();
			GetCredentialsForIdentityResponseUnmarshaller instance = GetCredentialsForIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> responseObject = new AmazonServiceResult<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse>((GetCredentialsForIdentityRequest)req, (GetCredentialsForIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal GetIdResponse GetId(GetIdRequest request)
		{
			GetIdRequestMarshaller marshaller = new GetIdRequestMarshaller();
			GetIdResponseUnmarshaller instance = GetIdResponseUnmarshaller.Instance;
			return Invoke<GetIdRequest, GetIdResponse>(request, marshaller, instance);
		}

		public void GetIdAsync(GetIdRequest request, AmazonServiceCallback<GetIdRequest, GetIdResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetIdRequestMarshaller marshaller = new GetIdRequestMarshaller();
			GetIdResponseUnmarshaller instance = GetIdResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetIdRequest, GetIdResponse> responseObject = new AmazonServiceResult<GetIdRequest, GetIdResponse>((GetIdRequest)req, (GetIdResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal GetIdentityPoolRolesResponse GetIdentityPoolRoles(GetIdentityPoolRolesRequest request)
		{
			GetIdentityPoolRolesRequestMarshaller marshaller = new GetIdentityPoolRolesRequestMarshaller();
			GetIdentityPoolRolesResponseUnmarshaller instance = GetIdentityPoolRolesResponseUnmarshaller.Instance;
			return Invoke<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse>(request, marshaller, instance);
		}

		public void GetIdentityPoolRolesAsync(string identityPoolId, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			GetIdentityPoolRolesRequest getIdentityPoolRolesRequest = new GetIdentityPoolRolesRequest();
			getIdentityPoolRolesRequest.IdentityPoolId = identityPoolId;
			GetIdentityPoolRolesAsync(getIdentityPoolRolesRequest, callback, options);
		}

		public void GetIdentityPoolRolesAsync(GetIdentityPoolRolesRequest request, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetIdentityPoolRolesRequestMarshaller marshaller = new GetIdentityPoolRolesRequestMarshaller();
			GetIdentityPoolRolesResponseUnmarshaller instance = GetIdentityPoolRolesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> responseObject = new AmazonServiceResult<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse>((GetIdentityPoolRolesRequest)req, (GetIdentityPoolRolesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal GetOpenIdTokenResponse GetOpenIdToken(GetOpenIdTokenRequest request)
		{
			GetOpenIdTokenRequestMarshaller marshaller = new GetOpenIdTokenRequestMarshaller();
			GetOpenIdTokenResponseUnmarshaller instance = GetOpenIdTokenResponseUnmarshaller.Instance;
			return Invoke<GetOpenIdTokenRequest, GetOpenIdTokenResponse>(request, marshaller, instance);
		}

		public void GetOpenIdTokenAsync(string identityId, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null)
		{
			GetOpenIdTokenRequest getOpenIdTokenRequest = new GetOpenIdTokenRequest();
			getOpenIdTokenRequest.IdentityId = identityId;
			GetOpenIdTokenAsync(getOpenIdTokenRequest, callback, options);
		}

		public void GetOpenIdTokenAsync(GetOpenIdTokenRequest request, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetOpenIdTokenRequestMarshaller marshaller = new GetOpenIdTokenRequestMarshaller();
			GetOpenIdTokenResponseUnmarshaller instance = GetOpenIdTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetOpenIdTokenRequest, GetOpenIdTokenResponse> responseObject = new AmazonServiceResult<GetOpenIdTokenRequest, GetOpenIdTokenResponse>((GetOpenIdTokenRequest)req, (GetOpenIdTokenResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal GetOpenIdTokenForDeveloperIdentityResponse GetOpenIdTokenForDeveloperIdentity(GetOpenIdTokenForDeveloperIdentityRequest request)
		{
			GetOpenIdTokenForDeveloperIdentityRequestMarshaller marshaller = new GetOpenIdTokenForDeveloperIdentityRequestMarshaller();
			GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller instance = GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse>(request, marshaller, instance);
		}

		public void GetOpenIdTokenForDeveloperIdentityAsync(GetOpenIdTokenForDeveloperIdentityRequest request, AmazonServiceCallback<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetOpenIdTokenForDeveloperIdentityRequestMarshaller marshaller = new GetOpenIdTokenForDeveloperIdentityRequestMarshaller();
			GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller instance = GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse> responseObject = new AmazonServiceResult<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse>((GetOpenIdTokenForDeveloperIdentityRequest)req, (GetOpenIdTokenForDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal ListIdentitiesResponse ListIdentities(ListIdentitiesRequest request)
		{
			ListIdentitiesRequestMarshaller marshaller = new ListIdentitiesRequestMarshaller();
			ListIdentitiesResponseUnmarshaller instance = ListIdentitiesResponseUnmarshaller.Instance;
			return Invoke<ListIdentitiesRequest, ListIdentitiesResponse>(request, marshaller, instance);
		}

		public void ListIdentitiesAsync(ListIdentitiesRequest request, AmazonServiceCallback<ListIdentitiesRequest, ListIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListIdentitiesRequestMarshaller marshaller = new ListIdentitiesRequestMarshaller();
			ListIdentitiesResponseUnmarshaller instance = ListIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListIdentitiesRequest, ListIdentitiesResponse> responseObject = new AmazonServiceResult<ListIdentitiesRequest, ListIdentitiesResponse>((ListIdentitiesRequest)req, (ListIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal ListIdentityPoolsResponse ListIdentityPools(ListIdentityPoolsRequest request)
		{
			ListIdentityPoolsRequestMarshaller marshaller = new ListIdentityPoolsRequestMarshaller();
			ListIdentityPoolsResponseUnmarshaller instance = ListIdentityPoolsResponseUnmarshaller.Instance;
			return Invoke<ListIdentityPoolsRequest, ListIdentityPoolsResponse>(request, marshaller, instance);
		}

		public void ListIdentityPoolsAsync(ListIdentityPoolsRequest request, AmazonServiceCallback<ListIdentityPoolsRequest, ListIdentityPoolsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListIdentityPoolsRequestMarshaller marshaller = new ListIdentityPoolsRequestMarshaller();
			ListIdentityPoolsResponseUnmarshaller instance = ListIdentityPoolsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListIdentityPoolsRequest, ListIdentityPoolsResponse> responseObject = new AmazonServiceResult<ListIdentityPoolsRequest, ListIdentityPoolsResponse>((ListIdentityPoolsRequest)req, (ListIdentityPoolsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal LookupDeveloperIdentityResponse LookupDeveloperIdentity(LookupDeveloperIdentityRequest request)
		{
			LookupDeveloperIdentityRequestMarshaller marshaller = new LookupDeveloperIdentityRequestMarshaller();
			LookupDeveloperIdentityResponseUnmarshaller instance = LookupDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse>(request, marshaller, instance);
		}

		public void LookupDeveloperIdentityAsync(LookupDeveloperIdentityRequest request, AmazonServiceCallback<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			LookupDeveloperIdentityRequestMarshaller marshaller = new LookupDeveloperIdentityRequestMarshaller();
			LookupDeveloperIdentityResponseUnmarshaller instance = LookupDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse> responseObject = new AmazonServiceResult<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse>((LookupDeveloperIdentityRequest)req, (LookupDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal MergeDeveloperIdentitiesResponse MergeDeveloperIdentities(MergeDeveloperIdentitiesRequest request)
		{
			MergeDeveloperIdentitiesRequestMarshaller marshaller = new MergeDeveloperIdentitiesRequestMarshaller();
			MergeDeveloperIdentitiesResponseUnmarshaller instance = MergeDeveloperIdentitiesResponseUnmarshaller.Instance;
			return Invoke<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse>(request, marshaller, instance);
		}

		public void MergeDeveloperIdentitiesAsync(MergeDeveloperIdentitiesRequest request, AmazonServiceCallback<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			MergeDeveloperIdentitiesRequestMarshaller marshaller = new MergeDeveloperIdentitiesRequestMarshaller();
			MergeDeveloperIdentitiesResponseUnmarshaller instance = MergeDeveloperIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse> responseObject = new AmazonServiceResult<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse>((MergeDeveloperIdentitiesRequest)req, (MergeDeveloperIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal SetIdentityPoolRolesResponse SetIdentityPoolRoles(SetIdentityPoolRolesRequest request)
		{
			SetIdentityPoolRolesRequestMarshaller marshaller = new SetIdentityPoolRolesRequestMarshaller();
			SetIdentityPoolRolesResponseUnmarshaller instance = SetIdentityPoolRolesResponseUnmarshaller.Instance;
			return Invoke<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse>(request, marshaller, instance);
		}

		public void SetIdentityPoolRolesAsync(string identityPoolId, Dictionary<string, string> roles, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			SetIdentityPoolRolesRequest setIdentityPoolRolesRequest = new SetIdentityPoolRolesRequest();
			setIdentityPoolRolesRequest.IdentityPoolId = identityPoolId;
			setIdentityPoolRolesRequest.Roles = roles;
			SetIdentityPoolRolesAsync(setIdentityPoolRolesRequest, callback, options);
		}

		public void SetIdentityPoolRolesAsync(SetIdentityPoolRolesRequest request, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			SetIdentityPoolRolesRequestMarshaller marshaller = new SetIdentityPoolRolesRequestMarshaller();
			SetIdentityPoolRolesResponseUnmarshaller instance = SetIdentityPoolRolesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> responseObject = new AmazonServiceResult<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse>((SetIdentityPoolRolesRequest)req, (SetIdentityPoolRolesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal UnlinkDeveloperIdentityResponse UnlinkDeveloperIdentity(UnlinkDeveloperIdentityRequest request)
		{
			UnlinkDeveloperIdentityRequestMarshaller marshaller = new UnlinkDeveloperIdentityRequestMarshaller();
			UnlinkDeveloperIdentityResponseUnmarshaller instance = UnlinkDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse>(request, marshaller, instance);
		}

		public void UnlinkDeveloperIdentityAsync(UnlinkDeveloperIdentityRequest request, AmazonServiceCallback<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UnlinkDeveloperIdentityRequestMarshaller marshaller = new UnlinkDeveloperIdentityRequestMarshaller();
			UnlinkDeveloperIdentityResponseUnmarshaller instance = UnlinkDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse> responseObject = new AmazonServiceResult<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse>((UnlinkDeveloperIdentityRequest)req, (UnlinkDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal UnlinkIdentityResponse UnlinkIdentity(UnlinkIdentityRequest request)
		{
			UnlinkIdentityRequestMarshaller marshaller = new UnlinkIdentityRequestMarshaller();
			UnlinkIdentityResponseUnmarshaller instance = UnlinkIdentityResponseUnmarshaller.Instance;
			return Invoke<UnlinkIdentityRequest, UnlinkIdentityResponse>(request, marshaller, instance);
		}

		public void UnlinkIdentityAsync(UnlinkIdentityRequest request, AmazonServiceCallback<UnlinkIdentityRequest, UnlinkIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UnlinkIdentityRequestMarshaller marshaller = new UnlinkIdentityRequestMarshaller();
			UnlinkIdentityResponseUnmarshaller instance = UnlinkIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UnlinkIdentityRequest, UnlinkIdentityResponse> responseObject = new AmazonServiceResult<UnlinkIdentityRequest, UnlinkIdentityResponse>((UnlinkIdentityRequest)req, (UnlinkIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		internal UpdateIdentityPoolResponse UpdateIdentityPool(UpdateIdentityPoolRequest request)
		{
			UpdateIdentityPoolRequestMarshaller marshaller = new UpdateIdentityPoolRequestMarshaller();
			UpdateIdentityPoolResponseUnmarshaller instance = UpdateIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse>(request, marshaller, instance);
		}

		public void UpdateIdentityPoolAsync(UpdateIdentityPoolRequest request, AmazonServiceCallback<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UpdateIdentityPoolRequestMarshaller marshaller = new UpdateIdentityPoolRequestMarshaller();
			UpdateIdentityPoolResponseUnmarshaller instance = UpdateIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse> responseObject = new AmazonServiceResult<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse>((UpdateIdentityPoolRequest)req, (UpdateIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, marshaller, instance, options, callbackHelper);
		}

		IClientConfig IAmazonService.get_Config()
		{
			return base.Config;
		}
	}
}
