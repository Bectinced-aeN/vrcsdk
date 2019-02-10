using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core.BestHTTP;
using VRC.Core.BestHTTP.Caching;

public class ImageDownloader : MonoBehaviour
{
	public delegate void OnImageDownloaded(Texture2D image);

	private const int MAX_CACHED_IMAGES = 100;

	private const int kNumberOfFramesBetweenLoads = 3;

	private static Queue<string> cachedImageQueue;

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

	public static void DownloadImage(string imageUrl, OnImageDownloaded onImageDownload, string fallbackImageUrl = "", bool isRetry = false)
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
				cachedImageQueue = new Queue<string>();
			}
			if (downloadedImages.ContainsKey(imageUrl))
			{
				onImageDownload(downloadedImages[imageUrl]);
			}
			else if (downloadingImages.ContainsKey(imageUrl))
			{
				downloadingImages[imageUrl].Add(onImageDownload);
			}
			else
			{
				try
				{
					downloadingImages[imageUrl] = new List<OnImageDownloaded>();
					HTTPManager.SendRequest(imageUrl, HTTPMethods.Get, HTTPManager.KeepAliveDefaultValue, disableCache: false, delegate(HTTPRequest request, HTTPResponse response)
					{
						Action loadImage2 = delegate
						{
							if (response != null)
							{
								if (cachedImageQueue.Count > 100)
								{
									string key = cachedImageQueue.Dequeue();
									downloadedImages.Remove(key);
								}
								downloadedImages[imageUrl] = response.DataAsTexture2D;
								cachedImageQueue.Enqueue(imageUrl);
								onImageDownload(response.DataAsTexture2D);
								foreach (OnImageDownloaded item in downloadingImages[imageUrl])
								{
									item(response.DataAsTexture2D);
								}
								downloadingImages.Remove(imageUrl);
							}
							else
							{
								Debug.LogError((object)("No response received: " + ((request.Exception == null) ? "No Exception" : (request.Exception.Message + "\n" + request.Exception.StackTrace))));
								DownloadFallbackOrUseErrorImage(fallbackImageUrl, onImageDownload);
							}
						};
						if (response != null && response.Data == null)
						{
							downloadingImages.Remove(imageUrl);
							HTTPCacheService.DeleteEntity(request.CurrentUri);
							if (!isRetry)
							{
								DownloadImage(imageUrl, onImageDownload, fallbackImageUrl, isRetry: true);
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
			DownloadImage(fallbackImageUrl, onImageDownload, string.Empty);
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
