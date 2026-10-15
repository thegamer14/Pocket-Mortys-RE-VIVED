namespace FyberPlugin
{
	public class BannerSize
	{
		public const int FULL_WIDTH = -1;

		public const int AUTO_HEIGHT = -2;

		public int height { get; internal set; }

		public int width { get; internal set; }

		public BannerSizeOrientation orientation { get; internal set; }

		public BannerSize(int width, int height)
		{
			orientation = BannerSizeOrientation.None;
			this.height = height;
			this.width = width;
		}

		public int GetWidth()
		{
			return width;
		}

		public int GetHeight()
		{
			return height;
		}

		public BannerSizeOrientation GetOrientation()
		{
			return orientation;
		}

		public BannerSize WithOrientation(BannerSizeOrientation orientation)
		{
			this.orientation = orientation;
			return this;
		}

		public override string ToString()
		{
			string text = ((width == -1) ? "full_width " : ((width != -2) ? width.ToString() : "smart_width "));
			string text2 = ((height == -1) ? " full_height" : ((height != -2) ? height.ToString() : " smart_height"));
			return "(" + text + "x" + text2 + ")" + ((orientation == BannerSizeOrientation.None) ? string.Empty : (" - Orientation:" + orientation));
		}
	}
}
