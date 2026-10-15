using System;
using System.Collections.Generic;

namespace BestHTTP.Authentication
{
	internal static class DigestStore
	{
		private static Dictionary<string, Digest> Digests = new Dictionary<string, Digest>();

		private static object Locker = new object();

		private static string[] SupportedAlgorithms = new string[2] { "digest", "basic" };

		internal static Digest Get(Uri uri)
		{
			lock (Locker)
			{
				Digest value = null;
				if (Digests.TryGetValue(uri.Host, out value) && !value.IsUriProtected(uri))
				{
					return null;
				}
				return value;
			}
		}

		public static Digest GetOrCreate(Uri uri)
		{
			lock (Locker)
			{
				Digest value = null;
				if (!Digests.TryGetValue(uri.Host, out value))
				{
					Digests.Add(uri.Host, value = new Digest(uri));
				}
				return value;
			}
		}

		public static void Remove(Uri uri)
		{
			lock (Locker)
			{
				Digests.Remove(uri.Host);
			}
		}

		public static string FindBest(List<string> authHeaders)
		{
			if (authHeaders == null || authHeaders.Count == 0)
			{
				return string.Empty;
			}
			List<string> list = new List<string>(authHeaders.Count);
			for (int i = 0; i < authHeaders.Count; i++)
			{
				list.Add(authHeaders[i].ToLower());
			}
			int j;
			for (j = 0; j < SupportedAlgorithms.Length; j++)
			{
				int num = list.FindIndex((string header) => header.StartsWith(SupportedAlgorithms[j]));
				if (num != -1)
				{
					return authHeaders[num];
				}
			}
			return string.Empty;
		}
	}
}
