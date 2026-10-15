using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BestHTTP.Authentication;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.TcpClient.General;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;

namespace BestHTTP
{
	internal sealed class HTTPConnection : ConnectionBase
	{
		private TcpClient Client;

		private Stream Stream;

		private KeepAliveHeader KeepAlive;

		public override bool IsRemovable
		{
			get
			{
				if (base.IsRemovable)
				{
					return true;
				}
				if (base.IsFree && KeepAlive != null && DateTime.UtcNow - LastProcessTime >= KeepAlive.TimeOut)
				{
					return true;
				}
				return false;
			}
		}

		internal HTTPConnection(string serverAddress)
			: base(serverAddress)
		{
		}

		protected override void ThreadFunc(object param)
		{
			bool flag = false;
			bool flag2 = false;
			RetryCauses retryCauses = RetryCauses.None;
			try
			{
				if (!base.HasProxy && base.CurrentRequest.HasProxy)
				{
					base.Proxy = base.CurrentRequest.Proxy;
				}
				if (TryLoadAllFromCache())
				{
					return;
				}
				if (Client != null && !Client.IsConnected())
				{
					Close();
				}
				do
				{
					if (retryCauses == RetryCauses.Reconnect)
					{
						Close();
						Thread.Sleep(100);
					}
					base.LastProcessedUri = base.CurrentRequest.CurrentUri;
					retryCauses = RetryCauses.None;
					Connect();
					if (base.State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					if (!base.CurrentRequest.DisableCache)
					{
						HTTPCacheService.SetHeaders(base.CurrentRequest);
					}
					bool flag3 = false;
					try
					{
						base.CurrentRequest.SendOutTo(Stream);
						flag3 = true;
					}
					catch (Exception ex)
					{
						Close();
						if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
						{
							throw new Exception("AbortRequested");
						}
						if (flag || base.CurrentRequest.DisableRetry)
						{
							throw ex;
						}
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (!flag3)
					{
						continue;
					}
					bool flag4 = Receive();
					if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					if (!flag4 && !flag && !base.CurrentRequest.DisableRetry)
					{
						flag = true;
						retryCauses = RetryCauses.Reconnect;
					}
					if (base.CurrentRequest.Response == null)
					{
						continue;
					}
					if (base.CurrentRequest.IsCookiesEnabled)
					{
						CookieJar.Set(base.CurrentRequest.Response);
					}
					switch (base.CurrentRequest.Response.StatusCode)
					{
					case 401:
					{
						string text2 = DigestStore.FindBest(base.CurrentRequest.Response.GetHeaderValues("www-authenticate"));
						if (!string.IsNullOrEmpty(text2))
						{
							Digest orCreate2 = DigestStore.GetOrCreate(base.CurrentRequest.CurrentUri);
							orCreate2.ParseChallange(text2);
							if (base.CurrentRequest.Credentials != null && orCreate2.IsUriProtected(base.CurrentRequest.CurrentUri) && (!base.CurrentRequest.HasHeader("Authorization") || orCreate2.Stale))
							{
								retryCauses = RetryCauses.Authenticate;
							}
						}
						break;
					}
					case 407:
					{
						if (!base.CurrentRequest.HasProxy)
						{
							break;
						}
						string text = DigestStore.FindBest(base.CurrentRequest.Response.GetHeaderValues("proxy-authenticate"));
						if (!string.IsNullOrEmpty(text))
						{
							Digest orCreate = DigestStore.GetOrCreate(base.CurrentRequest.Proxy.Address);
							orCreate.ParseChallange(text);
							if (base.CurrentRequest.Proxy.Credentials != null && orCreate.IsUriProtected(base.CurrentRequest.Proxy.Address) && (!base.CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
							{
								retryCauses = RetryCauses.ProxyAuthenticate;
							}
						}
						break;
					}
					case 301:
					case 302:
					case 307:
					case 308:
						if (base.CurrentRequest.RedirectCount < base.CurrentRequest.MaxRedirects)
						{
							base.CurrentRequest.RedirectCount++;
							string firstHeaderValue = base.CurrentRequest.Response.GetFirstHeaderValue("location");
							if (string.IsNullOrEmpty(firstHeaderValue))
							{
								throw new MissingFieldException(string.Format("Got redirect status({0}) without 'location' header!", base.CurrentRequest.Response.StatusCode.ToString()));
							}
							Uri redirectUri = GetRedirectUri(firstHeaderValue);
							if (HTTPManager.Logger.Level == Loglevels.All)
							{
								HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Redirected to Location: '{1}' redirectUri: '{1}'", base.CurrentRequest.CurrentUri.ToString(), firstHeaderValue, redirectUri));
							}
							if (!base.CurrentRequest.CallOnBeforeRedirection(redirectUri))
							{
								HTTPManager.Logger.Information("HTTPConnection", "OnBeforeRedirection returned False");
								break;
							}
							base.CurrentRequest.RemoveHeader("Host");
							base.CurrentRequest.SetHeader("Referer", base.CurrentRequest.CurrentUri.ToString());
							base.CurrentRequest.RedirectUri = redirectUri;
							base.CurrentRequest.Response = null;
							bool flag5 = true;
							base.CurrentRequest.IsRedirected = flag5;
							flag2 = flag5;
						}
						break;
					}
					TryStoreInCache();
					bool flag6 = base.CurrentRequest.Response == null || base.CurrentRequest.Response.HasHeaderWithValue("connection", "close");
					bool flag7 = !base.CurrentRequest.Response.IsClosedManually && !base.CurrentRequest.IsKeepAlive;
					if (flag6 || flag7)
					{
						Close();
					}
					else
					{
						if (base.CurrentRequest.Response == null)
						{
							continue;
						}
						List<string> headerValues = base.CurrentRequest.Response.GetHeaderValues("keep-alive");
						if (headerValues != null && headerValues.Count > 0)
						{
							if (KeepAlive == null)
							{
								KeepAlive = new KeepAliveHeader();
							}
							KeepAlive.Parse(headerValues);
						}
					}
				}
				while (retryCauses != RetryCauses.None);
			}
			catch (TimeoutException exception)
			{
				base.CurrentRequest.Response = null;
				base.CurrentRequest.Exception = exception;
				base.CurrentRequest.State = HTTPRequestStates.ConnectionTimedOut;
				Close();
			}
			catch (Exception exception2)
			{
				if (base.CurrentRequest != null)
				{
					if (base.CurrentRequest.UseStreaming)
					{
						HTTPCacheService.DeleteEntity(base.CurrentRequest.CurrentUri);
					}
					base.CurrentRequest.Response = null;
					switch (base.State)
					{
					case HTTPConnectionStates.AbortRequested:
					case HTTPConnectionStates.Closed:
						base.CurrentRequest.State = HTTPRequestStates.Aborted;
						break;
					case HTTPConnectionStates.TimedOut:
						base.CurrentRequest.State = HTTPRequestStates.TimedOut;
						break;
					default:
						base.CurrentRequest.Exception = exception2;
						base.CurrentRequest.State = HTTPRequestStates.Error;
						break;
					}
				}
				Close();
			}
			finally
			{
				if (base.CurrentRequest != null)
				{
					lock (HTTPManager.Locker)
					{
						if (base.CurrentRequest != null && base.CurrentRequest.Response != null && base.CurrentRequest.Response.IsUpgraded)
						{
							base.State = HTTPConnectionStates.Upgraded;
						}
						else
						{
							base.State = (flag2 ? HTTPConnectionStates.Redirected : ((Client != null) ? HTTPConnectionStates.WaitForRecycle : HTTPConnectionStates.Closed));
						}
						if (base.CurrentRequest.State == HTTPRequestStates.Processing && (base.State == HTTPConnectionStates.Closed || base.State == HTTPConnectionStates.WaitForRecycle))
						{
							if (base.CurrentRequest.Response != null)
							{
								base.CurrentRequest.State = HTTPRequestStates.Finished;
							}
							else
							{
								base.CurrentRequest.Exception = new Exception(string.Format("Remote server closed the connection before sending response header! Previous request state: {0}. Connection state: {1}", base.CurrentRequest.State.ToString(), base.State.ToString()));
								base.CurrentRequest.State = HTTPRequestStates.Error;
							}
						}
						if (base.CurrentRequest.State == HTTPRequestStates.ConnectionTimedOut)
						{
							base.State = HTTPConnectionStates.Closed;
						}
						LastProcessTime = DateTime.UtcNow;
						if (OnConnectionRecycled != null)
						{
							RecycleNow();
						}
					}
					HTTPCacheService.SaveLibrary();
					CookieJar.Persist();
				}
			}
		}

		private void Connect()
		{
			Uri uri = ((!base.CurrentRequest.HasProxy) ? base.CurrentRequest.CurrentUri : base.CurrentRequest.Proxy.Address);
			if (Client == null)
			{
				Client = new TcpClient();
			}
			if (!Client.Connected)
			{
				Client.ConnectTimeout = base.CurrentRequest.ConnectTimeout;
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Verbose("HTTPConnection", string.Format("'{0}' - Connecting to {1}:{2}", base.CurrentRequest.CurrentUri.ToString(), uri.Host, uri.Port.ToString()));
				}
				Client.Connect(uri.Host, uri.Port);
				if ((int)HTTPManager.Logger.Level <= 1)
				{
					HTTPManager.Logger.Information("HTTPConnection", "Connected to " + uri.Host + ":" + uri.Port);
				}
			}
			else if ((int)HTTPManager.Logger.Level <= 1)
			{
				HTTPManager.Logger.Information("HTTPConnection", "Already connected to " + uri.Host + ":" + uri.Port);
			}
			base.StartTime = DateTime.UtcNow;
			if (Stream != null)
			{
				return;
			}
			bool flag = HTTPProtocolFactory.IsSecureProtocol(base.CurrentRequest.CurrentUri);
			Stream = Client.GetStream();
			if (base.HasProxy && (!base.Proxy.IsTransparent || (flag && base.Proxy.NonTransparentForHTTPS)))
			{
				BinaryWriter binaryWriter = new BinaryWriter(Stream);
				bool flag2;
				do
				{
					flag2 = false;
					string text = string.Format("CONNECT {0}:{1} HTTP/1.1", base.CurrentRequest.CurrentUri.Host, base.CurrentRequest.CurrentUri.Port);
					HTTPManager.Logger.Information("HTTPConnection", "Sending " + text);
					binaryWriter.SendAsASCII(text);
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Proxy-Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII(string.Format("Host: {0}:{1}", base.CurrentRequest.CurrentUri.Host, base.CurrentRequest.CurrentUri.Port));
					binaryWriter.Write(HTTPRequest.EOL);
					if (base.HasProxy && base.Proxy.Credentials != null)
					{
						switch (base.Proxy.Credentials.Type)
						{
						case AuthenticationTypes.Basic:
							binaryWriter.Write(string.Format("Proxy-Authorization: {0}", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(base.Proxy.Credentials.UserName + ":" + base.Proxy.Credentials.Password))).GetASCIIBytes());
							binaryWriter.Write(HTTPRequest.EOL);
							break;
						case AuthenticationTypes.Unknown:
						case AuthenticationTypes.Digest:
						{
							Digest digest = DigestStore.Get(base.Proxy.Address);
							if (digest != null)
							{
								string text2 = digest.GenerateResponseHeader(base.CurrentRequest, base.Proxy.Credentials);
								if (!string.IsNullOrEmpty(text2))
								{
									binaryWriter.Write(string.Format("Proxy-Authorization: {0}", text2).GetASCIIBytes());
									binaryWriter.Write(HTTPRequest.EOL);
								}
							}
							break;
						}
						}
					}
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Flush();
					base.CurrentRequest.ProxyResponse = new HTTPResponse(base.CurrentRequest, Stream, false, false);
					if (!base.CurrentRequest.ProxyResponse.Receive())
					{
						throw new Exception("Connection to the Proxy Server failed!");
					}
					if ((int)HTTPManager.Logger.Level <= 1)
					{
						HTTPManager.Logger.Information("HTTPConnection", "Proxy returned - status code: " + base.CurrentRequest.ProxyResponse.StatusCode + " message: " + base.CurrentRequest.ProxyResponse.Message);
					}
					int statusCode = base.CurrentRequest.ProxyResponse.StatusCode;
					if (statusCode == 407)
					{
						string text3 = DigestStore.FindBest(base.CurrentRequest.ProxyResponse.GetHeaderValues("proxy-authenticate"));
						if (!string.IsNullOrEmpty(text3))
						{
							Digest orCreate = DigestStore.GetOrCreate(base.Proxy.Address);
							orCreate.ParseChallange(text3);
							if (base.Proxy.Credentials != null && orCreate.IsUriProtected(base.Proxy.Address) && (!base.CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
							{
								flag2 = true;
							}
						}
					}
					else if (!base.CurrentRequest.ProxyResponse.IsSuccess)
					{
						throw new Exception(string.Format("Proxy returned Status Code: \"{0}\", Message: \"{1}\" and Response: {2}", base.CurrentRequest.ProxyResponse.StatusCode, base.CurrentRequest.ProxyResponse.Message, base.CurrentRequest.ProxyResponse.DataAsText));
					}
				}
				while (flag2);
			}
			if (!flag)
			{
				return;
			}
			if (base.CurrentRequest.UseAlternateSSL)
			{
				TlsClientProtocol tlsClientProtocol = new TlsClientProtocol(Client.GetStream(), new SecureRandom());
				List<string> list = new List<string>(1);
				list.Add(base.CurrentRequest.CurrentUri.Host);
				tlsClientProtocol.Connect(new LegacyTlsClient(base.CurrentRequest.CurrentUri, (base.CurrentRequest.CustomCertificateVerifyer != null) ? base.CurrentRequest.CustomCertificateVerifyer : new AlwaysValidVerifyer(), base.CurrentRequest.CustomClientCredentialsProvider, list));
				Stream = tlsClientProtocol.Stream;
				return;
			}
			SslStream sslStream = new SslStream(Client.GetStream(), false, (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => base.CurrentRequest.CallCustomCertificationValidator(cert, chain));
			if (!sslStream.IsAuthenticated)
			{
				sslStream.AuthenticateAsClient(base.CurrentRequest.CurrentUri.Host);
			}
			Stream = sslStream;
		}

		private bool Receive()
		{
			SupportedProtocols protocol = ((base.CurrentRequest.ProtocolHandler != SupportedProtocols.Unknown) ? base.CurrentRequest.ProtocolHandler : HTTPProtocolFactory.GetProtocolFromUri(base.CurrentRequest.CurrentUri));
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - protocol: {1}", base.CurrentRequest.CurrentUri.ToString(), protocol.ToString()));
			}
			base.CurrentRequest.Response = HTTPProtocolFactory.Get(protocol, base.CurrentRequest, Stream, base.CurrentRequest.UseStreaming, false);
			if (!base.CurrentRequest.Response.Receive())
			{
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - Failed! Response will be null, returning with false.", base.CurrentRequest.CurrentUri.ToString()));
				}
				base.CurrentRequest.Response = null;
				return false;
			}
			if (base.CurrentRequest.Response.StatusCode == 304)
			{
				if (base.CurrentRequest.IsRedirected)
				{
					if (!LoadFromCache(base.CurrentRequest.RedirectUri))
					{
						LoadFromCache(base.CurrentRequest.Uri);
					}
				}
				else
				{
					LoadFromCache(base.CurrentRequest.Uri);
				}
			}
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Receive - Finished Successfully!", base.CurrentRequest.CurrentUri.ToString()));
			}
			return true;
		}

		private bool LoadFromCache(Uri uri)
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - LoadFromCache for Uri: {1}", base.CurrentRequest.CurrentUri.ToString(), uri.ToString()));
			}
			HTTPCacheFileInfo entity = HTTPCacheService.GetEntity(uri);
			if (entity == null)
			{
				HTTPManager.Logger.Warning("HTTPConnection", string.Format("{0} - LoadFromCache for Uri: {1} - Cached entity not found!", base.CurrentRequest.CurrentUri.ToString(), uri.ToString()));
				return false;
			}
			base.CurrentRequest.Response.CacheFileInfo = entity;
			int length;
			using (Stream stream = entity.GetBodyStream(out length))
			{
				if (stream == null)
				{
					return false;
				}
				if (!base.CurrentRequest.Response.HasHeader("content-length"))
				{
					base.CurrentRequest.Response.Headers.Add("content-length", new List<string>(1) { length.ToString() });
				}
				base.CurrentRequest.Response.IsFromCache = true;
				if (!base.CurrentRequest.CacheOnly)
				{
					base.CurrentRequest.Response.ReadRaw(stream, length);
				}
			}
			return true;
		}

		private bool TryLoadAllFromCache()
		{
			if (base.CurrentRequest.DisableCache || !HTTPCacheService.IsSupported)
			{
				return false;
			}
			try
			{
				if (HTTPCacheService.IsCachedEntityExpiresInTheFuture(base.CurrentRequest))
				{
					if (HTTPManager.Logger.Level == Loglevels.All)
					{
						HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - TryLoadAllFromCache - whole response loading from cache", base.CurrentRequest.CurrentUri.ToString()));
					}
					base.CurrentRequest.Response = HTTPCacheService.GetFullResponse(base.CurrentRequest);
					if (base.CurrentRequest.Response != null)
					{
						return true;
					}
				}
			}
			catch
			{
				HTTPCacheService.DeleteEntity(base.CurrentRequest.CurrentUri);
			}
			return false;
		}

		private void TryStoreInCache()
		{
			if (!base.CurrentRequest.UseStreaming && !base.CurrentRequest.DisableCache && base.CurrentRequest.Response != null && HTTPCacheService.IsSupported && HTTPCacheService.IsCacheble(base.CurrentRequest.CurrentUri, base.CurrentRequest.MethodType, base.CurrentRequest.Response))
			{
				if (base.CurrentRequest.IsRedirected)
				{
					HTTPCacheService.Store(base.CurrentRequest.Uri, base.CurrentRequest.MethodType, base.CurrentRequest.Response);
				}
				else
				{
					HTTPCacheService.Store(base.CurrentRequest.CurrentUri, base.CurrentRequest.MethodType, base.CurrentRequest.Response);
				}
			}
		}

		private Uri GetRedirectUri(string location)
		{
			Uri uri = null;
			try
			{
				uri = new Uri(location);
				if (uri.IsFile || uri.AbsolutePath == location)
				{
					uri = null;
				}
			}
			catch (UriFormatException)
			{
				uri = null;
			}
			if (uri == null)
			{
				Uri uri2 = base.CurrentRequest.Uri;
				UriBuilder uriBuilder = new UriBuilder(uri2.Scheme, uri2.Host, uri2.Port, location);
				uri = uriBuilder.Uri;
			}
			return uri;
		}

		internal override void Abort(HTTPConnectionStates newState)
		{
			base.State = newState;
			HTTPConnectionStates state = base.State;
			if (state == HTTPConnectionStates.TimedOut)
			{
				base.TimedOutStart = DateTime.UtcNow;
			}
			if (Stream != null)
			{
				Stream.Dispose();
			}
		}

		private void Close()
		{
			KeepAlive = null;
			base.LastProcessedUri = null;
			if (Client == null)
			{
				return;
			}
			try
			{
				Client.Close();
			}
			catch
			{
			}
			finally
			{
				Stream = null;
				Client = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			Close();
			base.Dispose(disposing);
		}
	}
}
