using System;
using System.Collections.Generic;
using BestHTTP.ServerSentEvents;
using UnityEngine;

public class NetworkEventSource : MonoBehaviour
{
	public delegate void SourceErrorDelegate(string error);

	public delegate void SourceChangedDelegate();

	public delegate void DisconnectTimeoutDelegate();

	public delegate void EventResponseDelegate(NetworkEventSourceResponse response);

	private EventSource _eventSource;

	public Dictionary<string, List<EventResponseDelegate>> _registeredHandlers = new Dictionary<string, List<EventResponseDelegate>>();

	private float _keepaliveTimeout = -1f;

	private float _keepaliveTimeoutCounter = -1f;

	public event SourceErrorDelegate SourceError;

	public event SourceChangedDelegate OpenedEvent;

	public event SourceChangedDelegate ClosedEvent;

	public event DisconnectTimeoutDelegate KeepaliveTimeoutEvent;

	public void SetDisconnectAfterTimeout(float value)
	{
		DebugLog("SetKeepaliveTimeout(" + value + ")");
		_keepaliveTimeout = value;
		_keepaliveTimeoutCounter = value;
	}

	private void OnDestroy()
	{
		_registeredHandlers.Clear();
	}

	private void Update()
	{
		CheckKeepaliveTimeout();
	}

	private void CheckKeepaliveTimeout()
	{
		if (_eventSource == null || _eventSource.State != States.Open || !(_keepaliveTimeout > 0f) || !(_keepaliveTimeoutCounter > 0f))
		{
			return;
		}
		_keepaliveTimeoutCounter = Mathf.Max(0f, _keepaliveTimeoutCounter - Time.deltaTime);
		if (_keepaliveTimeoutCounter <= 0f)
		{
			DebugLog("CheckDisconnectTimeout() _keepaliveTimeoutCounter reached 0, dispatching DisconnectTimeoutEvent()");
			if (this.KeepaliveTimeoutEvent != null)
			{
				this.KeepaliveTimeoutEvent();
			}
		}
	}

	public void Open(string sourceURI)
	{
		DebugLog("Open(" + sourceURI + ")");
		if (_eventSource != null)
		{
			Close();
		}
		_eventSource = new EventSource(new Uri(sourceURI));
		_eventSource.InternalRequest.SetHeader("X-PMCLIENT", string.Format("{0}/v2.0.3", Application.platform));
		AddEvents(_eventSource);
		_eventSource.Open();
	}

	public void Close()
	{
		DebugLog("Close()");
		if (_eventSource == null)
		{
			DebugLog("Trying to disconnect from _eventSource when it is null");
		}
		else
		{
			_eventSource.Close();
		}
	}

	private void AddEvents(EventSource eventSource)
	{
		if (eventSource != null)
		{
			eventSource.OnClosed += OnClosedHandler;
			eventSource.OnError += OnErrorHandler;
			eventSource.OnOpen += OnOpenHandler;
			eventSource.OnRetry += OnRetryHandler;
			eventSource.OnStateChanged += OnStateChangedHandler;
			eventSource.OnMessage += OnMessageHandler;
		}
	}

	private void RemoveEvents(EventSource eventSource)
	{
		if (eventSource != null)
		{
			eventSource.OnClosed -= OnClosedHandler;
			eventSource.OnError -= OnErrorHandler;
			eventSource.OnOpen -= OnOpenHandler;
			eventSource.OnRetry -= OnRetryHandler;
			eventSource.OnStateChanged -= OnStateChangedHandler;
			eventSource.OnMessage -= OnMessageHandler;
		}
	}

	public void On(string eventName, EventResponseDelegate handler)
	{
		DebugLog(string.Format("EventSourceOn(eventName:{0}, handler{1}", eventName, handler));
		AddInternalHandler(eventName, handler);
	}

	public void Off(string eventName, EventResponseDelegate handler)
	{
		DebugLog(string.Format("EventSourceOff(eventName:{0}, handler{1}", eventName, handler));
		RemoveInternalHandler(eventName, handler);
	}

	public bool IsClosed()
	{
		return _eventSource == null || _eventSource.State == States.Closed || _eventSource.State == States.Closing;
	}

	private void AddInternalHandler(string eventName, EventResponseDelegate handler)
	{
		if (!_registeredHandlers.ContainsKey(eventName))
		{
			_registeredHandlers.Add(eventName, new List<EventResponseDelegate>());
		}
		else if (_registeredHandlers[eventName].Contains(handler))
		{
			DebugLog("Trying to add socket event and handler that is already registered.");
			return;
		}
		_registeredHandlers[eventName].Add(handler);
		DebugEventHandlers();
	}

	private void RemoveInternalHandler(string eventName, EventResponseDelegate handler)
	{
		if (_registeredHandlers.ContainsKey(eventName) && _registeredHandlers[eventName].Contains(handler))
		{
			_registeredHandlers[eventName].Remove(handler);
			if (_registeredHandlers[eventName].Count == 0)
			{
				_registeredHandlers.Remove(eventName);
			}
		}
		DebugEventHandlers();
	}

	private void CallHandlers(string eventName, bool removeOnceCalled = false, string data = null)
	{
		if (!_registeredHandlers.ContainsKey(eventName))
		{
			return;
		}
		List<EventResponseDelegate> list = new List<EventResponseDelegate>(_registeredHandlers[eventName]);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num](new NetworkEventSourceResponse(data));
			if (removeOnceCalled)
			{
				RemoveInternalHandler(eventName, list[num]);
			}
		}
	}

	private void OnMessageHandler(EventSource eventSource, Message message)
	{
		_keepaliveTimeoutCounter = _keepaliveTimeout;
		if (message == null)
		{
			DebugLog("OnMessageHandler() - message is null");
			return;
		}
		DebugLog(string.Format("OnMessageHandler() eventName:{0}, data:{1}", message.Event, message.Data));
		CallHandlers(message.Event, false, message.Data);
	}

	private void OnStateChangedHandler(EventSource eventSource, States oldState, States newState)
	{
		DebugLog(string.Format("OnStateChangedHandler() oldState:{0}, newState:{1}", oldState.ToString(), newState.ToString()));
	}

	private bool OnRetryHandler(EventSource eventSource)
	{
		DebugLog("OnRetryHandler()");
		return false;
	}

	private void OnOpenHandler(EventSource eventSource)
	{
		DebugLog("OnOpenHandler()");
		if (this.OpenedEvent != null)
		{
			this.OpenedEvent();
		}
	}

	private void OnErrorHandler(EventSource eventSource, string error)
	{
		DebugLog(string.Format("OnErrorHandler() error:{0}", error));
		if (this.SourceError != null)
		{
			this.SourceError(error);
		}
	}

	private void OnClosedHandler(EventSource eventSource)
	{
		DebugLog("OnClosedHandler()");
		RemoveEvents(eventSource);
		if (_eventSource == eventSource)
		{
			_eventSource = null;
		}
		if (this.ClosedEvent != null)
		{
			this.ClosedEvent();
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_EVENT_SOURCE)
		{
		}
	}

	protected void DebugEventHandlers()
	{
		if (!NetworkTestDefs.DEBUG_EVENT_SOURCE)
		{
			return;
		}
		string text = string.Format("NetworkController - Event Source Handlers: ({0}):\n", _registeredHandlers.Count);
		foreach (KeyValuePair<string, List<EventResponseDelegate>> registeredHandler in _registeredHandlers)
		{
			string text2 = text;
			text = text2 + "  " + registeredHandler.Key + " has " + registeredHandler.Value.Count + " handlers:\n";
			foreach (EventResponseDelegate item in registeredHandler.Value)
			{
				text = text + "    " + item.ToString() + "\n";
			}
		}
	}
}
