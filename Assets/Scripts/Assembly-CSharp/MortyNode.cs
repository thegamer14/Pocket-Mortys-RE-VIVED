using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MortyNode : AdvancedNode
{
	private const float _kErraticCountdown = 4.5f;

	private const float _kExhaustedCountdown = 3f;

	private GameObject _erraticIcon;

	private GameObject _exhaustedIcon;

	private GameObject _faintedIcon;

	private GameObject _interactionPointRun;

	private eMortyNodeState _state;

	private float _stateCountdown;

	private bool interactionTrigger;

	public MortyData mortyData
	{
		get
		{
			return (MortyData)_data;
		}
		private set
		{
			_data = value;
		}
	}

	private void Awake()
	{
		_erraticIcon = base.transform.FindChild("Icons/Erratic").gameObject;
		_exhaustedIcon = base.transform.FindChild("Icons/Exhausted").gameObject;
		_faintedIcon = base.transform.FindChild("Icons/Fainted").gameObject;
		_interactionPointRun = base.transform.FindChild("InteractionPointRun").gameObject;
	}

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		base.SetUp(data, initialDirection, applyRotation);
		if (Application.isPlaying)
		{
			_erraticIcon.SetActive(false);
			_exhaustedIcon.SetActive(false);
			_faintedIcon.SetActive(false);
			RefreshStatus();
		}
	}

	private void Update()
	{
		if (_state == eMortyNodeState.ERRATIC)
		{
			_stateCountdown -= Time.deltaTime;
			if (_stateCountdown <= 0f)
			{
				SetStateExhausted();
			}
		}
		else
		{
			if (_state != eMortyNodeState.EXHAUSTED)
			{
				return;
			}
			_stateCountdown -= Time.deltaTime;
			if (_stateCountdown <= 0f)
			{
				if (interactionTrigger)
				{
					SetStateErratic();
				}
				else
				{
					SetStateNormal();
				}
			}
		}
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = MortyDefs.AssetID(data.id);
		}
	}

	private void SetStateNormal()
	{
		UnsetCurrentState();
		_state = eMortyNodeState.NORMAL;
		if (_aiWanderModule != null)
		{
			_aiWanderModule.SetStateNormal();
		}
	}

	private void SetStateFainted()
	{
		UnsetCurrentState();
		_state = eMortyNodeState.FAINTED;
		_faintedIcon.SetActive(true);
	}

	private void SetStateErratic(eDirection direction = eDirection.NONE, float delay = 0f)
	{
		UnsetCurrentState();
		_state = eMortyNodeState.ERRATIC;
		_stateCountdown = 4.5f;
		TweenErraticIconIn();
		if (_aiWanderModule != null)
		{
			_aiWanderModule.enabled = true;
			_aiWanderModule.SetStateErratic(direction, delay);
		}
	}

	private void SetStateExhausted()
	{
		UnsetCurrentState();
		_state = eMortyNodeState.EXHAUSTED;
		_stateCountdown = 3f;
		TweenExhaustedIconIn();
		if (_aiWanderModule != null)
		{
			_aiWanderModule.enabled = true;
			_aiWanderModule.SetStateExhausted();
		}
	}

	public void RefreshStatus()
	{
		if (mortyData.isFainted)
		{
			SetStateFainted();
		}
	}

	private void UnsetCurrentState()
	{
		switch (_state)
		{
		case eMortyNodeState.FAINTED:
			_faintedIcon.SetActive(false);
			break;
		case eMortyNodeState.ERRATIC:
			StartCoroutine(TweenErraticIconOut());
			break;
		case eMortyNodeState.EXHAUSTED:
			StartCoroutine(TweenExhaustedIconOut());
			break;
		}
	}

	public override void InteractionStarted(eDirection direction, bool isTrigger)
	{
		if (isTrigger)
		{
			interactionTrigger = true;
			if (_state == eMortyNodeState.NORMAL)
			{
				SetStateErratic(direction, 0.8f);
			}
		}
		else
		{
			SetStateNormal();
			base.InteractionStarted(direction, isTrigger);
		}
	}

	public override void InteractionFinished()
	{
		interactionTrigger = false;
	}

	public override void ActivateInteractionPoint(bool show)
	{
		base.ActivateInteractionPoint(show);
		if (_interactionPointRun != null)
		{
			_interactionPointRun.SetActive(show);
		}
	}

	public void TweenErraticIconIn()
	{
		_erraticIcon.SetActive(true);
		_erraticIcon.transform.DOKill();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(_erraticIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear));
		sequence.Append(_erraticIcon.transform.DOPunchScale(Vector3.one * 0.4f, 0.2f));
		sequence.Play();
	}

	public IEnumerator TweenErraticIconOut()
	{
		_erraticIcon.transform.DOKill();
		yield return _erraticIcon.transform.DOScale(0f, 0.4f).WaitForCompletion();
		_erraticIcon.SetActive(false);
	}

	public void TweenExhaustedIconIn()
	{
		_exhaustedIcon.SetActive(true);
		_exhaustedIcon.transform.DOKill();
		Sequence sequence = DOTween.Sequence();
		sequence.Append(_exhaustedIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear));
		sequence.Append(_exhaustedIcon.transform.DOPunchScale(Vector3.one * 0.4f, 0.2f));
		sequence.Play();
	}

	public IEnumerator TweenExhaustedIconOut()
	{
		_exhaustedIcon.transform.DOKill();
		yield return _exhaustedIcon.transform.DOScale(0f, 0.4f).WaitForCompletion();
		_exhaustedIcon.SetActive(false);
	}
}
