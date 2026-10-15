using System;
using System.Collections.Generic;

[Serializable]
public class BattleTurnResultResponse
{
	public eMPBattleTurnResult outcome;

	public bool battle_abandoned;

	public Dictionary<string, object>[] turn_datas;

	public Dictionary<string, object> player_datas;
}
