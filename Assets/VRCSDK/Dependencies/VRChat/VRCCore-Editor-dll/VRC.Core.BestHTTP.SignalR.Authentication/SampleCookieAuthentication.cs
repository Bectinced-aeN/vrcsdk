using System;
using VRC.Core.BestHTTP.Cookies;

namespace VRC.Core.BestHTTP.SignalR.Authentication
{
	internal sealed class SampleCookieAuthentication : IAuthenticationProvider
	{
		private HTTPRequest AuthRequest;

		private Cookie Cookie;

		public Uri AuthUri
		{
			get;
			private set;
		}

		public string UserName
		{
			get;
			private set;
		}

		public string Password
		{
			get;
			private set;
		}

		public string UserRoles
		{
			get;
			private set;
		}

		public bool IsPreAuthRequired
		{
			get;
			private set;
		}

		public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

		public event OnAuthenticationFailedDelegate OnAuthenticationFailed;

		public SampleCookieAuthentication(Uri authUri, string user, string passwd, string roles)
		{
			AuthUri = authUri;
			UserName = user;
			Password = passwd;
			UserRoles = roles;
			IsPreAuthRequired = true;
		}

		public void StartAuthentication()
		{
			AuthRequest = new HTTPRequest(AuthUri, HTTPMethods.Post, OnAuthRequestFinished);
			AuthRequest.AddField("userName", UserName);
			AuthRequest.AddField("Password", Password);
			AuthRequest.AddField("roles", UserRoles);
			AuthRequest.Send();
		}

		public void PrepareRequest(HTTPRequest request, RequestTypes type)
		{
			request.Cookies.Add(Cookie);
		}

		private void OnAuthRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			AuthRequest = null;
			string reason = string.Empty;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					Cookie = ((resp.Cookies == null) ? null : resp.Cookies.Find((Cookie c) => c.Name.Equals(".ASPXAUTH")));
					if (Cookie != null)
					{
						HTTPManager.Logger.Information("CookieAuthentication", "Auth. Cookie found!");
						if (this.OnAuthenticationSucceded != null)
						{
							this.OnAuthenticationSucceded(this);
						}
						return;
					}
					HTTPManager.Logger.Warning("CookieAuthentication", reason = "Auth. Cookie NOT found!");
				}
				else
				{
					HTTPManager.Logger.Warning("CookieAuthentication", reason = $"Request Finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}");
				}
				break;
			case HTTPRequestStates.Error:
				HTTPManager.Logger.Warning("CookieAuthentication", reason = "Request Finished with Error! " + ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace)));
				break;
			case HTTPRequestStates.Aborted:
				HTTPManager.Logger.Warning("CookieAuthentication", reason = "Request Aborted!");
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				HTTPManager.Logger.Error("CookieAuthentication", reason = "Connection Timed Out!");
				break;
			case HTTPRequestStates.TimedOut:
				HTTPManager.Logger.Error("CookieAuthentication", reason = "Processing the request Timed Out!");
				break;
			}
			if (this.OnAuthenticationFailed != null)
			{
				this.OnAuthenticationFailed(this, reason);
			}
		}
	}
}
