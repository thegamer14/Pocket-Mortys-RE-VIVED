using System.Collections;
using DG.Tweening;
using UnityEngine;

public class StateBase : MonoBehaviour
{
	public delegate void StateBaseDelegate(StateBase self);

	protected bool _dontDestroyUI;

	protected string _uiResourcePath;

	protected RectTransform _uiRectTrans;

	protected UIBase _ui;

	private Coroutine _transition;

	private eTransitionDirection _lastTransitionDirection;

	protected TransInfo _transInInfo;

	protected bool _useBackgroundFade;

	protected Color _backgroundFadeColor;

	protected float _backgroundFadeDuration;

	public eStateStatus status { get; private set; }

	public bool hideStateBellow { get; protected set; }

	public bool isInputActive { get; private set; }

	public bool showLoadingScreen { get; protected set; }

	public event StateBaseDelegate TransitionInCompleteEvent;

	public event StateBaseDelegate TransitionOutCompleteEvent;

	public virtual void Awake()
	{
		status = eStateStatus.DEACTIVE;
		hideStateBellow = true;
		_lastTransitionDirection = eTransitionDirection.OUT;
		_uiResourcePath = null;
		_useBackgroundFade = false;
		_backgroundFadeColor = new Color(0f, 0f, 0f, 0.4f);
		_backgroundFadeDuration = -1f;
	}

	public virtual void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		_transInInfo = transInInfo;
		status = eStateStatus.ACTIVE;
		CreateUI(uiContainerTrans);
		TransitionSetUp(transInInfo);
		Create();
		AddEvents();
	}

	protected virtual IEnumerator ActivateComplete()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Transition(eTransitionDirection.IN, _transInInfo);
	}

	public virtual void Cache(TransInfo transOutInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		Transition(eTransitionDirection.OUT, transOutInfo, eStateStatus.CACHED);
	}

	public virtual void Uncache(TransInfo transInInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		status = eStateStatus.ACTIVE;
		Transition(eTransitionDirection.IN, transInInfo);
	}

	public virtual void Deactivate(TransInfo transOutInfo)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		Transition(eTransitionDirection.OUT, transOutInfo);
	}

	public virtual void SetAsCurrentComplete()
	{
	}

	protected virtual void Create()
	{
	}

	protected virtual void CreateUI(Transform containerTrans)
	{
		if (_ui == null)
		{
			if (_uiResourcePath == null)
			{
				return;
			}
			_uiRectTrans = UI.Instantiate(_uiResourcePath, containerTrans);
			if (_uiRectTrans != null)
			{
				_ui = _uiRectTrans.GetComponent<UIBase>();
				if (_ui != null)
				{
					_ui.SetLabels();
				}
			}
		}
		else
		{
			_ui.gameObject.SetActive(true);
			_ui.transform.SetParent(containerTrans, false);
		}
	}

	protected virtual void Destroy()
	{
		if (_ui != null)
		{
			if (!_dontDestroyUI)
			{
				_ui.Destroy();
				Object.Destroy(_ui.gameObject);
				_ui = null;
			}
			else
			{
				_ui.transform.SetParent(base.transform, false);
				_ui.gameObject.SetActive(false);
			}
		}
		RemoveEvents();
	}

	protected virtual void DestroyCache()
	{
	}

	protected virtual void AddEvents()
	{
		LocalizationDefs.LanguageChangedEvent += LanguageChangedHandler;
	}

	protected virtual void RemoveEvents()
	{
		LocalizationDefs.LanguageChangedEvent -= LanguageChangedHandler;
	}

	public virtual void ManualUpdate(float deltaTime)
	{
		if (_ui != null)
		{
			_ui.ManualUpdate(deltaTime);
		}
	}

	public bool IsActive()
	{
		return (status != eStateStatus.DEACTIVE) ? true : false;
	}

	public bool IsActiveAndNotTransitioning()
	{
		return status == eStateStatus.ACTIVE && _transition == null;
	}

	public bool IsTransitioningOut()
	{
		return (_transition != null && _lastTransitionDirection == eTransitionDirection.OUT) ? true : false;
	}

	public bool IsTransitioning()
	{
		return _transition != null;
	}

	public void PreLoadUI()
	{
		CreateUI(base.transform);		if (_ui == null){ UnityEngine.Debug.LogWarning("[StateBase] no UI for " + GetType().Name); return; }		_ui.gameObject.SetActive(false);
	}

	public virtual void ActivateInput(bool isActive)
	{
		if (status == eStateStatus.ACTIVE)
		{
			if (_ui != null)
			{
				_ui.ActivateInput(isActive);
			}
			isInputActive = isActive;
		}
	}

	public void TransitionSetUp(TransInfo info)
	{
		ActivateInput(false);
		if (info != null)
		{
			switch (info.type)
			{
			case eTransition.FADE:
				App.states.TransitionFadeSetUp(this, (FadeTransInfo)info);
				break;
			case eTransition.POP:
				TransitionPopSetUp(eTransitionDirection.IN, (PopTransInfo)info);
				break;
			case eTransition.SLIDE:
				TransitionSlideSetUp(eTransitionDirection.IN, (SlideTransInfo)info);
				break;
			case eTransition.CUSTOM:
				break;
			}
		}
	}

	private void Transition(eTransitionDirection transitionDirection, TransInfo info, eStateStatus outEndStatus = eStateStatus.DEACTIVE)
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		if (_transition == null || _lastTransitionDirection != transitionDirection)
		{
			ActivateInput(false);
			if (_useBackgroundFade && info != null)
			{
				TransitionBackground(transitionDirection, info.duration);
			}
			_transition = StartCoroutine(TransitionCo(transitionDirection, info, outEndStatus));
		}
	}

	private IEnumerator TransitionCo(eTransitionDirection transitionDirection, TransInfo info, eStateStatus outEndStatus = eStateStatus.DEACTIVE)
	{
		_lastTransitionDirection = transitionDirection;
		if (info != null)
		{
			switch (info.type)
			{
			case eTransition.FADE:
				yield return StartCoroutine(App.states.TransitionFade(this, transitionDirection, (FadeTransInfo)info));
				break;
			case eTransition.POP:
				yield return StartCoroutine(TransitionPop(transitionDirection, (PopTransInfo)info));
				break;
			case eTransition.SLIDE:
				yield return StartCoroutine(TransitionSlide(transitionDirection, (SlideTransInfo)info));
				break;
			case eTransition.CUSTOM:
				yield return StartCoroutine(TransitionCustom(transitionDirection, (CustomTransInfo)info));
				break;
			}
		}
		yield return new WaitForEndOfFrame();
		_transition = null;
		if (transitionDirection == eTransitionDirection.IN)
		{
			TransitionInComplete();
			yield break;
		}
		TransitionOutComplete();
		status = outEndStatus;
		if (status == eStateStatus.DEACTIVE)
		{
			Destroy();
		}
		else if (status == eStateStatus.CACHED)
		{
			DestroyCache();
		}
	}

	protected void TransitionBackground(eTransitionDirection transitionDirection, float infoDuration)
	{
		App.states.BackgroundFade(this, transitionDirection, _backgroundFadeColor, (!(_backgroundFadeDuration < 0f)) ? _backgroundFadeDuration : infoDuration);
	}

	protected virtual void TransitionInComplete()
	{
		ActivateInput(true);
		if (this.TransitionInCompleteEvent != null)
		{
			this.TransitionInCompleteEvent(this);
		}
	}

	protected virtual void TransitionOutComplete()
	{
		if (this.TransitionOutCompleteEvent != null)
		{
			this.TransitionOutCompleteEvent(this);
		}
	}

	private void TransitionPopSetUp(eTransitionDirection transitionDirection, PopTransInfo info)
	{
		_ui.transform.localScale = ((transitionDirection != eTransitionDirection.OUT) ? info.from : info.to);
	}

	protected virtual IEnumerator TransitionPop(eTransitionDirection transitionDirection, PopTransInfo info)
	{
		TransitionPopSetUp(transitionDirection, info);
		Tween tween = ShortcutExtensions.DOScale(endValue: (transitionDirection != eTransitionDirection.OUT) ? info.to : info.from, target: _ui.transform, duration: info.duration);
		tween.SetEase((transitionDirection != eTransitionDirection.OUT) ? Ease.OutBack : Ease.InBack);
		tween.SetUpdate(true);
		yield return tween.WaitForCompletion();
	}

	private Vector3 TransitionSlideSetUp(eTransitionDirection transitionDirection, SlideTransInfo info)
	{
		Vector3 vector = Vector3.zero;
		switch (info.direction)
		{
		case eDirection.RIGHT:
			vector = new Vector3(Screen.width, 0f, 0f);
			break;
		case eDirection.LEFT:
			vector = new Vector3(-Screen.width, 0f, 0f);
			break;
		case eDirection.UP:
			vector = new Vector3(0f, Screen.height, 0f);
			break;
		case eDirection.DOWN:
			vector = new Vector3(0f, -Screen.height, 0f);
			break;
		}
		_ui.transform.localPosition = ((transitionDirection != eTransitionDirection.IN) ? Vector3.zero : vector);
		return vector;
	}

	protected virtual IEnumerator TransitionSlide(eTransitionDirection transitionDirection, SlideTransInfo info)
	{
		Vector3 destination = TransitionSlideSetUp(transitionDirection, info);
		Tween tween = ShortcutExtensions.DOLocalMove(endValue: (transitionDirection != eTransitionDirection.IN) ? destination : Vector3.zero, target: _ui.transform, duration: info.duration);
		tween.SetEase(Ease.InSine);
		tween.SetUpdate(true);
		yield return tween.WaitForCompletion();
	}

	protected virtual IEnumerator TransitionCustom(eTransitionDirection transitionDirection, CustomTransInfo info)
	{
		yield break;
	}

	public virtual void StopTransition()
	{
		if (TestDefs.DEBUG_STATE_CONTROLLER)
		{
		}
		if (_transition != null)
		{
			StopCoroutine(_transition);
			_transition = null;
		}
	}

	public virtual void ShowUILayer()
	{
		App.states.GetUILayerForState(this).gameObject.SetActive(true);
	}

	public virtual void HideUILayer()
	{
		App.states.GetUILayerForState(this).gameObject.SetActive(false);
	}

	protected virtual void LanguageChangedHandler()
	{
		_ui.SetLabels();
	}
}
