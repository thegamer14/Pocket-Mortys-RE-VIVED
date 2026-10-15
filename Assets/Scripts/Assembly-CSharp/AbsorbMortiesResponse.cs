using System;
using System.Collections.Generic;

[Serializable]
public class AbsorbMortiesResponse
{
	public List<string> removed;

	public Dictionary<string, object> modified;

	public Dictionary<string, object> created;

	public Dictionary<string, object>[] decks;
}
