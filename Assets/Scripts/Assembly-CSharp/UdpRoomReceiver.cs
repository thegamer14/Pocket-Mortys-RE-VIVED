using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UdpRoomReceiver : MonoBehaviour
{
	public delegate void NodePositionReceivedDelegate(string netId, MPNodePlacementData placementData, bool fromNewNode);

	public UdpChannel udpChannel;

	private Dictionary<string, ushort> _lastSeqNum = new Dictionary<string, ushort>();

	public event NodePositionReceivedDelegate NodePlacementReceivedEvent;

	private void Start()
	{
		AddEvents();
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	private void AddEvents()
	{
		udpChannel.PacketReceivedEvent += PacketReceivedHandler;
		App.net.room.UserRemovedEvent += UserRemovedHandler;
	}

	private void RemoveEvents()
	{
		udpChannel.PacketReceivedEvent -= PacketReceivedHandler;
		App.net.room.UserRemovedEvent -= UserRemovedHandler;
	}

	public void PacketReceivedHandler(byte[] byteArray)
	{
		if (byteArray.Length % 24 == 0)
		{
			int num = byteArray.Length / 24;
			for (int i = 0; i < num; i++)
			{
				PlacementReceivedHandler(byteArray.SubArray(i * 24, 24));
			}
		}
	}

	private void PlacementReceivedHandler(byte[] byteArray)
	{
		if (this.NodePlacementReceivedEvent == null)
		{
			return;
		}
		string text = null;
		ushort num = 0;
		int num2 = 0;
		int num3 = 0;
		byte[] array = null;
		eDirection eDirection2 = eDirection.NONE;
		try
		{
			text = Utils.BitConverter.ToGuid(byteArray, 0).ToString();
			num = byteArray[16];
			num2 = Utils.BitConverter.ToUInt16(byteArray, 17);
			num3 = Utils.BitConverter.ToUInt16(byteArray, 19);
			array = byteArray.SubArray(21, 2);
			eDirection2 = (eDirection)byteArray[23];
		}
		catch (Exception)
		{
		}
		if (!text.Equals(App.net.player.data.netId))
		{
			DebugLog(string.Format("Received id:{0}, seqNum:{1}, x:{2}, y:{3}, coordHistByte:{4}, dir:{5}", text, num, num2, num3, new BitArray(array).ToBitString(), eDirection2));
			bool fromNewNode = !_lastSeqNum.ContainsKey(text);
			if (_lastSeqNum.ContainsKey(text) && !IsSequenceMoreRecent(num, _lastSeqNum[text], 255))
			{
				DebugLog(string.Format("Ignoring old packet. id:{0} seqNum:{1} last:{2}", text, num, _lastSeqNum[text]));
				return;
			}
			_lastSeqNum[text] = num;
			Coord coord = new Coord(num2, num3);
			Coord[] positionHistoryCoords = GetPositionHistoryCoords(coord, array);
			this.NodePlacementReceivedEvent(text, new MPNodePlacementData(coord, eDirection2, positionHistoryCoords, null), fromNewNode);
		}
	}

	public void ClearLastSeqNumDict()
	{
		_lastSeqNum.Clear();
	}

	private void UserRemovedHandler(MPUserData userData)
	{
		_lastSeqNum.Remove(userData.netId);
	}

	private bool IsSequenceMoreRecent(int newSeq, int oldSeq, int max)
	{
		return (newSeq > oldSeq && newSeq - oldSeq <= max / 2) || (oldSeq > newSeq && oldSeq - newSeq > max / 2);
	}

	private Coord[] GetPositionHistoryCoords(Coord coord, byte[] historyBytes)
	{
		try
		{
			BitArray bitArray = new BitArray(historyBytes);
			DebugLog(bitArray.ToBitString() + " (received history bits)");
			BitArray bitArray2 = new BitArray(4);
			for (int i = 0; i < 4; i++)
			{
				bitArray2[bitArray2.Count - 1 - i] = bitArray[bitArray.Count - 1 - i];
			}
			int[] array = new int[1];
			bitArray2.CopyTo(array, 0);
			int num = array[0];
			if (num < 0 || num > 6)
			{
				return new Coord[0];
			}
			DebugLog("positionHistoryCount=" + num);
			Coord[] array2 = new Coord[num];
			for (int j = 0; j < array2.Length; j++)
			{
				int num2 = j * 2;
				array2[j] = ((j != 0) ? new Coord(array2[j - 1].x, array2[j - 1].z) : new Coord(coord.x, coord.z));
				if (bitArray[num2 + 1])
				{
					array2[j].z += ((!bitArray[num2]) ? 1 : (-1));
				}
				else
				{
					array2[j].x += ((!bitArray[num2]) ? 1 : (-1));
				}
			}
			DebugLog("returning histroy coords: " + new List<Coord>(array2).ToStringElements());
			return array2;
		}
		catch (Exception)
		{
			return new Coord[0];
		}
	}

	private void DebugLog(string text)
	{
		if (!NetworkTestDefs.DEBUG_UDP_ROOM_RECEIVER)
		{
		}
	}
}
