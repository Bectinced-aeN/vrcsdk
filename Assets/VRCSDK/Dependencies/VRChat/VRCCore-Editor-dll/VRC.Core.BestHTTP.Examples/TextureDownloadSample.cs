using System;
using UnityEngine;

namespace VRC.Core.BestHTTP.Examples
{
	internal sealed class TextureDownloadSample : MonoBehaviour
	{
		private const string BaseURL = "https://VRC.Core.BestHTTP.azurewebsites.net/Content/";

		private string[] Images = new string[9]
		{
			"One.png",
			"Two.png",
			"Three.png",
			"Four.png",
			"Five.png",
			"Six.png",
			"Seven.png",
			"Eight.png",
			"Nine.png"
		};

		private Texture2D[] Textures = (Texture2D[])new Texture2D[9];

		private bool allDownloadedFromLocalCache;

		private int finishedCount;

		private Vector2 scrollPos;

		public TextureDownloadSample()
			: this()
		{
		}

		private void Awake()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			HTTPManager.MaxConnectionPerServer = 1;
			for (int i = 0; i < Images.Length; i++)
			{
				Textures[i] = new Texture2D(100, 150);
			}
		}

		private void OnDestroy()
		{
			HTTPManager.MaxConnectionPerServer = 4;
		}

		private void OnGUI()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				scrollPos = GUILayout.BeginScrollView(scrollPos, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.SelectionGrid(0, (Texture[])Textures, 3, (GUILayoutOption[])new GUILayoutOption[0]);
				if (finishedCount == Images.Length && allDownloadedFromLocalCache)
				{
					GUIHelper.DrawCenteredText("All images loaded from the local cache!");
				}
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Max Connection/Server: ", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.Width(150f)
				});
				GUILayout.Label(HTTPManager.MaxConnectionPerServer.ToString(), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.Width(20f)
				});
				HTTPManager.MaxConnectionPerServer = (byte)GUILayout.HorizontalSlider((float)(int)HTTPManager.MaxConnectionPerServer, 1f, 10f, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Start Download", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					DownloadImages();
				}
				GUILayout.EndScrollView();
			});
		}

		private void DownloadImages()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			allDownloadedFromLocalCache = true;
			finishedCount = 0;
			for (int i = 0; i < Images.Length; i++)
			{
				Textures[i] = new Texture2D(100, 150);
				HTTPRequest hTTPRequest = new HTTPRequest(new Uri("https://VRC.Core.BestHTTP.azurewebsites.net/Content/" + Images[i]), ImageDownloaded);
				hTTPRequest.UseAlternateSSL = true;
				hTTPRequest.Tag = Textures[i];
				hTTPRequest.Send();
			}
		}

		private void ImageDownloaded(HTTPRequest req, HTTPResponse resp)
		{
			finishedCount++;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					Texture2D val = req.Tag as Texture2D;
					val.LoadImage(resp.Data);
					allDownloadedFromLocalCache = (allDownloadedFromLocalCache && resp.IsFromCache);
				}
				else
				{
					Debug.LogWarning((object)$"Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
				}
				break;
			case HTTPRequestStates.Error:
				Debug.LogError((object)("Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace))));
				break;
			case HTTPRequestStates.Aborted:
				Debug.LogWarning((object)"Request Aborted!");
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				Debug.LogError((object)"Connection Timed Out!");
				break;
			case HTTPRequestStates.TimedOut:
				Debug.LogError((object)"Processing the request Timed Out!");
				break;
			}
		}
	}
}
