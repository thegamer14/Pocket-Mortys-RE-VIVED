using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class TournamentSaveData : SaveDataBase
{
	private const string _kEventDatasKey = "EventDatasKey";

	private const string _kTimestampKey = "TimestampKey";

	private const string _kDirtyRickActiveKey = "DirtyRickActiveKey";

	private const string _kOwnedTicketsKey = "OwnedTicketsKey";

	private const string _kHotFixMissingTicketsResolvedKey = "HotFixMissingTicketsResolvedKey";

	public Dictionary<string, Dictionary<string, TournamentEventData>> eventDatas;

	public double timestamp;

	public bool dirtyRickActive;

	public List<string> ownedTickets;

	public bool hotFixMissingTicketsResolved;

	protected override int _targetVersionID
	{
		get
		{
			return 1;
		}
	}

	public TournamentSaveData()
	{
	}

	public TournamentSaveData(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected override void SetDefaults()
	{
		base.SetDefaults();
		eventDatas = new Dictionary<string, Dictionary<string, TournamentEventData>>();
		timestamp = -1.0;
		dirtyRickActive = false;
		ownedTickets = new List<string>();
		hotFixMissingTicketsResolved = false;
	}

	protected override void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		base.SerializeValues(info, context);
		info.AddValue("EventDatasKey", eventDatas, typeof(Dictionary<string, Dictionary<string, TournamentEventData>>));
		info.AddValue("TimestampKey", timestamp, typeof(double));
		info.AddValue("DirtyRickActiveKey", dirtyRickActive, typeof(bool));
		info.AddValue("OwnedTicketsKey", ownedTickets, typeof(List<string>));
		info.AddValue("HotFixMissingTicketsResolvedKey", hotFixMissingTicketsResolved, typeof(bool));
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
			case "EventDatasKey":
				eventDatas = (Dictionary<string, Dictionary<string, TournamentEventData>>)current.Value;
				break;
			case "TimestampKey":
				timestamp = (double)current.Value;
				break;
			case "DirtyRickActiveKey":
				dirtyRickActive = (bool)current.Value;
				break;
			case "OwnedTicketsKey":
				ownedTickets = (List<string>)current.Value;
				break;
			case "HotFixMissingTicketsResolvedKey":
				hotFixMissingTicketsResolved = (bool)current.Value;
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
