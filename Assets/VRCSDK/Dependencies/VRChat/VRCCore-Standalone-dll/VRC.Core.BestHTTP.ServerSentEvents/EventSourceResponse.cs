using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace VRC.Core.BestHTTP.ServerSentEvents
{
	internal sealed class EventSourceResponse : HTTPResponse, IProtocol
	{
		public Action<EventSourceResponse, Message> OnMessage;

		public Action<EventSourceResponse> OnClosed;

		private object FrameLock = new object();

		private byte[] LineBuffer = new byte[1024];

		private int LineBufferPos;

		private Message CurrentMessage;

		private List<Message> CompletedMessages = new List<Message>();

		public bool IsClosed
		{
			get;
			private set;
		}

		internal EventSourceResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
			: base(request, stream, isStreamed, isFromCache)
		{
			base.IsClosedManually = true;
		}

		void IProtocol.HandleEvents()
		{
			lock (FrameLock)
			{
				if (CompletedMessages.Count > 0)
				{
					if (OnMessage != null)
					{
						for (int i = 0; i < CompletedMessages.Count; i++)
						{
							try
							{
								OnMessage(this, CompletedMessages[i]);
							}
							catch (Exception ex)
							{
								HTTPManager.Logger.Exception("EventSourceMessage", "HandleEvents - OnMessage", ex);
							}
						}
					}
					CompletedMessages.Clear();
				}
			}
			if (IsClosed)
			{
				CompletedMessages.Clear();
				if (OnClosed != null)
				{
					try
					{
						OnClosed(this);
					}
					catch (Exception ex2)
					{
						HTTPManager.Logger.Exception("EventSourceMessage", "HandleEvents - OnClosed", ex2);
					}
					finally
					{
						OnClosed = null;
					}
				}
			}
		}

		internal override bool Receive(int forceReadRawContentLength = -1, bool readPayloadData = true)
		{
			bool flag = base.Receive(forceReadRawContentLength, readPayloadData: false);
			base.IsUpgraded = (flag && base.StatusCode == 200 && HasHeaderWithValue("content-type", "text/event-stream"));
			if (!base.IsUpgraded)
			{
				ReadPayload(forceReadRawContentLength);
			}
			return flag;
		}

		internal void StartReceive()
		{
			if (base.IsUpgraded)
			{
				new Thread(ReceiveThreadFunc).Start();
			}
		}

		private void ReceiveThreadFunc(object param)
		{
			try
			{
				if (HasHeaderWithValue("transfer-encoding", "chunked"))
				{
					ReadChunked(Stream);
				}
				else
				{
					ReadRaw(Stream, -1);
				}
			}
			catch (ThreadAbortException)
			{
				baseRequest.State = HTTPRequestStates.Aborted;
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
			}
			finally
			{
				IsClosed = true;
			}
		}

		private new void ReadChunked(Stream stream)
		{
			int num = ReadChunkLength(stream);
			byte[] array = new byte[num];
			while (num != 0)
			{
				if (array.Length < num)
				{
					Array.Resize(ref array, num);
				}
				int num2 = 0;
				do
				{
					int num3 = stream.Read(array, num2, num - num2);
					if (num3 == 0)
					{
						throw new Exception("The remote server closed the connection unexpectedly!");
					}
					num2 += num3;
				}
				while (num2 < num);
				FeedData(array, num2);
				HTTPResponse.ReadTo(stream, 10);
				num = ReadChunkLength(stream);
			}
			ReadHeaders(stream);
		}

		private new void ReadRaw(Stream stream, int contentLength)
		{
			byte[] array = new byte[1024];
			int num;
			do
			{
				num = stream.Read(array, 0, array.Length);
				FeedData(array, num);
			}
			while (num > 0);
		}

		public void FeedData(byte[] buffer, int count)
		{
			if (count == -1)
			{
				count = buffer.Length;
			}
			if (count != 0)
			{
				int num = 0;
				int num2;
				do
				{
					num2 = -1;
					int num3 = 1;
					for (int i = num; i < count; i++)
					{
						if (num2 != -1)
						{
							break;
						}
						if (buffer[i] == 13)
						{
							if (i + 1 < count && buffer[i + 1] == 10)
							{
								num3 = 2;
							}
							num2 = i;
						}
						else if (buffer[i] == 10)
						{
							num2 = i;
						}
					}
					int num4 = (num2 != -1) ? num2 : count;
					if (LineBuffer.Length < LineBufferPos + (num4 - num))
					{
						Array.Resize(ref LineBuffer, LineBufferPos + (num4 - num));
					}
					Array.Copy(buffer, num, LineBuffer, LineBufferPos, num4 - num);
					LineBufferPos += num4 - num;
					if (num2 == -1)
					{
						break;
					}
					ParseLine(LineBuffer, LineBufferPos);
					LineBufferPos = 0;
					num = num2 + num3;
				}
				while (num2 != -1 && num < count);
			}
		}

		private void ParseLine(byte[] buffer, int count)
		{
			if (count == 0)
			{
				if (CurrentMessage != null)
				{
					lock (FrameLock)
					{
						CompletedMessages.Add(CurrentMessage);
					}
					CurrentMessage = null;
				}
			}
			else if (buffer[0] != 58)
			{
				int num = -1;
				for (int i = 0; i < count; i++)
				{
					if (num != -1)
					{
						break;
					}
					if (buffer[i] == 58)
					{
						num = i;
					}
				}
				string @string;
				string text;
				if (num != -1)
				{
					@string = Encoding.UTF8.GetString(buffer, 0, num);
					if (num + 1 < count && buffer[num + 1] == 32)
					{
						num++;
					}
					num++;
					if (num >= count)
					{
						return;
					}
					text = Encoding.UTF8.GetString(buffer, num, count - num);
				}
				else
				{
					@string = Encoding.UTF8.GetString(buffer, 0, count);
					text = string.Empty;
				}
				if (CurrentMessage == null)
				{
					CurrentMessage = new Message();
				}
				switch (@string)
				{
				case "id":
					CurrentMessage.Id = text;
					break;
				case "event":
					CurrentMessage.Event = text;
					break;
				case "data":
					if (CurrentMessage.Data != null)
					{
						CurrentMessage.Data += Environment.NewLine;
					}
					CurrentMessage.Data += text;
					break;
				case "retry":
					if (int.TryParse(text, out int result))
					{
						CurrentMessage.Retry = TimeSpan.FromMilliseconds((double)result);
					}
					break;
				}
			}
		}
	}
}
