using System;

[Serializable]
public class ESStartResponse
{
	public string session_id;

	public string ping_url;

	public float ping_interval;

	public float keep_alive;
}
