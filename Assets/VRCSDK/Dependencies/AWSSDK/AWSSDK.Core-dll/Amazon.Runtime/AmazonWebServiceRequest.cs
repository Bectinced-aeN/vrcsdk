using Amazon.Runtime.Internal;
using System;
using System.Collections.Generic;

namespace Amazon.Runtime
{
	public abstract class AmazonWebServiceRequest : IAmazonWebServiceRequest
	{
		internal RequestEventHandler mBeforeRequestEvent;

		private Dictionary<string, object> requestState;

		EventHandler<StreamTransferProgressArgs> IAmazonWebServiceRequest.StreamUploadProgressCallback
		{
			get;
			set;
		}

		Dictionary<string, object> IAmazonWebServiceRequest.RequestState
		{
			get
			{
				if (requestState == null)
				{
					requestState = new Dictionary<string, object>();
				}
				return requestState;
			}
		}

		bool IAmazonWebServiceRequest.UseSigV4
		{
			get;
			set;
		}

		protected virtual bool Expect100Continue => false;

		protected virtual bool IncludeSHA256Header => true;

		internal event RequestEventHandler BeforeRequestEvent
		{
			add
			{
				lock (this)
				{
					mBeforeRequestEvent = (RequestEventHandler)Delegate.Combine(mBeforeRequestEvent, value);
				}
			}
			remove
			{
				lock (this)
				{
					mBeforeRequestEvent = (RequestEventHandler)Delegate.Remove(mBeforeRequestEvent, value);
				}
			}
		}

		void IAmazonWebServiceRequest.AddBeforeRequestHandler(RequestEventHandler handler)
		{
			BeforeRequestEvent += handler;
		}

		void IAmazonWebServiceRequest.RemoveBeforeRequestHandler(RequestEventHandler handler)
		{
			BeforeRequestEvent -= handler;
		}

		internal void FireBeforeRequestEvent(object sender, RequestEventArgs args)
		{
			if (mBeforeRequestEvent != null)
			{
				mBeforeRequestEvent(sender, args);
			}
		}

		internal bool GetExpect100Continue()
		{
			return Expect100Continue;
		}

		internal bool GetIncludeSHA256Header()
		{
			return IncludeSHA256Header;
		}
	}
}
