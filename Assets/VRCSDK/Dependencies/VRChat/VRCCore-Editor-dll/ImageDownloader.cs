using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;

public class ImageDownloader : MonoBehaviour
{
	public delegate void OnImageDownloaded(Texture2D image);

	private const int MAX_CACHED_IMAGES = 200;

	private const int MIN_CACHED_IMAGES = 100;

	private const int kNumberOfFramesBetweenLoads = 3;

	private static List<string> cachedImageQueue;

	public static Dictionary<string, Texture2D> downloadedImages;

	public static Dictionary<string, List<OnImageDownloaded>> downloadingImages;

	private Queue<Action> mQueuedImageLoads = new Queue<Action>();

	private int mFramesUntilNextLoad = 3;

	public static ImageDownloader Instance
	{
		get;
		private set;
	}

	public ImageDownloader()
		: this()
	{
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError((object)"ImageDownloader: Other instance already detected");
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
			Object.DontDestroyOnLoad(this.get_gameObject());
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void Update()
	{
		ProcessLoadQueue();
	}

	public static void TrimCache(int size)
	{
		if (cachedImageQueue != null)
		{
			if (size == 0)
			{
				downloadedImages.Clear();
				cachedImageQueue.Clear();
			}
			else
			{
				int num = cachedImageQueue.Count - size;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						downloadedImages.Remove(cachedImageQueue[i]);
					}
					cachedImageQueue.RemoveRange(0, num);
				}
			}
		}
	}

	private static void EncacheTexture(string cacheRef, Texture2D tex)
	{
		if (cachedImageQueue.Contains(cacheRef))
		{
			cachedImageQueue.Remove(cacheRef);
			cachedImageQueue.Add(cacheRef);
		}
		else
		{
			downloadedImages[cacheRef] = tex;
			cachedImageQueue.Add(cacheRef);
			if (cachedImageQueue.Count > 200)
			{
				TrimCache(100);
				Resources.UnloadUnusedAssets();
			}
		}
	}

	public static void DownloadImage(string imageUrl, int imageSize, OnImageDownloaded onImageDownload, string fallbackImageUrl = "", bool isRetry = false)
	{
		ImageDownloader imageDownloaderQueue = CreateInstanceIfNeeded();
		if (!string.IsNullOrEmpty(imageUrl))
		{
			if (downloadingImages == null)
			{
				downloadingImages = new Dictionary<string, List<OnImageDownloaded>>();
			}
			if (downloadedImages == null)
			{
				downloadedImages = new Dictionary<string, Texture2D>();
			}
			if (cachedImageQueue == null)
			{
				cachedImageQueue = new List<string>();
			}
			string cacheRef = imageUrl + ":" + imageSize;
			if (downloadedImages.ContainsKey(cacheRef))
			{
				onImageDownload(downloadedImages[cacheRef]);
			}
			else if (downloadingImages.ContainsKey(cacheRef))
			{
				downloadingImages[cacheRef].Add(onImageDownload);
			}
			else
			{
				try
				{
					string url = imageUrl;
					bool flag = false;
					if (imageSize != 0 && imageUrl.StartsWith("https://api.vrchat.cloud/api/1/file/"))
					{
						string[] array = imageUrl.Remove(0, "https://api.vrchat.cloud/api/1/file/".Length).Split('/');
						if (array.Length == 2 || (array.Length == 3 && array[2] == "file"))
						{
							string text = array[0];
							string text2 = array[1];
							url = "https://api.vrchat.cloud/api/1/image/" + text + "/" + text2 + "/" + imageSize.ToString();
							flag = true;
						}
					}
					downloadingImages[cacheRef] = new List<OnImageDownloaded>();
					HTTPManager.SendRequest(url, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, disableCache: false, delegate(HTTPRequest request, HTTPResponse response)
					{
						Action loadImage2 = delegate
						{
							if (response != null)
							{
								Texture2D dataAsTexture2D = response.DataAsTexture2D;
								EncacheTexture(cacheRef, dataAsTexture2D);
								onImageDownload(dataAsTexture2D);
								foreach (OnImageDownloaded item in downloadingImages[cacheRef])
								{
									item(response.DataAsTexture2D);
								}
								downloadingImages.Remove(cacheRef);
							}
							else
							{
								Debug.LogError((object)("No response received: " + ((request.Exception == null) ? "No Exception" : (request.Exception.Message + "\n" + request.Exception.StackTrace))));
								DownloadFallbackOrUseErrorImage(fallbackImageUrl, onImageDownload);
							}
						};
						if (response != null && response.Data == null)
						{
							downloadingImages.Remove(cacheRef);
							HTTPCacheService.DeleteEntity(request.CurrentUri);
							if (!isRetry)
							{
								DownloadImage(imageUrl, imageSize, onImageDownload, fallbackImageUrl, isRetry: true);
							}
						}
						else if (imageDownloaderQueue != null)
						{
							imageDownloaderQueue.QueueImageLoad(loadImage2);
						}
					});
				}
				catch (Exception ex)
				{
					Exception e;
					Exception ex2 = e = ex;
					Action loadImage = delegate
					{
						Debug.Log((object)("Could not download image " + imageUrl + " - " + e.Message));
						DownloadFallbackOrUseErrorImage(fallbackImageUrl, onImageDownload);
					};
					imageDownloaderQueue.QueueImageLoad(loadImage);
				}
			}
		}
	}

	private static void DownloadFallbackOrUseErrorImage(string fallbackImageUrl, OnImageDownloaded onImageDownload)
	{
		if (string.IsNullOrEmpty(fallbackImageUrl))
		{
			Texture2D image = Resources.Load("no_image", typeof(Texture2D)) as Texture2D;
			onImageDownload(image);
		}
		else
		{
			DownloadImage(fallbackImageUrl, 0, onImageDownload, string.Empty);
		}
	}

	private static ImageDownloader CreateInstanceIfNeeded()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (Instance == null)
		{
			GameObject val = new GameObject("ImageDownloader");
			val.set_tag("VRCGlobalRoot");
			return val.AddComponent<ImageDownloader>();
		}
		return Instance;
	}

	private void QueueImageLoad(Action loadImage)
	{
		mQueuedImageLoads.Enqueue(loadImage);
		ResetWaitForNextLoad();
	}

	private void ResetWaitForNextLoad()
	{
		mFramesUntilNextLoad = 3;
	}

	private void ProcessLoadQueue()
	{
		if (mFramesUntilNextLoad > 0)
		{
			mFramesUntilNextLoad--;
		}
		else if (mQueuedImageLoads.Count != 0)
		{
			Action action = mQueuedImageLoads.Dequeue();
			action();
			ResetWaitForNextLoad();
		}
	}
}
