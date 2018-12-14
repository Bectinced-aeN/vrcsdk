using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRC.Core.BestHTTP.Examples
{
	internal sealed class LargeFileDownloadSample : MonoBehaviour
	{
		private const string URL = "http://ipv4.download.thinkbroadband.com/100MB.zip";

		private HTTPRequest request;

		private string status = string.Empty;

		private float progress;

		private int fragmentSize = 4096;

		public LargeFileDownloadSample()
			: this()
		{
		}

		private void Awake()
		{
			if (PlayerPrefs.HasKey("DownloadLength"))
			{
				progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
			}
		}

		private void OnDestroy()
		{
			if (request != null && request.State < HTTPRequestStates.Finished)
			{
				request.OnProgress = null;
				request.Callback = null;
				request.Abort();
			}
		}

		private void OnGUI()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
			{
				GUILayout.Label("Request status: " + status, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Space(5f);
				GUILayout.Label(string.Format("Progress: {0:P2} of {1:N0}Mb", progress, PlayerPrefs.GetInt("DownloadLength") / 1048576), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.HorizontalSlider(progress, 0f, 1f, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Space(50f);
				if (request == null)
				{
					GUILayout.Label($"Desired Fragment Size: {(float)fragmentSize / 1024f:N} KBytes", (GUILayoutOption[])new GUILayoutOption[0]);
					fragmentSize = (int)GUILayout.HorizontalSlider((float)fragmentSize, 4096f, 1.048576E+07f, (GUILayoutOption[])new GUILayoutOption[0]);
					GUILayout.Space(5f);
					string text = (!PlayerPrefs.HasKey("DownloadProgress")) ? "Start Download" : "Continue Download";
					if (GUILayout.Button(text, (GUILayoutOption[])new GUILayoutOption[0]))
					{
						StreamLargeFileTest();
					}
				}
				else if (request.State == HTTPRequestStates.Processing && GUILayout.Button("Abort Download", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					request.Abort();
				}
			});
		}

		private void StreamLargeFileTest()
		{
			request = new HTTPRequest(new Uri("http://ipv4.download.thinkbroadband.com/100MB.zip"), delegate(HTTPRequest req, HTTPResponse resp)
			{
				switch (req.State)
				{
				case HTTPRequestStates.Processing:
					if (!PlayerPrefs.HasKey("DownloadLength"))
					{
						string firstHeaderValue = resp.GetFirstHeaderValue("content-length");
						if (!string.IsNullOrEmpty(firstHeaderValue))
						{
							PlayerPrefs.SetInt("DownloadLength", int.Parse(firstHeaderValue));
						}
					}
					ProcessFragments(resp.GetStreamedFragments());
					status = "Processing";
					break;
				case HTTPRequestStates.Finished:
					if (resp.IsSuccess)
					{
						ProcessFragments(resp.GetStreamedFragments());
						if (resp.IsStreamingFinished)
						{
							status = "Streaming finished!";
							PlayerPrefs.DeleteKey("DownloadProgress");
							PlayerPrefs.Save();
							request = null;
						}
						else
						{
							status = "Processing";
						}
					}
					else
					{
						status = $"Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}";
						Debug.LogWarning((object)status);
						request = null;
					}
					break;
				case HTTPRequestStates.Error:
					status = "Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
					Debug.LogError((object)status);
					request = null;
					break;
				case HTTPRequestStates.Aborted:
					status = "Request Aborted!";
					Debug.LogWarning((object)status);
					request = null;
					break;
				case HTTPRequestStates.ConnectionTimedOut:
					status = "Connection Timed Out!";
					Debug.LogError((object)status);
					request = null;
					break;
				case HTTPRequestStates.TimedOut:
					status = "Processing the request Timed Out!";
					Debug.LogError((object)status);
					request = null;
					break;
				}
			});
			if (PlayerPrefs.HasKey("DownloadProgress"))
			{
				request.SetRangeHeader(PlayerPrefs.GetInt("DownloadProgress"));
			}
			else
			{
				PlayerPrefs.SetInt("DownloadProgress", 0);
			}
			request.DisableCache = true;
			request.UseStreaming = true;
			request.StreamFragmentSize = fragmentSize;
			request.Send();
		}

		private void ProcessFragments(List<byte[]> fragments)
		{
			if (fragments != null && fragments.Count > 0)
			{
				for (int i = 0; i < fragments.Count; i++)
				{
					int num = PlayerPrefs.GetInt("DownloadProgress") + fragments[i].Length;
					PlayerPrefs.SetInt("DownloadProgress", num);
				}
				PlayerPrefs.Save();
				progress = (float)PlayerPrefs.GetInt("DownloadProgress") / (float)PlayerPrefs.GetInt("DownloadLength");
			}
		}
	}
}
