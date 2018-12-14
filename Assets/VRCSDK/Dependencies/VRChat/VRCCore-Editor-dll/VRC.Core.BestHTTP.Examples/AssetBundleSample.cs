using System;
using System.Collections;
using UnityEngine;

namespace VRC.Core.BestHTTP.Examples
{
	internal sealed class AssetBundleSample : MonoBehaviour
	{
		private const string URL = "https://VRC.Core.BestHTTP.azurewebsites.net/Content/AssetBundle.html";

		private string status = "Waiting for user interaction";

		private AssetBundle cachedBundle;

		private Texture2D texture;

		private bool downloading;

		public AssetBundleSample()
			: this()
		{
		}

		private void OnGUI()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				GUILayout.Label("Status: " + status, (GUILayoutOption[])new GUILayoutOption[0]);
				if (texture != null)
				{
					GUILayout.Box(texture, (GUILayoutOption[])new GUILayoutOption[1]
					{
						GUILayout.MaxHeight(256f)
					});
				}
				if (!downloading && GUILayout.Button("Start Download", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					UnloadBundle();
					this.StartCoroutine(DownloadAssetBundle());
				}
			});
		}

		private void OnDestroy()
		{
			UnloadBundle();
		}

		private IEnumerator DownloadAssetBundle()
		{
			downloading = true;
			HTTPRequest request = new HTTPRequest(new Uri("https://VRC.Core.BestHTTP.azurewebsites.net/Content/AssetBundle.html")).Send();
			status = "Download started";
			while (request.State < HTTPRequestStates.Finished)
			{
				yield return (object)new WaitForSeconds(0.1f);
				status += ".";
			}
			switch (request.State)
			{
			case HTTPRequestStates.Finished:
				if (request.Response.IsSuccess)
				{
					status = $"AssetBundle downloaded! Loaded from local cache: {request.Response.IsFromCache.ToString()}";
					AssetBundleCreateRequest async = AssetBundle.LoadFromMemoryAsync(request.Response.Data);
					yield return (object)async;
					yield return (object)this.StartCoroutine(ProcessAssetBundle(async.get_assetBundle()));
				}
				else
				{
					status = $"Request finished Successfully, but the server sent an error. Status Code: {request.Response.StatusCode}-{request.Response.Message} Message: {request.Response.DataAsText}";
					Debug.LogWarning((object)status);
				}
				break;
			case HTTPRequestStates.Error:
				status = "Request Finished with Error! " + ((request.Exception == null) ? "No Exception" : (request.Exception.Message + "\n" + request.Exception.StackTrace));
				Debug.LogError((object)status);
				break;
			case HTTPRequestStates.Aborted:
				status = "Request Aborted!";
				Debug.LogWarning((object)status);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				status = "Connection Timed Out!";
				Debug.LogError((object)status);
				break;
			case HTTPRequestStates.TimedOut:
				status = "Processing the request Timed Out!";
				Debug.LogError((object)status);
				break;
			}
			downloading = false;
		}

		private IEnumerator ProcessAssetBundle(AssetBundle bundle)
		{
			if (!(bundle == null))
			{
				cachedBundle = bundle;
				AssetBundleRequest asyncAsset = cachedBundle.LoadAssetAsync("9443182_orig", typeof(Texture2D));
				yield return (object)asyncAsset;
				texture = (asyncAsset.get_asset() as Texture2D);
			}
		}

		private void UnloadBundle()
		{
			if (cachedBundle != null)
			{
				cachedBundle.Unload(true);
				cachedBundle = null;
			}
		}
	}
}
