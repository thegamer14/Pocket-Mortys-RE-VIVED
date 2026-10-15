using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NetworkErrorIcon : MonoBehaviour
{
	private Coroutine _animateOnCo;

	private Image _image;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void ShowWithAnim()
	{
		if (_animateOnCo != null)
		{
			StopCoroutine(_animateOnCo);
		}
		base.gameObject.SetActive(true);
		_animateOnCo = StartCoroutine(ShowWithAnimCo());
	}

	private IEnumerator ShowWithAnimCo()
	{
		base.transform.localScale = Vector3.zero;
		_image.color = new Color(1f, 1f, 1f, 0f);
		_image.DOFade(1f, 0.2f);
		yield return base.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
		for (int i = 0; i < 3; i++)
		{
			yield return base.transform.DOScale(1.2f, 0.2f).SetEase(Ease.InCirc).WaitForCompletion();
			yield return base.transform.DOScale(1f, 0.2f).SetEase(Ease.OutCirc).WaitForCompletion();
		}
		_image.DOFade(0f, 0.2f);
		yield return base.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).WaitForCompletion();
		base.gameObject.SetActive(false);
		_animateOnCo = null;
	}
}
