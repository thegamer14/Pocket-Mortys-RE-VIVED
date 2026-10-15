using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlotResult : MonoBehaviour
{
	private Image _floor;

	private Image _result;

	private Image _resultCopy;

	private Image _staticBack;

	private Image _staticFront;

	private Image _blob;

	private Sequence resultSequenceColor;

	private Sequence resultSequenceShake;

	public bool isFilled { get; private set; }

	private void Awake()
	{
		_floor = base.transform.FindChild("Floor").GetComponent<Image>();
		_result = base.transform.FindChild("Result").GetComponent<Image>();
		_resultCopy = base.transform.FindChild("ResultCopy").GetComponent<Image>();
		_staticBack = base.transform.FindChild("StaticBack").GetComponent<Image>();
		_staticFront = base.transform.FindChild("StaticFront").GetComponent<Image>();
		_blob = base.transform.FindChild("Blob").GetComponent<Image>();
	}

	public void SetUp()
	{
		SetSlotEmpty();
		_staticBack.gameObject.SetActive(false);
		_staticFront.gameObject.SetActive(false);
		_blob.gameObject.SetActive(false);
	}

	public void SetSlotEmpty()
	{
		ShowResultImage(false);
		isFilled = false;
	}

	private void SetSlotFilled(string itemAssetId)
	{
		if (string.IsNullOrEmpty(itemAssetId))
		{
			SetSlotEmpty();
			return;
		}
		ShowResultImage(true);
		_result.transform.localPosition = new Vector3(-50f, 10f, 0f);
		_resultCopy.transform.localPosition = new Vector3(53f, 12f, 0f);
		_result.transform.eulerAngles = Vector3.zero;
		_resultCopy.transform.eulerAngles = Vector3.zero;
		_result.color = new Color(1f, 1f, 1f, 0.54f);
		_resultCopy.color = new Color(1f, 1f, 1f, 0.33f);
		_result.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", itemAssetId, "front"));
		_result.SetNativeSize();
		_resultCopy.sprite = _result.sprite;
		_resultCopy.SetNativeSize();
		isFilled = true;
	}

	private void ShowResultImage(bool show)
	{
		_result.gameObject.SetActive(show);
		_resultCopy.gameObject.SetActive(show);
	}

	private void TweenResultImageIdle()
	{
		_result.transform.DOLocalMoveY(6f, 2.2f).SetRelative().SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);
		_result.transform.DORotate(new Vector3(0f, 0f, 4f), 3f).SetRelative().SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine);
		_resultCopy.transform.DOLocalMoveY(6f, 2.2f).SetRelative().SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine)
			.SetDelay(0.4f);
		_resultCopy.transform.DORotate(new Vector3(0f, 0f, 4f), 3f).SetRelative().SetLoops(-1, LoopType.Yoyo)
			.SetEase(Ease.InOutSine)
			.SetDelay(0.4f);
		resultSequenceColor = DOTween.Sequence();
		resultSequenceColor.Append(_resultCopy.DOColor(new Color(1f, 1f, 1f, 0.25f), 0.02f));
		resultSequenceColor.Append(_resultCopy.DOColor(new Color(1f, 1f, 1f, 0.3f), 0.02f));
		resultSequenceColor.AppendInterval(0.05f);
		resultSequenceColor.SetLoops(-1, LoopType.Restart);
		resultSequenceShake = DOTween.Sequence();
		resultSequenceShake.Append(_resultCopy.transform.DOShakePosition(0.05f, 30f, 3));
		resultSequenceShake.AppendInterval(4f);
		resultSequenceShake.SetLoops(-1, LoopType.Restart);
	}

	public IEnumerator TweenInResult(string itemAssetId)
	{
		KillResultTween();
		SetSlotFilled(itemAssetId);
		float startAlpha = _result.color.a;
		float startAlphaCopy = _resultCopy.color.a;
		_result.color = new Color(1f, 1f, 1f, 0f);
		_resultCopy.color = new Color(1f, 1f, 1f, 0f);
		_result.transform.DOLocalMove(new Vector3(50f, 0f), 0.6f).SetRelative();
		_result.DOColor(new Color(1f, 1f, 1f, startAlpha), 0.6f);
		_resultCopy.transform.DOLocalMove(-new Vector3(50f, 0f), 0.6f).SetRelative();
		_resultCopy.DOColor(new Color(1f, 1f, 1f, startAlphaCopy), 0.6f);
		yield return new WaitForSeconds(0.6f);
		TweenResultImageIdle();
	}

	public IEnumerator TweenOutResult()
	{
		KillResultTween();
		float startAlpha = _result.color.a;
		float startAlphaCopy = _resultCopy.color.a;
		_result.transform.DOLocalMove(-new Vector3(50f, 0f), 0.6f).SetRelative();
		_result.DOColor(new Color(1f, 1f, 1f, 0f), 0.6f);
		_resultCopy.transform.DOLocalMove(new Vector3(50f, 0f), 0.6f).SetRelative();
		_resultCopy.DOColor(new Color(1f, 1f, 1f, 0f), 0.6f);
		yield return new WaitForSeconds(1f);
		_result.color = new Color(1f, 1f, 1f, startAlpha);
		_resultCopy.color = new Color(1f, 1f, 1f, startAlphaCopy);
		SetSlotEmpty();
	}

	public IEnumerator TweenInFloor()
	{
		yield return _floor.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
	}

	public IEnumerator TweenInGlow()
	{
		_staticBack.gameObject.SetActive(true);
		_staticFront.gameObject.SetActive(true);
		_blob.gameObject.SetActive(true);
		_staticBack.color = new Color(1f, 1f, 1f, 0f);
		_staticFront.color = new Color(1f, 1f, 1f, 0f);
		_blob.rectTransform.localScale = Vector3.zero;
		_staticBack.DOColor(Color.white, 0.5f);
		_staticFront.DOColor(Color.white, 0.5f);
		yield return _blob.rectTransform.DOScale(1f, 2f);
	}

	private void KillResultTween()
	{
		if (resultSequenceColor != null)
		{
			resultSequenceColor.Kill();
		}
		if (resultSequenceShake != null)
		{
			resultSequenceShake.Kill();
		}
		_result.transform.DOKill();
		_result.DOKill();
		_resultCopy.transform.DOKill();
		_resultCopy.DOKill();
	}
}
