using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MPEmoteBubble : MonoBehaviour
{
	public float OffX;

	private float _onX;

	private float _emoteScale = 1.2f;

	private RectTransform _thisRectTrans;

	private Transform _emoteContainerTrans;

	private Coroutine _animCoroutine;

	private RectTransform _emoteRect;

	private void Awake()
	{
		_thisRectTrans = base.transform as RectTransform;
		_emoteContainerTrans = base.transform.FindChild("EmoteContainer");
		_onX = _thisRectTrans.anchoredPosition.x;
		base.gameObject.SetActive(false);
	}

	public void SnapOff()
	{
		if (_animCoroutine != null)
		{
			StopCoroutine(_animCoroutine);
			_animCoroutine = null;
			_thisRectTrans.DOKill();
			if (_emoteRect != null)
			{
				Object.Destroy(_emoteRect.gameObject);
			}
		}
		_thisRectTrans.anchoredPosition = new Vector2(OffX, _thisRectTrans.anchoredPosition.y);
		base.gameObject.SetActive(false);
	}

	public void AnimateOn(string emoteId)
	{
		if (_animCoroutine == null)
		{
			base.gameObject.SetActive(true);
			_animCoroutine = StartCoroutine(AnimateOnCo(emoteId));
		}
	}

	private IEnumerator AnimateOnCo(string emoteId)
	{
		_thisRectTrans.anchoredPosition = new Vector2(OffX, _thisRectTrans.anchoredPosition.y);
		_emoteRect = UI.Instantiate(string.Format("UI/MPBattle/Emotes/{0}", emoteId), _emoteContainerTrans);
		if (_emoteRect == null)
		{
			_animCoroutine = null;
			yield break;
		}
		_emoteRect.localScale = new Vector3(_emoteScale, _emoteScale, 1f);
		yield return new WaitForEndOfFrame();
		yield return _thisRectTrans.DOAnchorPos(new Vector2(_onX, _thisRectTrans.anchoredPosition.y), 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();
		yield return new WaitForSeconds(1.5f);
		yield return _thisRectTrans.DOAnchorPos(new Vector2(OffX, _thisRectTrans.anchoredPosition.y), 0.1f).SetEase(Ease.Linear).WaitForCompletion();
		Object.Destroy(_emoteRect.gameObject);
		base.gameObject.SetActive(false);
		_animCoroutine = null;
	}
}
