using System.Collections;
using DG.Tweening;
using UnityEngine;

public class NetworkFriendBattleIcon : MonoBehaviour
{
	private const float _kAnimTime = 0.2f;

	private Coroutine _animInCo;

	private Coroutine _animOutCo;

	public void Hide()
	{
		base.transform.localScale = Vector3.zero;
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
		yield return base.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
		_animOutCo = null;
		base.gameObject.SetActive(false);
	}
}
