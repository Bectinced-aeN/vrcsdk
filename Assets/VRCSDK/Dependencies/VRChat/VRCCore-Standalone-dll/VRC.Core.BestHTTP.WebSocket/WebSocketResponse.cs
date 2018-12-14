using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using VRC.Core.BestHTTP.Extensions;
using VRC.Core.BestHTTP.WebSocket.Frames;

namespace VRC.Core.BestHTTP.WebSocket
{
	internal sealed class WebSocketResponse : HTTPResponse, IProtocol, IHeartbeat
	{
		public Action<WebSocketResponse, string> OnText;

		public Action<WebSocketResponse, byte[]> OnBinary;

		public Action<WebSocketResponse, WebSocketFrameReader> OnIncompleteFrame;

		public Action<WebSocketResponse, ushort, string> OnClosed;

		private List<WebSocketFrameReader> IncompleteFrames = new List<WebSocketFrameReader>();

		private List<WebSocketFrameReader> CompletedFrames = new List<WebSocketFrameReader>();

		private WebSocketFrameReader CloseFrame;

		private object FrameLock = new object();

		private object SendLock = new object();

		private bool closeSent;

		private bool closed;

		private DateTime lastPing = DateTime.MinValue;

		public bool IsClosed => closed;

		public TimeSpan PingFrequnecy
		{
			get;
			private set;
		}

		public ushort MaxFragmentSize
		{
			get;
			private set;
		}

		internal WebSocketResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
			: base(request, stream, isStreamed, isFromCache)
		{
			base.IsClosedManually = true;
			closed = false;
			MaxFragmentSize = 32767;
		}

		void IProtocol.HandleEvents()
		{
			lock (FrameLock)
			{
				for (int i = 0; i < CompletedFrames.Count; i++)
				{
					WebSocketFrameReader webSocketFrameReader = CompletedFrames[i];
					try
					{
						switch (webSocketFrameReader.Type)
						{
						case WebSocketFrameTypes.Continuation:
							if (OnIncompleteFrame != null)
							{
								OnIncompleteFrame(this, webSocketFrameReader);
							}
							break;
						case WebSocketFrameTypes.Text:
							if (webSocketFrameReader.IsFinal)
							{
								if (OnText != null)
								{
									OnText(this, Encoding.UTF8.GetString(webSocketFrameReader.Data, 0, webSocketFrameReader.Data.Length));
								}
								break;
							}
							goto case WebSocketFrameTypes.Continuation;
						case WebSocketFrameTypes.Binary:
							if (webSocketFrameReader.IsFinal)
							{
								if (OnBinary != null)
								{
									OnBinary(this, webSocketFrameReader.Data);
								}
								break;
							}
							goto case WebSocketFrameTypes.Continuation;
						}
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("WebSocketResponse", "HandleEvents", ex);
					}
				}
				CompletedFrames.Clear();
			}
			if (IsClosed && OnClosed != null && baseRequest.State == HTTPRequestStates.Processing)
			{
				try
				{
					ushort arg = 0;
					string arg2 = string.Empty;
					if (CloseFrame != null && CloseFrame.Data != null && CloseFrame.Data.Length >= 2)
					{
						if (BitConverter.IsLittleEndian)
						{
							Array.Reverse(CloseFrame.Data, 0, 2);
						}
						arg = BitConverter.ToUInt16(CloseFrame.Data, 0);
						if (CloseFrame.Data.Length > 2)
						{
							arg2 = Encoding.UTF8.GetString(CloseFrame.Data, 2, CloseFrame.Data.Length - 2);
						}
					}
					OnClosed(this, arg, arg2);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("WebSocketResponse", "HandleEvents - OnClosed", ex2);
				}
			}
		}

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			if (lastPing == DateTime.MinValue)
			{
				lastPing = DateTime.UtcNow;
			}
			else if (DateTime.UtcNow - lastPing >= PingFrequnecy)
			{
				Send(new WebSocketPing(string.Empty));
				lastPing = DateTime.UtcNow;
			}
		}

		internal void StartReceive()
		{
			if (base.IsUpgraded)
			{
				new Thread(ReceiveThreadFunc).Start();
			}
		}

		public void Send(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message must not be null!");
			}
			Send(new WebSocketTextFrame(message));
		}

		public void Send(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data must not be null!");
			}
			if ((long)data.Length > (long)(int)MaxFragmentSize)
			{
				lock (SendLock)
				{
					Send(new WebSocketBinaryFrame(data, 0uL, MaxFragmentSize, isFinal: false));
					ulong num2;
					for (ulong num = MaxFragmentSize; num < (ulong)data.Length; num += num2)
					{
						num2 = Math.Min(MaxFragmentSize, (ulong)((long)data.Length - (long)num));
						Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= (ulong)data.Length));
					}
				}
			}
			else
			{
				Send(new WebSocketBinaryFrame(data));
			}
		}

		public void Send(byte[] data, ulong offset, ulong count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data must not be null!");
			}
			if (offset + count > (ulong)data.Length)
			{
				throw new ArgumentOutOfRangeException("offset + count >= data.Length");
			}
			if ((long)count > (long)(int)MaxFragmentSize)
			{
				lock (SendLock)
				{
					Send(new WebSocketBinaryFrame(data, offset, MaxFragmentSize, isFinal: false));
					ulong num2;
					for (ulong num = offset + MaxFragmentSize; num < count; num += num2)
					{
						num2 = Math.Min(MaxFragmentSize, count - num);
						Send(new WebSocketContinuationFrame(data, num, num2, num + num2 >= count));
					}
				}
			}
			else
			{
				Send(new WebSocketBinaryFrame(data, offset, count, isFinal: true));
			}
		}

		public void Send(IWebSocketFrameWriter frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame is null!");
			}
			if (!closed)
			{
				byte[] array = frame.Get();
				lock (SendLock)
				{
					Stream.Write(array, 0, array.Length);
					Stream.Flush();
				}
				if (frame.Type == WebSocketFrameTypes.ConnectionClose)
				{
					closeSent = true;
				}
			}
		}

		public void Close()
		{
			Close(1000, "Bye!");
		}

		public void Close(ushort code, string msg)
		{
			if (!closed)
			{
				Send(new WebSocketClose(code, msg));
			}
		}

		public void StartPinging(int frequency)
		{
			if (frequency < 100)
			{
				throw new ArgumentException("frequency must be at least 100 millisec!");
			}
			PingFrequnecy = TimeSpan.FromMilliseconds((double)frequency);
			HTTPManager.Heartbeats.Subscribe(this);
		}

		private void ReceiveThreadFunc(object param)
		{
			try
			{
				while (!closed)
				{
					try
					{
						WebSocketFrameReader webSocketFrameReader = new WebSocketFrameReader();
						webSocketFrameReader.Read(Stream);
						if (webSocketFrameReader.HasMask)
						{
							Close(1002, "Protocol Error: masked frame received from server!");
						}
						else if (webSocketFrameReader.IsFinal)
						{
							switch (webSocketFrameReader.Type)
							{
							case (WebSocketFrameTypes)3:
							case (WebSocketFrameTypes)4:
							case (WebSocketFrameTypes)5:
							case (WebSocketFrameTypes)6:
							case (WebSocketFrameTypes)7:
								break;
							case WebSocketFrameTypes.Continuation:
								if (OnIncompleteFrame != null)
								{
									lock (FrameLock)
									{
										CompletedFrames.Add(webSocketFrameReader);
									}
									break;
								}
								webSocketFrameReader.Assemble(IncompleteFrames);
								IncompleteFrames.Clear();
								goto case WebSocketFrameTypes.Text;
							case WebSocketFrameTypes.Text:
							case WebSocketFrameTypes.Binary:
								lock (FrameLock)
								{
									CompletedFrames.Add(webSocketFrameReader);
								}
								break;
							case WebSocketFrameTypes.Ping:
								if (!closeSent && !closed)
								{
									Send(new WebSocketPong(webSocketFrameReader));
								}
								break;
							case WebSocketFrameTypes.ConnectionClose:
								CloseFrame = webSocketFrameReader;
								if (!closeSent)
								{
									Send(new WebSocketClose());
								}
								closed = closeSent;
								break;
							}
						}
						else if (OnIncompleteFrame == null)
						{
							IncompleteFrames.Add(webSocketFrameReader);
						}
						else
						{
							lock (FrameLock)
							{
								CompletedFrames.Add(webSocketFrameReader);
							}
						}
					}
					catch (ThreadAbortException)
					{
						IncompleteFrames.Clear();
						baseRequest.State = HTTPRequestStates.Aborted;
						closed = true;
					}
					catch (Exception exception)
					{
						if (HTTPUpdateDelegator.IsCreated)
						{
							baseRequest.Exception = exception;
							baseRequest.State = HTTPRequestStates.Error;
						}
						else
						{
							baseRequest.State = HTTPRequestStates.Aborted;
						}
						closed = true;
					}
				}
			}
			finally
			{
				HTTPManager.Heartbeats.Unsubscribe(this);
			}
		}
	}
}
