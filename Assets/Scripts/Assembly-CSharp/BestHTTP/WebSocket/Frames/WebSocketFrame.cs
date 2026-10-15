using System;
using System.IO;
using BestHTTP.WebSocket.Extensions;

namespace BestHTTP.WebSocket.Frames
{
	public sealed class WebSocketFrame
	{
		public static readonly byte[] NoData = new byte[0];

		public WebSocketFrameTypes Type { get; private set; }

		public bool IsFinal { get; private set; }

		public byte Header { get; private set; }

		public byte[] Data { get; private set; }

		public bool UseExtensions { get; private set; }

		public WebSocketFrame(WebSocket webSocket, WebSocketFrameTypes type, byte[] data)
			: this(webSocket, type, data, true)
		{
		}

		public WebSocketFrame(WebSocket webSocket, WebSocketFrameTypes type, byte[] data, bool useExtensions)
			: this(webSocket, type, data, 0uL, (ulong)((data == null) ? 0 : data.Length), true, useExtensions)
		{
		}

		public WebSocketFrame(WebSocket webSocket, WebSocketFrameTypes type, byte[] data, bool isFinal, bool useExtensions)
			: this(webSocket, type, data, 0uL, (ulong)((data == null) ? 0 : data.Length), isFinal, useExtensions)
		{
		}

		public WebSocketFrame(WebSocket webSocket, WebSocketFrameTypes type, byte[] data, ulong pos, ulong length, bool isFinal, bool useExtensions)
		{
			Type = type;
			IsFinal = isFinal;
			UseExtensions = useExtensions;
			if (data != null)
			{
				Data = new byte[length];
				Array.Copy(data, (int)pos, Data, 0, (int)length);
			}
			else
			{
				data = NoData;
			}
			Header = (byte)((uint)(byte)(IsFinal ? 128 : 0) | (uint)Type);
			if (!UseExtensions || webSocket == null || webSocket.Extensions == null)
			{
				return;
			}
			for (int i = 0; i < webSocket.Extensions.Length; i++)
			{
				IExtension extension = webSocket.Extensions[i];
				if (extension != null)
				{
					Header |= extension.GetFrameHeader(this, Header);
					Data = extension.Encode(this);
				}
			}
		}

		public byte[] Get()
		{
			if (Data == null)
			{
				Data = NoData;
			}
			using (MemoryStream memoryStream = new MemoryStream(Data.Length + 9))
			{
				memoryStream.WriteByte(Header);
				if (Data.Length < 126)
				{
					memoryStream.WriteByte((byte)(0x80 | (byte)Data.Length));
				}
				else if (Data.Length < 65535)
				{
					memoryStream.WriteByte(254);
					byte[] bytes = BitConverter.GetBytes((ushort)Data.Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes, 0, bytes.Length);
					}
					memoryStream.Write(bytes, 0, bytes.Length);
				}
				else
				{
					memoryStream.WriteByte(byte.MaxValue);
					byte[] bytes2 = BitConverter.GetBytes((ulong)Data.Length);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes2, 0, bytes2.Length);
					}
					memoryStream.Write(bytes2, 0, bytes2.Length);
				}
				byte[] bytes3 = BitConverter.GetBytes(GetHashCode());
				memoryStream.Write(bytes3, 0, bytes3.Length);
				for (int i = 0; i < Data.Length; i++)
				{
					memoryStream.WriteByte((byte)(Data[i] ^ bytes3[i % 4]));
				}
				return memoryStream.ToArray();
			}
		}

		public WebSocketFrame[] Fragment(ushort maxFragmentSize)
		{
			if (Data == null)
			{
				return null;
			}
			if (Type != WebSocketFrameTypes.Binary && Type != WebSocketFrameTypes.Text)
			{
				return null;
			}
			if (Data.Length <= maxFragmentSize)
			{
				return null;
			}
			IsFinal = false;
			Header &= 127;
			int num = Data.Length / maxFragmentSize + ((Data.Length % maxFragmentSize == 0) ? (-1) : 0);
			WebSocketFrame[] array = new WebSocketFrame[num];
			ulong num3;
			for (ulong num2 = maxFragmentSize; num2 < (ulong)Data.Length; num2 += num3)
			{
				num3 = Math.Min(maxFragmentSize, (ulong)Data.Length - num2);
				array[array.Length - num--] = new WebSocketFrame(null, WebSocketFrameTypes.Continuation, Data, num2, num3, num2 + num3 >= (ulong)Data.Length, false);
			}
			byte[] array2 = new byte[maxFragmentSize];
			Array.Copy(Data, 0, array2, 0, maxFragmentSize);
			Data = array2;
			return array;
		}
	}
}
