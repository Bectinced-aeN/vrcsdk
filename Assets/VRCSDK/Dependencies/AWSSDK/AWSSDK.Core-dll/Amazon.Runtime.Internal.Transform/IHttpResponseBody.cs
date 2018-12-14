using System;
using System.IO;

namespace Amazon.Runtime.Internal.Transform
{
	public interface IHttpResponseBody : IDisposable
	{
		Stream OpenResponse();
	}
}
