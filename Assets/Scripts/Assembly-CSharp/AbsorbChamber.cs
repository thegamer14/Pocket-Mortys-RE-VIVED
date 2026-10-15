using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbsorbChamber : MonoBehaviour
{
	private const float _kShakeSpeed = 40f;

	private const float _kMaxShakeAmount = 20f;

	private const float _kMaxBubbleSpeedMult = 20f;

	public float _shakeAmount;

	public float _bubbleSpeedMult = 1f;

	private Vector2 _morty2Pos = new Vector2(110f, 36f);

	private Vector2 _morty3Pos = new Vector2(-110f, 36f);

	private MPCombineMortySprite _morty1;

	private MPCombineMortySprite _morty2;

	private MPCombineMortySprite _morty3;

	private RectTransform _mortysRectTrans;

	private CanvasGroup _mortysCanvasGroup;

	private GameObject _mortySpriteObj;

	private List<MPCombineMortySprite> _mortySprites;

	private AbsorbChamberBubbles[] _bubbles;

	private Image _slushOverlayImg;

	private RectTransform _rectTrans;

	private Image _blend1Img;

	private Image _blend2Img;

	private Tween _shakeTween;

	private Tween _bubbleSpeedTween;

	private void Awake()
	{
		_mortySprites = new List<MPCombineMortySprite>();
		_mortysRectTrans = base.transform.FindChild("Mortys").GetComponent<RectTransform>();
		_mortysCanvasGroup = base.transform.FindChild("Mortys").GetComponent<CanvasGroup>();
		_mortySpriteObj = base.transform.FindChild("Mortys/Morty").gameObject;
		_rectTrans = GetComponent<RectTransform>();
		_bubbles = GetComponentsInChildren<AbsorbChamberBubbles>();
		_slushOverlayImg = base.transform.FindChild("SlushOverlay").GetComponent<Image>();
		_blend1Img = base.transform.FindChild("BlendSpriteAnim1").GetComponent<Image>();
		_blend2Img = base.transform.FindChild("BlendSpriteAnim2").GetComponent<Image>();
		StopAbsorbAnim();
	}

	public void AddMorty(MPOpponentMortyData mortyData)
	{
		RectTransform rectTransform = UI.Instantiate(_mortySpriteObj, _mortySpriteObj.transform.parent, eUISibling.FIRST);
		MPCombineMortySprite component = rectTransform.GetComponent<MPCombineMortySprite>();
		_mortySprites.Add(component);
		if (_morty1 == null)
		{
			_morty1 = component;
		}
		else if (_morty2 == null)
		{
			_morty2 = component;
			rectTransform.anchoredPosition = _morty2Pos;
		}
		else if (_morty3 == null)
		{
			_morty3 = component;
			rectTransform.anchoredPosition = _morty3Pos;
		}
		else
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + Random.Range(-130f, 130f), rectTransform.anchoredPosition.y + Random.Range(20f, 50f));
		}
		component.inY = rectTransform.anchoredPosition.y;
		component.netId = mortyData.netId;
		component.gameObject.SetActive(true);
		component.SetPositionOut();
		StartCoroutine(component.SetMortyCo(mortyData.id));
	}

	public void RemoveMorty(MPOpponentMortyData mortyData)
	{
		MPCombineMortySprite mPCombineMortySprite = _mortySprites.Find((MPCombineMortySprite s) => s.netId.Equals(mortyData.netId));
		if (mPCombineMortySprite != null)
		{
			_mortySprites.Remove(mPCombineMortySprite);
			StartCoroutine(mPCombineMortySprite.AnimateOutAndDestroyCo());
		}
	}

	public IEnumerator PlayAbsorbAnimCo()
	{
		StartShakeAnim();
		SpeedUpBubbles();
		FadeSlushOverlay();
		FadeOutMortys();
		yield return StartCoroutine(FadeInBlendSpritesCo());
		SlowDownBubbles();
		yield return StartCoroutine(FadeOutBlendSpritesCo());
	}

	public void StopAbsorbAnim()
	{
		_shakeTween.Kill();
		_bubbleSpeedTween.Kill();
		_slushOverlayImg.DOKill();
		_blend1Img.DOKill();
		_blend2Img.DOKill();
		_shakeAmount = 0f;
		_mortysRectTrans.anchoredPosition = new Vector2(0f, _mortysRectTrans.anchoredPosition.y);
		_mortysCanvasGroup.alpha = 1f;
		SetBubbleSpeedMult(1f);
		_mortySpriteObj.SetActive(false);
		_slushOverlayImg.gameObject.SetActive(false);
		_blend1Img.gameObject.SetActive(false);
		_blend2Img.gameObject.SetActive(false);
	}

	private void StartShakeAnim()
	{
		_shakeTween = DOTween.To(() => _shakeAmount, delegate(float x)
		{
			_shakeAmount = x;
		}, 20f, 5f);
	}

	private void SpeedUpBubbles()
	{
		_bubbleSpeedTween = DOTween.To(() => _bubbleSpeedMult, delegate(float x)
		{
			SetBubbleSpeedMult(x);
		}, 20f, 3f);
	}

	private void SlowDownBubbles()
	{
		_bubbleSpeedTween = DOTween.To(() => _bubbleSpeedMult, delegate(float x)
		{
			SetBubbleSpeedMult(x);
		}, 1f, 3f);
	}

	private void SetBubbleSpeedMult(float val)
	{
		_bubbleSpeedMult = val;
		AbsorbChamberBubbles[] bubbles = _bubbles;
		foreach (AbsorbChamberBubbles absorbChamberBubbles in bubbles)
		{
			absorbChamberBubbles.speedMult = _bubbleSpeedMult;
		}
	}

	private void FadeSlushOverlay()
	{
		_slushOverlayImg.gameObject.SetActive(true);
		_slushOverlayImg.color = new Color(1f, 1f, 1f, 0f);
		_slushOverlayImg.DOFade(1f, 5f);
	}

	private void FadeOutMortys()
	{
		_mortysCanvasGroup.DOFade(0f, 2f).SetDelay(2f);
	}

	private IEnumerator FadeInBlendSpritesCo()
	{
		_blend1Img.gameObject.SetActive(true);
		_blend1Img.color = new Color(1f, 1f, 1f, 0f);
		_blend1Img.DOFade(1f, 2f);
		_blend2Img.gameObject.SetActive(true);
		_blend2Img.color = new Color(1f, 1f, 1f, 0f);
		yield return _blend2Img.DOFade(1f, 2f).WaitForCompletion();
	}

	private IEnumerator FadeOutBlendSpritesCo()
	{
		_blend1Img.DOFade(0f, 2f);
		yield return _blend2Img.DOFade(0f, 2f).WaitForCompletion();
	}

	private void Update()
	{
		if (_shakeAmount > 0f)
		{
			_mortysRectTrans.anchoredPosition = new Vector2(Mathf.Sin(Time.time * 40f) * _shakeAmount, _rectTrans.anchoredPosition.y);
		}
	}
}
