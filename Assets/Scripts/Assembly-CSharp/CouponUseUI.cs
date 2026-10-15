using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CouponUseUI : UIBase
{
	public delegate void UICouponUseBtnDelegate();

	public UICouponUseBtnDelegate BackEvent;

	public UICouponUseBtnDelegate BuyCouponsEvent;

	public UICouponUseBtnDelegate UseEvent;

	private BattleBackground _background;

	private RectTransform _machineTrans;

	private RectTransform _balls1;

	private RectTransform _balls2;

	private RectTransform _crankTrans;

	private RectTransform _prizeBall;

	private RectTransform _rickTrans;

	private Image _rickImg;

	private RectTransform _mortyTrans;

	private Transform _3DBall;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private CouponBar _couponBar;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background").GetComponent<BattleBackground>();
		_machineTrans = (RectTransform)base.transform.FindChild("Machine");
		_balls1 = (RectTransform)base.transform.FindChild("Machine/Balls1");
		_balls2 = (RectTransform)base.transform.FindChild("Machine/Balls2");
		_crankTrans = (RectTransform)base.transform.FindChild("Machine/Crank");
		_prizeBall = (RectTransform)base.transform.FindChild("Machine/PrizeBall");
		_rickTrans = (RectTransform)base.transform.FindChild("Rick");
		_rickImg = base.transform.FindChild("Rick").GetComponent<Image>();
		_mortyTrans = (RectTransform)base.transform.FindChild("Morty");
		_3DBall = base.transform.FindChild("3DBall");
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_couponBar = base.transform.FindChild("MenuBar/MainMenu/CouponBar").GetComponent<CouponBar>();
	}

	public void SetUp()
	{
		_mainMenu.SetBtnEnabled(1, PlayerDefs.CouponCount > 0);
		_couponBar.SetUp(PlayerDefs.CouponCount);
		string worldID = SharedStatInfo.worldInfo.worldID;
		string nodeSetID = SharedStatInfo.worldInfo.nodeSetID;
		if ((!string.IsNullOrEmpty(worldID) && !worldID.Equals("Citadel")) || (!string.IsNullOrEmpty(nodeSetID) && !nodeSetID.Equals("Citadel")))
		{
			_background.SetBackground(worldID, nodeSetID);
		}
		else
		{
			_background.SetBackground("Summer", "Summer");
		}
		_rickImg.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", PlayerDefs.AssetID, "back"));
		_rickImg.SetNativeSize();
	}

	public override void SetLabels()
	{
		_couponBar.SetLabels();
		_mainMenu.SetBtnTitle(1, TextDefs.COUPON_USE_BTN);
		if (PlayerDefs.CouponCount > 0)
		{
			_dialogue.SetText(TextDefs.COUPON_USE_TEXT);
		}
		else
		{
			_dialogue.SetText(TextDefs.COUPON_USE_OUT_TEXT);
		}
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		PlayerDefs.CouponCountChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		PlayerDefs.CouponCountChangedEvent -= CouponCountChangedHandler;
	}

	public void RewardFinished()
	{
		StartCoroutine(_mainMenu.TweenIn());
	}

	private IEnumerator UseCouponCo()
	{
		_dialogue.TweenText(TextDefs.COUPON_USE_SPENT_TEXT);
		StartCoroutine(_mainMenu.TweenOut());
		float cameraDuration = 3f;
		_machineTrans.DOAnchorPos(new Vector2(0f, 250f), cameraDuration).SetEase(Ease.InOutQuad).SetRelative(true);
		_rickTrans.DOAnchorPos(new Vector2(0f, 70f), cameraDuration).SetEase(Ease.InOutQuad).SetRelative(true);
		_mortyTrans.DOAnchorPos(new Vector2(0f, 70f), cameraDuration).SetEase(Ease.InOutQuad).SetRelative(true);
		_background.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 110f), cameraDuration).SetEase(Ease.InOutQuad)
			.SetRelative(true);
		App.sound.PlaySFX("Audio/SFX/GachaCrank");
		_balls1.DOShakeAnchorPos(1.3f, 5f, 6);
		_balls2.DOShakeAnchorPos(1.1f, 5f, 6);
		yield return _crankTrans.DORotate(new Vector3(0f, 0f, 120f), 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/GachaCrank");
		_balls1.DOShakeAnchorPos(1f, 5f, 6);
		_balls2.DOShakeAnchorPos(1.3f, 5f, 6);
		yield return _crankTrans.DORotate(new Vector3(0f, 0f, 240f), 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/GachaCrank");
		_balls1.DOShakeAnchorPos(1.3f, 5f, 6);
		_balls2.DOShakeAnchorPos(1f, 5f, 6);
		yield return _crankTrans.DORotate(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/GachaBallDrop");
		_prizeBall.DOAnchorPos(new Vector2(_prizeBall.anchoredPosition.x, -178f), 0.3f).SetEase(Ease.OutBounce);
		_prizeBall.DORotate(new Vector3(0f, 0f, -30f), 0.8f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(0.2f);
		_3DBall.DORotate(new Vector3(41f, 148f, 17f), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).SetDelay(0.5f);
		yield return _3DBall.DOLocalMoveY(0f, 1.8f).SetEase(Ease.OutBounce).WaitForCompletion();
		yield return new WaitForSeconds(0.3f);
		App.sound.PlaySFX("Audio/SFX/GachaBallShake");
		for (int i = 1; i <= 8; i++)
		{
			_3DBall.DOShakeRotation(0.1f, 7f, 10, 10f);
			yield return _3DBall.DOPunchPosition(new Vector3(3 * i, 3 * i, 0f), 0.1f).WaitForCompletion();
		}
		yield return _3DBall.DOScale(_3DBall.localScale.x * 3f, 0.2f).SetEase(Ease.InQuart).WaitForCompletion();
		_3DBall.gameObject.SetActive(false);
		if (UseEvent != null)
		{
			UseEvent();
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (BuyCouponsEvent != null)
			{
				BuyCouponsEvent();
			}
			break;
		case 1:
			StartCoroutine(UseCouponCo());
			break;
		case 2:
			if (BackEvent != null)
			{
				BackEvent();
			}
			break;
		}
	}

	private void CouponCountChangedHandler(int CouponCount)
	{
		_couponBar.SetCount(CouponCount);
		if (PlayerDefs.CouponCount > 0)
		{
			_dialogue.SetText(TextDefs.COUPON_USE_TEXT);
		}
		else
		{
			_dialogue.SetText(TextDefs.COUPON_USE_OUT_TEXT);
		}
		_mainMenu.SetBtnEnabled(1, CouponCount > 0);
	}
}
