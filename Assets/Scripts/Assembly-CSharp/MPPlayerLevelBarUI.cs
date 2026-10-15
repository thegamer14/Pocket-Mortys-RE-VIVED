using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPPlayerLevelBarUI : MonoBehaviour
{
	private RectTransform _rectTrans;

	private Image _fillImage;

	private Text _titleLabel;

	private Text _levelLabel;

	private Image[] _multipliers;

	private Vector2 _inPosition;

	public Vector2 outPosition;

	public float transitionDuration;

	private float _xpFrom;

	private float _xpTo;

	public int xpMultiplier { get; private set; }

	public int level { get; private set; }

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_fillImage = base.transform.FindChild("Fill").GetComponent<Image>();
		_titleLabel = base.transform.FindChild("Overlay/Title").GetComponent<Text>();
		_levelLabel = base.transform.FindChild("Overlay/Level").GetComponent<Text>();
		_multipliers = base.transform.FindChild("Multiplier").GetComponentsInChildren<Image>();
	}

	public void SetUp()
	{
		_titleLabel.text = TextDefs.MORTY_LEVEL_ABBREV;
		_inPosition = _rectTrans.anchoredPosition;
		RefreshAllValues();
	}

	public void RefreshAllValues()
	{
		SetXPTarget(App.net.player.data.level, App.net.player.data.level + 1, App.net.player.data.xp);
		SetLevel(App.net.player.data.level);
		SetMultiplier(App.net.player.data.xpMultiplier);
	}

	public void SetXPTarget(int levelFrom, int levelTo, int xp = -1)
	{
		_xpFrom = MPGameDefs.GetPlayerXPTarget(levelFrom);
		_xpTo = MPGameDefs.GetPlayerXPTarget(levelTo);
		if (xp > -1)
		{
			SetXP(xp);
		}
	}

	public void SetXP(float newXP, bool scaleBar = true)
	{
		_fillImage.fillAmount = Mathf.Clamp01(1f - (newXP - _xpFrom) / (_xpTo - _xpFrom));
	}

	public IEnumerator SetXPWithTween(float newXP)
	{
		float scaleX = Mathf.Clamp01(1f - (newXP - _xpFrom) / (_xpTo - _xpFrom));
		Tween tween = ShortcutExtensions46.DOFillAmount(duration: Mathf.Abs(_fillImage.fillAmount - scaleX), target: _fillImage, endValue: scaleX);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}

	public void SetLevel(int newLevel)
	{
		level = newLevel;
		_levelLabel.text = level.ToString();
	}

	private void SetMultiplier(int multiplier)
	{
		for (int i = 0; i < _multipliers.Length; i++)
		{
			if (multiplier > i)
			{
				_multipliers[i].gameObject.SetActive(true);
			}
			else
			{
				_multipliers[i].gameObject.SetActive(false);
			}
		}
		xpMultiplier = multiplier;
	}

	public IEnumerator TweenMultiplier(int multiplier)
	{
		SetMultiplier(multiplier);
		Image multiplierImage = _multipliers[multiplier - 1];
		multiplierImage.rectTransform.localScale = Vector3.one * 5f;
		Tween scaleTween = multiplierImage.rectTransform.DOScale(1f, 0.5f);
		Tween colorTween = multiplierImage.DOColor(Color.white, 0.5f);
		colorTween.SetLoops(-1, LoopType.Yoyo);
		yield return scaleTween.WaitForCompletion();
	}

	public IEnumerator TweenMultiplerToZero()
	{
		float delay = 0f;
		for (int num = _multipliers.Length - 1; num >= 0; num--)
		{
			if (_multipliers[num].IsActive())
			{
				_multipliers[num].DOColor(new Color(0f, 0f, 0f, 0f), 0.3f).SetDelay(delay);
				_multipliers[num].transform.DOScale(3f, 0.3f).SetDelay(delay);
				delay += 0.2f;
			}
		}
		yield return new WaitForSeconds(delay + 0.3f);
	}

	public void SetPositionIn()
	{
		base.transform.DOKill();
		_rectTrans.anchoredPosition = _inPosition;
	}

	public void SetPositionOut()
	{
		base.transform.DOKill();
		_rectTrans.anchoredPosition = outPosition;
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		SetPositionOut();
		Tween tween = _rectTrans.DOAnchorPos(_inPosition, transitionDuration);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		base.transform.DOKill();
		Tween tween = _rectTrans.DOAnchorPos(outPosition, transitionDuration);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}
}
