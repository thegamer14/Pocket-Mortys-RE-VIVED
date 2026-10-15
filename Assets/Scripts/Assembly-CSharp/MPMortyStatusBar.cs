using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPMortyStatusBar : MonoBehaviour
{
	private Text _title;

	private GameObject _faintedIcon;

	private GameObject _poisonedIcon;

	private GameObject _paralysedIcon;

	private void Awake()
	{
		_faintedIcon = base.transform.FindChild("Fainted").gameObject;
		_poisonedIcon = base.transform.FindChild("Poisoned").gameObject;
		_paralysedIcon = base.transform.FindChild("Paralysed").gameObject;
		Transform transform = base.transform.FindChild("Title");
		if (transform != null)
		{
			_title = transform.GetComponent<Text>();
		}
	}

	public void SetUp(MPOpponentMortyData morty, bool withAnim = false)
	{
		if (morty == null)
		{
			SetIcon(_faintedIcon, false);
			SetIcon(_poisonedIcon, false);
			SetIcon(_paralysedIcon, false);
			return;
		}
		bool newActiveVal = morty != null && morty.isFainted;
		bool newActiveVal2 = morty != null && morty.isPoisoned;
		bool newActiveVal3 = morty != null && morty.isParalysed;
		if (withAnim)
		{
			StartCoroutine(SetIconWithAnimation(_faintedIcon, newActiveVal));
			StartCoroutine(SetIconWithAnimation(_poisonedIcon, newActiveVal2));
			StartCoroutine(SetIconWithAnimation(_paralysedIcon, newActiveVal3));
		}
		else
		{
			SetIcon(_faintedIcon, newActiveVal);
			SetIcon(_poisonedIcon, newActiveVal2);
			SetIcon(_paralysedIcon, newActiveVal3);
		}
	}

	public void SetTitle(string text)
	{
		_title.text = text;
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
