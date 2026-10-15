namespace BestHTTP
{
	internal enum RetryCauses
	{
		None = 0,
		Reconnect = 1,
		Authenticate = 2,
		ProxyAuthenticate = 3
	}
}
