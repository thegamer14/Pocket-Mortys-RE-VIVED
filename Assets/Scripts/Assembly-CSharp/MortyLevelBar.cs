using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyLevelBar : MonoBehaviour
{
	private Transform _xpHolder;

	private Text _levelTitle;

	private Text _levelLabel;

	private Image _xpFill;

	private float _xpFrom;

	private float _xpTo;

	private float _xp;

	private void Awake()
	{
		_xpHolder = base.transform.FindChild("XPHolder");
		_levelTitle = base.transform.FindChild("LevelHolder/Title").GetComponent<Text>();
		_levelLabel = base.transform.FindChild("LevelHolder/Label").GetComponent<Text>();
		Transform transform = base.transform.FindChild("XPHolder/Fill");
		if (transform != null)
		{
			_xpFill = transform.GetComponent<Image>();
		}
	}

	protected void Start()
	{
		_levelTitle.text = TextDefs.MORTY_LEVEL_ABBREV;
	}

	public void SetXpHolderActive(bool isActive)
	{
		if (_xpHolder != null)
		{
			_xpHolder.gameObject.SetActive(isActive);
		}
	}

	public void SetLevel(int level)
	{
		_levelLabel.text = level.ToString();
	}

	public void SetXPTarget(float xpFrom, float xpTo, int xp = -1)
	{
		if (!(_xpFill == null))
		{
			_xpFrom = xpFrom;
			_xpTo = xpTo;
			if (xp > -1)
			{
				SetXP(xp);
			}
		}
	}

	public void SetXP(float newXP, bool scaleBar = true)
	{
		if (!(_xpFill == null))
		{
			_xp = newXP;
			_xpFill.transform.localScale = new Vector3((_xp - _xpFrom) / (_xpTo - _xpFrom), 1f, 1f);
		}
	}

	public IEnumerator SetXPWithTween(float newXP)
	{
		if (!(_xpFill == null))
		{
			_xp = newXP;
			float scaleX = (_xp - _xpFrom) / (_xpTo - _xpFrom);
			Tween tween = ShortcutExtensions.DOScaleX(duration: Mathf.Abs(scaleX - _xpFill.transform.localScale.x), target: _xpFill.transform, endValue: scaleX);
			tween.SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();
		}
	}
}
