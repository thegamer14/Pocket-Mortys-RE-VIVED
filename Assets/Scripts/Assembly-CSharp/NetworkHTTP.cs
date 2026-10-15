using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestHTTP;
using BestHTTP.Logger;
using UnityEngine;

public class NetworkHTTP : MonoBehaviour
{
	public delegate void NetworkHTTPResponseDelegate(NetworkHTTPRequest request, NetworkHTTPResponse response);

	public delegate void RequestDelegate();

	public delegate void RequestDelayedStateChangedDelegate(bool isDelayed);

	private List<NetworkHTTPRequest> _pendingRequests;

	private bool _requestsDelayed;

	public event RequestDelegate RequestErrorEvent;

	public event RequestDelayedStateChangedDelegate RequestDelayedStateChangedEvent;

	private void Awake()
	{
		_pendingRequests = new List<NetworkHTTPRequest>();
		if (NetworkTestDefs.DEBUG_HTTP)
		{
			HTTPManager.Logger.Level = Loglevels.Information;
		}
	}

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		_pendingRequests.Clear();
	}

	private void Update()
	{
		CheckForRetryPendingRequests();
		CheckForDelayedPendingRequests();
	}

	public void Request(NetworkHTTPRequest netRequest, NetworkHTTPResponseDelegate responseHandler = null)
	{
		RemoveDuplicateRequests(netRequest.path);
		string postDataJson = netRequest.GetPostDataJson();
		HTTPMethods hTTPMethods = ((postDataJson != null) ? HTTPMethods.Post : HTTPMethods.Get);
		if (hTTPMethods == HTTPMethods.Post)
		{
			DebugLog(string.Format("Request (path: {0})\ndata: {1}", netRequest.GetURI(), postDataJson));
		}
		netRequest.request = new HTTPRequest(netRequest.GetURI(), hTTPMethods, delegate(HTTPRequest originalRequest, HTTPResponse response)
		{
			RequestHandler(netRequest, response, responseHandler);
		});
		netRequest.request.AddHeader("X-PMCLIENT", string.Format("{0}/v2.0.3", Application.platform));
		netRequest.request.EnableSafeReadOnUnknownContentLength = true;
		if (hTTPMethods == HTTPMethods.Post)
		{
			netRequest.request.AddHeader("content-type", "application/json");
			netRequest.request.RawData = Encoding.UTF8.GetBytes(postDataJson);
		}
		_pendingRequests.Add(netRequest);
		netRequest.Send();
		DebugHTTPPendingRequests();
	}

	private void RequestHandler(NetworkHTTPRequest netRequest, HTTPResponse response, NetworkHTTPResponseDelegate responseHandler)
	{
		DebugHTTPResponse(netRequest, response);
		if (!_pendingRequests.Contains(netRequest))
		{
			return;
		}
		NetworkHTTPResponse networkHTTPResponse = ((response == null) ? null : new NetworkHTTPResponse(response.StatusCode, response.IsSuccess, NetworkDefs.HTTPRequestStateToNetworkReponseState(netRequest.request.State), response.DataAsText));
		if (networkHTTPResponse == null || !networkHTTPResponse.isSuccess)
		{
			netRequest.failedCount++;
			if (networkHTTPResponse != null && networkHTTPResponse.responseCode == 503)
			{
				if (netRequest.failedCount < 3)
				{
					netRequest.retryCountdown = 0f;
				}
				else
				{
					App.net.session.Disconnect(eNetSessionDisconnectType.LOAD_BALANCER_FAILED);
				}
				return;
			}
			if (this.RequestErrorEvent != null)
			{
				this.RequestErrorEvent();
			}
		}
		_pendingRequests.Remove(netRequest);
		if (responseHandler != null)
		{
			responseHandler(netRequest, networkHTTPResponse);
		}
		DebugHTTPPendingRequests();
	}

	private void RemoveDuplicateRequests(string path)
	{
		_pendingRequests.RemoveAll((NetworkHTTPRequest e) => e.path == path && !e.IsSent());
	}

	private void CheckForDelayedPendingRequests()
	{
		bool flag = _pendingRequests.Any((NetworkHTTPRequest e) => e.request.State == HTTPRequestStates.Processing && (double)(Time.time - e.requestTime) >= 0.5);
		if (_requestsDelayed != flag)
		{
			_requestsDelayed = flag;
			if (this.RequestDelayedStateChangedEvent != null)
			{
				this.RequestDelayedStateChangedEvent(_requestsDelayed);
			}
		}
	}

	private void CheckForRetryPendingRequests()
	{
		for (int num = _pendingRequests.Count - 1; num >= 0; num--)
		{
			if (_pendingRequests[num].retryCountdown > 0f)
			{
				_pendingRequests[num].retryCountdown -= Time.deltaTime;
				if (_pendingRequests[num].retryCountdown <= 0f)
				{
					_pendingRequests[num].retryCountdown = 0f;
					_pendingRequests[num].Send();
				}
			}
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_HTTP)
		{
		}
	}

	protected void DebugLogWarning(string str)
	{
	}

	private void DebugHTTPPendingRequests()
	{
		if (NetworkTestDefs.DEBUG_HTTP)
		{
			string text = string.Format("Pending Requests... {0}", _pendingRequests.Count);
			for (int i = 0; i < _pendingRequests.Count; i++)
			{
				text += string.Format("\n{0} - path: {1}, time: {2}, failed count: {3}, state: {4}", i, _pendingRequests[i].request.Uri, _pendingRequests[i].requestTime, _pendingRequests[i].failedCount, _pendingRequests[i].request.State);
			}
			DebugLog(text);
		}
	}

	private void DebugHTTPResponse(NetworkHTTPRequest netRequest, HTTPResponse response)
	{
		if (!NetworkTestDefs.DEBUG_HTTP)
		{
			return;
		}
		switch (netRequest.request.State)
		{
		case HTTPRequestStates.Finished:
			if (response.IsSuccess)
			{
				DebugLog(string.Format("Request complete (path: {0})\ndata: {1}", netRequest.request.Uri, response.DataAsText));
				break;
			}
			DebugLogWarning(string.Format("Request complete, but the server sent an error (path: {0})\nStatus Code: {1}-{2} data: {3}", netRequest.request.Uri, response.StatusCode, response.Message, response.DataAsText));
			break;
		case HTTPRequestStates.Error:
			DebugLogWarning(string.Format("Request Complete with Error (path: {0})\ndata: {1}", netRequest.request.Uri, (netRequest.request.Exception == null) ? "No Exception" : (netRequest.request.Exception.Message + "\n" + netRequest.request.Exception.StackTrace)));
			break;
		case HTTPRequestStates.Aborted:
			DebugLogWarning(string.Format("Request Aborted (path: {0})", netRequest.request.Uri));
			break;
		case HTTPRequestStates.ConnectionTimedOut:
			DebugLogWarning(string.Format("Connection Timed Out (path: {0})", netRequest.request.Uri));
			break;
		case HTTPRequestStates.TimedOut:
			DebugLogWarning(string.Format("Processing the request Timed Out (path: {0})", netRequest.request.Uri));
			break;
		default:
			DebugLogWarning(string.Format("Unkown reponse state (path: {0}, state: {1})", netRequest.request.Uri, netRequest.request.State.ToString()));
			break;
		}
	}
}
