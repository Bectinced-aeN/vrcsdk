using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Internal;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Amazon.S3.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Amazon.S3
{
	public class AmazonS3Client : AmazonServiceClient, IAmazonS3, IAmazonService, IDisposable
	{
		internal string GetPreSignedURLInternal(GetPreSignedUrlRequest request, bool useSigV2Fallback = true)
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Expected O, but got Unknown
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			if (this.get_Credentials() == null)
			{
				throw new AmazonS3Exception("Credentials must be specified, cannot call method anonymously");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request", "The PreSignedUrlRequest specified is null!");
			}
			if (!request.IsSetExpires())
			{
				throw new InvalidOperationException("The Expires specified is null!");
			}
			bool flag = AWSConfigsS3.UseSignatureVersion4;
			string text = AWS4Signer.DetermineSigningRegion(this.get_Config(), "s3", null, null);
			if (flag && string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("To use AWS4 signing, a region must be specified in the client configuration using the AuthenticationRegion or Region properties, or be determinable from the service URL.");
			}
			RegionEndpoint bySystemName = RegionEndpoint.GetBySystemName(text);
			if (bySystemName.GetEndpointForService("s3").get_SignatureVersionOverride() == "4" || bySystemName.GetEndpointForService("s3").get_SignatureVersionOverride() == null)
			{
				flag = true;
			}
			bool flag2 = useSigV2Fallback && !AWSConfigsS3.UseSigV4SetExplicitly;
			if (bySystemName == RegionEndpoint.USEast1 && flag2)
			{
				flag = false;
			}
			ImmutableCredentials credentials = this.get_Credentials().GetCredentials();
			IRequest val = Marshall(request, credentials.get_AccessKey(), credentials.get_Token(), flag);
			val.set_Endpoint(EndpointResolver.DetermineEndpoint(this.get_Config(), val));
			RequestContext val2 = new RequestContext(true);
			val2.set_Request(val);
			val2.set_ClientConfig(this.get_Config());
			AmazonS3PostMarshallHandler.ProcessRequestHandlers(new ExecutionContext(val2, null));
			RequestMetrics val3 = new RequestMetrics();
			string str;
			if (flag)
			{
				AWS4SigningResult val4 = new AWS4PreSignedUrlSigner().SignRequest(val, this.get_Config(), val3, credentials.get_AccessKey(), credentials.get_SecretKey());
				str = "&" + val4.get_ForQueryParameters();
			}
			else
			{
				S3Signer.SignRequest(val, val3, credentials.get_AccessKey(), credentials.get_SecretKey());
				str = val.get_Headers()["Authorization"];
				str = str.Substring(str.IndexOf(":", StringComparison.Ordinal) + 1);
				str = "&Signature=" + AmazonS3Util.UrlEncode(str, path: false);
			}
			string text2 = AmazonServiceClient.ComposeUrl(val).AbsoluteUri + str;
			Protocol protocol = DetermineProtocol();
			if (request.Protocol != protocol)
			{
				switch (protocol)
				{
				case Protocol.HTTP:
					text2 = text2.Replace("http://", "https://");
					break;
				case Protocol.HTTPS:
					text2 = text2.Replace("https://", "http://");
					break;
				}
			}
			return text2;
		}

		private static IRequest Marshall(GetPreSignedUrlRequest getPreSignedUrlRequest, string accessKey, string token, bool aws4Signing)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			IRequest val = new DefaultRequest(getPreSignedUrlRequest, "AmazonS3");
			val.set_HttpMethod(getPreSignedUrlRequest.Verb.ToString());
			HeadersCollection headers = getPreSignedUrlRequest.Headers;
			foreach (string key in headers.Keys)
			{
				val.get_Headers()[key] = headers[key];
			}
			AmazonS3Util.SetMetadataHeaders(val, getPreSignedUrlRequest.Metadata);
			if (!string.IsNullOrEmpty(token))
			{
				val.get_Headers()["x-amz-security-token"] = token;
			}
			if (getPreSignedUrlRequest.ServerSideEncryptionMethod != null && getPreSignedUrlRequest.ServerSideEncryptionMethod != ServerSideEncryptionMethod.None)
			{
				val.get_Headers().Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(ConstantClass.op_Implicit(getPreSignedUrlRequest.ServerSideEncryptionMethod)));
			}
			if (getPreSignedUrlRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-customer-algorithm", ConstantClass.op_Implicit(getPreSignedUrlRequest.ServerSideEncryptionCustomerMethod));
			}
			if (getPreSignedUrlRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				val.get_Headers().Add("x-amz-server-side-encryption-aws-kms-key-id", getPreSignedUrlRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			IDictionary<string, string> parameters = val.get_Parameters();
			StringBuilder stringBuilder = new StringBuilder("/");
			if (!string.IsNullOrEmpty(getPreSignedUrlRequest.BucketName))
			{
				stringBuilder.Append(S3Transforms.ToStringValue(getPreSignedUrlRequest.BucketName));
			}
			if (!string.IsNullOrEmpty(getPreSignedUrlRequest.Key))
			{
				if (stringBuilder.Length > 1)
				{
					stringBuilder.Append("/");
				}
				stringBuilder.Append(S3Transforms.ToStringValue(getPreSignedUrlRequest.Key));
			}
			DateTime d = aws4Signing ? AWSSDKUtils.get_CorrectedUtcNow() : new DateTime(1970, 1, 1);
			long num = Convert.ToInt64((getPreSignedUrlRequest.Expires.ToUniversalTime() - d).TotalSeconds);
			if (aws4Signing && num > 604800)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The maximum expiry period for a presigned url using AWS4 signing is {0} seconds", 604800L));
			}
			parameters.Add(aws4Signing ? "X-Amz-Expires" : "Expires", num.ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(token))
			{
				parameters.Add("x-amz-security-token", token);
			}
			if (!aws4Signing)
			{
				parameters.Add("AWSAccessKeyId", accessKey);
			}
			if (getPreSignedUrlRequest.IsSetVersionId())
			{
				val.AddSubResource("versionId", S3Transforms.ToStringValue(getPreSignedUrlRequest.VersionId));
			}
			ResponseHeaderOverrides responseHeaderOverrides = getPreSignedUrlRequest.ResponseHeaderOverrides;
			if (!string.IsNullOrEmpty(responseHeaderOverrides.CacheControl))
			{
				parameters.Add("response-cache-control", responseHeaderOverrides.CacheControl);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentType))
			{
				parameters.Add("response-content-type", responseHeaderOverrides.ContentType);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentLanguage))
			{
				parameters.Add("response-content-language", responseHeaderOverrides.ContentLanguage);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.Expires))
			{
				parameters.Add("response-expires", responseHeaderOverrides.Expires);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentDisposition))
			{
				parameters.Add("response-content-disposition", responseHeaderOverrides.ContentDisposition);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentEncoding))
			{
				parameters.Add("response-content-encoding", responseHeaderOverrides.ContentEncoding);
			}
			val.set_ResourcePath(stringBuilder.ToString());
			val.set_UseQueryString(true);
			return val;
		}

		private Protocol DetermineProtocol()
		{
			if (!this.get_Config().DetermineServiceURL().StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				return Protocol.HTTP;
			}
			return Protocol.HTTPS;
		}

		internal static void CleanupRequest(IRequest request)
		{
			PutObjectRequest putObjectRequest = request.get_OriginalRequest() as PutObjectRequest;
			if (putObjectRequest != null)
			{
				if (putObjectRequest.InputStream != null && (!string.IsNullOrEmpty(putObjectRequest.FilePath) || putObjectRequest.AutoCloseStream))
				{
					putObjectRequest.InputStream.Dispose();
				}
				if (!string.IsNullOrEmpty(putObjectRequest.FilePath) || !string.IsNullOrEmpty(putObjectRequest.ContentBody))
				{
					putObjectRequest.InputStream = null;
				}
			}
			UploadPartRequest uploadPartRequest = request.get_OriginalRequest() as UploadPartRequest;
			if (uploadPartRequest != null)
			{
				if (uploadPartRequest.IsSetFilePath() && uploadPartRequest.InputStream != null)
				{
					uploadPartRequest.InputStream.Dispose();
				}
				if (uploadPartRequest.IsSetFilePath())
				{
					uploadPartRequest.InputStream = null;
				}
			}
		}

		public void PostObjectAsync(PostObjectRequest request, AmazonServiceCallback<PostObjectRequest, PostObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
			{
				AmazonServiceResult<PostObjectRequest, PostObjectResponse> val = new AmazonServiceResult<PostObjectRequest, PostObjectResponse>((PostObjectRequest)req, (PostObjectResponse)res, ex, ao.get_State());
				if (callback != null)
				{
					callback.Invoke(val);
				}
			};
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					InferContentType(request);
					if (request.SignedPolicy == null)
					{
						CreateSignedPolicy(request);
					}
					PostObject(request, options, callbackHelper);
				}
				catch (Exception ex2)
				{
					callback.Invoke(new AmazonServiceResult<PostObjectRequest, PostObjectResponse>(request, (PostObjectResponse)null, ex2, options.get_State()));
				}
			});
		}

		private void InferContentType(PostObjectRequest request)
		{
			if (string.IsNullOrEmpty(request.Headers.ContentType))
			{
				if (request.Key.IndexOf('.') > -1)
				{
					request.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(request.Key.Substring(request.Key.LastIndexOf('.')));
				}
				else if (!string.IsNullOrEmpty(request.Path) && request.Path.IndexOf('.') > -1)
				{
					request.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(request.Key.Substring(request.Path.LastIndexOf('.')));
				}
				else
				{
					request.Headers.ContentType = "application/octet-stream";
				}
			}
		}

		private void CreateSignedPolicy(PostObjectRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> metadatum in request.Metadata)
			{
				string arg = metadatum.Key.StartsWith(S3Constants.PostFormDataXAmzPrefix, StringComparison.Ordinal) ? metadatum.Key : (S3Constants.PostFormDataMetaPrefix + metadatum.Key);
				stringBuilder.Append($",{{\"{arg}\": \"{metadatum.Value}\"}}");
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (string key in request.Headers.Keys)
			{
				stringBuilder2.Append($",{{\"{key}\": \"{request.Headers[key]}\"}}");
			}
			string text = null;
			int num = request.Key.LastIndexOf('/');
			text = ((num != -1) ? ("{\"expiration\": \"" + DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + request.Bucket + "\"},[\"starts-with\", \"$key\", \"" + request.Key.Substring(0, num) + "/\"],{\"acl\": \"" + request.CannedACL.get_Value() + "\"},[\"eq\", \"$Content-Type\", \"" + request.Headers.ContentType + "\"]" + stringBuilder.ToString() + stringBuilder2.ToString() + "]}") : ("{\"expiration\": \"" + DateTime.UtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + request.Bucket + "\"},[\"starts-with\", \"$key\", \"\"],{\"acl\": \"" + request.CannedACL.get_Value() + "\"},[\"eq\", \"$Content-Type\", \"" + request.Headers.ContentType + "\"]" + stringBuilder.ToString() + stringBuilder2.ToString() + "]}"));
			if (this.get_Config().get_SignatureVersion() == "2")
			{
				request.SignedPolicy = S3PostUploadSignedPolicy.GetSignedPolicy(text, this.get_Credentials());
			}
			else
			{
				request.SignedPolicy = S3PostUploadSignedPolicy.GetSignedPolicyV4(text, this.get_Credentials(), request.Region);
			}
		}

		private void PostObject(PostObjectRequest request, AsyncOptions options, Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper)
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Expected O, but got Unknown
			//IL_025d: Expected O, but got Unknown
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Expected O, but got Unknown
			string text = ((object)request.Region).Equals((object)RegionEndpoint.USEast1) ? "s3" : ("s3-" + request.Region.get_SystemName());
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			string uriString = (request.Bucket.IndexOf('.') <= -1) ? string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}.amazonaws.com", request.Bucket, text) : string.Format(CultureInfo.InvariantCulture, "https://{0}.amazonaws.com/{1}/", text, request.Bucket);
			Uri uri = new Uri(uriString);
			IHttpRequest<string> val = null;
			val = (((int)AWSConfigs.get_HttpClient() != 0) ? ((object)new UnityRequest(uri)) : ((object)new UnityWwwRequest(uri)));
			string text2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('=', 'z');
			dictionary["Content-Type"] = string.Format(CultureInfo.InvariantCulture, "multipart/form-data; boundary={0}", text2);
			dictionary["User-Agent"] = "User-Agent";
			val.set_Method("POST");
			using (MemoryStream memoryStream = new MemoryStream())
			{
				request.WriteFormData(text2, memoryStream);
				byte[] bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"file\"\r\n\r\n", text2));
				memoryStream.Write(bytes, 0, bytes.Length);
				using (Stream stream = (request.Path == null) ? request.InputStream : File.OpenRead(request.Path))
				{
					byte[] buffer = new byte[1024];
					int count;
					while ((count = stream.Read(buffer, 0, 1024)) > 0)
					{
						memoryStream.Write(buffer, 0, count);
					}
				}
				byte[] bytes2 = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "\r\n--{0}--", text2));
				memoryStream.Write(bytes2, 0, bytes2.Length);
				val.WriteToRequestBody((string)null, memoryStream.ToArray(), dictionary);
				EventHandler<StreamTransferProgressArgs> streamUploadProgressCallback = request.get_StreamUploadProgressCallback();
				if (streamUploadProgressCallback != null)
				{
					val.SetupProgressListeners((Stream)memoryStream, 0L, (object)request, streamUploadProgressCallback);
				}
			}
			AsyncRequestContext val2 = new AsyncRequestContext(this.get_Config().get_LogMetrics());
			val2.set_ClientConfig(this.get_Config());
			val2.set_OriginalRequest(request);
			val2.set_Action(callbackHelper);
			val2.set_AsyncOptions(options);
			val2.set_IsAsync(true);
			AsyncExecutionContext val3 = new AsyncExecutionContext(val2, new AsyncResponseContext());
			val.SetRequestHeaders(dictionary);
			val3.set_RuntimeState((object)val);
			val3.get_ResponseContext().set_AsyncResult(new RuntimeAsyncResult(val3.get_RequestContext().get_Callback(), val3.get_RequestContext().get_State()));
			val3.get_ResponseContext().get_AsyncResult().set_AsyncOptions(val3.get_RequestContext().get_AsyncOptions());
			val3.get_ResponseContext().get_AsyncResult().set_Action(val3.get_RequestContext().get_Action());
			val3.get_ResponseContext().get_AsyncResult().set_Request(val3.get_RequestContext().get_OriginalRequest());
			val.BeginGetResponse((AsyncCallback)ProcessPostResponse, (object)val3);
		}

		private void ProcessPostResponse(IAsyncResult result)
		{
			IAsyncExecutionContext val = null;
			IHttpRequest<string> val2 = null;
			try
			{
				val = (result.AsyncState as IAsyncExecutionContext);
				val2 = (val.get_RuntimeState() as IHttpRequest<string>);
				IWebResponseData httpResponse = val2.EndGetResponse(result);
				val.get_ResponseContext().set_HttpResponse(httpResponse);
			}
			catch (Exception exception)
			{
				val.get_ResponseContext().get_AsyncResult().set_Exception(exception);
			}
			finally
			{
				((IDisposable)val2).Dispose();
			}
			PostResponseHelper(result);
		}

		private void PostResponseHelper(IAsyncResult result)
		{
			IAsyncExecutionContext val = result.AsyncState as IAsyncExecutionContext;
			IWebResponseData httpResponse = val.get_ResponseContext().get_HttpResponse();
			RuntimeAsyncResult asyncResult = val.get_ResponseContext().get_AsyncResult();
			if (val.get_ResponseContext().get_AsyncResult().get_Exception() == null)
			{
				PostObjectResponse postObjectResponse = new PostObjectResponse();
				postObjectResponse.set_HttpStatusCode(httpResponse.get_StatusCode());
				postObjectResponse.set_ContentLength(httpResponse.get_ContentLength());
				if (httpResponse.IsHeaderPresent("x-amz-request-id"))
				{
					postObjectResponse.RequestId = httpResponse.GetHeaderValue("x-amz-request-id");
				}
				if (httpResponse.IsHeaderPresent("x-amz-id-2"))
				{
					postObjectResponse.HostId = httpResponse.GetHeaderValue("x-amz-id-2");
				}
				if (httpResponse.IsHeaderPresent("x-amz-version-id"))
				{
					postObjectResponse.VersionId = httpResponse.GetHeaderValue("x-amz-version-id");
				}
				PostObjectRequest request = val.get_RequestContext().get_OriginalRequest() as PostObjectRequest;
				asyncResult.set_Request(request);
				asyncResult.set_Response(postObjectResponse);
			}
			asyncResult.set_Exception(val.get_ResponseContext().get_AsyncResult().get_Exception());
			asyncResult.set_Action(val.get_RequestContext().get_Action());
			asyncResult.InvokeCallback();
		}

		public AmazonS3Client(AWSCredentials credentials)
			: this(credentials, new AmazonS3Config())
		{
		}

		public AmazonS3Client(AWSCredentials credentials, RegionEndpoint region)
		{
			AmazonS3Config amazonS3Config = new AmazonS3Config();
			amazonS3Config.set_RegionEndpoint(region);
			this._002Ector(credentials, amazonS3Config);
		}

		public AmazonS3Client(AWSCredentials credentials, AmazonS3Config clientConfig)
			: this(credentials, clientConfig)
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonS3Config())
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
		{
			AmazonS3Config amazonS3Config = new AmazonS3Config();
			amazonS3Config.set_RegionEndpoint(region);
			this._002Ector(awsAccessKeyId, awsSecretAccessKey, amazonS3Config);
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, AmazonS3Config clientConfig)
			: this(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonS3Config())
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
		{
			AmazonS3Config amazonS3Config = new AmazonS3Config();
			amazonS3Config.set_RegionEndpoint(region);
			this._002Ector(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, amazonS3Config);
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonS3Config clientConfig)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
		{
		}

		protected override AbstractAWSSigner CreateSigner()
		{
			return new S3Signer();
		}

		protected override void CustomizeRuntimePipeline(RuntimePipeline pipeline)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			pipeline.AddHandlerBefore<Marshaller>(new AmazonS3PreMarshallHandler());
			pipeline.AddHandlerAfter<Marshaller>(new AmazonS3PostMarshallHandler());
			pipeline.AddHandlerBefore<EndpointResolver>(new AmazonS3HttpDeleteHandler());
			pipeline.AddHandlerAfter<EndpointResolver>(new AmazonS3KmsHandler());
			pipeline.AddHandlerBefore<Unmarshaller>(new AmazonS3ResponseHandler());
			pipeline.AddHandlerAfter<ErrorCallbackHandler>(new AmazonS3ExceptionHandler());
			pipeline.AddHandlerAfter<Unmarshaller>(new AmazonS3RedirectHandler());
			pipeline.ReplaceHandler<RetryHandler>(new RetryHandler(new AmazonS3RetryPolicy(this.get_Config())));
		}

		protected override void Dispose(bool disposing)
		{
			this.Dispose(disposing);
		}

		public void AbortMultipartUploadAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			AbortMultipartUploadRequest abortMultipartUploadRequest = new AbortMultipartUploadRequest();
			abortMultipartUploadRequest.BucketName = bucketName;
			abortMultipartUploadRequest.Key = key;
			abortMultipartUploadRequest.UploadId = uploadId;
			AbortMultipartUploadAsync(abortMultipartUploadRequest, callback, options);
		}

		public void AbortMultipartUploadAsync(AbortMultipartUploadRequest request, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("AbortMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			AbortMultipartUploadRequestMarshaller abortMultipartUploadRequestMarshaller = new AbortMultipartUploadRequestMarshaller();
			AbortMultipartUploadResponseUnmarshaller instance = AbortMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AbortMultipartUploadRequest, AbortMultipartUploadResponse> val = new AmazonServiceResult<AbortMultipartUploadRequest, AbortMultipartUploadResponse>((AbortMultipartUploadRequest)req, (AbortMultipartUploadResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<AbortMultipartUploadRequest>(request, abortMultipartUploadRequestMarshaller, instance, options, action);
		}

		public void CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, AmazonServiceCallback<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("CompleteMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			CompleteMultipartUploadRequestMarshaller completeMultipartUploadRequestMarshaller = new CompleteMultipartUploadRequestMarshaller();
			CompleteMultipartUploadResponseUnmarshaller instance = CompleteMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse> val = new AmazonServiceResult<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>((CompleteMultipartUploadRequest)req, (CompleteMultipartUploadResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<CompleteMultipartUploadRequest>(request, completeMultipartUploadRequestMarshaller, instance, options, action);
		}

		public void CopyObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			CopyObjectRequest copyObjectRequest = new CopyObjectRequest();
			copyObjectRequest.SourceBucket = sourceBucket;
			copyObjectRequest.SourceKey = sourceKey;
			copyObjectRequest.DestinationBucket = destinationBucket;
			copyObjectRequest.DestinationKey = destinationKey;
			CopyObjectAsync(copyObjectRequest, callback, options);
		}

		public void CopyObjectAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			CopyObjectRequest copyObjectRequest = new CopyObjectRequest();
			copyObjectRequest.SourceBucket = sourceBucket;
			copyObjectRequest.SourceKey = sourceKey;
			copyObjectRequest.SourceVersionId = sourceVersionId;
			copyObjectRequest.DestinationBucket = destinationBucket;
			copyObjectRequest.DestinationKey = destinationKey;
			CopyObjectAsync(copyObjectRequest, callback, options);
		}

		public void CopyObjectAsync(CopyObjectRequest request, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("CopyObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			CopyObjectRequestMarshaller copyObjectRequestMarshaller = new CopyObjectRequestMarshaller();
			CopyObjectResponseUnmarshaller instance = CopyObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CopyObjectRequest, CopyObjectResponse> val = new AmazonServiceResult<CopyObjectRequest, CopyObjectResponse>((CopyObjectRequest)req, (CopyObjectResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<CopyObjectRequest>(request, copyObjectRequestMarshaller, instance, options, action);
		}

		public void CopyPartAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			CopyPartRequest copyPartRequest = new CopyPartRequest();
			copyPartRequest.SourceBucket = sourceBucket;
			copyPartRequest.SourceKey = sourceKey;
			copyPartRequest.DestinationBucket = destinationBucket;
			copyPartRequest.DestinationKey = destinationKey;
			copyPartRequest.UploadId = uploadId;
			CopyPartAsync(copyPartRequest, callback, options);
		}

		public void CopyPartAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			CopyPartRequest copyPartRequest = new CopyPartRequest();
			copyPartRequest.SourceBucket = sourceBucket;
			copyPartRequest.SourceKey = sourceKey;
			copyPartRequest.SourceVersionId = sourceVersionId;
			copyPartRequest.DestinationBucket = destinationBucket;
			copyPartRequest.DestinationKey = destinationKey;
			copyPartRequest.UploadId = uploadId;
			CopyPartAsync(copyPartRequest, callback, options);
		}

		public void CopyPartAsync(CopyPartRequest request, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("CopyPart is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			CopyPartRequestMarshaller copyPartRequestMarshaller = new CopyPartRequestMarshaller();
			CopyPartResponseUnmarshaller instance = CopyPartResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CopyPartRequest, CopyPartResponse> val = new AmazonServiceResult<CopyPartRequest, CopyPartResponse>((CopyPartRequest)req, (CopyPartResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<CopyPartRequest>(request, copyPartRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketAsync(string bucketName, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketRequest deleteBucketRequest = new DeleteBucketRequest();
			deleteBucketRequest.BucketName = bucketName;
			DeleteBucketAsync(deleteBucketRequest, callback, options);
		}

		public void DeleteBucketAsync(DeleteBucketRequest request, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketRequestMarshaller deleteBucketRequestMarshaller = new DeleteBucketRequestMarshaller();
			DeleteBucketResponseUnmarshaller instance = DeleteBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketRequest, DeleteBucketResponse> val = new AmazonServiceResult<DeleteBucketRequest, DeleteBucketResponse>((DeleteBucketRequest)req, (DeleteBucketResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketRequest>(request, deleteBucketRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketAnalyticsConfigurationAsync(DeleteBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketAnalyticsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketAnalyticsConfigurationRequestMarshaller deleteBucketAnalyticsConfigurationRequestMarshaller = new DeleteBucketAnalyticsConfigurationRequestMarshaller();
			DeleteBucketAnalyticsConfigurationResponseUnmarshaller instance = DeleteBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse> val = new AmazonServiceResult<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse>((DeleteBucketAnalyticsConfigurationRequest)req, (DeleteBucketAnalyticsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketAnalyticsConfigurationRequest>(request, deleteBucketAnalyticsConfigurationRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, AmazonServiceCallback<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketInventoryConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketInventoryConfigurationRequestMarshaller deleteBucketInventoryConfigurationRequestMarshaller = new DeleteBucketInventoryConfigurationRequestMarshaller();
			DeleteBucketInventoryConfigurationResponseUnmarshaller instance = DeleteBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse> val = new AmazonServiceResult<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse>((DeleteBucketInventoryConfigurationRequest)req, (DeleteBucketInventoryConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketInventoryConfigurationRequest>(request, deleteBucketInventoryConfigurationRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketMetricsConfigurationAsync(DeleteBucketMetricsConfigurationRequest request, AmazonServiceCallback<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketMetricsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketMetricsConfigurationRequestMarshaller deleteBucketMetricsConfigurationRequestMarshaller = new DeleteBucketMetricsConfigurationRequestMarshaller();
			DeleteBucketMetricsConfigurationResponseUnmarshaller instance = DeleteBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse> val = new AmazonServiceResult<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse>((DeleteBucketMetricsConfigurationRequest)req, (DeleteBucketMetricsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketMetricsConfigurationRequest>(request, deleteBucketMetricsConfigurationRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketPolicyAsync(string bucketName, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketPolicyRequest deleteBucketPolicyRequest = new DeleteBucketPolicyRequest();
			deleteBucketPolicyRequest.BucketName = bucketName;
			DeleteBucketPolicyAsync(deleteBucketPolicyRequest, callback, options);
		}

		public void DeleteBucketPolicyAsync(DeleteBucketPolicyRequest request, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketPolicy is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketPolicyRequestMarshaller deleteBucketPolicyRequestMarshaller = new DeleteBucketPolicyRequestMarshaller();
			DeleteBucketPolicyResponseUnmarshaller instance = DeleteBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> val = new AmazonServiceResult<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse>((DeleteBucketPolicyRequest)req, (DeleteBucketPolicyResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketPolicyRequest>(request, deleteBucketPolicyRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketReplicationAsync(DeleteBucketReplicationRequest request, AmazonServiceCallback<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketReplicationRequestMarshaller deleteBucketReplicationRequestMarshaller = new DeleteBucketReplicationRequestMarshaller();
			DeleteBucketReplicationResponseUnmarshaller instance = DeleteBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse> val = new AmazonServiceResult<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse>((DeleteBucketReplicationRequest)req, (DeleteBucketReplicationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketReplicationRequest>(request, deleteBucketReplicationRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketTaggingAsync(string bucketName, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketTaggingRequest deleteBucketTaggingRequest = new DeleteBucketTaggingRequest();
			deleteBucketTaggingRequest.BucketName = bucketName;
			DeleteBucketTaggingAsync(deleteBucketTaggingRequest, callback, options);
		}

		public void DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketTaggingRequestMarshaller deleteBucketTaggingRequestMarshaller = new DeleteBucketTaggingRequestMarshaller();
			DeleteBucketTaggingResponseUnmarshaller instance = DeleteBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> val = new AmazonServiceResult<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse>((DeleteBucketTaggingRequest)req, (DeleteBucketTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketTaggingRequest>(request, deleteBucketTaggingRequestMarshaller, instance, options, action);
		}

		public void DeleteBucketWebsiteAsync(string bucketName, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketWebsiteRequest deleteBucketWebsiteRequest = new DeleteBucketWebsiteRequest();
			deleteBucketWebsiteRequest.BucketName = bucketName;
			DeleteBucketWebsiteAsync(deleteBucketWebsiteRequest, callback, options);
		}

		public void DeleteBucketWebsiteAsync(DeleteBucketWebsiteRequest request, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteBucketWebsite is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteBucketWebsiteRequestMarshaller deleteBucketWebsiteRequestMarshaller = new DeleteBucketWebsiteRequestMarshaller();
			DeleteBucketWebsiteResponseUnmarshaller instance = DeleteBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> val = new AmazonServiceResult<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse>((DeleteBucketWebsiteRequest)req, (DeleteBucketWebsiteResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteBucketWebsiteRequest>(request, deleteBucketWebsiteRequestMarshaller, instance, options, action);
		}

		public void DeleteCORSConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			DeleteCORSConfigurationRequest deleteCORSConfigurationRequest = new DeleteCORSConfigurationRequest();
			deleteCORSConfigurationRequest.BucketName = bucketName;
			DeleteCORSConfigurationAsync(deleteCORSConfigurationRequest, callback, options);
		}

		public void DeleteCORSConfigurationAsync(DeleteCORSConfigurationRequest request, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteCORSConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteCORSConfigurationRequestMarshaller deleteCORSConfigurationRequestMarshaller = new DeleteCORSConfigurationRequestMarshaller();
			DeleteCORSConfigurationResponseUnmarshaller instance = DeleteCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> val = new AmazonServiceResult<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse>((DeleteCORSConfigurationRequest)req, (DeleteCORSConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteCORSConfigurationRequest>(request, deleteCORSConfigurationRequestMarshaller, instance, options, action);
		}

		public void DeleteLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			DeleteLifecycleConfigurationRequest deleteLifecycleConfigurationRequest = new DeleteLifecycleConfigurationRequest();
			deleteLifecycleConfigurationRequest.BucketName = bucketName;
			DeleteLifecycleConfigurationAsync(deleteLifecycleConfigurationRequest, callback, options);
		}

		public void DeleteLifecycleConfigurationAsync(DeleteLifecycleConfigurationRequest request, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteLifecycleConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteLifecycleConfigurationRequestMarshaller deleteLifecycleConfigurationRequestMarshaller = new DeleteLifecycleConfigurationRequestMarshaller();
			DeleteLifecycleConfigurationResponseUnmarshaller instance = DeleteLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> val = new AmazonServiceResult<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse>((DeleteLifecycleConfigurationRequest)req, (DeleteLifecycleConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteLifecycleConfigurationRequest>(request, deleteLifecycleConfigurationRequestMarshaller, instance, options, action);
		}

		public void DeleteObjectAsync(string bucketName, string key, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest();
			deleteObjectRequest.BucketName = bucketName;
			deleteObjectRequest.Key = key;
			DeleteObjectAsync(deleteObjectRequest, callback, options);
		}

		public void DeleteObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest();
			deleteObjectRequest.BucketName = bucketName;
			deleteObjectRequest.Key = key;
			deleteObjectRequest.VersionId = versionId;
			DeleteObjectAsync(deleteObjectRequest, callback, options);
		}

		public void DeleteObjectAsync(DeleteObjectRequest request, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteObjectRequestMarshaller deleteObjectRequestMarshaller = new DeleteObjectRequestMarshaller();
			DeleteObjectResponseUnmarshaller instance = DeleteObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectRequest, DeleteObjectResponse> val = new AmazonServiceResult<DeleteObjectRequest, DeleteObjectResponse>((DeleteObjectRequest)req, (DeleteObjectResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteObjectRequest>(request, deleteObjectRequestMarshaller, instance, options, action);
		}

		public void DeleteObjectsAsync(DeleteObjectsRequest request, AmazonServiceCallback<DeleteObjectsRequest, DeleteObjectsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteObjectsRequestMarshaller deleteObjectsRequestMarshaller = new DeleteObjectsRequestMarshaller();
			DeleteObjectsResponseUnmarshaller instance = DeleteObjectsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectsRequest, DeleteObjectsResponse> val = new AmazonServiceResult<DeleteObjectsRequest, DeleteObjectsResponse>((DeleteObjectsRequest)req, (DeleteObjectsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteObjectsRequest>(request, deleteObjectsRequestMarshaller, instance, options, action);
		}

		public void DeleteObjectTaggingAsync(DeleteObjectTaggingRequest request, AmazonServiceCallback<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("DeleteObjectTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			DeleteObjectTaggingRequestMarshaller deleteObjectTaggingRequestMarshaller = new DeleteObjectTaggingRequestMarshaller();
			DeleteObjectTaggingResponseUnmarshaller instance = DeleteObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse> val = new AmazonServiceResult<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse>((DeleteObjectTaggingRequest)req, (DeleteObjectTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<DeleteObjectTaggingRequest>(request, deleteObjectTaggingRequestMarshaller, instance, options, action);
		}

		public void GetACLAsync(string bucketName, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null)
		{
			GetACLRequest getACLRequest = new GetACLRequest();
			getACLRequest.BucketName = bucketName;
			GetACLAsync(getACLRequest, callback, options);
		}

		public void GetACLAsync(GetACLRequest request, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetACLRequestMarshaller getACLRequestMarshaller = new GetACLRequestMarshaller();
			GetACLResponseUnmarshaller instance = GetACLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetACLRequest, GetACLResponse> val = new AmazonServiceResult<GetACLRequest, GetACLResponse>((GetACLRequest)req, (GetACLResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetACLRequest>(request, getACLRequestMarshaller, instance, options, action);
		}

		public void GetBucketAccelerateConfigurationAsync(string bucketName, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetBucketAccelerateConfigurationRequest getBucketAccelerateConfigurationRequest = new GetBucketAccelerateConfigurationRequest();
			getBucketAccelerateConfigurationRequest.BucketName = bucketName;
			GetBucketAccelerateConfigurationAsync(getBucketAccelerateConfigurationRequest, callback, options);
		}

		public void GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketAccelerateConfigurationRequestMarshaller getBucketAccelerateConfigurationRequestMarshaller = new GetBucketAccelerateConfigurationRequestMarshaller();
			GetBucketAccelerateConfigurationResponseUnmarshaller instance = GetBucketAccelerateConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> val = new AmazonServiceResult<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse>((GetBucketAccelerateConfigurationRequest)req, (GetBucketAccelerateConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketAccelerateConfigurationRequest>(request, getBucketAccelerateConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetBucketAnalyticsConfigurationAsync(GetBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketAnalyticsConfigurationRequestMarshaller getBucketAnalyticsConfigurationRequestMarshaller = new GetBucketAnalyticsConfigurationRequestMarshaller();
			GetBucketAnalyticsConfigurationResponseUnmarshaller instance = GetBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse> val = new AmazonServiceResult<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse>((GetBucketAnalyticsConfigurationRequest)req, (GetBucketAnalyticsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketAnalyticsConfigurationRequest>(request, getBucketAnalyticsConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, AmazonServiceCallback<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketInventoryConfigurationRequestMarshaller getBucketInventoryConfigurationRequestMarshaller = new GetBucketInventoryConfigurationRequestMarshaller();
			GetBucketInventoryConfigurationResponseUnmarshaller instance = GetBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse> val = new AmazonServiceResult<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse>((GetBucketInventoryConfigurationRequest)req, (GetBucketInventoryConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketInventoryConfigurationRequest>(request, getBucketInventoryConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetBucketLocationAsync(string bucketName, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null)
		{
			GetBucketLocationRequest getBucketLocationRequest = new GetBucketLocationRequest();
			getBucketLocationRequest.BucketName = bucketName;
			GetBucketLocationAsync(getBucketLocationRequest, callback, options);
		}

		public void GetBucketLocationAsync(GetBucketLocationRequest request, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketLocationRequestMarshaller getBucketLocationRequestMarshaller = new GetBucketLocationRequestMarshaller();
			GetBucketLocationResponseUnmarshaller instance = GetBucketLocationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketLocationRequest, GetBucketLocationResponse> val = new AmazonServiceResult<GetBucketLocationRequest, GetBucketLocationResponse>((GetBucketLocationRequest)req, (GetBucketLocationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketLocationRequest>(request, getBucketLocationRequestMarshaller, instance, options, action);
		}

		public void GetBucketLoggingAsync(string bucketName, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			GetBucketLoggingRequest getBucketLoggingRequest = new GetBucketLoggingRequest();
			getBucketLoggingRequest.BucketName = bucketName;
			GetBucketLoggingAsync(getBucketLoggingRequest, callback, options);
		}

		public void GetBucketLoggingAsync(GetBucketLoggingRequest request, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketLoggingRequestMarshaller getBucketLoggingRequestMarshaller = new GetBucketLoggingRequestMarshaller();
			GetBucketLoggingResponseUnmarshaller instance = GetBucketLoggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketLoggingRequest, GetBucketLoggingResponse> val = new AmazonServiceResult<GetBucketLoggingRequest, GetBucketLoggingResponse>((GetBucketLoggingRequest)req, (GetBucketLoggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketLoggingRequest>(request, getBucketLoggingRequestMarshaller, instance, options, action);
		}

		public void GetBucketMetricsConfigurationAsync(GetBucketMetricsConfigurationRequest request, AmazonServiceCallback<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketMetricsConfigurationRequestMarshaller getBucketMetricsConfigurationRequestMarshaller = new GetBucketMetricsConfigurationRequestMarshaller();
			GetBucketMetricsConfigurationResponseUnmarshaller instance = GetBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse> val = new AmazonServiceResult<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse>((GetBucketMetricsConfigurationRequest)req, (GetBucketMetricsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketMetricsConfigurationRequest>(request, getBucketMetricsConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetBucketNotificationAsync(string bucketName, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			GetBucketNotificationRequest getBucketNotificationRequest = new GetBucketNotificationRequest();
			getBucketNotificationRequest.BucketName = bucketName;
			GetBucketNotificationAsync(getBucketNotificationRequest, callback, options);
		}

		public void GetBucketNotificationAsync(GetBucketNotificationRequest request, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketNotificationRequestMarshaller getBucketNotificationRequestMarshaller = new GetBucketNotificationRequestMarshaller();
			GetBucketNotificationResponseUnmarshaller instance = GetBucketNotificationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketNotificationRequest, GetBucketNotificationResponse> val = new AmazonServiceResult<GetBucketNotificationRequest, GetBucketNotificationResponse>((GetBucketNotificationRequest)req, (GetBucketNotificationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketNotificationRequest>(request, getBucketNotificationRequestMarshaller, instance, options, action);
		}

		public void GetBucketPolicyAsync(string bucketName, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			GetBucketPolicyRequest getBucketPolicyRequest = new GetBucketPolicyRequest();
			getBucketPolicyRequest.BucketName = bucketName;
			GetBucketPolicyAsync(getBucketPolicyRequest, callback, options);
		}

		public void GetBucketPolicyAsync(GetBucketPolicyRequest request, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketPolicyRequestMarshaller getBucketPolicyRequestMarshaller = new GetBucketPolicyRequestMarshaller();
			GetBucketPolicyResponseUnmarshaller instance = GetBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketPolicyRequest, GetBucketPolicyResponse> val = new AmazonServiceResult<GetBucketPolicyRequest, GetBucketPolicyResponse>((GetBucketPolicyRequest)req, (GetBucketPolicyResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketPolicyRequest>(request, getBucketPolicyRequestMarshaller, instance, options, action);
		}

		public void GetBucketReplicationAsync(GetBucketReplicationRequest request, AmazonServiceCallback<GetBucketReplicationRequest, GetBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("GetBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketReplicationRequestMarshaller getBucketReplicationRequestMarshaller = new GetBucketReplicationRequestMarshaller();
			GetBucketReplicationResponseUnmarshaller instance = GetBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketReplicationRequest, GetBucketReplicationResponse> val = new AmazonServiceResult<GetBucketReplicationRequest, GetBucketReplicationResponse>((GetBucketReplicationRequest)req, (GetBucketReplicationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketReplicationRequest>(request, getBucketReplicationRequestMarshaller, instance, options, action);
		}

		public void GetBucketRequestPaymentAsync(string bucketName, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			GetBucketRequestPaymentRequest getBucketRequestPaymentRequest = new GetBucketRequestPaymentRequest();
			getBucketRequestPaymentRequest.BucketName = bucketName;
			GetBucketRequestPaymentAsync(getBucketRequestPaymentRequest, callback, options);
		}

		public void GetBucketRequestPaymentAsync(GetBucketRequestPaymentRequest request, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketRequestPaymentRequestMarshaller getBucketRequestPaymentRequestMarshaller = new GetBucketRequestPaymentRequestMarshaller();
			GetBucketRequestPaymentResponseUnmarshaller instance = GetBucketRequestPaymentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> val = new AmazonServiceResult<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse>((GetBucketRequestPaymentRequest)req, (GetBucketRequestPaymentResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketRequestPaymentRequest>(request, getBucketRequestPaymentRequestMarshaller, instance, options, action);
		}

		public void GetBucketTaggingAsync(GetBucketTaggingRequest request, AmazonServiceCallback<GetBucketTaggingRequest, GetBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketTaggingRequestMarshaller getBucketTaggingRequestMarshaller = new GetBucketTaggingRequestMarshaller();
			GetBucketTaggingResponseUnmarshaller instance = GetBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketTaggingRequest, GetBucketTaggingResponse> val = new AmazonServiceResult<GetBucketTaggingRequest, GetBucketTaggingResponse>((GetBucketTaggingRequest)req, (GetBucketTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketTaggingRequest>(request, getBucketTaggingRequestMarshaller, instance, options, action);
		}

		public void GetBucketVersioningAsync(string bucketName, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			GetBucketVersioningRequest getBucketVersioningRequest = new GetBucketVersioningRequest();
			getBucketVersioningRequest.BucketName = bucketName;
			GetBucketVersioningAsync(getBucketVersioningRequest, callback, options);
		}

		public void GetBucketVersioningAsync(GetBucketVersioningRequest request, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketVersioningRequestMarshaller getBucketVersioningRequestMarshaller = new GetBucketVersioningRequestMarshaller();
			GetBucketVersioningResponseUnmarshaller instance = GetBucketVersioningResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketVersioningRequest, GetBucketVersioningResponse> val = new AmazonServiceResult<GetBucketVersioningRequest, GetBucketVersioningResponse>((GetBucketVersioningRequest)req, (GetBucketVersioningResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketVersioningRequest>(request, getBucketVersioningRequestMarshaller, instance, options, action);
		}

		public void GetBucketWebsiteAsync(string bucketName, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			GetBucketWebsiteRequest getBucketWebsiteRequest = new GetBucketWebsiteRequest();
			getBucketWebsiteRequest.BucketName = bucketName;
			GetBucketWebsiteAsync(getBucketWebsiteRequest, callback, options);
		}

		public void GetBucketWebsiteAsync(GetBucketWebsiteRequest request, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetBucketWebsiteRequestMarshaller getBucketWebsiteRequestMarshaller = new GetBucketWebsiteRequestMarshaller();
			GetBucketWebsiteResponseUnmarshaller instance = GetBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketWebsiteRequest, GetBucketWebsiteResponse> val = new AmazonServiceResult<GetBucketWebsiteRequest, GetBucketWebsiteResponse>((GetBucketWebsiteRequest)req, (GetBucketWebsiteResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetBucketWebsiteRequest>(request, getBucketWebsiteRequestMarshaller, instance, options, action);
		}

		public void GetCORSConfigurationAsync(string bucketName, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetCORSConfigurationRequest getCORSConfigurationRequest = new GetCORSConfigurationRequest();
			getCORSConfigurationRequest.BucketName = bucketName;
			GetCORSConfigurationAsync(getCORSConfigurationRequest, callback, options);
		}

		public void GetCORSConfigurationAsync(GetCORSConfigurationRequest request, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetCORSConfigurationRequestMarshaller getCORSConfigurationRequestMarshaller = new GetCORSConfigurationRequestMarshaller();
			GetCORSConfigurationResponseUnmarshaller instance = GetCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCORSConfigurationRequest, GetCORSConfigurationResponse> val = new AmazonServiceResult<GetCORSConfigurationRequest, GetCORSConfigurationResponse>((GetCORSConfigurationRequest)req, (GetCORSConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetCORSConfigurationRequest>(request, getCORSConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetLifecycleConfigurationRequest getLifecycleConfigurationRequest = new GetLifecycleConfigurationRequest();
			getLifecycleConfigurationRequest.BucketName = bucketName;
			GetLifecycleConfigurationAsync(getLifecycleConfigurationRequest, callback, options);
		}

		public void GetLifecycleConfigurationAsync(GetLifecycleConfigurationRequest request, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetLifecycleConfigurationRequestMarshaller getLifecycleConfigurationRequestMarshaller = new GetLifecycleConfigurationRequestMarshaller();
			GetLifecycleConfigurationResponseUnmarshaller instance = GetLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> val = new AmazonServiceResult<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse>((GetLifecycleConfigurationRequest)req, (GetLifecycleConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetLifecycleConfigurationRequest>(request, getLifecycleConfigurationRequestMarshaller, instance, options, action);
		}

		public void GetObjectAsync(string bucketName, string key, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			GetObjectRequest getObjectRequest = new GetObjectRequest();
			getObjectRequest.BucketName = bucketName;
			getObjectRequest.Key = key;
			GetObjectAsync(getObjectRequest, callback, options);
		}

		public void GetObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			GetObjectRequest getObjectRequest = new GetObjectRequest();
			getObjectRequest.BucketName = bucketName;
			getObjectRequest.Key = key;
			getObjectRequest.VersionId = versionId;
			GetObjectAsync(getObjectRequest, callback, options);
		}

		public void GetObjectAsync(GetObjectRequest request, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetObjectRequestMarshaller getObjectRequestMarshaller = new GetObjectRequestMarshaller();
			GetObjectResponseUnmarshaller instance = GetObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectRequest, GetObjectResponse> val = new AmazonServiceResult<GetObjectRequest, GetObjectResponse>((GetObjectRequest)req, (GetObjectResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetObjectRequest>(request, getObjectRequestMarshaller, instance, options, action);
		}

		public void GetObjectMetadataAsync(string bucketName, string key, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			GetObjectMetadataRequest getObjectMetadataRequest = new GetObjectMetadataRequest();
			getObjectMetadataRequest.BucketName = bucketName;
			getObjectMetadataRequest.Key = key;
			GetObjectMetadataAsync(getObjectMetadataRequest, callback, options);
		}

		public void GetObjectMetadataAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			GetObjectMetadataRequest getObjectMetadataRequest = new GetObjectMetadataRequest();
			getObjectMetadataRequest.BucketName = bucketName;
			getObjectMetadataRequest.Key = key;
			getObjectMetadataRequest.VersionId = versionId;
			GetObjectMetadataAsync(getObjectMetadataRequest, callback, options);
		}

		public void GetObjectMetadataAsync(GetObjectMetadataRequest request, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("GetObjectMetadata is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetObjectMetadataRequestMarshaller getObjectMetadataRequestMarshaller = new GetObjectMetadataRequestMarshaller();
			GetObjectMetadataResponseUnmarshaller instance = GetObjectMetadataResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectMetadataRequest, GetObjectMetadataResponse> val = new AmazonServiceResult<GetObjectMetadataRequest, GetObjectMetadataResponse>((GetObjectMetadataRequest)req, (GetObjectMetadataResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetObjectMetadataRequest>(request, getObjectMetadataRequestMarshaller, instance, options, action);
		}

		public void GetObjectTaggingAsync(GetObjectTaggingRequest request, AmazonServiceCallback<GetObjectTaggingRequest, GetObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetObjectTaggingRequestMarshaller getObjectTaggingRequestMarshaller = new GetObjectTaggingRequestMarshaller();
			GetObjectTaggingResponseUnmarshaller instance = GetObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectTaggingRequest, GetObjectTaggingResponse> val = new AmazonServiceResult<GetObjectTaggingRequest, GetObjectTaggingResponse>((GetObjectTaggingRequest)req, (GetObjectTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetObjectTaggingRequest>(request, getObjectTaggingRequestMarshaller, instance, options, action);
		}

		public void GetObjectTorrentAsync(string bucketName, string key, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null)
		{
			GetObjectTorrentRequest getObjectTorrentRequest = new GetObjectTorrentRequest();
			getObjectTorrentRequest.BucketName = bucketName;
			getObjectTorrentRequest.Key = key;
			GetObjectTorrentAsync(getObjectTorrentRequest, callback, options);
		}

		public void GetObjectTorrentAsync(GetObjectTorrentRequest request, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			GetObjectTorrentRequestMarshaller getObjectTorrentRequestMarshaller = new GetObjectTorrentRequestMarshaller();
			GetObjectTorrentResponseUnmarshaller instance = GetObjectTorrentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectTorrentRequest, GetObjectTorrentResponse> val = new AmazonServiceResult<GetObjectTorrentRequest, GetObjectTorrentResponse>((GetObjectTorrentRequest)req, (GetObjectTorrentResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<GetObjectTorrentRequest>(request, getObjectTorrentRequestMarshaller, instance, options, action);
		}

		internal void HeadBucketAsync(HeadBucketRequest request, AmazonServiceCallback<HeadBucketRequest, HeadBucketResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("HeadBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			HeadBucketRequestMarshaller headBucketRequestMarshaller = new HeadBucketRequestMarshaller();
			HeadBucketResponseUnmarshaller instance = HeadBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<HeadBucketRequest, HeadBucketResponse> val = new AmazonServiceResult<HeadBucketRequest, HeadBucketResponse>((HeadBucketRequest)req, (HeadBucketResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<HeadBucketRequest>(request, headBucketRequestMarshaller, instance, options, action);
		}

		public void InitiateMultipartUploadAsync(string bucketName, string key, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			InitiateMultipartUploadRequest initiateMultipartUploadRequest = new InitiateMultipartUploadRequest();
			initiateMultipartUploadRequest.BucketName = bucketName;
			initiateMultipartUploadRequest.Key = key;
			InitiateMultipartUploadAsync(initiateMultipartUploadRequest, callback, options);
		}

		public void InitiateMultipartUploadAsync(InitiateMultipartUploadRequest request, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("InitiateMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			InitiateMultipartUploadRequestMarshaller initiateMultipartUploadRequestMarshaller = new InitiateMultipartUploadRequestMarshaller();
			InitiateMultipartUploadResponseUnmarshaller instance = InitiateMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> val = new AmazonServiceResult<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse>((InitiateMultipartUploadRequest)req, (InitiateMultipartUploadResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<InitiateMultipartUploadRequest>(request, initiateMultipartUploadRequestMarshaller, instance, options, action);
		}

		public void ListBucketAnalyticsConfigurationsAsync(ListBucketAnalyticsConfigurationsRequest request, AmazonServiceCallback<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListBucketAnalyticsConfigurationsRequestMarshaller listBucketAnalyticsConfigurationsRequestMarshaller = new ListBucketAnalyticsConfigurationsRequestMarshaller();
			ListBucketAnalyticsConfigurationsResponseUnmarshaller instance = ListBucketAnalyticsConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse> val = new AmazonServiceResult<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse>((ListBucketAnalyticsConfigurationsRequest)req, (ListBucketAnalyticsConfigurationsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListBucketAnalyticsConfigurationsRequest>(request, listBucketAnalyticsConfigurationsRequestMarshaller, instance, options, action);
		}

		public void ListBucketInventoryConfigurationsAsync(ListBucketInventoryConfigurationsRequest request, AmazonServiceCallback<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListBucketInventoryConfigurationsRequestMarshaller listBucketInventoryConfigurationsRequestMarshaller = new ListBucketInventoryConfigurationsRequestMarshaller();
			ListBucketInventoryConfigurationsResponseUnmarshaller instance = ListBucketInventoryConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse> val = new AmazonServiceResult<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse>((ListBucketInventoryConfigurationsRequest)req, (ListBucketInventoryConfigurationsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListBucketInventoryConfigurationsRequest>(request, listBucketInventoryConfigurationsRequestMarshaller, instance, options, action);
		}

		public void ListBucketMetricsConfigurationsAsync(ListBucketMetricsConfigurationsRequest request, AmazonServiceCallback<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListBucketMetricsConfigurationsRequestMarshaller listBucketMetricsConfigurationsRequestMarshaller = new ListBucketMetricsConfigurationsRequestMarshaller();
			ListBucketMetricsConfigurationsResponseUnmarshaller instance = ListBucketMetricsConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse> val = new AmazonServiceResult<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse>((ListBucketMetricsConfigurationsRequest)req, (ListBucketMetricsConfigurationsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListBucketMetricsConfigurationsRequest>(request, listBucketMetricsConfigurationsRequestMarshaller, instance, options, action);
		}

		public void ListBucketsAsync(AmazonServiceCallback<ListBucketsRequest, ListBucketsResponse> callback, AsyncOptions options = null)
		{
			ListBucketsAsync(new ListBucketsRequest(), callback, options);
		}

		public void ListBucketsAsync(ListBucketsRequest request, AmazonServiceCallback<ListBucketsRequest, ListBucketsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListBucketsRequestMarshaller listBucketsRequestMarshaller = new ListBucketsRequestMarshaller();
			ListBucketsResponseUnmarshaller instance = ListBucketsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketsRequest, ListBucketsResponse> val = new AmazonServiceResult<ListBucketsRequest, ListBucketsResponse>((ListBucketsRequest)req, (ListBucketsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListBucketsRequest>(request, listBucketsRequestMarshaller, instance, options, action);
		}

		public void ListMultipartUploadsAsync(string bucketName, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			ListMultipartUploadsRequest listMultipartUploadsRequest = new ListMultipartUploadsRequest();
			listMultipartUploadsRequest.BucketName = bucketName;
			ListMultipartUploadsAsync(listMultipartUploadsRequest, callback, options);
		}

		public void ListMultipartUploadsAsync(string bucketName, string prefix, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			ListMultipartUploadsRequest listMultipartUploadsRequest = new ListMultipartUploadsRequest();
			listMultipartUploadsRequest.BucketName = bucketName;
			listMultipartUploadsRequest.Prefix = prefix;
			ListMultipartUploadsAsync(listMultipartUploadsRequest, callback, options);
		}

		public void ListMultipartUploadsAsync(ListMultipartUploadsRequest request, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("ListMultipartUploads is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListMultipartUploadsRequestMarshaller listMultipartUploadsRequestMarshaller = new ListMultipartUploadsRequestMarshaller();
			ListMultipartUploadsResponseUnmarshaller instance = ListMultipartUploadsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListMultipartUploadsRequest, ListMultipartUploadsResponse> val = new AmazonServiceResult<ListMultipartUploadsRequest, ListMultipartUploadsResponse>((ListMultipartUploadsRequest)req, (ListMultipartUploadsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListMultipartUploadsRequest>(request, listMultipartUploadsRequestMarshaller, instance, options, action);
		}

		public void ListObjectsAsync(string bucketName, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
			listObjectsRequest.BucketName = bucketName;
			ListObjectsAsync(listObjectsRequest, callback, options);
		}

		public void ListObjectsAsync(string bucketName, string prefix, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
			listObjectsRequest.BucketName = bucketName;
			listObjectsRequest.Prefix = prefix;
			ListObjectsAsync(listObjectsRequest, callback, options);
		}

		public void ListObjectsAsync(ListObjectsRequest request, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListObjectsRequestMarshaller listObjectsRequestMarshaller = new ListObjectsRequestMarshaller();
			ListObjectsResponseUnmarshaller instance = ListObjectsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> val = new AmazonServiceResult<ListObjectsRequest, ListObjectsResponse>((ListObjectsRequest)req, (ListObjectsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListObjectsRequest>(request, listObjectsRequestMarshaller, instance, options, action);
		}

		public void ListObjectsV2Async(ListObjectsV2Request request, AmazonServiceCallback<ListObjectsV2Request, ListObjectsV2Response> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListObjectsV2RequestMarshaller listObjectsV2RequestMarshaller = new ListObjectsV2RequestMarshaller();
			ListObjectsV2ResponseUnmarshaller instance = ListObjectsV2ResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListObjectsV2Request, ListObjectsV2Response> val = new AmazonServiceResult<ListObjectsV2Request, ListObjectsV2Response>((ListObjectsV2Request)req, (ListObjectsV2Response)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListObjectsV2Request>(request, listObjectsV2RequestMarshaller, instance, options, action);
		}

		public void ListPartsAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null)
		{
			ListPartsRequest listPartsRequest = new ListPartsRequest();
			listPartsRequest.BucketName = bucketName;
			listPartsRequest.Key = key;
			listPartsRequest.UploadId = uploadId;
			ListPartsAsync(listPartsRequest, callback, options);
		}

		public void ListPartsAsync(ListPartsRequest request, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("ListParts is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListPartsRequestMarshaller listPartsRequestMarshaller = new ListPartsRequestMarshaller();
			ListPartsResponseUnmarshaller instance = ListPartsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListPartsRequest, ListPartsResponse> val = new AmazonServiceResult<ListPartsRequest, ListPartsResponse>((ListPartsRequest)req, (ListPartsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListPartsRequest>(request, listPartsRequestMarshaller, instance, options, action);
		}

		public void ListVersionsAsync(string bucketName, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			ListVersionsRequest listVersionsRequest = new ListVersionsRequest();
			listVersionsRequest.BucketName = bucketName;
			ListVersionsAsync(listVersionsRequest, callback, options);
		}

		public void ListVersionsAsync(string bucketName, string prefix, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			ListVersionsRequest listVersionsRequest = new ListVersionsRequest();
			listVersionsRequest.BucketName = bucketName;
			listVersionsRequest.Prefix = prefix;
			ListVersionsAsync(listVersionsRequest, callback, options);
		}

		public void ListVersionsAsync(ListVersionsRequest request, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			ListVersionsRequestMarshaller listVersionsRequestMarshaller = new ListVersionsRequestMarshaller();
			ListVersionsResponseUnmarshaller instance = ListVersionsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListVersionsRequest, ListVersionsResponse> val = new AmazonServiceResult<ListVersionsRequest, ListVersionsResponse>((ListVersionsRequest)req, (ListVersionsResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<ListVersionsRequest>(request, listVersionsRequestMarshaller, instance, options, action);
		}

		public void PutACLAsync(PutACLRequest request, AmazonServiceCallback<PutACLRequest, PutACLResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutACL is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutACLRequestMarshaller putACLRequestMarshaller = new PutACLRequestMarshaller();
			PutACLResponseUnmarshaller instance = PutACLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutACLRequest, PutACLResponse> val = new AmazonServiceResult<PutACLRequest, PutACLResponse>((PutACLRequest)req, (PutACLResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutACLRequest>(request, putACLRequestMarshaller, instance, options, action);
		}

		public void PutBucketAsync(string bucketName, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null)
		{
			PutBucketRequest putBucketRequest = new PutBucketRequest();
			putBucketRequest.BucketName = bucketName;
			PutBucketAsync(putBucketRequest, callback, options);
		}

		public void PutBucketAsync(PutBucketRequest request, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketRequestMarshaller putBucketRequestMarshaller = new PutBucketRequestMarshaller();
			PutBucketResponseUnmarshaller instance = PutBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketRequest, PutBucketResponse> val = new AmazonServiceResult<PutBucketRequest, PutBucketResponse>((PutBucketRequest)req, (PutBucketResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketRequest>(request, putBucketRequestMarshaller, instance, options, action);
		}

		public void PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, AmazonServiceCallback<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketAccelerateConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketAccelerateConfigurationRequestMarshaller putBucketAccelerateConfigurationRequestMarshaller = new PutBucketAccelerateConfigurationRequestMarshaller();
			PutBucketAccelerateConfigurationResponseUnmarshaller instance = PutBucketAccelerateConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse> val = new AmazonServiceResult<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse>((PutBucketAccelerateConfigurationRequest)req, (PutBucketAccelerateConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketAccelerateConfigurationRequest>(request, putBucketAccelerateConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutBucketAnalyticsConfigurationAsync(PutBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketAnalyticsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketAnalyticsConfigurationRequestMarshaller putBucketAnalyticsConfigurationRequestMarshaller = new PutBucketAnalyticsConfigurationRequestMarshaller();
			PutBucketAnalyticsConfigurationResponseUnmarshaller instance = PutBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse> val = new AmazonServiceResult<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse>((PutBucketAnalyticsConfigurationRequest)req, (PutBucketAnalyticsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketAnalyticsConfigurationRequest>(request, putBucketAnalyticsConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutBucketInventoryConfigurationAsync(PutBucketInventoryConfigurationRequest request, AmazonServiceCallback<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketInventoryConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketInventoryConfigurationRequestMarshaller putBucketInventoryConfigurationRequestMarshaller = new PutBucketInventoryConfigurationRequestMarshaller();
			PutBucketInventoryConfigurationResponseUnmarshaller instance = PutBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse> val = new AmazonServiceResult<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse>((PutBucketInventoryConfigurationRequest)req, (PutBucketInventoryConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketInventoryConfigurationRequest>(request, putBucketInventoryConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutBucketLoggingAsync(PutBucketLoggingRequest request, AmazonServiceCallback<PutBucketLoggingRequest, PutBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketLogging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketLoggingRequestMarshaller putBucketLoggingRequestMarshaller = new PutBucketLoggingRequestMarshaller();
			PutBucketLoggingResponseUnmarshaller instance = PutBucketLoggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketLoggingRequest, PutBucketLoggingResponse> val = new AmazonServiceResult<PutBucketLoggingRequest, PutBucketLoggingResponse>((PutBucketLoggingRequest)req, (PutBucketLoggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketLoggingRequest>(request, putBucketLoggingRequestMarshaller, instance, options, action);
		}

		public void PutBucketMetricsConfigurationAsync(PutBucketMetricsConfigurationRequest request, AmazonServiceCallback<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketMetricsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketMetricsConfigurationRequestMarshaller putBucketMetricsConfigurationRequestMarshaller = new PutBucketMetricsConfigurationRequestMarshaller();
			PutBucketMetricsConfigurationResponseUnmarshaller instance = PutBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse> val = new AmazonServiceResult<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse>((PutBucketMetricsConfigurationRequest)req, (PutBucketMetricsConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketMetricsConfigurationRequest>(request, putBucketMetricsConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutBucketNotificationAsync(PutBucketNotificationRequest request, AmazonServiceCallback<PutBucketNotificationRequest, PutBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketNotification is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketNotificationRequestMarshaller putBucketNotificationRequestMarshaller = new PutBucketNotificationRequestMarshaller();
			PutBucketNotificationResponseUnmarshaller instance = PutBucketNotificationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketNotificationRequest, PutBucketNotificationResponse> val = new AmazonServiceResult<PutBucketNotificationRequest, PutBucketNotificationResponse>((PutBucketNotificationRequest)req, (PutBucketNotificationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketNotificationRequest>(request, putBucketNotificationRequestMarshaller, instance, options, action);
		}

		public void PutBucketPolicyAsync(string bucketName, string policy, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			PutBucketPolicyRequest putBucketPolicyRequest = new PutBucketPolicyRequest();
			putBucketPolicyRequest.BucketName = bucketName;
			putBucketPolicyRequest.Policy = policy;
			PutBucketPolicyAsync(putBucketPolicyRequest, callback, options);
		}

		public void PutBucketPolicyAsync(string bucketName, string policy, string contentMD5, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			PutBucketPolicyRequest putBucketPolicyRequest = new PutBucketPolicyRequest();
			putBucketPolicyRequest.BucketName = bucketName;
			putBucketPolicyRequest.Policy = policy;
			putBucketPolicyRequest.ContentMD5 = contentMD5;
			PutBucketPolicyAsync(putBucketPolicyRequest, callback, options);
		}

		public void PutBucketPolicyAsync(PutBucketPolicyRequest request, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketPolicy is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketPolicyRequestMarshaller putBucketPolicyRequestMarshaller = new PutBucketPolicyRequestMarshaller();
			PutBucketPolicyResponseUnmarshaller instance = PutBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketPolicyRequest, PutBucketPolicyResponse> val = new AmazonServiceResult<PutBucketPolicyRequest, PutBucketPolicyResponse>((PutBucketPolicyRequest)req, (PutBucketPolicyResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketPolicyRequest>(request, putBucketPolicyRequestMarshaller, instance, options, action);
		}

		public void PutBucketReplicationAsync(PutBucketReplicationRequest request, AmazonServiceCallback<PutBucketReplicationRequest, PutBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketReplicationRequestMarshaller putBucketReplicationRequestMarshaller = new PutBucketReplicationRequestMarshaller();
			PutBucketReplicationResponseUnmarshaller instance = PutBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketReplicationRequest, PutBucketReplicationResponse> val = new AmazonServiceResult<PutBucketReplicationRequest, PutBucketReplicationResponse>((PutBucketReplicationRequest)req, (PutBucketReplicationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketReplicationRequest>(request, putBucketReplicationRequestMarshaller, instance, options, action);
		}

		public void PutBucketRequestPaymentAsync(string bucketName, RequestPaymentConfiguration requestPaymentConfiguration, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			PutBucketRequestPaymentRequest putBucketRequestPaymentRequest = new PutBucketRequestPaymentRequest();
			putBucketRequestPaymentRequest.BucketName = bucketName;
			putBucketRequestPaymentRequest.RequestPaymentConfiguration = requestPaymentConfiguration;
			PutBucketRequestPaymentAsync(putBucketRequestPaymentRequest, callback, options);
		}

		public void PutBucketRequestPaymentAsync(PutBucketRequestPaymentRequest request, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketRequestPayment is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketRequestPaymentRequestMarshaller putBucketRequestPaymentRequestMarshaller = new PutBucketRequestPaymentRequestMarshaller();
			PutBucketRequestPaymentResponseUnmarshaller instance = PutBucketRequestPaymentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> val = new AmazonServiceResult<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse>((PutBucketRequestPaymentRequest)req, (PutBucketRequestPaymentResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketRequestPaymentRequest>(request, putBucketRequestPaymentRequestMarshaller, instance, options, action);
		}

		public void PutBucketTaggingAsync(string bucketName, List<Tag> tagSet, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			PutBucketTaggingRequest putBucketTaggingRequest = new PutBucketTaggingRequest();
			putBucketTaggingRequest.BucketName = bucketName;
			putBucketTaggingRequest.TagSet = tagSet;
			PutBucketTaggingAsync(putBucketTaggingRequest, callback, options);
		}

		public void PutBucketTaggingAsync(PutBucketTaggingRequest request, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketTaggingRequestMarshaller putBucketTaggingRequestMarshaller = new PutBucketTaggingRequestMarshaller();
			PutBucketTaggingResponseUnmarshaller instance = PutBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketTaggingRequest, PutBucketTaggingResponse> val = new AmazonServiceResult<PutBucketTaggingRequest, PutBucketTaggingResponse>((PutBucketTaggingRequest)req, (PutBucketTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketTaggingRequest>(request, putBucketTaggingRequestMarshaller, instance, options, action);
		}

		public void PutBucketVersioningAsync(PutBucketVersioningRequest request, AmazonServiceCallback<PutBucketVersioningRequest, PutBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketVersioning is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketVersioningRequestMarshaller putBucketVersioningRequestMarshaller = new PutBucketVersioningRequestMarshaller();
			PutBucketVersioningResponseUnmarshaller instance = PutBucketVersioningResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketVersioningRequest, PutBucketVersioningResponse> val = new AmazonServiceResult<PutBucketVersioningRequest, PutBucketVersioningResponse>((PutBucketVersioningRequest)req, (PutBucketVersioningResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketVersioningRequest>(request, putBucketVersioningRequestMarshaller, instance, options, action);
		}

		public void PutBucketWebsiteAsync(string bucketName, WebsiteConfiguration websiteConfiguration, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			PutBucketWebsiteRequest putBucketWebsiteRequest = new PutBucketWebsiteRequest();
			putBucketWebsiteRequest.BucketName = bucketName;
			putBucketWebsiteRequest.WebsiteConfiguration = websiteConfiguration;
			PutBucketWebsiteAsync(putBucketWebsiteRequest, callback, options);
		}

		public void PutBucketWebsiteAsync(PutBucketWebsiteRequest request, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutBucketWebsite is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutBucketWebsiteRequestMarshaller putBucketWebsiteRequestMarshaller = new PutBucketWebsiteRequestMarshaller();
			PutBucketWebsiteResponseUnmarshaller instance = PutBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketWebsiteRequest, PutBucketWebsiteResponse> val = new AmazonServiceResult<PutBucketWebsiteRequest, PutBucketWebsiteResponse>((PutBucketWebsiteRequest)req, (PutBucketWebsiteResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutBucketWebsiteRequest>(request, putBucketWebsiteRequestMarshaller, instance, options, action);
		}

		public void PutCORSConfigurationAsync(string bucketName, CORSConfiguration configuration, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			PutCORSConfigurationRequest putCORSConfigurationRequest = new PutCORSConfigurationRequest();
			putCORSConfigurationRequest.BucketName = bucketName;
			putCORSConfigurationRequest.Configuration = configuration;
			PutCORSConfigurationAsync(putCORSConfigurationRequest, callback, options);
		}

		public void PutCORSConfigurationAsync(PutCORSConfigurationRequest request, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutCORSConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutCORSConfigurationRequestMarshaller putCORSConfigurationRequestMarshaller = new PutCORSConfigurationRequestMarshaller();
			PutCORSConfigurationResponseUnmarshaller instance = PutCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutCORSConfigurationRequest, PutCORSConfigurationResponse> val = new AmazonServiceResult<PutCORSConfigurationRequest, PutCORSConfigurationResponse>((PutCORSConfigurationRequest)req, (PutCORSConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutCORSConfigurationRequest>(request, putCORSConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutLifecycleConfigurationAsync(string bucketName, LifecycleConfiguration configuration, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			PutLifecycleConfigurationRequest putLifecycleConfigurationRequest = new PutLifecycleConfigurationRequest();
			putLifecycleConfigurationRequest.BucketName = bucketName;
			putLifecycleConfigurationRequest.Configuration = configuration;
			PutLifecycleConfigurationAsync(putLifecycleConfigurationRequest, callback, options);
		}

		public void PutLifecycleConfigurationAsync(PutLifecycleConfigurationRequest request, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutLifecycleConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutLifecycleConfigurationRequestMarshaller putLifecycleConfigurationRequestMarshaller = new PutLifecycleConfigurationRequestMarshaller();
			PutLifecycleConfigurationResponseUnmarshaller instance = PutLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> val = new AmazonServiceResult<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse>((PutLifecycleConfigurationRequest)req, (PutLifecycleConfigurationResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutLifecycleConfigurationRequest>(request, putLifecycleConfigurationRequestMarshaller, instance, options, action);
		}

		public void PutObjectAsync(PutObjectRequest request, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutObjectRequestMarshaller putObjectRequestMarshaller = new PutObjectRequestMarshaller();
			PutObjectResponseUnmarshaller instance = PutObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutObjectRequest, PutObjectResponse> val = new AmazonServiceResult<PutObjectRequest, PutObjectResponse>((PutObjectRequest)req, (PutObjectResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutObjectRequest>(request, putObjectRequestMarshaller, instance, options, action);
		}

		public void PutObjectTaggingAsync(PutObjectTaggingRequest request, AmazonServiceCallback<PutObjectTaggingRequest, PutObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("PutObjectTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			PutObjectTaggingRequestMarshaller putObjectTaggingRequestMarshaller = new PutObjectTaggingRequestMarshaller();
			PutObjectTaggingResponseUnmarshaller instance = PutObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutObjectTaggingRequest, PutObjectTaggingResponse> val = new AmazonServiceResult<PutObjectTaggingRequest, PutObjectTaggingResponse>((PutObjectTaggingRequest)req, (PutObjectTaggingResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<PutObjectTaggingRequest>(request, putObjectTaggingRequestMarshaller, instance, options, action);
		}

		public void RestoreObjectAsync(string bucketName, string key, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public void RestoreObjectAsync(string bucketName, string key, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.Days = days;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public void RestoreObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.VersionId = versionId;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public void RestoreObjectAsync(string bucketName, string key, string versionId, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.VersionId = versionId;
			restoreObjectRequest.Days = days;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public void RestoreObjectAsync(RestoreObjectRequest request, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			RestoreObjectRequestMarshaller restoreObjectRequestMarshaller = new RestoreObjectRequestMarshaller();
			RestoreObjectResponseUnmarshaller instance = RestoreObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<RestoreObjectRequest, RestoreObjectResponse> val = new AmazonServiceResult<RestoreObjectRequest, RestoreObjectResponse>((RestoreObjectRequest)req, (RestoreObjectResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<RestoreObjectRequest>(request, restoreObjectRequestMarshaller, instance, options, action);
		}

		public void UploadPartAsync(UploadPartRequest request, AmazonServiceCallback<UploadPartRequest, UploadPartResponse> callback, AsyncOptions options = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AWSConfigs.get_HttpClient() == 0)
			{
				throw new InvalidOperationException("UploadPart is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? ((object)new AsyncOptions()) : ((object)options));
			UploadPartRequestMarshaller uploadPartRequestMarshaller = new UploadPartRequestMarshaller();
			UploadPartResponseUnmarshaller instance = UploadPartResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> action = null;
			if (callback != null)
			{
				action = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UploadPartRequest, UploadPartResponse> val = new AmazonServiceResult<UploadPartRequest, UploadPartResponse>((UploadPartRequest)req, (UploadPartResponse)res, ex, ao.get_State());
					callback.Invoke(val);
				};
			}
			this.BeginInvoke<UploadPartRequest>(request, uploadPartRequestMarshaller, instance, options, action);
		}

		IClientConfig IAmazonService.get_Config()
		{
			return this.get_Config();
		}
	}
}
