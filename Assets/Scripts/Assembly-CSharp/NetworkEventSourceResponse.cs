using System;

[Serializable]
public class NetworkEventSourceResponse
{
	public string jsonStr;

	public NetworkEventSourceResponse(string jsonStr = null)
	{
		this.jsonStr = jsonStr;
	}
}
