using System;
using BestHTTP;
using LitJson;
using UnityEngine;

public class NetworkHTTPRequest
{
	public string endPoint = NetworkDefs.gameEndPoint;

	public string path;

	public object postData;

	public string[] uriParts;

	public HTTPRequest request;

	public float requestTime;

	public int failedCount;

	public float retryCountdown;

	public void Send()
	{
		if (request != null)
		{
			if (failedCount == 0)
			{
				requestTime = Time.time;
			}
			request.Send();
		}
	}

	public bool IsSent()
	{
		return request.State == HTTPRequestStates.Queued || request.State == HTTPRequestStates.Processing;
	}

	public Uri GetURI()
	{
		string text = NetworkDefs.CombineUri(endPoint, path);
		if (uriParts != null)
		{
			string[] array = uriParts;
			foreach (string text2 in array)
			{
				text = NetworkDefs.CombineUri(text, text2);
			}
		}
		return new Uri(text);
	}

	public string GetPostDataJson()
	{
		if (postData == null)
		{
			return null;
		}
		JsonWriter jsonWriter = new JsonWriter();
		JsonMapper.ToJson(postData, jsonWriter);
		return jsonWriter.ToString();
	}
}
