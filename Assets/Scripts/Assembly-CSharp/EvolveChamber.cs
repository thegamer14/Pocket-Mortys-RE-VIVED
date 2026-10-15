using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EvolveChamber : MonoBehaviour
{
	private MPCombineMortySprite _morty;

	private CanvasGroup _tubingOnCanvas;

	private List<Image> _gasImages;

	private void Awake()
	{
		_morty = base.transform.Find("Morty").GetComponent<MPCombineMortySprite>();
		_tubingOnCanvas = base.transform.FindChild("TubingOn").GetComponent<CanvasGroup>();
		_gasImages = new List<Image>();
		for (int i = 0; i < 6; i++)
		{
			_gasImages.Add(base.transform.Find("Gas" + (i + 1)).GetComponent<Image>());
			_gasImages[i].gameObject.SetActive(false);
		}
		_tubingOnCanvas.gameObject.SetActive(false);
	}

	public void SetMorty(string mortyId)
	{
		_morty.SetMorty(mortyId);
	}

	public IEnumerator SetMortyCo(string mortyId)
	{
		yield return StartCoroutine(_morty.SetMortyCo(mortyId));
		yield return new WaitForSeconds(1f);
	}

	public IEnumerator PlaySprayAnimCo()
	{
		yield return StartCoroutine(SetTubingImagesCo(true));
		foreach (Image gasImg in _gasImages)
		{
			gasImg.gameObject.SetActive(true);
			gasImg.color = new Color(1f, 1f, 1f, 0f);
			gasImg.DOFade(1f, 0.2f);
			gasImg.DOFade(0f, 0.4f).SetDelay(0.2f);
			gasImg.transform.localScale = Vector3.zero;
			gasImg.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
		}
		yield return StartCoroutine(SetTubingImagesCo(false));
	}

	private IEnumerator SetTubingImagesCo(bool val)
	{
		if (val)
		{
			_tubingOnCanvas.gameObject.SetActive(true);
			_tubingOnCanvas.alpha = 0f;
			yield return _tubingOnCanvas.DOFade(1f, 0.5f).WaitForCompletion();
		}
		else
		{
			yield return _tubingOnCanvas.DOFade(0f, 0.5f).WaitForCompletion();
			_tubingOnCanvas.gameObject.SetActive(false);
		}
	}
}
