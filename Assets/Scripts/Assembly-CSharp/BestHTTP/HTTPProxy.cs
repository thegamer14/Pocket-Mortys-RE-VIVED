using System;
using BestHTTP.Authentication;

namespace BestHTTP
{
	public sealed class HTTPProxy
	{
		public Uri Address { get; set; }

		public Credentials Credentials { get; set; }

		public bool IsTransparent { get; set; }

		public bool SendWholeUri { get; set; }

		public bool NonTransparentForHTTPS { get; set; }

		public HTTPProxy(Uri address)
			: this(address, null, false)
		{
		}

		public HTTPProxy(Uri address, Credentials credentials)
			: this(address, credentials, false)
		{
		}

		public HTTPProxy(Uri address, Credentials credentials, bool isTransparent)
			: this(address, credentials, isTransparent, true)
		{
		}

		public HTTPProxy(Uri address, Credentials credentials, bool isTransparent, bool sendWholeUri)
			: this(address, credentials, isTransparent, true, true)
		{
		}

		public HTTPProxy(Uri address, Credentials credentials, bool isTransparent, bool sendWholeUri, bool nonTransparentForHTTPS)
		{
			Address = address;
			Credentials = credentials;
			IsTransparent = isTransparent;
			SendWholeUri = sendWholeUri;
			NonTransparentForHTTPS = nonTransparentForHTTPS;
		}
	}
}
