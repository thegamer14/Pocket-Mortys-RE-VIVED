using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TournamentEventCell : CellBase
{
	public Sprite completeSprite;

	private Text _nameLabel;

	private Transform _numberTrans;

	private Text _numberText;

	private Transform _countdownTrans;

	private Text _countdownTitle;

	private Text _countdownLabel;

	private double _lastCountdown;

	protected override void Awake()
	{
		base.Awake();
		_nameLabel = _backgroundImage.transform.FindChild("NameLabel").GetComponent<Text>();
		_numberTrans = base.transform.FindChild("Number");
		_numberText = _numberTrans.FindChild("Text").GetComponent<Text>();
		_countdownTrans = base.transform.FindChild("Countdown");
		if (_countdownTrans != null)
		{
			_countdownTitle = base.transform.FindChild("Countdown/Title").GetComponent<Text>();
			_countdownLabel = base.transform.FindChild("Countdown/Label").GetComponent<Text>();
		}
		_lastCountdown = -999.0;
	}

	public void SetUp(string id, int index, eTournamentEventCellState state)
	{
		base.SetUp(id);
		SetEnabled(false);
		SetUpBackgroundSprite(state);
		SetUpNumber(index, state);
		EnableCountdown(false);
		_nameLabel.text = TournamentDefs.EventName(id);
		if (_countdownTrans != null)
		{
			_countdownTitle.text = TextDefs.TOURNAMENT_EVENT_CELL_COUNTDOWN_TITLE;
		}
	}

	private void SetUpBackgroundSprite(eTournamentEventCellState state)
	{
		if (state == eTournamentEventCellState.OPEN || state == eTournamentEventCellState.COMPLETE)
		{
			_backgroundImage.sprite = completeSprite;
			_backgroundImage.SetNativeSize();
		}
	}

	private void SetUpNumber(int index, eTournamentEventCellState state)
	{
		_numberText.text = (index + 1).ToString();
		if (state == eTournamentEventCellState.OPEN)
		{
			_numberTrans.DOScale(0.8f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
		}
	}

	public void EnableCountdown(bool enabled)
	{
		if (_countdownTrans != null)
		{
			_countdownTrans.gameObject.SetActive(enabled);
		}
	}

	public void SetCountdown(double countdown)
	{
		double num = countdown + 59.0;
		if (_countdownTrans != null)
		{
			int num2 = (int)Math.Floor(num / 3600.0);
			int num3 = (int)Math.Floor(num % 3600.0 / 60.0);
			int num4 = (int)Math.Floor(_lastCountdown % 3600.0 / 60.0);
			if (num3 != num4)
			{
				string text = string.Empty;
				if (num2 > 0)
				{
					text += string.Format("{0}h ", num2);
				}
				text += string.Format("{0}mins", num3);
				_countdownLabel.text = text;
			}
		}
		_lastCountdown = num;
	}
}
