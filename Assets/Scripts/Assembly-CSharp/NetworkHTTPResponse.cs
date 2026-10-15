using System;

[Serializable]
public class NetworkHTTPResponse
{
	public int responseCode;

	public bool isSuccess;

	public eHTTPReponseState responseState;

	public string jsonStr;

	public NetworkHTTPResponse(int responseCode, bool isSuccess = false, eHTTPReponseState responseState = eHTTPReponseState.Unkown, string jsonStr = null)
	{
		this.responseCode = responseCode;
		this.isSuccess = isSuccess;
		this.responseState = responseState;
		this.jsonStr = jsonStr;
	}
}
