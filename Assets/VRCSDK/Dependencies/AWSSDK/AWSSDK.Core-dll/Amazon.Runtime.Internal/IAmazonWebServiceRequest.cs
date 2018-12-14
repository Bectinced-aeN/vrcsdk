using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal
{
	public interface IAmazonWebServiceRequest
	{
		EventHandler<StreamTransferProgressArgs> StreamUploadProgressCallback
		{
			get;
			set;
		}

		Dictionary<string, object> RequestState
		{
			get;
		}

		bool UseSigV4
		{
			get;
			set;
		}

		void AddBeforeRequestHandler(RequestEventHandler handler);

		void RemoveBeforeRequestHandler(RequestEventHandler handler);
	}
}
