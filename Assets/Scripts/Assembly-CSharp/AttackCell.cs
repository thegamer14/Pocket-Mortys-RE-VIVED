using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackCell : CellBase
{
	[HideInInspector]
	public AttackData data;

	protected override void Awake()
	{
		base.Awake();
	}

	public void SetUp(AttackData attackData)
	{
		data = attackData;
		base.SetUp(attackData.id);
		base.transform.FindChild("NameLabel").GetComponent<Text>().text = attackData.name;
		base.transform.FindChild("Element").GetComponent<MortyElementImage>().Set(attackData.element);
		base.transform.FindChild("PPLabel").GetComponent<Text>().text = string.Format("{0}: {1}", TextDefs.MORTY_STAT_PP, attackData.pp);
		eAttackCatagory attackCatagory = AttackDefs.GetAttackCatagory(attackData.id);
		switch (attackCatagory)
		{
		case eAttackCatagory.Golden:
			base.transform.FindChild("Stat1Label").gameObject.SetActive(false);
			base.transform.FindChild("Stat2Label").gameObject.SetActive(false);
			base.transform.FindChild("DescriptionLabel").GetComponent<Text>().text = attackData.description;
			break;
		case eAttackCatagory.Buff:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_1 + " " + TextDefs.GetMortyStatTextForType(attackData.effectDatas[0].stat);
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_2 + " " + AttackDefs.GetAttackStatEffectTitle(attackData.effectDatas[0]);
			break;
		case eAttackCatagory.Debuff:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_1 + " " + TextDefs.GetMortyStatTextForType(attackData.effectDatas[0].stat);
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_2 + " " + AttackDefs.GetAttackStatEffectTitle(attackData.effectDatas[0]);
			break;
		case eAttackCatagory.Normal:
			base.transform.FindChild("DescriptionLabel").gameObject.SetActive(false);
			base.transform.FindChild("Stat1Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_1 + " " + attackData.effectDatas[0].power;
			base.transform.FindChild("Stat2Label").GetComponent<Text>().text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_2 + " " + attackData.effectDatas[0].accuracy * 100f + "%";
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
