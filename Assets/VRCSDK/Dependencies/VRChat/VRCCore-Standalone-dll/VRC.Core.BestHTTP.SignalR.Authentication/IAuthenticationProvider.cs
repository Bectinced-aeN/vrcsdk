namespace VRC.Core.BestHTTP.SignalR.Authentication
{
	internal interface IAuthenticationProvider
	{
		bool IsPreAuthRequired
		{
			get;
		}

		event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;

		event OnAuthenticationFailedDelegate OnAuthenticationFailed;

		void StartAuthentication();

		void PrepareRequest(HTTPRequest request, RequestTypes type);
	}
}
