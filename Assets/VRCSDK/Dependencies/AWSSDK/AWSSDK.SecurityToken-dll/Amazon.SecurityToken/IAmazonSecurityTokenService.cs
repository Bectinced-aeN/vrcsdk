using Amazon.Runtime;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SecurityToken.Model;
using System;

namespace Amazon.SecurityToken
{
	public interface IAmazonSecurityTokenService : IDisposable, ICoreAmazonSTS, IAmazonService
	{
		void AssumeRoleAsync(AssumeRoleRequest request, AmazonServiceCallback<AssumeRoleRequest, AssumeRoleResponse> callback, AsyncOptions options = null);

		void AssumeRoleWithSAMLAsync(AssumeRoleWithSAMLRequest request, AmazonServiceCallback<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse> callback, AsyncOptions options = null);

		void AssumeRoleWithWebIdentityAsync(AssumeRoleWithWebIdentityRequest request, AmazonServiceCallback<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> callback, AsyncOptions options = null);

		void DecodeAuthorizationMessageAsync(DecodeAuthorizationMessageRequest request, AmazonServiceCallback<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse> callback, AsyncOptions options = null);

		void GetCallerIdentityAsync(GetCallerIdentityRequest request, AmazonServiceCallback<GetCallerIdentityRequest, GetCallerIdentityResponse> callback, AsyncOptions options = null);

		void GetFederationTokenAsync(GetFederationTokenRequest request, AmazonServiceCallback<GetFederationTokenRequest, GetFederationTokenResponse> callback, AsyncOptions options = null);

		void GetSessionTokenAsync(GetSessionTokenRequest request, AmazonServiceCallback<GetSessionTokenRequest, GetSessionTokenResponse> callback, AsyncOptions options = null);
	}
}
