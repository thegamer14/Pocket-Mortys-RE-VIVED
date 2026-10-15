using System;
using System.Collections.Generic;

[Serializable]
public class BattleStartResponse
{
	public string battle_id;

	public string battle_type;

	public Dictionary<string, object> player;

	public Dictionary<string, object> opponent;
}
