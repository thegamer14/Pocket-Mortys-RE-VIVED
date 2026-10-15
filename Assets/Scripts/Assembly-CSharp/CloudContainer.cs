using System;
using System.Runtime.Serialization;

[Serializable]
public class CloudContainer : ISerializable
{
	private const string _kLastOpenedTimestamp = "LastOpenedTimestampKey";

	private const string _kLastModifiedTimestamp = "LastModifiedTimestampKey";

	private const string _kSettingsKey = "SettingsKey";

	private const string _kPlayerKey = "PlayerKey";

	private const string _kSegmentedWorldKey = "SegmentedWorldKey";

	private const string _kTournamentKey = "TournamentKey";

	private const string _kMultiplayerKey = "MultiplayerKey";

	public eCloudContainerType containerType;

	public long lastOpenedTimestamp;

	public long lastModifiedTimestamp;

	public SettingsSaveData settings;

	public PlayerSaveData player;

	public SegmentedWorldSaveData segmentedWorld;

	public TournamentSaveData tournament;

	public MultiplayerSaveData multiplayer;

	public CloudContainer()
	{
		lastOpenedTimestamp = DateTime.UtcNow.Ticks;
		lastModifiedTimestamp = DateTime.UtcNow.Ticks;
		settings = null;
		player = null;
		segmentedWorld = null;
		tournament = null;
		multiplayer = null;
	}

	public CloudContainer(SerializationInfo info, StreamingContext context)
	{
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "LastOpenedTimestampKey":
				lastOpenedTimestamp = (long)current.Value;
				break;
			case "LastModifiedTimestampKey":
				lastModifiedTimestamp = (long)current.Value;
				break;
			case "SettingsKey":
				settings = (SettingsSaveData)current.Value;
				break;
			case "PlayerKey":
				player = (PlayerSaveData)current.Value;
				break;
			case "SegmentedWorldKey":
				segmentedWorld = (SegmentedWorldSaveData)current.Value;
				break;
			case "TournamentKey":
				tournament = (TournamentSaveData)current.Value;
				break;
			case "MultiplayerKey":
				multiplayer = (MultiplayerSaveData)current.Value;
				break;
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("LastOpenedTimestampKey", lastOpenedTimestamp, typeof(long));
		info.AddValue("LastModifiedTimestampKey", lastModifiedTimestamp, typeof(long));
		info.AddValue("SettingsKey", settings, typeof(SettingsSaveData));
		info.AddValue("PlayerKey", player, typeof(PlayerSaveData));
		info.AddValue("SegmentedWorldKey", segmentedWorld, typeof(SegmentedWorldSaveData));
		info.AddValue("TournamentKey", tournament, typeof(TournamentSaveData));
		info.AddValue("MultiplayerKey", multiplayer, typeof(MultiplayerSaveData));
	}
}
