using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BestHTTP.Extensions;
using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket
{
	public sealed class WebSocketResponse : HTTPResponse, IHeartbeat, IProtocol
	{
		public Action<WebSocketResponse, string> OnText;

		public Action<WebSocketResponse, byte[]> OnBinary;

		public Action<WebSocketResponse, WebSocketFrameReader> OnIncompleteFrame;

		public Action<WebSocketResponse, ushort, string> OnClosed;

		private int _bufferedAmount;

		private List<WebSocketFrameReader> IncompleteFrames = new List<WebSocketFrameReader>();

		private List<WebSocketFrameReader> CompletedFrames = new List<WebSocketFrameReader>();

		private WebSocketFrameReader CloseFrame;

		private object FrameLock = new object();

		private object SendLock = new object();

		private List<WebSocketFrame> unsentFrames = new List<WebSocketFrame>();

		private AutoResetEvent newFrameSignal = new AutoResetEvent(false);

		private volatile bool sendThreadCreated;

		private volatile bool closeSent;

		private volatile bool closed;

		private DateTime lastPing = DateTime.MinValue;

		public WebSocket WebSocket { get; internal set; }

		public bool IsClosed
		{
			get
			{
				return closed;
			}
		}

		public TimeSpan PingFrequnecy { get; private set; }

		public ushort MaxFragmentSize { get; private set; }

		public int BufferedAmount
		{
			get
			{
				return _bufferedAmount;
			}
		}

		internal WebSocketResponse(HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
			: base(request, stream, isStreamed, isFromCache)
		{
			base.IsClosedManually = true;
			closed = false;
			MaxFragmentSize = 32767;
		}

		internal void StartReceive()
		{
			if (base.IsUpgraded)
			{
				ThreadPool.QueueUserWorkItem(ReceiveThreadFunc);
			}
		}

		internal void CloseStream()
		{
			ConnectionBase connectionWith = HTTPManager.GetConnectionWith(baseRequest);
			if (connectionWith != null)
			{
				connectionWith.Abort(HTTPConnectionStates.Closed);
			}
		}

		public void Send(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message must not be null!");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			Send(new WebSocketFrame(WebSocket, WebSocketFrameTypes.Text, bytes));
		}

		public void Send(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data must not be null!");
			}
			WebSocketFrame webSocketFrame = new WebSocketFrame(WebSocket, WebSocketFrameTypes.Binary, data);
			if (webSocketFrame.Data != null && webSocketFrame.Data.Length > MaxFragmentSize)
			{
				WebSocketFrame[] array = webSocketFrame.Fragment(MaxFragmentSize);
				lock (SendLock)
				{
					Send(webSocketFrame);
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							Send(array[i]);
						}
					}
					return;
				}
			}
			Send(webSocketFrame);
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
			WebSocketFrame webSocketFrame = new WebSocketFrame(WebSocket, WebSocketFrameTypes.Binary, data, offset, count, true, true);
			if (webSocketFrame.Data != null && webSocketFrame.Data.Length > MaxFragmentSize)
			{
				WebSocketFrame[] array = webSocketFrame.Fragment(MaxFragmentSize);
				lock (SendLock)
				{
					Send(webSocketFrame);
					if (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							Send(array[i]);
						}
					}
					return;
				}
			}
			Send(webSocketFrame);
		}

		public void Send(WebSocketFrame frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame is null!");
			}
			if (closed)
			{
				return;
			}
			lock (SendLock)
			{
				unsentFrames.Add(frame);
				Interlocked.Add(ref _bufferedAmount, frame.Data.Length);
				if (!sendThreadCreated)
				{
					ThreadPool.QueueUserWorkItem(SendThreadFunc);
					sendThreadCreated = true;
				}
			}
			HTTPManager.Logger.Information("WebSocketResponse", "Signaling SendThread!");
			newFrameSignal.Set();
		}

		public void Close()
		{
			Close(1000, "Bye!");
		}

		public void Close(ushort code, string msg)
		{
			if (!closed)
			{
				Send(new WebSocketFrame(WebSocket, WebSocketFrameTypes.ConnectionClose, WebSocket.EncodeCloseData(code, msg)));
			}
		}

		public void StartPinging(int frequency)
		{
			if (frequency < 100)
			{
				throw new ArgumentException("frequency must be at least 100 milliseconds!");
			}
			PingFrequnecy = TimeSpan.FromMilliseconds(frequency);
			HTTPManager.Heartbeats.Subscribe(this);
		}

		private void SendThreadFunc(object param)
		{
			List<WebSocketFrame> list = new List<WebSocketFrame>();
			try
			{
				while (!closed && !closeSent)
				{
					HTTPManager.Logger.Information("WebSocketResponse", "SendThread - Waiting...");
					newFrameSignal.WaitOne();
					try
					{
						lock (SendLock)
						{
							for (int num = unsentFrames.Count - 1; num >= 0; num--)
							{
								list.Add(unsentFrames[num]);
							}
							unsentFrames.Clear();
						}
						if ((int)HTTPManager.Logger.Level <= 1)
						{
							HTTPManager.Logger.Information("WebSocketResponse", "SendThread - Wait is over, " + list.Count + " new frames!");
						}
						while (list.Count > 0)
						{
							WebSocketFrame webSocketFrame = list[list.Count - 1];
							list.RemoveAt(list.Count - 1);
							if (!closeSent)
							{
								byte[] array = webSocketFrame.Get();
								Stream.Write(array, 0, array.Length);
								Stream.Flush();
								if (webSocketFrame.Type == WebSocketFrameTypes.ConnectionClose)
								{
									closeSent = true;
								}
							}
							Interlocked.Add(ref _bufferedAmount, -webSocketFrame.Data.Length);
						}
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
				sendThreadCreated = false;
				HTTPManager.Logger.Information("WebSocketResponse", "SendThread - Closed!");
			}
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
							continue;
						}
						if (!webSocketFrameReader.IsFinal)
						{
							if (OnIncompleteFrame == null)
							{
								IncompleteFrames.Add(webSocketFrameReader);
								continue;
							}
							lock (FrameLock)
							{
								CompletedFrames.Add(webSocketFrameReader);
							}
							continue;
						}
						switch (webSocketFrameReader.Type)
						{
						case WebSocketFrameTypes.Continuation:
							if (OnIncompleteFrame == null)
							{
								webSocketFrameReader.Assemble(IncompleteFrames);
								IncompleteFrames.Clear();
								goto case WebSocketFrameTypes.Text;
							}
							lock (FrameLock)
							{
								CompletedFrames.Add(webSocketFrameReader);
							}
							break;
						case WebSocketFrameTypes.Text:
						case WebSocketFrameTypes.Binary:
							webSocketFrameReader.DecodeWithExtensions(WebSocket);
							lock (FrameLock)
							{
								CompletedFrames.Add(webSocketFrameReader);
							}
							break;
						case WebSocketFrameTypes.Ping:
							if (!closeSent && !closed)
							{
								Send(new WebSocketFrame(WebSocket, WebSocketFrameTypes.Pong, webSocketFrameReader.Data));
							}
							break;
						case WebSocketFrameTypes.ConnectionClose:
							CloseFrame = webSocketFrameReader;
							if (!closeSent)
							{
								Send(new WebSocketFrame(WebSocket, WebSocketFrameTypes.ConnectionClose, null));
							}
							closed = closeSent;
							break;
						}
					}
					catch (ThreadAbortException)
					{
						IncompleteFrames.Clear();
						baseRequest.State = HTTPRequestStates.Aborted;
						closed = true;
						newFrameSignal.Set();
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
						newFrameSignal.Set();
					}
				}
			}
			finally
			{
				HTTPManager.Heartbeats.Unsubscribe(this);
			}
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
						WebSocketFrameTypes type = webSocketFrameReader.Type;
						if (type == WebSocketFrameTypes.Continuation)
						{
							goto IL_0041;
						}
						if (type != WebSocketFrameTypes.Text)
						{
							if (type == WebSocketFrameTypes.Binary)
							{
								if (!webSocketFrameReader.IsFinal)
								{
									goto IL_0041;
								}
								if (OnBinary != null)
								{
									OnBinary(this, webSocketFrameReader.Data);
								}
							}
						}
						else
						{
							if (!webSocketFrameReader.IsFinal)
							{
								goto IL_0041;
							}
							if (OnText != null)
							{
								OnText(this, webSocketFrameReader.DataAsText);
							}
						}
						goto end_IL_0021;
						IL_0041:
						if (OnIncompleteFrame != null)
						{
							OnIncompleteFrame(this, webSocketFrameReader);
						}
						end_IL_0021:;
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("WebSocketResponse", "HandleEvents", ex);
					}
				}
				CompletedFrames.Clear();
			}
			if (!IsClosed || OnClosed == null || baseRequest.State != HTTPRequestStates.Processing)
			{
				return;
			}
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

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			if (lastPing == DateTime.MinValue)
			{
				lastPing = DateTime.UtcNow;
			}
			else if (DateTime.UtcNow - lastPing >= PingFrequnecy)
			{
				try
				{
					Send(new WebSocketFrame(WebSocket, WebSocketFrameTypes.Ping, Encoding.UTF8.GetBytes(string.Empty)));
				}
				catch
				{
					closed = true;
					HTTPManager.Heartbeats.Unsubscribe(this);
				}
				lastPing = DateTime.UtcNow;
			}
		}
	}
}
