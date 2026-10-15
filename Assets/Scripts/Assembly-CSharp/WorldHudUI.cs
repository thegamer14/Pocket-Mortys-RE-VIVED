using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WorldHudUI : MonoBehaviour
{
	private const int _kOutPositionY = 200;

	private const float _kInOutDuration = 0.3f;

	private RectTransform _rectTrans;

	private RectTransform _objectiveBarTrans;

	private WorldHudObjectiveUI _itemObjective;

	private WorldHudObjectiveUI _trainerObjective;

	private WorldHudObjectiveUI _gymLeaderObjective;

	private bool _isIn;

	private float _startingPositionY;

	private bool _itemsObjectiveWaitingTick;

	private bool _trainersObjectiveWaitingTick;

	private bool _gymLeaderObjectiveWaitingTick;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_objectiveBarTrans = base.transform.Find("ObjectiveIcons").GetComponent<RectTransform>();
		_itemObjective = base.transform.Find("ObjectiveIcons/Items").GetComponent<WorldHudObjectiveUI>();
		_trainerObjective = base.transform.Find("ObjectiveIcons/Trainers").GetComponent<WorldHudObjectiveUI>();
		_gymLeaderObjective = base.transform.Find("ObjectiveIcons/GymLeaders").GetComponent<WorldHudObjectiveUI>();
		_startingPositionY = base.transform.localPosition.y;
	}

	public void SetUp(bool showObjectives)
	{
		SetPositionOut();
		SetObjectiveBarVisible(showObjectives);
		if (_objectiveBarTrans.gameObject.activeSelf)
		{
			_itemObjective.SetUp(false);
			_trainerObjective.SetUp(false);
			_gymLeaderObjective.SetUp(false);
		}
	}

	public void SetObjectiveBarVisible(bool value)
	{
		_objectiveBarTrans.gameObject.SetActive(value);
	}

	public void TickWaitingObjectives()
	{
		if (_itemsObjectiveWaitingTick && _itemObjective.gameObject.activeInHierarchy)
		{
			_itemObjective.TweenTicked();
		}
		if (_trainersObjectiveWaitingTick && _trainerObjective.gameObject.activeInHierarchy)
		{
			_trainerObjective.TweenTicked();
		}
		if (_gymLeaderObjectiveWaitingTick && _gymLeaderObjective.gameObject.activeInHierarchy)
		{
			_gymLeaderObjective.TweenTicked();
		}
		_itemsObjectiveWaitingTick = false;
		_trainersObjectiveWaitingTick = false;
		_gymLeaderObjectiveWaitingTick = false;
	}

	public void SetItemsTicked(bool value, bool isTween)
	{
		if (isTween && value)
		{
			if (_isIn && _itemObjective.gameObject.activeInHierarchy)
			{
				_itemObjective.TweenTicked();
			}
			else
			{
				_itemsObjectiveWaitingTick = true;
			}
		}
		else
		{
			_itemObjective.SetUp(value);
		}
	}

	public void SetTrainersTicked(bool value, bool isTween)
	{
		if (isTween && value)
		{
			if (_isIn && _trainerObjective.gameObject.activeInHierarchy)
			{
				_trainerObjective.TweenTicked();
			}
			else
			{
				_trainersObjectiveWaitingTick = true;
			}
		}
		else
		{
			_trainerObjective.SetUp(value);
		}
	}

	public void SetGymLeaderTicked(bool value, bool isTween)
	{
		if (isTween && value)
		{
			if (_isIn && _gymLeaderObjective.gameObject.activeInHierarchy)
			{
				_gymLeaderObjective.TweenTicked();
			}
			else
			{
				_gymLeaderObjectiveWaitingTick = true;
			}
		}
		else
		{
			_gymLeaderObjective.SetUp(value);
		}
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, _startingPositionY);
		_isIn = true;
		TickWaitingObjectives();
	}

	public void SetPositionOut()
	{
		_isIn = false;
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, 200f);
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, _startingPositionY), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
		_isIn = true;
		TickWaitingObjectives();
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		_isIn = false;
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, 200f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
	}
}
