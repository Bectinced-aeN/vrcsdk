using System;
using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SocketIO.Events
{
	internal sealed class EventDescriptor
	{
		private SocketIOCallback[] CallbackArray;

		public List<SocketIOCallback> Callbacks
		{
			get;
			private set;
		}

		public bool OnlyOnce
		{
			get;
			private set;
		}

		public bool AutoDecodePayload
		{
			get;
			private set;
		}

		public EventDescriptor(bool onlyOnce, bool autoDecodePayload, SocketIOCallback callback)
		{
			OnlyOnce = onlyOnce;
			AutoDecodePayload = autoDecodePayload;
			Callbacks = new List<SocketIOCallback>(1);
			if (callback != null)
			{
				Callbacks.Add(callback);
			}
		}

		public void Call(Socket socket, Packet packet, params object[] args)
		{
			if (CallbackArray == null || CallbackArray.Length < Callbacks.Count)
			{
				Array.Resize(ref CallbackArray, Callbacks.Count);
			}
			Callbacks.CopyTo(CallbackArray);
			for (int i = 0; i < CallbackArray.Length; i++)
			{
				try
				{
					CallbackArray[i](socket, packet, args);
				}
				catch (Exception ex)
				{
					((ISocket)socket).EmitError(SocketIOErrors.User, ex.Message + " " + ex.StackTrace);
					HTTPManager.Logger.Exception("EventDescriptor", "Call", ex);
				}
				if (OnlyOnce)
				{
					Callbacks.Remove(CallbackArray[i]);
				}
				CallbackArray[i] = null;
			}
		}
	}
}
