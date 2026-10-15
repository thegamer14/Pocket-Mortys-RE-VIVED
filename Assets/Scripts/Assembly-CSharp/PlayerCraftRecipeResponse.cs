using System;
using System.Collections.Generic;

[Serializable]
public class PlayerCraftRecipeResponse
{
	public Dictionary<string, object>[] owned_items;

	public Dictionary<string, object> result;
}
