using System;
using System.Collections.Generic;

[Serializable]
public class PlayerHealActiveMortyDeckResponse
{
	public int streak;

	public Dictionary<string, object>[] owned_morties;
}
