using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyEvolveBar : MonoBehaviour
{
	private float _kBarWidth = 214f;

	private RectTransform _fillTrans;

	private HorizontalLayoutGroup _notchContainerGroup;

	private GameObject _notchObj;

	private GameObject _fullyEvolvedObj;

	private Text _title;

	private List<GameObject> _notchObjects;

	private float _evolutionPoints;

	private int _evolutionRequirement;

	private float _fillDefaultWidth;

	private void Awake()
	{
		_notchObjects = new List<GameObject>();
		_fillTrans = base.transform.FindChild("Mask/Fill") as RectTransform;
		_notchContainerGroup = base.transform.FindChild("Mask/NotchContainer").GetComponent<HorizontalLayoutGroup>();
		_notchObj = base.transform.FindChild("Mask/NotchContainer/Notch").gameObject;
		_fullyEvolvedObj = base.transform.FindChild("FullyEvolved").gameObject;
		_title = base.transform.FindChild("Title").GetComponent<Text>();
		_fillDefaultWidth = _fillTrans.rect.width;
		_notchObj.SetActive(false);
		base.transform.FindChild("FullyEvolved/Text").GetComponent<Text>().text = MPTextDefs.MP_MORTY_CELL_FULLY_EVOLVED;
	}

	private void Start()
	{
		_title.text = MPTextDefs.MP_MORTY_STATS_EVOLUTION_TITLE;
	}

	public void SetUp(int evolutionPoints, int evolutionRequirement)
	{
		_evolutionPoints = evolutionPoints;
		_evolutionRequirement = evolutionRequirement;
		if (evolutionPoints == evolutionRequirement)
		{
			_fullyEvolvedObj.SetActive(true);
			return;
		}
		_fullyEvolvedObj.SetActive(false);
		while (_notchObjects.Count > 0)
		{
			Object.Destroy(_notchObjects[0]);
			_notchObjects.RemoveAt(0);
		}
		_notchContainerGroup.spacing = _kBarWidth / (float)evolutionRequirement - 7f;
		while (_notchObjects.Count < evolutionRequirement - 1)
		{
			RectTransform rectTransform = UI.Instantiate(_notchObj, _notchObj.transform.parent);
			_notchObjects.Add(rectTransform.gameObject);
			rectTransform.gameObject.SetActive(true);
		}
		SetEvolutionPoints(_evolutionPoints);
	}

	public void SetEvolutionPoints(float newVal)
	{
		_evolutionPoints = newVal;
		_fillTrans.sizeDelta = new Vector2(_evolutionPoints / (float)_evolutionRequirement * _fillDefaultWidth, _fillTrans.sizeDelta.y);
	}

	public IEnumerator SetEvolutionPointsWithTween(float newVal)
	{
		if (newVal != _evolutionPoints)
		{
			float scaleX = newVal / (float)_evolutionRequirement;
			Tween textTween = DOTween.To(duration: Mathf.Abs(scaleX - _fillTrans.sizeDelta.x / _fillDefaultWidth), getter: () => _evolutionPoints, setter: delegate(float x)
			{
				_evolutionPoints = x;
			}, endValue: newVal);
			textTween.OnUpdate(delegate
			{
				SetEvolutionPoints(_evolutionPoints);
			});
			yield return textTween.WaitForCompletion();
		}
	}
}
