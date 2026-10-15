using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpChannel : MonoBehaviour
{
	public delegate void PacketReceivedDelegate(byte[] byteArray);

	public bool sendEnabled = true;

	public int readInterval = 20;

	public string channelName = "Unnamed";

	private IPEndPoint _receiveEndPoint;

	private UdpClient _udpClient;

	private Thread _udpReadThread;

	private Queue _receivedQueue = new Queue();

	public eUdpChannelState state { get; private set; }

	public event PacketReceivedDelegate PacketReceivedEvent;

	public void Open(string host, int receivePort)
	{
		DebugLog(string.Format("Open(receivePort:{0}, host:{1}, channelName:{2})", receivePort, host, channelName));
		if (state == eUdpChannelState.OPEN)
		{
			Close();
		}
		_receiveEndPoint = new IPEndPoint(IPAddress.Any, receivePort);
		if (_udpClient == null)
		{
			_udpClient = new UdpClient();
		}
		else
		{
			DebugLogWarning("Tring to create _udpClient when it already exists.");
		}
		if (_udpReadThread == null)
		{
			_udpReadThread = new Thread(UDPRead);
			_udpReadThread.Name = channelName;
			_udpReadThread.Start();
		}
		else
		{
			DebugLogWarning("Tring to create _udpReadThread when it already exists.");
		}
		try
		{
			_udpClient.Connect(host, receivePort);
		}
		catch (Exception ex)
		{
			DebugLogWarning("Error connecting to _udpClient: " + ex.Message);
		}
		state = eUdpChannelState.OPEN;
	}

	public void Close()
	{
		DebugLog("Close()");
		if (_udpReadThread != null)
		{
			_udpReadThread.Abort();
			_udpReadThread = null;
		}
		if (_udpClient != null)
		{
			_udpClient.Close();
			_udpClient = null;
		}
		_receivedQueue.Clear();
		state = eUdpChannelState.CLOSED;
	}

	private void OnDestroy()
	{
		Close();
	}

	public void Send(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			DebugLog("Send(string) failed - string is null or empty");
		}
		else
		{
			Send(Encoding.UTF8.GetBytes(str));
		}
	}

	public void Send(byte[] byteArray)
	{
		if (!sendEnabled)
		{
			return;
		}
		if (state != eUdpChannelState.OPEN)
		{
			DebugLog("Send(byte[]) failed, channel is not open.");
			return;
		}
		if (byteArray == null || byteArray.Length == 0)
		{
			DebugLog("Send(byte[]) failed, byteArray was null or empty.");
			return;
		}
		try
		{
			_udpClient.Send(byteArray, byteArray.Length);
		}
		catch (Exception ex)
		{
			DebugLog("Send() Exception: " + ex.ToString());
		}
	}

	private void UDPRead()
	{
		DebugLog("Started UDPRead thread");
		while (true)
		{
			try
			{
				while (_udpClient.Available > 0)
				{
					byte[] obj = _udpClient.Receive(ref _receiveEndPoint);
					_receivedQueue.Enqueue(obj);
				}
			}
			catch (Exception ex)
			{
				DebugLog("Receive Exception " + ex.ToString());
			}
			Thread.Sleep(readInterval);
		}
	}

	private void Update()
	{
		while (_receivedQueue.Count > 0)
		{
			byte[] byteArray = _receivedQueue.Dequeue() as byte[];
			if (this.PacketReceivedEvent != null)
			{
				this.PacketReceivedEvent(byteArray);
			}
		}
	}

	private void DebugLog(string text)
	{
		if (NetworkTestDefs.DEBUG_UDP)
		{
		}
	}

	private void DebugLogWarning(string text)
	{
	}
}
