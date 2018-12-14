using System;

namespace VRC.Core.BestHTTP
{
	internal delegate bool OnBeforeRedirectionDelegate(HTTPRequest originalRequest, HTTPResponse response, Uri redirectUri);
}
