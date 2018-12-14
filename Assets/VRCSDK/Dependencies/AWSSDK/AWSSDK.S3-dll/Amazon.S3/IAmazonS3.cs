using Amazon.Runtime;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;

namespace Amazon.S3
{
	public interface IAmazonS3 : IAmazonService, IDisposable
	{
		void PostObjectAsync(PostObjectRequest request, AmazonServiceCallback<PostObjectRequest, PostObjectResponse> callback, AsyncOptions options = null);

		void AbortMultipartUploadAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null);

		void AbortMultipartUploadAsync(AbortMultipartUploadRequest request, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null);

		void CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, AmazonServiceCallback<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse> callback, AsyncOptions options = null);

		void CopyObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null);

		void CopyObjectAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null);

		void CopyObjectAsync(CopyObjectRequest request, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null);

		void CopyPartAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null);

		void CopyPartAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null);

		void CopyPartAsync(CopyPartRequest request, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null);

		void DeleteBucketAsync(string bucketName, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null);

		void DeleteBucketAsync(DeleteBucketRequest request, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null);

		void DeleteBucketAnalyticsConfigurationAsync(DeleteBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, AmazonServiceCallback<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteBucketMetricsConfigurationAsync(DeleteBucketMetricsConfigurationRequest request, AmazonServiceCallback<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteBucketPolicyAsync(string bucketName, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null);

		void DeleteBucketPolicyAsync(DeleteBucketPolicyRequest request, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null);

		void DeleteBucketReplicationAsync(DeleteBucketReplicationRequest request, AmazonServiceCallback<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse> callback, AsyncOptions options = null);

		void DeleteBucketTaggingAsync(string bucketName, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null);

		void DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null);

		void DeleteBucketWebsiteAsync(string bucketName, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null);

		void DeleteBucketWebsiteAsync(DeleteBucketWebsiteRequest request, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null);

		void DeleteCORSConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteCORSConfigurationAsync(DeleteCORSConfigurationRequest request, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteLifecycleConfigurationAsync(DeleteLifecycleConfigurationRequest request, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void DeleteObjectAsync(string bucketName, string key, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null);

		void DeleteObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null);

		void DeleteObjectAsync(DeleteObjectRequest request, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null);

		void DeleteObjectsAsync(DeleteObjectsRequest request, AmazonServiceCallback<DeleteObjectsRequest, DeleteObjectsResponse> callback, AsyncOptions options = null);

		void DeleteObjectTaggingAsync(DeleteObjectTaggingRequest request, AmazonServiceCallback<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse> callback, AsyncOptions options = null);

		void GetACLAsync(string bucketName, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null);

		void GetACLAsync(GetACLRequest request, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null);

		void GetBucketAccelerateConfigurationAsync(string bucketName, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null);

		void GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null);

		void GetBucketAnalyticsConfigurationAsync(GetBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null);

		void GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, AmazonServiceCallback<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse> callback, AsyncOptions options = null);

		void GetBucketLocationAsync(string bucketName, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null);

		void GetBucketLocationAsync(GetBucketLocationRequest request, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null);

		void GetBucketLoggingAsync(string bucketName, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null);

		void GetBucketLoggingAsync(GetBucketLoggingRequest request, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null);

		void GetBucketMetricsConfigurationAsync(GetBucketMetricsConfigurationRequest request, AmazonServiceCallback<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse> callback, AsyncOptions options = null);

		void GetBucketNotificationAsync(string bucketName, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null);

		void GetBucketNotificationAsync(GetBucketNotificationRequest request, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null);

		void GetBucketPolicyAsync(string bucketName, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null);

		void GetBucketPolicyAsync(GetBucketPolicyRequest request, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null);

		void GetBucketReplicationAsync(GetBucketReplicationRequest request, AmazonServiceCallback<GetBucketReplicationRequest, GetBucketReplicationResponse> callback, AsyncOptions options = null);

		void GetBucketRequestPaymentAsync(string bucketName, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null);

		void GetBucketRequestPaymentAsync(GetBucketRequestPaymentRequest request, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null);

		void GetBucketTaggingAsync(GetBucketTaggingRequest request, AmazonServiceCallback<GetBucketTaggingRequest, GetBucketTaggingResponse> callback, AsyncOptions options = null);

		void GetBucketVersioningAsync(string bucketName, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null);

		void GetBucketVersioningAsync(GetBucketVersioningRequest request, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null);

		void GetBucketWebsiteAsync(string bucketName, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null);

		void GetBucketWebsiteAsync(GetBucketWebsiteRequest request, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null);

		void GetCORSConfigurationAsync(string bucketName, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null);

		void GetCORSConfigurationAsync(GetCORSConfigurationRequest request, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null);

		void GetLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void GetLifecycleConfigurationAsync(GetLifecycleConfigurationRequest request, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void GetObjectAsync(string bucketName, string key, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null);

		void GetObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null);

		void GetObjectAsync(GetObjectRequest request, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null);

		void GetObjectMetadataAsync(string bucketName, string key, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null);

		void GetObjectMetadataAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null);

		void GetObjectMetadataAsync(GetObjectMetadataRequest request, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null);

		void GetObjectTaggingAsync(GetObjectTaggingRequest request, AmazonServiceCallback<GetObjectTaggingRequest, GetObjectTaggingResponse> callback, AsyncOptions options = null);

		void GetObjectTorrentAsync(string bucketName, string key, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null);

		void GetObjectTorrentAsync(GetObjectTorrentRequest request, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null);

		void InitiateMultipartUploadAsync(string bucketName, string key, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null);

		void InitiateMultipartUploadAsync(InitiateMultipartUploadRequest request, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null);

		void ListBucketAnalyticsConfigurationsAsync(ListBucketAnalyticsConfigurationsRequest request, AmazonServiceCallback<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse> callback, AsyncOptions options = null);

		void ListBucketInventoryConfigurationsAsync(ListBucketInventoryConfigurationsRequest request, AmazonServiceCallback<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse> callback, AsyncOptions options = null);

		void ListBucketMetricsConfigurationsAsync(ListBucketMetricsConfigurationsRequest request, AmazonServiceCallback<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse> callback, AsyncOptions options = null);

		void ListBucketsAsync(ListBucketsRequest request, AmazonServiceCallback<ListBucketsRequest, ListBucketsResponse> callback, AsyncOptions options = null);

		void ListMultipartUploadsAsync(string bucketName, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null);

		void ListMultipartUploadsAsync(string bucketName, string prefix, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null);

		void ListMultipartUploadsAsync(ListMultipartUploadsRequest request, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null);

		void ListObjectsAsync(string bucketName, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null);

		void ListObjectsAsync(string bucketName, string prefix, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null);

		void ListObjectsAsync(ListObjectsRequest request, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null);

		void ListObjectsV2Async(ListObjectsV2Request request, AmazonServiceCallback<ListObjectsV2Request, ListObjectsV2Response> callback, AsyncOptions options = null);

		void ListPartsAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null);

		void ListPartsAsync(ListPartsRequest request, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null);

		void ListVersionsAsync(string bucketName, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null);

		void ListVersionsAsync(string bucketName, string prefix, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null);

		void ListVersionsAsync(ListVersionsRequest request, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null);

		void PutACLAsync(PutACLRequest request, AmazonServiceCallback<PutACLRequest, PutACLResponse> callback, AsyncOptions options = null);

		void PutBucketAsync(string bucketName, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null);

		void PutBucketAsync(PutBucketRequest request, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null);

		void PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, AmazonServiceCallback<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null);

		void PutBucketAnalyticsConfigurationAsync(PutBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null);

		void PutBucketInventoryConfigurationAsync(PutBucketInventoryConfigurationRequest request, AmazonServiceCallback<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse> callback, AsyncOptions options = null);

		void PutBucketLoggingAsync(PutBucketLoggingRequest request, AmazonServiceCallback<PutBucketLoggingRequest, PutBucketLoggingResponse> callback, AsyncOptions options = null);

		void PutBucketMetricsConfigurationAsync(PutBucketMetricsConfigurationRequest request, AmazonServiceCallback<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse> callback, AsyncOptions options = null);

		void PutBucketNotificationAsync(PutBucketNotificationRequest request, AmazonServiceCallback<PutBucketNotificationRequest, PutBucketNotificationResponse> callback, AsyncOptions options = null);

		void PutBucketPolicyAsync(string bucketName, string policy, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null);

		void PutBucketPolicyAsync(string bucketName, string policy, string contentMD5, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null);

		void PutBucketPolicyAsync(PutBucketPolicyRequest request, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null);

		void PutBucketReplicationAsync(PutBucketReplicationRequest request, AmazonServiceCallback<PutBucketReplicationRequest, PutBucketReplicationResponse> callback, AsyncOptions options = null);

		void PutBucketRequestPaymentAsync(string bucketName, RequestPaymentConfiguration requestPaymentConfiguration, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null);

		void PutBucketRequestPaymentAsync(PutBucketRequestPaymentRequest request, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null);

		void PutBucketTaggingAsync(string bucketName, List<Tag> tagSet, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null);

		void PutBucketTaggingAsync(PutBucketTaggingRequest request, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null);

		void PutBucketVersioningAsync(PutBucketVersioningRequest request, AmazonServiceCallback<PutBucketVersioningRequest, PutBucketVersioningResponse> callback, AsyncOptions options = null);

		void PutBucketWebsiteAsync(string bucketName, WebsiteConfiguration websiteConfiguration, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null);

		void PutBucketWebsiteAsync(PutBucketWebsiteRequest request, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null);

		void PutCORSConfigurationAsync(string bucketName, CORSConfiguration configuration, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null);

		void PutCORSConfigurationAsync(PutCORSConfigurationRequest request, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null);

		void PutLifecycleConfigurationAsync(string bucketName, LifecycleConfiguration configuration, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void PutLifecycleConfigurationAsync(PutLifecycleConfigurationRequest request, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null);

		void PutObjectAsync(PutObjectRequest request, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> callback, AsyncOptions options = null);

		void PutObjectTaggingAsync(PutObjectTaggingRequest request, AmazonServiceCallback<PutObjectTaggingRequest, PutObjectTaggingResponse> callback, AsyncOptions options = null);

		void RestoreObjectAsync(string bucketName, string key, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null);

		void RestoreObjectAsync(string bucketName, string key, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null);

		void RestoreObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null);

		void RestoreObjectAsync(string bucketName, string key, string versionId, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null);

		void RestoreObjectAsync(RestoreObjectRequest request, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null);

		void UploadPartAsync(UploadPartRequest request, AmazonServiceCallback<UploadPartRequest, UploadPartResponse> callback, AsyncOptions options = null);
	}
}
