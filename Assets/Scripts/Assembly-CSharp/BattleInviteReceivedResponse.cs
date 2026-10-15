using System;
using System.Collections.Generic;

[Serializable]
public class BattleInviteReceivedResponse
{
	public string battle_invite_id;

	public string battle_type;

	public float battle_invite_ttl;

	public Dictionary<string, object> from_player;

	public string token;
}
