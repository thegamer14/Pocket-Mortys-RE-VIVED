using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackLearnNewBox : MonoBehaviour
{
	private const float _positionInY = 154f;

	private const float _positionOutY = 700f;

	private const float _tweenSpeed = 1000f;

	private MortyElementImage _elementTypeImage;

	private Text _title;

	private Text _name;

	private Text _title1;

	private Text _title2;

	private Text _value1;

	private Text _value2;

	private int _positionX;

	private void Awake()
	{
		_elementTypeImage = base.transform.FindChild("ElementType").GetComponent<MortyElementImage>();
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_name = base.transform.FindChild("Name").GetComponent<Text>();
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

	public void SetUp(AttackDisplayInfo displayInfo, int positionX = 0)
	{
		_title.text = TextDefs.BATTLE_LEARN_NEW;
		_name.text = displayInfo.name;
		_positionX = positionX;
		switch (displayInfo.catagory)
		{
		default:
			SetUpNormal(displayInfo.element, displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Buff:
			SetUpBuff(displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Debuff:
			SetUpDebuff(displayInfo.popUpValue1, displayInfo.popUpValue2);
			break;
		case eAttackCatagory.Golden:
			SetUpGolden(displayInfo.description, displayInfo.element);
			break;
		}
	}

	public void SetUpNormal(string element, string infoValue1, string infoValue2)
	{
		if (!string.IsNullOrEmpty(element))
		{
			_elementTypeImage.Set(element, true);
		}
		else
		{
			_elementTypeImage.SetActive(false);
		}
		_title1.text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_NORMAL_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpBuff(string infoValue1, string infoValue2)
	{
		_elementTypeImage.SetActive(false);
		_title1.text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_BUFF_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpDebuff(string infoValue1, string infoValue2)
	{
		_elementTypeImage.SetActive(false);
		_title1.text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_1;
		_title2.text = TextDefs.ATTACK_STAT_POPUP_DEBUFF_TITLE_2;
		_value1.text = infoValue1;
		_value2.text = infoValue2;
	}

	public void SetUpGolden(string description, string element)
	{
		if (!string.IsNullOrEmpty(element))
		{
			_elementTypeImage.Set(element, true);
		}
		else
		{
			_elementTypeImage.SetActive(false);
		}
		_title1.text = description;
	}

	public void SetPositionIn()
	{
		base.gameObject.SetActive(true);
		base.transform.localPosition = new Vector2(_positionX, 154f);
	}

	public void SetPositionOut()
	{
		base.transform.localPosition = new Vector2(_positionX, 700f);
		base.gameObject.SetActive(false);
	}

	public IEnumerator TweenIn()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(true);
		yield return StartCoroutine(TweenInCo());
	}

	private IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		Tween tween = ShortcutExtensions.DOLocalMoveY(duration: Mathf.Abs((base.transform.localPosition.y - 154f) / 1000f), target: base.transform, endValue: 154f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOut(bool destroy = false)
	{
		StopAllCoroutines();
		if (base.gameObject.activeSelf)
		{
			yield return StartCoroutine(TweenOutCo());
		}
		if (destroy)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private IEnumerator TweenOutCo()
	{
		base.transform.DOKill();
		Tween tween = ShortcutExtensions.DOLocalMoveY(duration: Mathf.Abs((base.transform.localPosition.y - 700f) / 1000f), target: base.transform, endValue: 700f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}
}
