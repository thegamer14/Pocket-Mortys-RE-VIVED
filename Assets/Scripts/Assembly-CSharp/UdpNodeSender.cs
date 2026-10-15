using System;
using System.Collections;
using UnityEngine;
using Utils;

public class UdpNodeSender : MonoBehaviour
{
	public const int kMaxPositionHistoryCount = 6;

	public UdpChannel udpChannel;

	private ushort _seqNum;

	private Coord _lastCoord;

	private byte[] _positionHistory = new byte[2];

	private int _positionHistoryCount;

	private float _lastSentTime;

	public void SendPosition(Coord coord, eDirection direction, bool snap = false)
	{
		DebugLog(string.Concat("SendPosition(", coord, ", ", direction));
		if (snap)
		{
			_positionHistory = new byte[2];
			_positionHistoryCount = 0;
		}
		else
		{
			UpdatePositionHistory(coord);
		}
		byte[] array = new byte[24];
		Buffer.BlockCopy(Utils.BitConverter.GetBytes(new Guid(App.net.session.sessionID)), 0, array, 0, 16);
		Buffer.SetByte(array, 16, (byte)_seqNum);
		Buffer.BlockCopy(Utils.BitConverter.GetBytes((ushort)coord.x), 0, array, 17, 2);
		Buffer.BlockCopy(Utils.BitConverter.GetBytes((ushort)coord.z), 0, array, 19, 2);
		Buffer.BlockCopy(_positionHistory, 0, array, 21, 2);
		Buffer.SetByte(array, 23, (byte)direction);
		if (NetworkTestDefs.NODE_BROADCAST_FAIL_CHANCE == 0f || UnityEngine.Random.value > NetworkTestDefs.NODE_BROADCAST_FAIL_CHANCE)
		{
			udpChannel.Send(array);
		}
		_seqNum = (ushort)((_seqNum < 255) ? ((uint)(_seqNum + 1)) : 0u);
	}

	private void UpdatePositionHistory(Coord coord)
	{
		DebugLog("UpdatePositionHistory(coord:" + coord.ToString());
		if (coord == _lastCoord)
		{
			DebugLog("coord == _lastCoord, returning");
			return;
		}
		if (_positionHistoryCount == 0)
		{
			DebugLog("_positionHistoryCount == 0, returning");
			_positionHistoryCount++;
			return;
		}
		BitArray bitArray = new BitArray(_positionHistory);
		for (int num = 9; num >= 0; num--)
		{
			bitArray[num + 2] = bitArray[num];
		}
		if (coord.x != _lastCoord.x)
		{
			bitArray[0] = coord.x > _lastCoord.x;
			bitArray[1] = false;
		}
		else
		{
			bitArray[0] = coord.z > _lastCoord.z;
			bitArray[1] = true;
		}
		BitArray bitArray2 = new BitArray(new int[1] { _positionHistoryCount });
		for (int i = 0; i < 4; i++)
		{
			bitArray[12 + i] = bitArray2[i];
		}
		bitArray.CopyTo(_positionHistory, 0);
		_positionHistoryCount = Mathf.Min(_positionHistoryCount + 1, 6);
		_lastCoord = coord;
		DebugPositionHistory();
	}

	private void DebugPositionHistory()
	{
		if (NetworkTestDefs.DEBUG_UDP_NODE_SENDER)
		{
			BitArray bitArray = new BitArray(_positionHistory);
			string text = string.Empty;
			for (int i = 0; i <= 10; i += 2)
			{
				string text2 = text;
				text = text2 + Mathf.FloorToInt((float)i / 2f) + " : " + ((!bitArray[i]) ? "-" : "+") + ((!bitArray[i + 1]) ? "x" : "y") + "\n";
			}
			DebugLog(string.Format("Node position history: {0}\n{1}\n_positionHistoryCount: {2}", bitArray.ToBitString(), text, _positionHistoryCount));
		}
	}

	private void DebugLog(string text)
	{
		if (!NetworkTestDefs.DEBUG_UDP_NODE_SENDER)
		{
		}
	}
}
