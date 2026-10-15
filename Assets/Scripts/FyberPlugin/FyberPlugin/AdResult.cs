namespace FyberPlugin
{
	public sealed class AdResult
	{
		public AdFormat AdFormat { get; internal set; }

		public string Message { get; internal set; }

		public AdStatus Status { get; internal set; }
	}
}
