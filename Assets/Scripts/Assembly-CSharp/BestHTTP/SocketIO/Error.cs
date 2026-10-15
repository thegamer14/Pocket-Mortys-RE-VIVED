namespace BestHTTP.SocketIO
{
	public sealed class Error
	{
		public SocketIOErrors Code { get; private set; }

		public string Message { get; private set; }

		public Error(SocketIOErrors code, string msg)
		{
			Code = code;
			Message = msg;
		}

		public override string ToString()
		{
			return string.Format("Code: {0} Message: \"{1}\"", Code.ToString(), Message);
		}
	}
}
