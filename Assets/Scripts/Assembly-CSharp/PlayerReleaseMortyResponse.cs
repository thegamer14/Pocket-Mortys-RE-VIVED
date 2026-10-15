using System;
using System.Collections.Generic;

[Serializable]
public class PlayerReleaseMortyResponse
{
	public int active_deck_id;

	public Dictionary<string, object>[] decks;
}
