using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleTrainer : MonoBehaviour
{
	private const int _kInPositionLeftX = -410;

	private const int _kInPositionLeftY = -65;

	private const int _kInPositionRightX = 420;

	private const int _kInPositionRightY = 0;

	private const int _kInPlayerLevelPositionX = -150;

	private const float _kInOutDuration = 0.35f;

	private RectTransform _rectTrans;

	private Image _image;

	private eBattleOpponentSide _side;

	private string _assetId;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_image = base.transform.FindChild("Image").GetComponent<Image>();
	}

	public void SetUp(string assetId, eBattleOpponentSide side)
	{
		_side = side;
		SetImage(assetId);
	}

	private void SetImage(string assetId, bool isFlipped = false)
	{
		_assetId = assetId;
		if (_side == eBattleOpponentSide.RIGHT)
		{
			_image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", _assetId, "front"));
		}
		else
		{
			_image.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", _assetId, "back"));
		}
		if (isFlipped)
		{
			_image.transform.localScale = new Vector3(-1f, 1f, 1f);
			if (_side == eBattleOpponentSide.RIGHT)
			{
				Sprite sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", _assetId, "frontFlipped"));
				if (sprite != null)
				{
					_image.sprite = sprite;
				}
			}
		}
		_image.SetNativeSize();
	}

	public void SetPositionIn()
	{
		base.transform.DOKill();
		base.gameObject.SetActive(true);
		_rectTrans.anchoredPosition = new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), (_side == eBattleOpponentSide.LEFT) ? (-65) : 0);
	}

	public void SetPositionOut()
	{
		base.transform.DOKill();
		float num = App.states.mainCanvas.rectTransform.sizeDelta.x / 2f + _image.rectTransform.sizeDelta.x / 2f;
		if (_side == eBattleOpponentSide.LEFT)
		{
			num *= -1f;
		}
		_rectTrans.anchoredPosition = new Vector2(num, (_side == eBattleOpponentSide.LEFT) ? (-65) : 0);
		base.gameObject.SetActive(false);
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		SetPositionOut();
		base.gameObject.SetActive(true);
		Tween tween = _rectTrans.DOAnchorPos(new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), (_side == eBattleOpponentSide.LEFT) ? (-65) : 0), 0.35f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenInPlayerLevelCo()
	{
		base.transform.DOKill();
		SetPositionOut();
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, 0f);
		_side = eBattleOpponentSide.RIGHT;
		SetImage(_assetId, true);
		base.gameObject.SetActive(true);
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(-150f, 0f), 0.35f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOutCo()
	{
		base.transform.DOKill();
		float posX = App.states.mainCanvas.rectTransform.sizeDelta.x / 2f + _image.rectTransform.sizeDelta.x / 2f;
		if (_side == eBattleOpponentSide.LEFT)
		{
			posX *= -1f;
		}
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(posX, _rectTrans.anchoredPosition.y), 0.35f);
		yield return tween.WaitForCompletion();
		base.gameObject.SetActive(false);
	}
}
