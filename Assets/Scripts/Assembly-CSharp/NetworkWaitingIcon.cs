using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NetworkWaitingIcon : MonoBehaviour
{
	private const float _kAnimTime = 0.2f;

	private Vector3 _waitingRotBy = new Vector3(0f, 0f, -400f);

	private Coroutine _animInCo;

	private Coroutine _animOutCo;

	private Image _image;

	private void Awake()
	{
		_image = GetComponent<Image>();
	}

	private void Update()
	{
		base.transform.Rotate(_waitingRotBy * Time.deltaTime);
	}

	public void Hide()
	{
		base.transform.localScale = Vector3.zero;
		_image.color = new Color(1f, 1f, 1f, 0f);
		base.gameObject.SetActive(false);
	}

	public void ShowWithAnim()
	{
		if (_animInCo == null)
		{
			if (_animOutCo != null)
			{
				StopCoroutine(_animOutCo);
				_animOutCo = null;
			}
			base.gameObject.SetActive(true);
			_animInCo = StartCoroutine(ShowWithAnimCo());
		}
	}

	private IEnumerator ShowWithAnimCo()
	{
		_image.DOFade(1f, 0.2f);
		yield return base.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
		_animInCo = null;
	}

	public void HideWithAnim()
	{
		if (_animOutCo == null)
		{
			if (_animInCo != null)
			{
				StopCoroutine(_animInCo);
				_animInCo = null;
			}
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
				_animOutCo = StartCoroutine(HideWithAnimCo());
			}
		}
	}

	private IEnumerator HideWithAnimCo()
	{
		_image.DOFade(0f, 0.2f);
		yield return base.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
		_animOutCo = null;
		base.gameObject.SetActive(false);
	}
}
