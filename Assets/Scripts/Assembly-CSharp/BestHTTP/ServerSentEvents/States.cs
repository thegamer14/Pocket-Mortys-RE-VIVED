namespace BestHTTP.ServerSentEvents
{
	public enum States
	{
		Initial = 0,
		Connecting = 1,
		Open = 2,
		Retrying = 3,
		Closing = 4,
		Closed = 5
	}
}
