using System.Net;

namespace Amazon.Runtime.Internal.Transform
{
	public interface IWebResponseData
	{
		long ContentLength
		{
			get;
		}

		string ContentType
		{
			get;
		}

		HttpStatusCode StatusCode
		{
			get;
		}

		bool IsSuccessStatusCode
		{
			get;
		}

		IHttpResponseBody ResponseBody
		{
			get;
		}

		string[] GetHeaderNames();

		bool IsHeaderPresent(string headerName);

		string GetHeaderValue(string headerName);
	}
}
