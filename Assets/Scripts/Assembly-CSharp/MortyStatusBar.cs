using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyStatusBar : MonoBehaviour
{
	private Text _title;

	private GameObject _faintedIcon;

	private GameObject _poisonedIcon;

	private GameObject _paralysedIcon;

	private GameObject _attackUpIcon;

	private GameObject _defenceUpIcon;

	private GameObject _speedUpIcon;

	private GameObject _accuracyUpIcon;

	private GameObject _evadeUpIcon;

	private GameObject _attackDownIcon;

	private GameObject _defenceDownIcon;

	private GameObject _speedDownIcon;

	private GameObject _accuracyDownIcon;

	private GameObject _evadeDownIcon;

	private GameObject _bootampIcon;

	private void Awake()
	{
		_faintedIcon = base.transform.FindChild("Fainted").gameObject;
		_poisonedIcon = base.transform.FindChild("Poisoned").gameObject;
		_paralysedIcon = base.transform.FindChild("Paralysed").gameObject;
		_attackUpIcon = base.transform.FindChild("AttackUp").gameObject;
		_defenceUpIcon = base.transform.FindChild("DefenceUp").gameObject;
		_speedUpIcon = base.transform.FindChild("SpeedUp").gameObject;
		_accuracyUpIcon = base.transform.FindChild("AccuracyUp").gameObject;
		_evadeUpIcon = base.transform.FindChild("EvadeUp").gameObject;
		_attackDownIcon = base.transform.FindChild("AttackDown").gameObject;
		_defenceDownIcon = base.transform.FindChild("DefenceDown").gameObject;
		_speedDownIcon = base.transform.FindChild("SpeedDown").gameObject;
		_accuracyDownIcon = base.transform.FindChild("AccuracyDown").gameObject;
		_evadeDownIcon = base.transform.FindChild("EvadeDown").gameObject;
		_bootampIcon = base.transform.FindChild("BootcampIcon").gameObject;
		SetAllIcons(false);
		Transform transform = base.transform.FindChild("Title");
		if (transform != null)
		{
			_title = transform.GetComponent<Text>();
		}
	}

	public void SetUp(MortyData morty, bool withAnim = false)
	{
		if (morty == null)
		{
			SetAllIcons(false);
			return;
		}
		bool newActiveVal = morty != null && morty.isFainted;
		bool newActiveVal2 = morty != null && morty.isPoisoned;
		bool newActiveVal3 = morty != null && morty.isParalysed;
		bool newActiveVal4 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.attackModifier > 0f;
		bool newActiveVal5 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.defenceModifier > 0f;
		bool newActiveVal6 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.speedModifier > 0f;
		bool newActiveVal7 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.accuracyModifier > 0f;
		bool newActiveVal8 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.evadeModifier > 0f;
		bool newActiveVal9 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.attackModifier < 0f;
		bool newActiveVal10 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.defenceModifier < 0f;
		bool newActiveVal11 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.speedModifier < 0f;
		bool newActiveVal12 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.accuracyModifier < 0f;
		bool newActiveVal13 = TestDefs.DEBUG_STAT_ICONS && morty != null && morty.evadeModifier < 0f;
		bool newActiveVal14 = morty != null && BootcampDefs.IsMortyOnBootcamp(morty);
		if (withAnim)
		{
			StartCoroutine(SetIconWithAnimation(_faintedIcon, newActiveVal));
			StartCoroutine(SetIconWithAnimation(_poisonedIcon, newActiveVal2));
			StartCoroutine(SetIconWithAnimation(_paralysedIcon, newActiveVal3));
			StartCoroutine(SetIconWithAnimation(_attackUpIcon, newActiveVal4, morty.attackModifier));
			StartCoroutine(SetIconWithAnimation(_defenceUpIcon, newActiveVal5, morty.defenceModifier));
			StartCoroutine(SetIconWithAnimation(_speedUpIcon, newActiveVal6, morty.speedModifier));
			StartCoroutine(SetIconWithAnimation(_accuracyUpIcon, newActiveVal7, morty.accuracyModifier));
			StartCoroutine(SetIconWithAnimation(_evadeUpIcon, newActiveVal8, morty.evadeModifier));
			StartCoroutine(SetIconWithAnimation(_attackDownIcon, newActiveVal9, morty.attackModifier));
			StartCoroutine(SetIconWithAnimation(_defenceDownIcon, newActiveVal10, morty.defenceModifier));
			StartCoroutine(SetIconWithAnimation(_speedDownIcon, newActiveVal11, morty.speedModifier));
			StartCoroutine(SetIconWithAnimation(_accuracyDownIcon, newActiveVal12, morty.accuracyModifier));
			StartCoroutine(SetIconWithAnimation(_evadeDownIcon, newActiveVal13, morty.evadeModifier));
			StartCoroutine(SetIconWithAnimation(_bootampIcon, newActiveVal14));
		}
		else
		{
			SetIcon(_faintedIcon, newActiveVal);
			SetIcon(_poisonedIcon, newActiveVal2);
			SetIcon(_paralysedIcon, newActiveVal3);
			SetIcon(_attackUpIcon, newActiveVal4, morty.attackModifier);
			SetIcon(_defenceUpIcon, newActiveVal5, morty.defenceModifier);
			SetIcon(_speedUpIcon, newActiveVal6, morty.speedModifier);
			SetIcon(_accuracyUpIcon, newActiveVal7, morty.accuracyModifier);
			SetIcon(_evadeUpIcon, newActiveVal8, morty.evadeModifier);
			SetIcon(_attackDownIcon, newActiveVal9, morty.attackModifier);
			SetIcon(_defenceDownIcon, newActiveVal10, morty.defenceModifier);
			SetIcon(_speedDownIcon, newActiveVal11, morty.speedModifier);
			SetIcon(_accuracyDownIcon, newActiveVal12, morty.accuracyModifier);
			SetIcon(_evadeDownIcon, newActiveVal13, morty.evadeModifier);
			SetIcon(_bootampIcon, newActiveVal14);
		}
	}

	public void SetTitle(string text)
	{
		_title.text = text;
	}

	private void SetAllIcons(bool activeVal)
	{
		SetIcon(_faintedIcon, activeVal);
		SetIcon(_poisonedIcon, activeVal);
		SetIcon(_paralysedIcon, activeVal);
		SetIcon(_attackUpIcon, activeVal);
		SetIcon(_defenceUpIcon, activeVal);
		SetIcon(_speedUpIcon, activeVal);
		SetIcon(_accuracyUpIcon, activeVal);
		SetIcon(_evadeUpIcon, activeVal);
		SetIcon(_attackDownIcon, activeVal);
		SetIcon(_defenceDownIcon, activeVal);
		SetIcon(_speedDownIcon, activeVal);
		SetIcon(_accuracyDownIcon, activeVal);
		SetIcon(_evadeDownIcon, activeVal);
		SetIcon(_bootampIcon, activeVal);
	}

	private void SetIcon(GameObject iconObj, bool newActiveVal, float value = 0f)
	{
		iconObj.SetActive(newActiveVal);
		if (newActiveVal)
		{
			SetIconValue(iconObj, value);
		}
	}

	private IEnumerator SetIconWithAnimation(GameObject iconObj, bool newActiveVal, float value = 0f)
	{
		if (newActiveVal)
		{
			SetIconValue(iconObj, value);
		}
		if (!iconObj.activeSelf && newActiveVal)
		{
			iconObj.SetActive(true);
			iconObj.transform.DOKill(true);
			iconObj.transform.localScale = Vector3.zero;
			iconObj.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		}
		else if (iconObj.activeSelf && !newActiveVal)
		{
			iconObj.transform.DOKill(true);
			iconObj.transform.localScale = Vector3.one;
			yield return iconObj.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
			iconObj.SetActive(false);
		}
	}

	private void SetIconValue(GameObject iconObj, float value)
	{
		Transform transform = iconObj.transform.FindChild("Value");
		if (transform != null)
		{
			Text component = transform.GetComponent<Text>();
			if (component != null)
			{
				component.text = value.ToString();
			}
		}
	}
}
