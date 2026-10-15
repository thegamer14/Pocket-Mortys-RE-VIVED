namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsAuthentication
	{
		void NotifyServerCertificate(Certificate serverCertificate);

		TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest);
	}
}
