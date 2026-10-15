using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SegmentedWorldSaveData : SaveDataBase
{
	private const string _kWorldIDKey = "WorldIDKey";

	private const string _kNodeSetIDKey = "NodeSetIDKey";

	private const string _kEntranceKey = "EntranceIDKey";

	private const string _kSegmentsKey = "SegmentKey";

	private const string _kGymLeaderKey = "GymLeaderKey";

	private const string _kPlayerPositionKey = "PlayerPositionKey";

	private const string _kPlayerDirectionKey = "PlayerDirectionKey";

	private const string _kPlayerMortyPositionsKey = "PlayerMortyPositionsKey";

	private const string _kPlayerMortyDirectionsKey = "PlayerMortyDirectionsKey";

	public string worldID;

	public string nodeSetID;

	public string entranceID;

	public List<SegmentSaveData> segments;

	public TrainerData gymLeader;

	public SerializableVector3 playerPosition;

	public eDirection playerDirection;

	public SerializableVector3[] playerMortyPositions;

	public eDirection[] playerMortyDirections;

	protected override int _targetVersionID
	{
		get
		{
			return 1;
		}
	}

	public SegmentedWorldSaveData()
	{
	}

	public SegmentedWorldSaveData(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected override void SetDefaults()
	{
		base.SetDefaults();
		worldID = null;
		nodeSetID = null;
		entranceID = null;
		segments = null;
		gymLeader = null;
		playerPosition = Vector3.zero;
		playerDirection = eDirection.NONE;
		playerMortyPositions = null;
		playerMortyDirections = null;
	}

	protected override void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		base.SerializeValues(info, context);
		info.AddValue("WorldIDKey", worldID, typeof(string));
		info.AddValue("NodeSetIDKey", nodeSetID, typeof(string));
		info.AddValue("EntranceIDKey", entranceID, typeof(string));
		info.AddValue("SegmentKey", segments, typeof(List<SegmentSaveData>));
		info.AddValue("GymLeaderKey", gymLeader, typeof(TrainerData));
		info.AddValue("PlayerPositionKey", playerPosition, typeof(SerializableVector3));
		info.AddValue("PlayerDirectionKey", playerDirection, typeof(eDirection));
		info.AddValue("PlayerMortyPositionsKey", playerMortyPositions, typeof(SerializableVector3[]));
		info.AddValue("PlayerMortyDirectionsKey", playerMortyDirections, typeof(eDirection[]));
	}

	protected override void DeserializeValues(SerializationInfo info, StreamingContext context)
	{
		base.DeserializeValues(info, context);
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "WorldIDKey":
				worldID = (string)current.Value;
				break;
			case "NodeSetIDKey":
				nodeSetID = (string)current.Value;
				break;
			case "EntranceIDKey":
				entranceID = (string)current.Value;
				break;
			case "SegmentKey":
				segments = (List<SegmentSaveData>)current.Value;
				break;
			case "GymLeaderKey":
				gymLeader = (TrainerData)current.Value;
				break;
			case "PlayerPositionKey":
				playerPosition = (SerializableVector3)current.Value;
				break;
			case "PlayerDirectionKey":
				playerDirection = (eDirection)current.Value;
				break;
			case "PlayerMortyPositionsKey":
				playerMortyPositions = (SerializableVector3[])current.Value;
				break;
			case "PlayerMortyDirectionsKey":
				playerMortyDirections = (eDirection[])current.Value;
				break;
			}
		}
	}

	protected override void UpdateToVersion2(SerializationInfo info)
	{
	}

	protected override void UpdateToVersion3(SerializationInfo info)
	{
	}

	protected override void UpdateToVersion4(SerializationInfo info)
	{
	}
}
