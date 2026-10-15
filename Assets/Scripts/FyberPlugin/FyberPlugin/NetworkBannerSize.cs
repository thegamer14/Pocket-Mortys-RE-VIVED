namespace FyberPlugin
{
	public class NetworkBannerSize
	{
		public string network { get; internal set; }

		public BannerSize size { get; internal set; }

		public NetworkBannerSize(string network, BannerSize size)
		{
			this.size = size;
			this.network = network;
		}

		public string GetNetwork()
		{
			return network;
		}

		public BannerSize GetSize()
		{
			return size;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", network, size);
		}
	}
}
