using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPCombineMortySprite : MonoBehaviour
{
	private const float _kOutByY = 1000f;

	private const float _kAnimDuration = 0.2f;

	public string netId;

	public float inY;

	private RectTransform _rectTrans;

	private Image _image;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_image = GetComponent<Image>();
		inY = _rectTrans.anchoredPosition.y;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, inY - 1000f);
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, inY);
	}

	public IEnumerator TweenOutCo()
	{
		_rectTrans.DOKill();
		yield return ShortcutExtensions.DOLocalMoveY(duration: (_rectTrans.anchoredPosition.y - (inY - 1000f)) / (inY - (inY - 1000f)) * 0.2f, target: _rectTrans, endValue: inY - 1000f).SetEase(Ease.InQuad).WaitForCompletion();
	}

	public IEnumerator SetMortyCo(string mortyId)
	{
		yield return StartCoroutine(TweenOutCo());
		if (!string.IsNullOrEmpty(mortyId))
		{
			_image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(mortyId), "front"));
			_image.SetNativeSize();
			yield return _rectTrans.DOLocalMoveY(inY, 0.2f).SetEase(Ease.OutBack).WaitForCompletion();
		}
	}

	public IEnumerator AnimateOutAndDestroyCo()
	{
		yield return StartCoroutine(TweenOutCo());
		Object.Destroy(base.gameObject);
	}

	public void SetMorty(string mortyId)
	{
		if (string.IsNullOrEmpty(mortyId))
		{
			_image.sprite = null;
			SetPositionOut();
		}
		else
		{
			_image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(mortyId), "front"));
			_image.SetNativeSize();
			SetPositionIn();
		}
	}
}
