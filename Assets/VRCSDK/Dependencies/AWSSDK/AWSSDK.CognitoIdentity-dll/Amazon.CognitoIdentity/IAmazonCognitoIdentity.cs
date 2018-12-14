using Amazon.CognitoIdentity.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity
{
	public interface IAmazonCognitoIdentity : IAmazonService, IDisposable
	{
		void CreateIdentityPoolAsync(CreateIdentityPoolRequest request, AmazonServiceCallback<CreateIdentityPoolRequest, CreateIdentityPoolResponse> callback, AsyncOptions options = null);

		void DeleteIdentitiesAsync(DeleteIdentitiesRequest request, AmazonServiceCallback<DeleteIdentitiesRequest, DeleteIdentitiesResponse> callback, AsyncOptions options = null);

		void DeleteIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null);

		void DeleteIdentityPoolAsync(DeleteIdentityPoolRequest request, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null);

		void DescribeIdentityAsync(string identityId, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null);

		void DescribeIdentityAsync(DescribeIdentityRequest request, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null);

		void DescribeIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null);

		void DescribeIdentityPoolAsync(DescribeIdentityPoolRequest request, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null);

		void GetCredentialsForIdentityAsync(string identityId, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null);

		void GetCredentialsForIdentityAsync(string identityId, Dictionary<string, string> logins, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null);

		void GetCredentialsForIdentityAsync(GetCredentialsForIdentityRequest request, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null);

		void GetIdAsync(GetIdRequest request, AmazonServiceCallback<GetIdRequest, GetIdResponse> callback, AsyncOptions options = null);

		void GetIdentityPoolRolesAsync(string identityPoolId, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null);

		void GetIdentityPoolRolesAsync(GetIdentityPoolRolesRequest request, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null);

		void GetOpenIdTokenAsync(string identityId, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null);

		void GetOpenIdTokenAsync(GetOpenIdTokenRequest request, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null);

		void GetOpenIdTokenForDeveloperIdentityAsync(GetOpenIdTokenForDeveloperIdentityRequest request, AmazonServiceCallback<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse> callback, AsyncOptions options = null);

		void ListIdentitiesAsync(ListIdentitiesRequest request, AmazonServiceCallback<ListIdentitiesRequest, ListIdentitiesResponse> callback, AsyncOptions options = null);

		void ListIdentityPoolsAsync(ListIdentityPoolsRequest request, AmazonServiceCallback<ListIdentityPoolsRequest, ListIdentityPoolsResponse> callback, AsyncOptions options = null);

		void LookupDeveloperIdentityAsync(LookupDeveloperIdentityRequest request, AmazonServiceCallback<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse> callback, AsyncOptions options = null);

		void MergeDeveloperIdentitiesAsync(MergeDeveloperIdentitiesRequest request, AmazonServiceCallback<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse> callback, AsyncOptions options = null);

		void SetIdentityPoolRolesAsync(string identityPoolId, Dictionary<string, string> roles, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null);

		void SetIdentityPoolRolesAsync(SetIdentityPoolRolesRequest request, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null);

		void UnlinkDeveloperIdentityAsync(UnlinkDeveloperIdentityRequest request, AmazonServiceCallback<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse> callback, AsyncOptions options = null);

		void UnlinkIdentityAsync(UnlinkIdentityRequest request, AmazonServiceCallback<UnlinkIdentityRequest, UnlinkIdentityResponse> callback, AsyncOptions options = null);

		void UpdateIdentityPoolAsync(UpdateIdentityPoolRequest request, AmazonServiceCallback<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse> callback, AsyncOptions options = null);
	}
}
