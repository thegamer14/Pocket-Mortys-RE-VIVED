using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPRegisterAvatarUI : UIBase
{
	public delegate void CustomizationUIIDBtnDelegate(string avatarID);

	public delegate void CustomizationUIBtnDelegate();

	private const int _kAvatarMenuMinimumCellCount = 5;

	private const float _kAvatarMenuPaddingTop = 85f;

	private const float _kAvatarMenuPaddingBottom = 100f;

	private const float _kAvatarMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private RectTransform _hologramTrans;

	private Image _hologramImg;

	private Image _avatarImg;

	private Image _avatarOverlayImg;

	private ScrollMenu _avatarMenu;

	private string selectedAvatarID;

	private Coroutine _animateAvatarCo;

	private Color _defaultAvatarOverlayColor;

	public event CustomizationUIIDBtnDelegate UseEvent;

	public event CustomizationUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_avatarMenu = base.transform.FindChild("AvatarMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_hologramTrans = base.transform.FindChild("HologramSprite") as RectTransform;
		_hologramImg = _hologramTrans.GetComponent<Image>();
		_avatarImg = base.transform.FindChild("AvatarSprite").GetComponent<Image>();
		_avatarOverlayImg = base.transform.FindChild("AvatarSprite/Overlay").GetComponent<Image>();
		_defaultAvatarOverlayColor = _avatarOverlayImg.color;
	}

	public void SetUp()
	{
		selectedAvatarID = null;
		_avatarMenu.SetUp(1, false, 85f, 100f, -4f);
		CreateCells();
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnEnabled(0, false);
		_avatarImg.enabled = false;
		_avatarOverlayImg.enabled = false;
		_hologramTrans.gameObject.SetActive(false);
		if (MPSharedStateInfo.registrationInfo != null && MPSharedStateInfo.registrationInfo.avatarID != null)
		{
			_avatarMenu.TryCenterAndSelectCellWithID(MPSharedStateInfo.registrationInfo.avatarID);
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_REGISTER_AVATAR_BTN_NEXT);
		_dialogue.SetText(MPTextDefs.MP_REGISTER_AVATAR_DIAL);
	}

	protected override void AddEvents()
	{
		_avatarMenu.SelectedEvent += CellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_avatarMenu.SelectedEvent -= CellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateCells()
	{
		List<string> avatarsForRegisterSelect = MPPlayerAvatarDefs.GetAvatarsForRegisterSelect();
		int num = Mathf.Max(5, avatarsForRegisterSelect.Count);
		for (int i = 0; i < num; i++)
		{
			AddCell((i >= avatarsForRegisterSelect.Count) ? null : avatarsForRegisterSelect[i]);
		}
	}

	private void AddCell(string id)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "SalesmanRickAvatarCell"));
		SalesmanRickAvatarCell component = rectTransform.GetComponent<SalesmanRickAvatarCell>();
		component.SetUp(id, eAvatarCellState.UNLOCKED, MPPlayerAvatarDefs.Name(id), MPPlayerAvatarDefs.CurrencyType(id), 0, MPPlayerAvatarDefs.AssetID(id));
		component.SetAsActive(false);
		_avatarMenu.AddCell(component);
	}

	private void RefreshForSelectedAvatarID()
	{
		if (!string.IsNullOrEmpty(selectedAvatarID))
		{
			_mainMenu.SetBtnEnabled(0, true);
		}
		else
		{
			_mainMenu.SetBtnEnabled(0, false);
		}
	}

	private IEnumerator AnimateAvatarCo(string toAvatarId, bool locked = false)
	{
		App.sound.PlaySFX("Audio/SFX/CraftUseItem");
		Sprite newSprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPPlayerAvatarDefs.AssetID(toAvatarId), "front"));
		_avatarImg.sprite = newSprite;
		_avatarImg.SetNativeSize();
		_avatarOverlayImg.sprite = newSprite;
		_avatarOverlayImg.SetNativeSize();
		if (locked)
		{
			_avatarImg.color = Color.black;
			_avatarOverlayImg.color = Color.black;
			_hologramImg.color = Color.black;
		}
		else
		{
			_avatarImg.color = Color.white;
			_avatarOverlayImg.color = _defaultAvatarOverlayColor;
			_hologramImg.color = Color.white;
		}
		_avatarImg.enabled = false;
		_avatarOverlayImg.enabled = false;
		_hologramTrans.gameObject.SetActive(true);
		_hologramTrans.DOKill();
		_hologramTrans.localScale = new Vector3(0.4f, 1.25f, 1f);
		yield return _hologramTrans.DOScale(Vector3.one, 0.1f).WaitForCompletion();
		_hologramTrans.gameObject.SetActive(false);
		_avatarOverlayImg.enabled = true;
		yield return new WaitForSeconds(0.1f);
		_avatarOverlayImg.enabled = false;
		_avatarImg.enabled = true;
		_animateAvatarCo = null;
	}

	public void CellSelectedHandler(CellBase cell, int index)
	{
		SalesmanRickAvatarCell salesmanRickAvatarCell = (SalesmanRickAvatarCell)cell;
		selectedAvatarID = salesmanRickAvatarCell.id;
		RefreshForSelectedAvatarID();
		if (_animateAvatarCo != null)
		{
			StopCoroutine(_animateAvatarCo);
		}
		_animateAvatarCo = StartCoroutine(AnimateAvatarCo(selectedAvatarID));
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.UseEvent != null)
			{
				this.UseEvent(selectedAvatarID);
			}
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
