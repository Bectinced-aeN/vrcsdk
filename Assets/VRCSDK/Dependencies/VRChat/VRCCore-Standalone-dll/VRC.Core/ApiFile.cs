using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using VRC.Core.BestHTTP;

namespace VRC.Core
{
	public class ApiFile : ApiModel
	{
		public enum Status
		{
			None,
			Waiting,
			Queued,
			Complete,
			Error
		}

		public enum Category
		{
			Simple,
			Multipart,
			Queued
		}

		public class Version : ApiModel
		{
			public enum FileType
			{
				Full,
				Delta
			}

			public class FileDescriptor : ApiModel
			{
				public enum Type
				{
					file,
					delta,
					signature
				}

				[ApiField(Required = false)]
				public Status status
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public string url
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public string md5
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public Category category
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public int sizeInBytes
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public string fileName
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public string uploadId
				{
					get;
					private set;
				}

				[ApiField(Required = false)]
				public List<string> cdns
				{
					get;
					private set;
				}

				public FileDescriptor()
				{
					status = Status.None;
					url = string.Empty;
					md5 = string.Empty;
					category = Category.Simple;
					sizeInBytes = 0;
				}

				public void Init(FileDescriptor desc)
				{
					status = desc.status;
					url = desc.url;
					md5 = desc.md5;
					category = desc.category;
					sizeInBytes = desc.sizeInBytes;
				}
			}

			[ApiField]
			public int version
			{
				get;
				private set;
			}

			[ApiField]
			public Status status
			{
				get;
				private set;
			}

			[ApiField]
			public DateTime created_at
			{
				get;
				private set;
			}

			[ApiField]
			public FileDescriptor file
			{
				get;
				private set;
			}

			[ApiField]
			public FileDescriptor signature
			{
				get;
				private set;
			}

			[ApiField]
			public FileDescriptor delta
			{
				get;
				private set;
			}

			[ApiField]
			public bool deleted
			{
				get;
				private set;
			}

			public Version()
			{
				version = 0;
				status = Status.None;
				created_at = default(DateTime);
				file = new FileDescriptor();
				signature = new FileDescriptor();
				delta = new FileDescriptor();
				deleted = false;
			}

			public void Init(Version v)
			{
				version = v.version;
				status = v.status;
				created_at = v.created_at;
				file.Init(v.file);
				signature.Init(v.signature);
				delta.Init(v.delta);
				deleted = v.deleted;
			}

			public FileDescriptor GetFileDescriptor(FileDescriptor.Type fileDescriptorType)
			{
				switch (fileDescriptorType)
				{
				case FileDescriptor.Type.file:
					return file;
				case FileDescriptor.Type.delta:
					return delta;
				case FileDescriptor.Type.signature:
					return signature;
				default:
					Debug.LogError((object)("Unknown FileDescriptor.Type " + fileDescriptorType));
					return file;
				}
			}
		}

		private class DownloadContext
		{
			public Action<byte[]> OnSuccess;

			public Action<string> OnError;

			public DownloadContext(Action<byte[]> onSuccess, Action<string> onError)
			{
				OnSuccess = onSuccess;
				OnError = onError;
			}
		}

		public class UploadStatus : ApiModel
		{
			[ApiField]
			public List<string> etags
			{
				get;
				set;
			}

			[ApiField]
			public double nextPartNumber
			{
				get;
				set;
			}

			[ApiField]
			public double maxParts
			{
				get;
				set;
			}

			[ApiField]
			public List<object> parts
			{
				get;
				set;
			}

			[ApiField]
			public string uploadId
			{
				get;
				set;
			}

			[ApiField]
			public string fileName
			{
				get;
				set;
			}

			public UploadStatus()
			{
			}

			public UploadStatus(string id, int version, Version.FileDescriptor.Type descriptor, string action)
				: base("file/" + id + "/" + version.ToString() + "/" + descriptor.ToString() + "/" + action)
			{
				base.id = null;
			}
		}

		public const bool kDefaultUseFileAPI = false;

		[ApiField]
		public string name
		{
			get;
			private set;
		}

		[ApiField]
		public string ownerId
		{
			get;
			private set;
		}

		[ApiField]
		public string mimeType
		{
			get;
			private set;
		}

		[ApiField]
		public string extension
		{
			get;
			private set;
		}

		[ApiField]
		public List<Version> versions
		{
			get;
			private set;
		}

		[DefaultValue(false)]
		public bool IsInitialized
		{
			get;
			private set;
		}

		[DefaultValue(false)]
		public bool IsPendingInit
		{
			get;
			private set;
		}

		public ApiFile()
			: base("file")
		{
			versions = new List<Version>();
		}

		public ApiFile(ApiFile file)
			: this()
		{
			if (file != null)
			{
				base.id = file.id;
				name = file.name;
				ownerId = file.ownerId;
				mimeType = file.mimeType;
				extension = file.extension;
				versions = file.versions.Select(delegate(Version v)
				{
					Version version = new Version();
					version.Init(v);
					return version;
				}).ToList();
				IsInitialized = file.IsInitialized;
			}
		}

		public ApiFile(string fileId)
			: this()
		{
			if (!string.IsNullOrEmpty(fileId))
			{
				base.id = fileId;
				Fetch(null, delegate(ApiContainer c)
				{
					Debug.LogError((object)("Could not fetch ApiFile with ID: " + fileId + " because " + c.Error));
				});
			}
		}

		public static void Create(string name, string mimeType, string extension, Action<ApiContainer> successCallback, Action<ApiContainer> errorCallback)
		{
			ApiFile apiFile = new ApiFile();
			apiFile.name = name;
			apiFile.mimeType = mimeType;
			apiFile.extension = extension;
			ApiFile apiFile2 = apiFile;
			apiFile2.Save(successCallback, errorCallback);
		}

		public void Refresh(Action<ApiContainer> onSuccess, Action<ApiContainer> onError)
		{
			Fetch(onSuccess, onError, null, disableCache: true);
		}

		public static void DownloadFile(string url, Action<byte[]> onSuccess, Action<string> onError, Action<int, int> onProgress)
		{
			HTTPRequest hTTPRequest = null;
			try
			{
				hTTPRequest = new HTTPRequest(new Uri(url), isKeepAlive: false, disableCache: true, null);
				hTTPRequest.OnProgress = delegate(HTTPRequest req, int downloaded, int length)
				{
					if (onProgress != null)
					{
						onProgress(downloaded, length);
					}
				};
				hTTPRequest.Tag = new DownloadContext(onSuccess, onError);
				hTTPRequest.Callback = OnDownloadFileCompleted;
				hTTPRequest.Timeout = TimeSpan.FromMinutes(10.0);
				Debug.Log((object)("ApiFile: Download: " + url));
				hTTPRequest.Send();
			}
			catch (Exception)
			{
				string obj = (hTTPRequest == null) ? ("Bad request url: " + url) : hTTPRequest.Exception.Message;
				onError?.Invoke(obj);
			}
		}

		public static string ParseFileIdFromFileAPIUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return string.Empty;
			}
			Uri uri = new Uri(url);
			Match match = Regex.Match(uri.AbsolutePath, "/api/\\d+/file/([^/]+)");
			if (match.Success && match.Groups != null && match.Groups.Count == 2)
			{
				return match.Groups[1].Value;
			}
			return string.Empty;
		}

		private static void OnDownloadFileCompleted(HTTPRequest request, HTTPResponse response)
		{
			DownloadContext downloadContext = request.Tag as DownloadContext;
			switch (request.State)
			{
			case HTTPRequestStates.Finished:
				if (response.IsSuccess)
				{
					Debug.Log((object)("ApiFile: Download complete: " + request.Uri.OriginalString));
					if (downloadContext.OnSuccess != null)
					{
						downloadContext.OnSuccess(response.Data);
					}
				}
				else
				{
					Debug.LogError((object)string.Format("ApiFile: Download: " + request.Uri.OriginalString + " - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", response.StatusCode, response.Message, response.DataAsText));
					if (downloadContext.OnError != null)
					{
						downloadContext.OnError("(" + response.StatusCode + ") " + ((response.Message == null) ? string.Empty : response.Message) + ((response.DataAsText == null) ? string.Empty : (": " + response.DataAsText)));
					}
				}
				break;
			case HTTPRequestStates.Error:
				Debug.LogError((object)("ApiFile: Download: " + request.Uri.OriginalString + " - Request Finished with Error! " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message)));
				if (downloadContext.OnError != null)
				{
					downloadContext.OnError("Request Finished with Error: " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message));
				}
				break;
			case HTTPRequestStates.Aborted:
				Debug.LogWarning((object)("ApiFile: Download: " + request.Uri.OriginalString + " - Request Aborted"));
				if (downloadContext.OnError != null)
				{
					downloadContext.OnError("The download was cancelled");
				}
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				Debug.LogError((object)("ApiFile: Download: " + request.Uri.OriginalString + " - Connection Timed Out!"));
				if (downloadContext.OnError != null)
				{
					downloadContext.OnError("Connection timed out");
				}
				break;
			case HTTPRequestStates.TimedOut:
				Debug.LogError((object)("ApiFile: Download: " + request.Uri.OriginalString + " - Processing the request Timed Out!"));
				if (downloadContext.OnError != null)
				{
					downloadContext.OnError("Processing the request timed out");
				}
				break;
			}
		}

		public override bool ShouldCache()
		{
			return false;
		}

		public override bool SetApiFieldsFromJson(Dictionary<string, object> fields, ref string Error)
		{
			if (base.SetApiFieldsFromJson(fields, ref Error))
			{
				IsInitialized = true;
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return $"[id: {base.id}; name: {name}; mimeType: {mimeType}; extension: {extension}; ownerId: {ownerId}]";
		}

		public string ToStringBrief()
		{
			return base.id;
		}

		public void CreateNewVersion(Version.FileType fileType, string fileOrDeltaMd5Base64, long fileOrDeltaSizeInBytes, string signatureMd5Base64, long signatureSizeInBytes, Action<ApiContainer> successCallback, Action<ApiContainer> errorCallback)
		{
			if (!IsInitialized)
			{
				Debug.LogError((object)"Unable to create new file version: file not initialized.");
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["signatureMd5"] = signatureMd5Base64;
				dictionary["signatureSizeInBytes"] = signatureSizeInBytes;
				dictionary[(fileType != 0) ? "deltaMd5" : "fileMd5"] = fileOrDeltaMd5Base64;
				dictionary[(fileType != 0) ? "deltaSizeInBytes" : "fileSizeInBytes"] = fileOrDeltaSizeInBytes;
				ApiModelContainer<ApiFile> apiModelContainer = new ApiModelContainer<ApiFile>(this);
				apiModelContainer.OnSuccess = successCallback;
				apiModelContainer.OnError = errorCallback;
				ApiModelContainer<ApiFile> responseContainer = apiModelContainer;
				API.SendRequest(MakeRequestEndpoint(), HTTPMethods.Post, responseContainer, dictionary, needsAPIKey: true, Application.get_isEditor());
			}
		}

		public bool HasExistingVersion()
		{
			int num = versions.Count - 1;
			while (num > 0 && (versions[num].deleted || versions[num].status != Status.Complete))
			{
				num--;
			}
			return num > 0;
		}

		public bool HasExistingOrPendingVersion()
		{
			int num = GetLatestVersionNumber();
			while (num > 0 && (versions[num] == null || versions[num].deleted))
			{
				num--;
			}
			return num > 0;
		}

		public int GetLatestVersionNumber()
		{
			return (versions != null) ? Mathf.Max(versions.Count - 1, 0) : (-1);
		}

		public int GetLatestCompleteVersionNumber()
		{
			if (!HasExistingVersion())
			{
				return 0;
			}
			int num = versions.Count - 1;
			while (num > 0 && versions[num].status != Status.Complete)
			{
				num--;
			}
			return num;
		}

		public Version GetVersion(int v)
		{
			if (versions == null || v <= 0 || v >= versions.Count)
			{
				return null;
			}
			return versions[v];
		}

		public Version GetLatestCompleteVersion()
		{
			return GetVersion(GetLatestCompleteVersionNumber());
		}

		public Version GetLatestVersion()
		{
			return GetVersion(GetLatestVersionNumber());
		}

		public void DeleteVersion(int versionNumber, Action<ApiContainer> successCallback = null, Action<ApiContainer> errorCallback = null)
		{
			if (!IsInitialized)
			{
				Debug.LogError((object)"Unable to delete file: file not initialized.");
			}
			else if (versionNumber <= 0 || versionNumber >= versions.Count)
			{
				Debug.LogError((object)("ApiFile(" + base.id + "): version to delete is invalid: " + versionNumber));
			}
			else
			{
				ApiModelContainer<ApiFile> apiModelContainer = new ApiModelContainer<ApiFile>(this);
				apiModelContainer.OnSuccess = successCallback;
				apiModelContainer.OnError = errorCallback;
				ApiModelContainer<ApiFile> responseContainer = apiModelContainer;
				API.SendRequest("file/" + base.id + "/" + versionNumber, HTTPMethods.Delete, responseContainer);
			}
		}

		public void DeleteLatestVersion(Action<ApiContainer> successCallback = null, Action<ApiContainer> errorCallback = null)
		{
			if (!IsInitialized)
			{
				Debug.LogError((object)"Unable to delete file: file not initialized.");
			}
			else
			{
				int latestVersionNumber = GetLatestVersionNumber();
				if (latestVersionNumber <= 0 || latestVersionNumber >= versions.Count)
				{
					Debug.LogError((object)("ApiFile(" + base.id + "): version to delete is invalid: " + latestVersionNumber));
				}
				else if (latestVersionNumber == 1)
				{
					string nameTemp = name;
					string mimeTypeTemp = mimeType;
					string extTemp = extension;
					API.Delete<ApiFile>(base.id, delegate
					{
						Create(nameTemp, mimeTypeTemp, extTemp, successCallback, errorCallback);
					}, errorCallback);
				}
				else
				{
					DeleteVersion(latestVersionNumber, successCallback, errorCallback);
				}
			}
		}

		public void DownloadSignature(Action<byte[]> onSuccess, Action<string> onError, Action<int, int> onProgress)
		{
			DownloadFile(GetSignatureURL(), onSuccess, onError, onProgress);
		}

		public string GetFileURL()
		{
			return GetFileURL(GetLatestCompleteVersionNumber());
		}

		public string GetFileURL(int versionNumber)
		{
			return GetFileApiURL(versionNumber, Version.FileDescriptor.Type.file);
		}

		public string GetFileRawURL()
		{
			return GetFileRawURL(GetLatestCompleteVersionNumber());
		}

		public string GetFileRawURL(int versionNumber)
		{
			Version version = GetVersion(versionNumber);
			return (version == null) ? string.Empty : version.file.url;
		}

		public string GetDeltaURL()
		{
			return GetDeltaURL(GetLatestCompleteVersionNumber());
		}

		public string GetDeltaURL(int versionNumber)
		{
			return GetFileApiURL(versionNumber, Version.FileDescriptor.Type.delta);
		}

		public string GetDeltaRawURL()
		{
			return GetDeltaRawURL(GetLatestCompleteVersionNumber());
		}

		public string GetDeltaRawURL(int versionNumber)
		{
			Version version = GetVersion(versionNumber);
			return (version == null) ? string.Empty : version.delta.url;
		}

		public string GetSignatureURL()
		{
			return GetSignatureURL(GetLatestCompleteVersionNumber());
		}

		public string GetSignatureURL(int versionNumber)
		{
			return GetFileApiURL(versionNumber, Version.FileDescriptor.Type.signature);
		}

		public string GetSignatureRawURL()
		{
			return GetSignatureRawURL(GetLatestCompleteVersionNumber());
		}

		public string GetSignatureRawURL(int versionNumber)
		{
			Version version = GetVersion(versionNumber);
			return (version == null) ? string.Empty : version.signature.url;
		}

		public string GetFileApiURL(int versionNumber, Version.FileDescriptor.Type type)
		{
			Version version = GetVersion(versionNumber);
			if (version == null)
			{
				return string.Empty;
			}
			return API.GetApiUrl() + "file/" + base.id + "/" + versionNumber + "/" + type.ToString();
		}

		public string GetFileMD5()
		{
			return GetFileMD5(GetLatestCompleteVersionNumber());
		}

		public string GetFileMD5(int versionNumber)
		{
			Version version = GetVersion(versionNumber);
			return (version == null) ? string.Empty : version.file.md5;
		}

		public Version.FileDescriptor GetFileDescriptor(int versionNumber, Version.FileDescriptor.Type fileDescriptorType)
		{
			return GetVersion(versionNumber)?.GetFileDescriptor(fileDescriptorType);
		}

		public bool IsLatestVersionQueued(bool checkDelta = true)
		{
			Version version = GetVersion(GetLatestVersionNumber());
			if (version == null)
			{
				return false;
			}
			if (version.status == Status.Queued)
			{
				return true;
			}
			if ((!checkDelta && version.file != null && version.file.status == Status.Queued) || (checkDelta && version.delta != null && version.delta.status == Status.Queued) || (version.signature != null && version.signature.status == Status.Queued))
			{
				return true;
			}
			return false;
		}

		public bool HasQueuedOperation(bool checkDelta = true)
		{
			if (IsWaitingForUpload())
			{
				return false;
			}
			if (HasExistingOrPendingVersion())
			{
				return IsLatestVersionQueued(checkDelta);
			}
			return false;
		}

		public bool IsWaitingForUpload()
		{
			if (HasExistingOrPendingVersion())
			{
				Version version = GetVersion(GetLatestVersionNumber());
				if (version.status == Status.Waiting)
				{
					return true;
				}
				if ((version.file != null && version.file.status == Status.Waiting) || (version.delta != null && version.delta.status == Status.Waiting) || (version.signature != null && version.signature.status == Status.Waiting))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsInErrorState()
		{
			if (HasExistingOrPendingVersion())
			{
				Version version = GetVersion(GetLatestVersionNumber());
				if (version.status == Status.Error)
				{
					return true;
				}
				if ((version.file != null && version.file.status == Status.Error) || (version.delta != null && version.delta.status == Status.Error) || (version.signature != null && version.signature.status == Status.Error))
				{
					return true;
				}
			}
			return false;
		}

		public void StartSimpleUpload(Version.FileDescriptor.Type fileDescriptorType, Action<ApiContainer> onSuccess, Action<ApiContainer> onError)
		{
			if (!IsInitialized)
			{
				onError?.Invoke(new ApiContainer
				{
					Error = "Unable to upload file: file not initialized."
				});
			}
			else
			{
				int latestVersionNumber = GetLatestVersionNumber();
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(latestVersionNumber, fileDescriptorType);
				if (fileDescriptor == null)
				{
					onError?.Invoke(new ApiContainer
					{
						Error = "Version record doesn't exist"
					});
				}
				else
				{
					UploadStatus uploadStatus = new UploadStatus(base.id, latestVersionNumber, fileDescriptorType, "start");
					ApiDictContainer apiDictContainer = new ApiDictContainer("url");
					apiDictContainer.OnSuccess = onSuccess;
					apiDictContainer.OnError = onError;
					ApiDictContainer responseContainer = apiDictContainer;
					API.SendPutRequest(uploadStatus.Endpoint, responseContainer);
				}
			}
		}

		public void StartMultipartUpload(Version.FileDescriptor.Type fileDescriptorType, int partNumber, Action<ApiContainer> onSuccess, Action<ApiContainer> onError)
		{
			if (!IsInitialized)
			{
				onError?.Invoke(new ApiContainer
				{
					Error = "Unable to upload file: file not initialized."
				});
			}
			else
			{
				int latestVersionNumber = GetLatestVersionNumber();
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(latestVersionNumber, fileDescriptorType);
				if (fileDescriptor == null)
				{
					onError?.Invoke(new ApiContainer
					{
						Error = "Version record doesn't exist"
					});
				}
				else
				{
					UploadStatus uploadStatus = new UploadStatus(base.id, latestVersionNumber, fileDescriptorType, "start");
					ApiDictContainer apiDictContainer = new ApiDictContainer("url");
					apiDictContainer.OnSuccess = onSuccess;
					apiDictContainer.OnError = onError;
					ApiDictContainer responseContainer = apiDictContainer;
					API.SendPutRequest(uploadStatus.Endpoint + "?partNumber=" + partNumber, responseContainer);
				}
			}
		}

		public void FinishUpload(Version.FileDescriptor.Type fileDescriptorType, List<string> multipartEtags, Action<ApiContainer> onSuccess, Action<ApiContainer> onError)
		{
			if (!IsInitialized)
			{
				onError?.Invoke(new ApiContainer
				{
					Error = "Unable to finish upload of file: file not initialized."
				});
			}
			else
			{
				int latestVersionNumber = GetLatestVersionNumber();
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(latestVersionNumber, fileDescriptorType);
				if (fileDescriptor == null)
				{
					onError?.Invoke(new ApiContainer
					{
						Error = "Version record doesn't exist"
					});
				}
				else
				{
					UploadStatus uploadStatus = new UploadStatus(base.id, latestVersionNumber, fileDescriptorType, "finish");
					uploadStatus.etags = multipartEtags;
					UploadStatus uploadStatus2 = uploadStatus;
					uploadStatus2.Put(onSuccess, onError);
				}
			}
		}

		public void GetUploadStatus(int versionNumber, Version.FileDescriptor.Type fileDescriptorType, Action<ApiContainer> onSuccess = null, Action<ApiContainer> onError = null)
		{
			if (!IsInitialized)
			{
				onError?.Invoke(new ApiContainer
				{
					Error = "Upload status not retrieved: file not initialized."
				});
			}
			else
			{
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(versionNumber, fileDescriptorType);
				if (fileDescriptor == null)
				{
					onError?.Invoke(new ApiContainer
					{
						Error = "Upload status not retrieved: unknown file descriptor."
					});
				}
				else
				{
					UploadStatus uploadStatus = new UploadStatus(base.id, versionNumber, fileDescriptorType, "status");
					uploadStatus.Fetch(onSuccess, onError);
				}
			}
		}

		public static HttpRequest PutSimpleFileToURL(string url, string filename, string contentType, string md5Base64, Action onSuccess, Action<string> onError, Action<long, long> onProgress)
		{
			Debug.Log((object)("PutSimpleFileToURL: " + filename + " => " + url + " (" + contentType + ")"));
			HTTPRequest request = null;
			try
			{
				request = new HTTPRequest(new Uri(url), HTTPMethods.Put);
			}
			catch (Exception)
			{
				if (onError != null)
				{
					onError("Invalid URL: " + url);
				}
				return null;
				IL_00a1:;
			}
			request.ConnectTimeout = TimeSpan.FromSeconds(20.0);
			request.Timeout = TimeSpan.FromMinutes(60.0);
			request.AddHeader("content-type", contentType);
			request.AddHeader("content-md5", md5Base64);
			byte[] array = null;
			try
			{
				array = File.ReadAllBytes(filename);
			}
			catch (Exception ex2)
			{
				if (onError != null)
				{
					onError("Couldn't read file: " + filename + "\n" + ex2.Message);
				}
				return null;
				IL_013c:;
			}
			request.RawData = array;
			request.Callback = delegate(HTTPRequest originalRequest, HTTPResponse response)
			{
				switch (request.State)
				{
				case HTTPRequestStates.Finished:
					if (response.IsSuccess)
					{
						Debug.Log((object)("PutSimpleFileToURL: Upload complete: " + request.Uri.OriginalString));
						if (onSuccess != null)
						{
							onSuccess();
						}
					}
					else
					{
						Debug.LogError((object)string.Format("PutSimpleFileToURL: " + request.Uri.OriginalString + " - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", response.StatusCode, response.Message, response.DataAsText));
						if (onError != null)
						{
							onError("(" + response.StatusCode + ") " + ((response.Message == null) ? string.Empty : response.Message) + ((response.DataAsText == null) ? string.Empty : (": " + response.DataAsText)));
						}
					}
					break;
				case HTTPRequestStates.Error:
					Debug.LogError((object)("PutSimpleFileToURL: " + request.Uri.OriginalString + " - Request Finished with Error! " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message)));
					if (onError != null)
					{
						onError("Request Finished with Error: " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message));
					}
					break;
				case HTTPRequestStates.Aborted:
					Debug.LogWarning((object)("PutSimpleFileToURL: " + request.Uri.OriginalString + " - Request Aborted"));
					if (onError != null)
					{
						onError("The upload was cancelled");
					}
					break;
				case HTTPRequestStates.ConnectionTimedOut:
					Debug.LogError((object)("PutSimpleFileToURL: " + request.Uri.OriginalString + " - Connection Timed Out!"));
					if (onError != null)
					{
						onError("Connection timed out");
					}
					break;
				case HTTPRequestStates.TimedOut:
					Debug.LogError((object)("PutSimpleFileToURL: " + request.Uri.OriginalString + " - Processing the request Timed Out!"));
					if (onError != null)
					{
						onError("Processing the request timed out");
					}
					break;
				}
			};
			request.OnUploadProgress = delegate(HTTPRequest originalRequest, long uploaded, long length)
			{
				if (onProgress != null)
				{
					onProgress(uploaded, length);
				}
			};
			request.Send();
			return new HttpRequest(request);
		}

		public static HttpRequest PutMultipartDataToURL(string url, byte[] buffer, int bufferLength, string contentType, Action<string> onSuccess, Action<string> onError, Action<long, long> onProgress)
		{
			Debug.Log((object)("PutMultipartDataToURL: [" + bufferLength + "] => " + url + " (" + contentType + ")"));
			HTTPRequest request = null;
			try
			{
				request = new HTTPRequest(new Uri(url), HTTPMethods.Put);
			}
			catch (Exception)
			{
				if (onError != null)
				{
					onError("Invalid URL: " + url);
				}
				return null;
				IL_00a6:;
			}
			request.ConnectTimeout = TimeSpan.FromSeconds(20.0);
			request.Timeout = TimeSpan.FromMinutes(60.0);
			request.AddHeader("content-length", bufferLength.ToString());
			byte[] array = new byte[bufferLength];
			Array.Copy(buffer, array, bufferLength);
			request.RawData = array;
			request.Callback = delegate(HTTPRequest originalRequest, HTTPResponse response)
			{
				switch (request.State)
				{
				case HTTPRequestStates.Finished:
					if (response.IsSuccess)
					{
						Debug.Log((object)("PutMultipartDataToURL: Upload complete: " + request.Uri.OriginalString));
						string text = response.GetFirstHeaderValue("etag");
						if (string.IsNullOrEmpty(text))
						{
							text = string.Empty;
						}
						text = text.Trim('"', '\'');
						if (onSuccess != null)
						{
							onSuccess(text);
						}
					}
					else
					{
						Debug.LogError((object)string.Format("PutMultipartDataToURL: " + request.Uri.OriginalString + " - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", response.StatusCode, response.Message, response.DataAsText));
						if (onError != null)
						{
							onError("(" + response.StatusCode + ") " + ((response.Message == null) ? string.Empty : response.Message) + ((response.DataAsText == null) ? string.Empty : (": " + response.DataAsText)));
						}
					}
					break;
				case HTTPRequestStates.Error:
					Debug.LogError((object)("PutMultipartDataToURL: " + request.Uri.OriginalString + " - Request Finished with Error! " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message)));
					if (onError != null)
					{
						onError("Request Finished with Error: " + ((request.Exception == null) ? "(No Exception)" : request.Exception.Message));
					}
					break;
				case HTTPRequestStates.Aborted:
					Debug.LogWarning((object)("PutMultipartDataToURL: " + request.Uri.OriginalString + " - Request Aborted"));
					if (onError != null)
					{
						onError("The upload was cancelled");
					}
					break;
				case HTTPRequestStates.ConnectionTimedOut:
					Debug.LogError((object)("PutMultipartDataToURL: " + request.Uri.OriginalString + " - Connection Timed Out!"));
					if (onError != null)
					{
						onError("Connection timed out");
					}
					break;
				case HTTPRequestStates.TimedOut:
					Debug.LogError((object)("PutMultipartDataToURL: " + request.Uri.OriginalString + " - Processing the request Timed Out!"));
					if (onError != null)
					{
						onError("Processing the request timed out");
					}
					break;
				}
			};
			request.OnUploadProgress = delegate(HTTPRequest originalRequest, long uploaded, long length)
			{
				if (onProgress != null)
				{
					onProgress(uploaded, length);
				}
			};
			request.Send();
			return new HttpRequest(request);
		}
	}
}
