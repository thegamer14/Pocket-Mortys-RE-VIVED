using System;

namespace GooglePlayGames
{
	internal interface TokenClient
	{
		string GetEmail();

		string GetAccessToken();

		void GetIdToken(string serverClientId, Action<string> idTokenCallback);

		void SetRationale(string rationale);
	}
}
