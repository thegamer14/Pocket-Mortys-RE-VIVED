using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleMorty : MonoBehaviour
{
	private const int _kInPositionLeftX = -410;

	private const int _kInPositionLeftY = -440;

	private const int _kFaintPositionLeftY = -1000;

	private const int _kInPositionRightX = 420;

	private const int _kInPositionRightY = 345;

	private const int _kFaintPositionRightY = -400;

	private const int _kIdleMoveAmountX = 30;

	private const float _kInOutDuration = 0.35f;

	private const float _kFaintDuration = 1.75f;

	private const float _kShakeDuration = 0.8f;

	private const float _kShakeAmount = 50f;

	private RectTransform _rectTrans;

	private Image _image;

	private Transform faintedStars;

	private Sequence idleTween;

	private eBattleOpponentSide _side;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_image = base.transform.FindChild("Image").GetComponent<Image>();
		faintedStars = base.transform.FindChild("FaintedStars");
	}

	public void SetUp(eBattleOpponentSide side)
	{
		_side = side;
		faintedStars.gameObject.SetActive(false);
	}

	public IEnumerator SetImage(string assetId, bool flip = false)
	{
		Sprite newSprite = null;
		newSprite = ((_side != eBattleOpponentSide.LEFT || flip) ? Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetId, "front")) : Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetId, "back")));
		_image.sprite = newSprite;
		_image.SetNativeSize();
		yield return new WaitForEndOfFrame();
	}

	public void PrePositionSet()
	{
		base.transform.DOKill();
		faintedStars.gameObject.SetActive(false);
	}

	public void SetPositionIn()
	{
		PrePositionSet();
		_rectTrans.anchoredPosition = new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), (_side != eBattleOpponentSide.LEFT) ? 345 : (-440));
	}

	public void SetPositionOut()
	{
		PrePositionSet();
		float num = App.states.mainCanvas.rectTransform.sizeDelta.x / 2f + _image.rectTransform.sizeDelta.x / 2f + 30f;
		if (_side == eBattleOpponentSide.LEFT)
		{
			num *= -1f;
		}
		_rectTrans.anchoredPosition = new Vector2(num, (_side != eBattleOpponentSide.LEFT) ? 345 : (-440));
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		PrePositionSet();
		SetPositionOut();
		Tween tween = _rectTrans.DOAnchorPos(new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), (_side != eBattleOpponentSide.LEFT) ? 345 : (-440)), 0.35f);
		yield return tween.WaitForCompletion();
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		PrePositionSet();
		float posX = App.states.mainCanvas.rectTransform.sizeDelta.x / 2f + _image.rectTransform.sizeDelta.x / 2f;
		if (_side == eBattleOpponentSide.LEFT)
		{
			posX *= -1f;
		}
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(posX, _rectTrans.anchoredPosition.y), 0.35f);
		yield return tween.WaitForCompletion();
	}

	public void TweenFaint()
	{
		StartCoroutine(TweenFaintCo());
	}

	public IEnumerator TweenFaintCo()
	{
		base.transform.DOKill();
		faintedStars.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.4f);
		yield return _rectTrans.DOAnchorPos(new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), _rectTrans.anchoredPosition.y + 20f), 0.15f).WaitForCompletion();
		yield return _rectTrans.DOAnchorPos(new Vector2((_side != eBattleOpponentSide.LEFT) ? 420 : (-410), (_side != eBattleOpponentSide.LEFT) ? (-400) : (-1000)), 1.75f).WaitForCompletion();
	}

	public void TweenHit()
	{
		_rectTrans.DOPunchPosition(new Vector3(50f, 0f), 0.8f);
	}

	public void StartIdleTween(bool isScaleIn)
	{
		if (idleTween == null)
		{
			idleTween = DOTween.Sequence();
			idleTween.Insert(0f, _image.transform.DOLocalMoveX((_side != eBattleOpponentSide.LEFT) ? (_image.transform.localPosition.x + 30f) : (_image.transform.localPosition.x - 30f), 3.5f));
			idleTween.Insert(0f, _image.transform.DOScale((!isScaleIn) ? 1f : 1.04f, 3.5f));
			idleTween.SetEase(Ease.InOutSine);
			idleTween.SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			idleTween.Play();
		}
	}

	public void StopIdleTween()
	{
		if (idleTween != null)
		{
			idleTween.Pause();
		}
	}
}
