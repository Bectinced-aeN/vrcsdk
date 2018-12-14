using System;
using System.Collections.Generic;
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
			none,
			waiting,
			queued,
			complete,
			error
		}

		public enum Category
		{
			simple,
			multipart,
			queued
		}

		public class Version
		{
			public enum FileType
			{
				Full,
				Delta
			}

			public class FileDescriptor
			{
				public enum Type
				{
					file,
					delta,
					signature
				}

				private Status mStatus;

				protected string mUrl;

				protected string mMD5;

				private Category mCategory;

				private int mSizeInBytes;

				public Status status => mStatus;

				public string url => mUrl;

				public string md5 => mMD5;

				public Category category => mCategory;

				public int sizeInBytes => mSizeInBytes;

				public FileDescriptor()
				{
					mStatus = Status.none;
					mUrl = string.Empty;
					mMD5 = string.Empty;
					mCategory = Category.simple;
					mSizeInBytes = 0;
				}

				public void Init(FileDescriptor desc)
				{
					mStatus = desc.mStatus;
					mUrl = desc.mUrl;
					mMD5 = desc.mMD5;
					mCategory = desc.mCategory;
					mSizeInBytes = desc.mSizeInBytes;
				}

				public void Init(Dictionary<string, object> jsonObject)
				{
					mStatus = (Status)(int)Enum.Parse(typeof(Status), jsonObject["status"] as string);
					mUrl = (jsonObject["url"] as string);
					mMD5 = (jsonObject["md5"] as string);
					mCategory = (Category)(int)Enum.Parse(typeof(Category), jsonObject["category"] as string);
					mSizeInBytes = Convert.ToInt32(jsonObject["sizeInBytes"]);
				}
			}

			private int mVersion;

			private Status mStatus;

			protected DateTime mCreatedAt;

			private FileDescriptor mFile;

			private FileDescriptor mSignature;

			private FileDescriptor mDelta;

			private bool mDeleted;

			public int version => mVersion;

			public Status status => mStatus;

			public DateTime created_at => mCreatedAt;

			public FileDescriptor file => mFile;

			public FileDescriptor signature => mSignature;

			public FileDescriptor delta => mDelta;

			public bool deleted => mDeleted;

			public Version()
			{
				mVersion = 0;
				mStatus = Status.none;
				mCreatedAt = default(DateTime);
				mFile = new FileDescriptor();
				mSignature = new FileDescriptor();
				mDelta = new FileDescriptor();
				mDeleted = false;
			}

			public void Init(Version v)
			{
				mVersion = v.mVersion;
				mStatus = v.mStatus;
				mCreatedAt = v.mCreatedAt;
				mFile.Init(v.mFile);
				mSignature.Init(v.mSignature);
				mDelta.Init(v.mDelta);
				mDeleted = v.mDeleted;
			}

			public void Init(Dictionary<string, object> jsonObject)
			{
				mVersion = Convert.ToInt32(jsonObject["version"]);
				mStatus = (Status)(int)Enum.Parse(typeof(Status), jsonObject["status"] as string);
				mCreatedAt = DateTime.Parse(jsonObject["created_at"] as string);
				if (jsonObject.ContainsKey("file") && jsonObject["file"] != null)
				{
					mFile.Init(jsonObject["file"] as Dictionary<string, object>);
				}
				if (jsonObject.ContainsKey("signature") && jsonObject["signature"] != null)
				{
					mSignature.Init(jsonObject["signature"] as Dictionary<string, object>);
				}
				if (jsonObject.ContainsKey("delta") && jsonObject["delta"] != null)
				{
					mDelta.Init(jsonObject["delta"] as Dictionary<string, object>);
				}
				mDeleted = (jsonObject.ContainsKey("deleted") && Convert.ToBoolean(jsonObject["deleted"]));
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

		public class UploadStatus
		{
			public List<string> etags = new List<string>();

			public int nextPartNumber;

			public void Init(UploadStatus u)
			{
				etags = u.etags.ToList();
				nextPartNumber = u.nextPartNumber;
			}

			public void Init(Dictionary<string, object> jsonObject)
			{
				etags = Tools.ObjListToStringList((List<object>)jsonObject["etags"]);
				nextPartNumber = Convert.ToInt32(jsonObject["nextPartNumber"]);
			}
		}

		private string mName;

		private string mOwnerId;

		private string mMimeType;

		private string mExtension;

		public List<Version> versions;

		private bool mIsInitialized;

		private bool mIsPendingInit;

		public string name => mName;

		public string ownerId => mOwnerId;

		public string mimeType => mMimeType;

		public string extension => mExtension;

		public static void Create(string name, string mimeType, string extension, Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["name"] = name;
			dictionary["mimeType"] = mimeType;
			dictionary["extension"] = extension;
			ApiModel.SendPostRequest("file", dictionary, delegate(Dictionary<string, object> obj)
			{
				ApiFile apiFile = ScriptableObject.CreateInstance<ApiFile>();
				apiFile.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiFile);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public static void Get(string fileId, Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			ApiModel.SendGetRequest("file/" + fileId, delegate(Dictionary<string, object> obj)
			{
				ApiFile apiFile = ScriptableObject.CreateInstance<ApiFile>();
				apiFile.Init(obj);
				if (successCallback != null)
				{
					successCallback(apiFile);
				}
			}, delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
		}

		public void Refresh(Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			Get(base.id, delegate(ApiFile file)
			{
				Init(file);
				if (successCallback != null)
				{
					successCallback(file);
				}
			}, errorCallback);
		}

		public static void Delete(string fileId, Action successCallback, Action<string> errorCallback)
		{
			ApiModel.SendRequest("file/" + fileId, HTTPMethods.Delete, (Dictionary<string, string>)null, (Action<Dictionary<string, object>>)delegate
			{
				if (successCallback != null)
				{
					successCallback();
				}
			}, (Action<string>)delegate(string obj)
			{
				if (errorCallback != null)
				{
					errorCallback(obj);
				}
			});
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

		public void Init()
		{
			mId = string.Empty;
			mName = string.Empty;
			mOwnerId = string.Empty;
			mMimeType = string.Empty;
			mExtension = string.Empty;
			versions = new List<Version>();
			mIsInitialized = false;
		}

		public void Init(Dictionary<string, object> jsonObject)
		{
			Init();
			Fill(jsonObject);
			mIsInitialized = true;
		}

		public void Init(ApiFile file)
		{
			if (file == null)
			{
				Init();
			}
			else
			{
				mId = file.mId;
				mName = file.mName;
				mOwnerId = file.mOwnerId;
				mMimeType = file.mMimeType;
				mExtension = file.mExtension;
				versions.Clear();
				foreach (Version version2 in file.versions)
				{
					Version version = new Version();
					version.Init(version2);
					versions.Add(version2);
				}
				mIsInitialized = file.mIsInitialized;
			}
		}

		public void Init(string fileId)
		{
			if (string.IsNullOrEmpty(fileId))
			{
				Init();
			}
			else
			{
				mId = fileId;
				mIsPendingInit = true;
				Get(fileId, delegate(ApiFile file)
				{
					Init(file);
					mIsPendingInit = false;
				}, delegate
				{
					mIsPendingInit = false;
					Debug.LogError((object)("Could not fetch ApiFile with ID: " + fileId));
				});
			}
		}

		public void Fill(Dictionary<string, object> jsonObject)
		{
			mId = (jsonObject["id"] as string);
			mName = (jsonObject["name"] as string);
			mOwnerId = (jsonObject["ownerId"] as string);
			mMimeType = (jsonObject["mimeType"] as string);
			mExtension = (jsonObject["extension"] as string);
			versions.Clear();
			List<object> list = jsonObject["versions"] as List<object>;
			foreach (object item in list)
			{
				Version version = new Version();
				version.Init(item as Dictionary<string, object>);
				versions.Add(version);
			}
		}

		public bool IsInitialized()
		{
			return mIsInitialized;
		}

		public bool IsPendingInit()
		{
			return mIsPendingInit;
		}

		public override string ToString()
		{
			return $"[id: {base.id}; name: {name}; mimeType: {mimeType}; extension: {extension}; ownerId: {ownerId}]";
		}

		public string ToStringBrief()
		{
			return base.id;
		}

		public void CreateNewVersion(Version.FileType fileType, string fileOrDeltaMd5Base64, long fileOrDeltaSizeInBytes, string signatureMd5Base64, long signatureSizeInBytes, Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			if (CheckInitialized(errorCallback))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["signatureMd5"] = signatureMd5Base64;
				dictionary["signatureSizeInBytes"] = signatureSizeInBytes.ToString();
				dictionary[(fileType != 0) ? "deltaMd5" : "fileMd5"] = fileOrDeltaMd5Base64;
				dictionary[(fileType != 0) ? "deltaSizeInBytes" : "fileSizeInBytes"] = fileOrDeltaSizeInBytes.ToString();
				ApiModel.SendPostRequest("file/" + base.id, dictionary, delegate(Dictionary<string, object> obj)
				{
					if (ValidateResponse(obj, errorCallback))
					{
						Fill(obj);
						if (successCallback != null)
						{
							successCallback(this);
						}
					}
				}, delegate(string obj)
				{
					if (errorCallback != null)
					{
						errorCallback(obj);
					}
				});
			}
		}

		public bool HasExistingVersion()
		{
			int num = versions.Count - 1;
			while (num > 0 && (versions[num].deleted || versions[num].status != Status.complete))
			{
				num--;
			}
			return num > 0;
		}

		public bool HasExistingOrPendingVersion()
		{
			int num = GetLatestVersionNumber();
			while (num > 0 && versions[num].deleted)
			{
				num--;
			}
			return num > 0;
		}

		public int GetLatestVersionNumber()
		{
			return Mathf.Max(versions.Count - 1, 0);
		}

		public int GetLatestCompleteVersionNumber()
		{
			if (!HasExistingVersion())
			{
				return 0;
			}
			int num = versions.Count - 1;
			while (num > 0 && versions[num].status != Status.complete)
			{
				num--;
			}
			return num;
		}

		public Version GetVersion(int v)
		{
			if (v <= 0 || v >= versions.Count)
			{
				Debug.LogError((object)("ApiFile(" + base.id + "): version " + v + " does not exist"));
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

		public void DeleteVersion(int versionNumber, Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			if (CheckInitialized(errorCallback))
			{
				if (versionNumber <= 0 || versionNumber >= versions.Count)
				{
					if (errorCallback != null)
					{
						errorCallback("ApiFile(" + base.id + "): version to delete is invalid: " + versionNumber);
					}
				}
				else
				{
					ApiModel.SendRequest("file/" + base.id + "/" + versionNumber, HTTPMethods.Delete, null, delegate(Dictionary<string, object> obj)
					{
						if (ValidateResponse(obj, errorCallback))
						{
							Fill(obj);
							if (successCallback != null)
							{
								successCallback(this);
							}
						}
					}, delegate(string obj)
					{
						if (errorCallback != null)
						{
							errorCallback(obj);
						}
					});
				}
			}
		}

		public void DeleteLatestVersion(Action<ApiFile> successCallback, Action<string> errorCallback)
		{
			if (CheckInitialized(errorCallback))
			{
				int latestVersionNumber = GetLatestVersionNumber();
				if (latestVersionNumber <= 0 || latestVersionNumber >= versions.Count)
				{
					if (errorCallback != null)
					{
						errorCallback("ApiFile(" + base.id + "): version to delete is invalid: " + latestVersionNumber);
					}
				}
				else if (latestVersionNumber == 1)
				{
					string nameTemp = name;
					string mimeTypeTemp = mimeType;
					string extTemp = extension;
					Delete(base.id, delegate
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
			return ApiModel.GetApiUrl() + "file/" + base.id + "/" + versionNumber + "/" + type.ToString();
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

		public bool HasQueuedOperation()
		{
			if (IsWaitingForUpload())
			{
				return false;
			}
			if (HasExistingOrPendingVersion())
			{
				Version version = GetVersion(GetLatestVersionNumber());
				if (version.status == Status.queued)
				{
					return true;
				}
				if ((version.file != null && version.file.status == Status.queued) || (version.delta != null && version.delta.status == Status.queued) || (version.signature != null && version.signature.status == Status.queued))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsWaitingForUpload()
		{
			if (HasExistingOrPendingVersion())
			{
				Version version = GetVersion(GetLatestVersionNumber());
				if (version.status == Status.waiting)
				{
					return true;
				}
				if ((version.file != null && version.file.status == Status.waiting) || (version.delta != null && version.delta.status == Status.waiting) || (version.signature != null && version.signature.status == Status.waiting))
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
				if (version.status == Status.error)
				{
					return true;
				}
				if ((version.file != null && version.file.status == Status.error) || (version.delta != null && version.delta.status == Status.error) || (version.signature != null && version.signature.status == Status.error))
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckInitialized(Action<string> errorCallback)
		{
			if (!IsInitialized())
			{
				errorCallback?.Invoke("ApiFile is not initialized: " + base.id);
				return false;
			}
			return true;
		}

		private bool ValidateResponse(Dictionary<string, object> response, Action<string> errorCallback)
		{
			bool flag = true && response.ContainsKey("name") && string.Compare(name, response["name"] as string) == 0 && response.ContainsKey("ownerId") && response.ContainsKey("mimeType") && string.Compare(mimeType, response["mimeType"] as string) == 0 && response.ContainsKey("extension") && string.Compare(extension, response["extension"] as string) == 0;
			if (!flag)
			{
				errorCallback?.Invoke("APIFile query response was invalid, fields do not match");
			}
			return flag;
		}

		public void StartSimpleUpload(Version.FileDescriptor.Type fileDescriptorType, Action<string> onSuccess, Action<string> onError)
		{
			if (CheckInitialized(onError))
			{
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(GetLatestVersionNumber(), fileDescriptorType);
				if (fileDescriptor == null)
				{
					if (onError != null)
					{
						onError("Version record doesn't exist");
					}
				}
				else
				{
					int latestVersionNumber = GetLatestVersionNumber();
					ApiModel.SendPutRequest("file/" + base.id + "/" + latestVersionNumber + "/" + fileDescriptorType.ToString() + "/start", delegate(Dictionary<string, object> obj)
					{
						string obj2 = obj["url"] as string;
						if (onSuccess != null)
						{
							onSuccess(obj2);
						}
					}, delegate(string obj)
					{
						if (onError != null)
						{
							onError(obj);
						}
					});
				}
			}
		}

		public void StartMultipartUpload(Version.FileDescriptor.Type fileDescriptorType, int partNumber, Action<string> onSuccess, Action<string> onError)
		{
			if (CheckInitialized(onError))
			{
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(GetLatestVersionNumber(), fileDescriptorType);
				if (fileDescriptor == null)
				{
					if (onError != null)
					{
						onError("Version record doesn't exist");
					}
				}
				else
				{
					int latestVersionNumber = GetLatestVersionNumber();
					ApiModel.SendPutRequest("file/" + base.id + "/" + latestVersionNumber + "/" + fileDescriptorType.ToString() + "/start?partNumber=" + partNumber, delegate(Dictionary<string, object> obj)
					{
						string obj2 = obj["url"] as string;
						if (onSuccess != null)
						{
							onSuccess(obj2);
						}
					}, delegate(string obj)
					{
						if (onError != null)
						{
							onError(obj);
						}
					});
				}
			}
		}

		public void FinishUpload(Version.FileDescriptor.Type fileDescriptorType, List<string> multipartEtags, Action<ApiFile> onSuccess, Action<string> onError)
		{
			if (CheckInitialized(onError))
			{
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(GetLatestVersionNumber(), fileDescriptorType);
				if (fileDescriptor == null)
				{
					if (onError != null)
					{
						onError("Version record doesn't exist");
					}
				}
				else
				{
					Dictionary<string, object> dictionary = null;
					if (multipartEtags != null)
					{
						dictionary = new Dictionary<string, object>();
						dictionary["etags"] = multipartEtags;
					}
					int latestVersionNumber = GetLatestVersionNumber();
					ApiModel.SendPutRequest("file/" + base.id + "/" + latestVersionNumber + "/" + fileDescriptorType.ToString() + "/finish", dictionary, delegate(Dictionary<string, object> obj)
					{
						ApiFile apiFile = ScriptableObject.CreateInstance<ApiFile>();
						apiFile.Init(obj);
						if (onSuccess != null)
						{
							onSuccess(apiFile);
						}
					}, delegate(string obj)
					{
						if (onError != null)
						{
							onError(obj);
						}
					});
				}
			}
		}

		public void GetUploadStatus(int versionNumber, Version.FileDescriptor.Type fileDescriptorType, Action<UploadStatus> successCallback, Action<string> errorCallback)
		{
			if (CheckInitialized(errorCallback))
			{
				Version.FileDescriptor fileDescriptor = GetFileDescriptor(versionNumber, fileDescriptorType);
				if (fileDescriptor == null)
				{
					if (errorCallback != null)
					{
						errorCallback("Version record doesn't exist");
					}
				}
				else
				{
					ApiModel.SendGetRequest("file/" + base.id + "/" + versionNumber + "/" + fileDescriptorType.ToString() + "/status", delegate(Dictionary<string, object> obj)
					{
						UploadStatus uploadStatus = new UploadStatus();
						uploadStatus.Init(obj);
						if (successCallback != null)
						{
							successCallback(uploadStatus);
						}
					}, delegate(string obj)
					{
						if (errorCallback != null)
						{
							errorCallback(obj);
						}
					});
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
