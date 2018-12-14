namespace VRC.Core.BestHTTP.SignalR.Authentication
{
	internal class HeaderAuthenticator : IAuthenticationProvider
	{
		public string User
		{
			get;
			private set;
		}

		public string Roles
		{
			get;
			private set;
		}

		public bool IsPreAuthRequired => false;

		public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

		public event OnAuthenticationFailedDelegate OnAuthenticationFailed;

		public HeaderAuthenticator(string user, string roles)
		{
			User = user;
			Roles = roles;
		}

		public void StartAuthentication()
		{
		}

		public void PrepareRequest(HTTPRequest request, RequestTypes type)
		{
			request.SetHeader("username", User);
			request.SetHeader("roles", Roles);
		}
	}
}
