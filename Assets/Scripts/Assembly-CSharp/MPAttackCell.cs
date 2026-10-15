using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPAttackCell : CellBase
{
	[HideInInspector]
	public MPAttackData data;

	protected override void Awake()
	{
		base.Awake();
	}

	public void SetUp(MPAttackData attackData)
	{
		data = attackData;
		base.SetUp(attackData.id);
		base.transform.FindChild("NameLabel").GetComponent<Text>().text = MPAttackDefs.Name(attackData.id);
		base.transform.FindChild("Element").GetComponent<MortyElementImage>().Set(MPAttackDefs.ElementType(attackData.id));
		base.transform.FindChild("PPLabel").GetComponent<Text>().text = string.Format("{0}: {1}", TextDefs.MORTY_STAT_PP, MPAttackDefs.PP(attackData.id));
		AttackDisplayInfo attackDisplayInfo = new AttackDisplayInfo(attackData.id, attackData.PP);
		eAttackCatagory attackCatagory = MPAttackDefs.GetAttackCatagory(attackData.id);
		switch (attackCatagory)
		{
		case eAttackCatagory.Golden:
			base.transform.FindChild("Stat1Label").gameObject.SetActive(false);
			base.transform.FindChild("Stat2Label").gameObject.SetActive(false);
			base.transform.FindChild("DescriptionLabel").GetComponent<Text>().text = attackDisplayInfo.description;
			break;
		case eAttackCatagory.Buff:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_1 + " " + attackDisplayInfo.popUpValue1;
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_2 + " " + attackDisplayInfo.popUpValue2;
			break;
		case eAttackCatagory.Debuff:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_1 + " " + attackDisplayInfo.popUpValue1;
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_2 + " " + attackDisplayInfo.popUpValue2;
			break;
		case eAttackCatagory.Normal:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_1 + " " + attackDisplayInfo.popUpValue1;
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_2 + " " + attackDisplayInfo.popUpValue2;
			break;
		}
		Sprite sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Cells/Attacks/{0}", attackCatagory.ToString()));
		base.transform.FindChild("Background").GetComponent<Image>().sprite = sprite;
		_defaultSpr = sprite;
		selectedSpr = sprite;
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
