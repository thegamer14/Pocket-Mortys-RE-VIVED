using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SocketIO.Transports
{
	internal sealed class PollingTransport : ITransport
	{
		private enum PayloadTypes : byte
		{
			Text = 0,
			Binary = 1
		}

		private HTTPRequest LastRequest;

		private HTTPRequest PollRequest;

		private Packet PacketWithAttachment;

		private List<Packet> lonelyPacketList = new List<Packet>(1);

		public TransportTypes Type
		{
			get
			{
				return TransportTypes.Polling;
			}
		}

		public TransportStates State { get; private set; }

		public SocketManager Manager { get; private set; }

		public bool IsRequestInProgress
		{
			get
			{
				return LastRequest != null;
			}
		}

		public bool IsPollingInProgress
		{
			get
			{
				return PollRequest != null;
			}
		}

		public PollingTransport(SocketManager manager)
		{
			Manager = manager;
		}

		public void Open()
		{
			string text = "{0}?EIO={1}&transport=polling&t={2}-{3}{5}";
			if (Manager.Handshake != null)
			{
				text += "&sid={4}";
			}
			bool flag = !Manager.Options.QueryParamsOnlyForHandshake || (Manager.Options.QueryParamsOnlyForHandshake && Manager.Handshake == null);
			HTTPRequest hTTPRequest = new HTTPRequest(new Uri(string.Format(text, Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), (Manager.Handshake == null) ? string.Empty : Manager.Handshake.Sid, (!flag) ? string.Empty : Manager.Options.BuildQueryParams())), OnRequestFinished);
			hTTPRequest.DisableCache = true;
			hTTPRequest.DisableRetry = true;
			hTTPRequest.Send();
			State = TransportStates.Opening;
		}

		public void Close()
		{
			if (State != TransportStates.Closed)
			{
				State = TransportStates.Closed;
			}
		}

		public void Send(Packet packet)
		{
			try
			{
				lonelyPacketList.Add(packet);
				Send(lonelyPacketList);
			}
			finally
			{
				lonelyPacketList.Clear();
			}
		}

		public void Send(List<Packet> packets)
		{
			if (State != TransportStates.Open)
			{
				throw new Exception("Transport is not in Open state!");
			}
			if (IsRequestInProgress)
			{
				throw new Exception("Sending packets are still in progress!");
			}
			byte[] array = null;
			try
			{
				array = packets[0].EncodeBinary();
				for (int i = 1; i < packets.Count; i++)
				{
					byte[] array2 = packets[i].EncodeBinary();
					Array.Resize(ref array, array.Length + array2.Length);
					Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
				}
				packets.Clear();
			}
			catch (Exception ex)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				return;
			}
			LastRequest = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}", Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())), HTTPMethods.Post, OnRequestFinished);
			LastRequest.DisableCache = true;
			LastRequest.SetHeader("Content-Type", "application/octet-stream");
			LastRequest.RawData = array;
			LastRequest.Send();
		}

		private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			LastRequest = null;
			if (State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if ((int)HTTPManager.Logger.Level <= 0)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					if (req.MethodType != HTTPMethods.Post)
					{
						ParseResponse(resp);
					}
				}
				else
				{
					text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", resp.StatusCode, resp.Message, resp.DataAsText, req.CurrentUri);
				}
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)Manager).OnTransportError((ITransport)this, text);
			}
		}

		public void Poll()
		{
			if (PollRequest == null && State != TransportStates.Paused)
			{
				PollRequest = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}", Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())), HTTPMethods.Get, OnPollRequestFinished);
				PollRequest.DisableCache = true;
				PollRequest.DisableRetry = true;
				PollRequest.Send();
			}
		}

		private void OnPollRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			PollRequest = null;
			if (State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if ((int)HTTPManager.Logger.Level <= 0)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					ParseResponse(resp);
					break;
				}
				text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", resp.StatusCode, resp.Message, resp.DataAsText, req.CurrentUri);
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)Manager).OnTransportError((ITransport)this, text);
			}
		}

		private void OnPacket(Packet packet)
		{
			if (packet.AttachmentCount != 0 && !packet.HasAllAttachment)
			{
				PacketWithAttachment = packet;
				return;
			}
			if (packet.TransportEvent == TransportEventTypes.Open)
			{
				if (State != TransportStates.Opening)
				{
					HTTPManager.Logger.Warning("PollingTransport", "Received 'Open' packet while state is '" + State.ToString() + "'");
				}
				else
				{
					State = TransportStates.Open;
				}
			}
			((IManager)Manager).OnPacket(packet);
		}

		private void ParseResponse(HTTPResponse resp)
		{
			try
			{
				if (resp == null || resp.Data == null || resp.Data.Length < 1)
				{
					return;
				}
				int num;
				for (int i = 0; i < resp.Data.Length; i += num)
				{
					PayloadTypes payloadTypes = (PayloadTypes)resp.Data[i++];
					num = 0;
					for (byte b = resp.Data[i++]; b != byte.MaxValue; b = resp.Data[i++])
					{
						num = num * 10 + b;
					}
					Packet packet = null;
					switch (payloadTypes)
					{
					case PayloadTypes.Text:
						packet = new Packet(Encoding.UTF8.GetString(resp.Data, i, num));
						break;
					case PayloadTypes.Binary:
						if (PacketWithAttachment != null)
						{
							i++;
							num--;
							byte[] array = new byte[num];
							Array.Copy(resp.Data, i, array, 0, num);
							PacketWithAttachment.AddAttachmentFromServer(array, true);
							if (PacketWithAttachment.HasAllAttachment)
							{
								packet = PacketWithAttachment;
								PacketWithAttachment = null;
							}
						}
						break;
					}
					if (packet != null)
					{
						try
						{
							OnPacket(packet);
						}
						catch (Exception ex)
						{
							HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket", ex);
							((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex2.Message + " " + ex2.StackTrace);
				HTTPManager.Logger.Exception("PollingTransport", "ParseResponse", ex2);
			}
		}
	}
}
