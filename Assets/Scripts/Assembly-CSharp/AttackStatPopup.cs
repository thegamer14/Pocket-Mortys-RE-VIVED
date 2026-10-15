using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackStatPopup : MonoBehaviour
{
	private Text _title1;

	private Text _title2;

	private Text _value1;

	private Text _value2;

	private void Awake()
	{
		Transform transform = base.transform.FindChild("Stat1/Title");
		if (transform != null)
		{
			_title1 = transform.GetComponent<Text>();
		}
		Transform transform2 = base.transform.FindChild("Stat2/Title");
		if (transform2 != null)
		{
			_title2 = transform2.GetComponent<Text>();
		}
		Transform transform3 = base.transform.FindChild("Stat1/Value");
		if (transform3 != null)
		{
			_value1 = transform3.GetComponent<Text>();
		}
		Transform transform4 = base.transform.FindChild("Stat2/Value");
		if (transform4 != null)
		{
			_value2 = transform4.GetComponent<Text>();
		}
	}

	public void SetUp(AttackDisplayInfo displayInfo)
	{
		switch (displayInfo.catagory)
		{
		case eAttackCatagory.Normal:
			SetUpNormal(displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Buff:
			SetUpBuff(displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Debuff:
			SetUpDebuff(displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Golden:
			SetUpGolden(displayInfo.description);
			break;
		}
	}

	public void SetUpNormal(string infoValue1, string infoValue2)
	{
		_title1.text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpBuff(string infoValue1, string infoValue2)
	{
		_title1.text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpDebuff(string infoValue1, string infoValue2)
	{
		_title1.text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpGolden(string description)
	{
		_title1.text = description;
	}

	public void TweenIn()
	{
		StopAllCoroutines();
		base.transform.DOKill();
		StartCoroutine(TweenInCo());
	}

	private IEnumerator TweenInCo()
	{
		base.transform.localScale = Vector3.zero;
		yield return base.transform.DOScale(1f, 0.2f).WaitForCompletion();
	}

	public void TweenOut(bool destroyOnComplete)
	{
		StopAllCoroutines();
		base.transform.DOKill();
		StartCoroutine(TweenOutCo(destroyOnComplete));
	}

	private IEnumerator TweenOutCo(bool destroyOnComplete)
	{
		yield return base.transform.DOScale(0f, 0.2f).WaitForCompletion();
		if (destroyOnComplete)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
