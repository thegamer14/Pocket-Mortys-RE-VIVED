namespace BestHTTP.SignalR.Messages
{
	public sealed class KeepAliveMessage : IServerMessage
	{
		MessageTypes IServerMessage.Type
		{
			get
			{
				return MessageTypes.KeepAlive;
			}
		}

		void IServerMessage.Parse(object data)
		{
		}
	}
}
