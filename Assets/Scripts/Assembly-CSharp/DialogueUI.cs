using System;
using System.Collections;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
	public delegate void DialogueControllerDelegate();

	private DialogueBox _currentBox;

	private DialogueBox _speechBoxLeft;

	private DialogueBox _speechBoxRight;

	private DialogueBox _actionBox;

	private DialogueData _data;

	private int _dataIndex;

	private int _pageIndex;

	private AdvancedNode _nodeTrigger;

	private Action<AdvancedNode> _callback;

	public bool inUse
	{
		get
		{
			return _data != null;
		}
	}

	public event DialogueControllerDelegate DialogueStartedEvent;

	public event DialogueControllerDelegate DialogueFinishedEvent;

	private void Awake()
	{
		Transform transform = base.transform.FindChild("SpeechBoxLeft");
		Transform transform2 = base.transform.FindChild("SpeechBoxRight");
		Transform transform3 = base.transform.FindChild("ActionBox");
		if (transform != null)
		{
			_speechBoxLeft = transform.GetComponent<DialogueBox>();
		}
		if (transform2 != null)
		{
			_speechBoxRight = transform2.GetComponent<DialogueBox>();
		}
		if (transform3 != null)
		{
			_actionBox = transform3.GetComponent<DialogueBox>();
		}
	}

	private void Start()
	{
		if (_speechBoxLeft != null)
		{
			_speechBoxLeft.SetUp(eDirection.LEFT);
		}
		if (_speechBoxRight != null)
		{
			_speechBoxRight.SetUp(eDirection.RIGHT);
		}
		if (_speechBoxLeft != null)
		{
			_speechBoxLeft.SetPositionOut();
		}
		if (_speechBoxRight != null)
		{
			_speechBoxRight.SetPositionOut();
		}
		if (_actionBox != null)
		{
			_actionBox.SetPositionOut();
		}
		AddEvents(true);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			if (_speechBoxLeft != null)
			{
				_speechBoxLeft.ContinuePressedEvent += ContinuePressedHandler;
			}
			if (_speechBoxRight != null)
			{
				_speechBoxRight.ContinuePressedEvent += ContinuePressedHandler;
			}
			if (_actionBox != null)
			{
				_actionBox.ContinuePressedEvent += ContinuePressedHandler;
			}
		}
		else
		{
			if (_speechBoxLeft != null)
			{
				_speechBoxLeft.ContinuePressedEvent -= ContinuePressedHandler;
			}
			if (_speechBoxRight != null)
			{
				_speechBoxRight.ContinuePressedEvent -= ContinuePressedHandler;
			}
			if (_actionBox != null)
			{
				_actionBox.ContinuePressedEvent -= ContinuePressedHandler;
			}
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	private void Update()
	{
		if (_currentBox != null)
		{
			_currentBox.ManualUpdate();
		}
	}

	public void DisplaySpeech(DialogueData data, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dataIndex = 0;
		_pageIndex = -1;
		_data = data;
		_nodeTrigger = nodeTrigger;
		_callback = callback;
		_currentBox = ((_data.entries[_dataIndex].direction != eDirection.LEFT) ? _speechBoxRight : _speechBoxLeft);
		StartCoroutine(DisplayCurrentBox());
	}

	public void DisplayAction(string text, AdvancedNode nodeTrigger = null, Action<AdvancedNode> callback = null)
	{
		_dataIndex = 0;
		_pageIndex = -1;
		_data = new DialogueData(text);
		_nodeTrigger = nodeTrigger;
		_callback = callback;
		_currentBox = _actionBox;
		StartCoroutine(DisplayCurrentBox());
	}

	private IEnumerator DisplayCurrentBox()
	{
		if (this.DialogueStartedEvent != null)
		{
			this.DialogueStartedEvent();
		}
		_currentBox.gameObject.SetActive(true);
		_currentBox.SetText(string.Empty);
		_currentBox.ShowBtn(false);
		yield return StartCoroutine(_currentBox.SetCharacterImage(_data.entries[_dataIndex].assetID));
		_currentBox.SetCharacterName(_data.entries[_dataIndex].name);
		yield return StartCoroutine(_currentBox.TweenIn());
		yield return StartCoroutine(DisplayNextPage());
	}

	public IEnumerator DisplayNextPage()
	{
		_pageIndex++;
		if (_pageIndex >= _data.entries[_dataIndex].text.Length)
		{
			_pageIndex = 0;
			_dataIndex++;
			if (_dataIndex >= _data.entries.Length)
			{
				StartCoroutine(FinishCurrentBox());
				yield break;
			}
			yield return StartCoroutine(_currentBox.TweenOut());
			if (_data.entries[_dataIndex].direction == eDirection.LEFT)
			{
				_currentBox = _speechBoxLeft;
			}
			else
			{
				_currentBox = _speechBoxRight;
			}
			yield return StartCoroutine(_currentBox.SetCharacterImage(_data.entries[_dataIndex].assetID));
			_currentBox.SetCharacterName(_data.entries[_dataIndex].name);
			_currentBox.SetText(string.Empty);
			yield return StartCoroutine(_currentBox.TweenIn());
		}
		yield return StartCoroutine(_currentBox.TweenText(_data.entries[_dataIndex].text[_pageIndex]));
		_currentBox.ShowBtn(true);
	}

	private IEnumerator FinishCurrentBox()
	{
		yield return StartCoroutine(_currentBox.TweenOut());
		_currentBox = null;
		_data = null;
		if (_nodeTrigger != null)
		{
			_nodeTrigger.InteractionFinished();
		}
		if (_callback != null)
		{
			_callback(null);
		}
		if (this.DialogueFinishedEvent != null)
		{
			this.DialogueFinishedEvent();
		}
	}

	public void ContinuePressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/CellSelect");
		_currentBox.ShowBtn(false);
		StartCoroutine(DisplayNextPage());
	}
}
