using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class BattleTutorialTipTypesUI : BattleTutorialTipUI
{
	private RectTransform _typeBoxRectTrans;

	private RectTransform _typePointerRectTrans;

	protected override void Awake()
	{
		base.Awake();
		_typeBoxRectTrans = base.transform.FindChild("TypeBox").GetComponent<RectTransform>();
		_typePointerRectTrans = base.transform.FindChild("TypePointer").GetComponent<RectTransform>();
	}

	public override void SetUp(string description, Vector2 descriptionPosition, Vector2 arrowPosition, int arrowRotation)
	{
		base.SetUp(description, descriptionPosition, arrowPosition, arrowRotation);
		_typeBoxRectTrans.localScale = Vector3.zero;
		_typePointerRectTrans.localScale = Vector3.one * 2f;
	}

	protected override IEnumerator TweenIn()
	{
		yield return _typePointerRectTrans.DOScale(1f, 1f).WaitForCompletion();
		_typePointerRectTrans.DORotate(new Vector3(0f, 0f, -360f), 3f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
		_typePointerRectTrans.DOScale(1.2f, 1f).SetLoops(-1, LoopType.Yoyo);
		yield return new WaitForSeconds(0.4f);
		_typeBoxRectTrans.DOScale(1f, 0.2f);
		yield return StartCoroutine(_003CTweenIn_003E__BaseCallProxy0());
	}

	protected override IEnumerator TweenOut()
	{
		_typeBoxRectTrans.DOScale(0f, 0.2f);
		yield return StartCoroutine(_003CTweenOut_003E__BaseCallProxy1());
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CTweenIn_003E__BaseCallProxy0()
	{
		return base.TweenIn();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CTweenOut_003E__BaseCallProxy1()
	{
		return base.TweenOut();
	}
}
