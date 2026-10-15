using System;
using System.Collections.Generic;

[Serializable]
public class PlayerUseItemResponse
{
	public string owned_morty_id;

	public string item_id;

	public bool is_success;

	public int effect_value;

	public Dictionary<string, object>[] owned_morties;

	public Dictionary<string, object>[] owned_items;

	public Dictionary<string, object>[] attacks_to_learn;
}
