using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPMortydexUI : UIBase
{
	public delegate void MortydexUIIDBtnDelegate(string id);

	public delegate void BagUIBtnDelegate();

	private const float _kItemMenuPaddingTop = 85f;

	private const float _kItemMenuPaddingBottom = 100f;

	private const float _kMortyOutPosX = 900f;

	private const float _kMortyTweenOutSpeed = 1600f;

	private const float _kMortyTweenInDuration = 0.25f;

	private Image _morty;

	private Text _seenTitle;

	private Text _seen;

	private Text _caughtTitle;

	private Text _caught;

	private ScrollMenu _mortyMenu;

	private string _selectedMortyID;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private float _mortyStartPosX;

	private Sequence _mortyTween;

	public event MortydexUIIDBtnDelegate AboutEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_morty = base.transform.FindChild("Morty").GetComponent<Image>();
		_seenTitle = base.transform.FindChild("DiscoveredBar/Container/SeenTitle").GetComponent<Text>();
		_seen = base.transform.FindChild("DiscoveredBar/Container/SeenBackground/Label").GetComponent<Text>();
		_caughtTitle = base.transform.FindChild("DiscoveredBar/Container/CaughtTitle").GetComponent<Text>();
		_caught = base.transform.FindChild("DiscoveredBar/Container/CaughtBackground/Label").GetComponent<Text>();
		_mortyMenu = base.transform.FindChild("ItemMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_mortyStartPosX = _morty.transform.localPosition.x;
	}

	public void SetUp(string mortyID)
	{
		_selectedMortyID = null;
		_mortyMenu.SetUp(1, false, 85f, 100f);
		CreateMortydexCells();
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnEnabled(0, false);
		ShowMorty(false);
		SetDiscoveredBar(App.net.player.data.mortydexSeen.Count, App.net.player.data.mortydexCaught.Count);
		_mortyMenu.TryCenterAndSelectCellWithID(mortyID);
		SwapMorty(mortyID, 0.3f);
	}

	public override void SetLabels()
	{
		_seenTitle.text = "   " + TextDefs.MORTYDEX_SEEN_TITLE;
		_caughtTitle.text = "   " + TextDefs.MORTYDEX_CAUGHT_TITLE;
		_mainMenu.SetBtnTitle(0, TextDefs.MORTYDEX_SELECTED_MENU_BTN_1);
		_dialogue.SetText(TextDefs.MORTYDEX_DIAL);
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += ItemCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= ItemCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateMortydexCells()
	{
		SortedList<int, string> listOfOrderedMortyIds = MPMortyDefs.GetListOfOrderedMortyIds();
		foreach (KeyValuePair<int, string> keyValuePair in listOfOrderedMortyIds)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortydexCell"));
			MortydexCell component = rectTransform.GetComponent<MortydexCell>();
			bool isSeen = App.net.player.data.mortydexSeen.Contains(keyValuePair.Value);
			bool isCaught = App.net.player.data.mortydexCaught.Contains(keyValuePair.Value);
			bool isOwned = App.net.player.data.morties.FirstOrDefault((MPOpponentMortyData e) => e.id == keyValuePair.Value) != null;
			component.SetUp(keyValuePair.Value, MPMortyDefs.AssetID(keyValuePair.Value), MPMortyDefs.Name(keyValuePair.Value), MPMortyDefs.GetMortyNumberText(keyValuePair.Value), isSeen, isCaught, isOwned);
			_mortyMenu.AddCell(component);
		}
	}

	private void SetDiscoveredBar(int seen, int caught)
	{
		_seen.text = seen.ToString();
		_caught.text = caught.ToString();
	}

	private void ShowMorty(bool value)
	{
		_morty.transform.localPosition = new Vector3((!value) ? 900f : _mortyStartPosX, _morty.transform.localPosition.y);
	}

	private void SetMorty(string id)
	{
		_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(id), "front"));
		_morty.SetNativeSize();
	}

	private void SwapMorty(string id = null, float delay = 0f)
	{
		if (_mortyTween != null)
		{
			_mortyTween.Kill();
			_mortyTween = null;
		}
		float num = Mathf.Abs((_morty.rectTransform.localPosition.x - 900f) / 1600f);
		_mortyTween = DOTween.Sequence();
		_mortyTween.AppendInterval(delay);
		if (num > 0f)
		{
			_mortyTween.Append(_morty.rectTransform.DOLocalMoveX(900f, num).SetEase(Ease.Linear));
		}
		if (!string.IsNullOrEmpty(id))
		{
			_mortyTween.AppendCallback(delegate
			{
				SetMorty(id);
			});
			_mortyTween.Append(_morty.rectTransform.DOLocalMoveX(_mortyStartPosX, 0.25f).SetEase(Ease.Linear));
		}
		_mortyTween.Play();
	}

	public void ItemCellSelectedHandler(CellBase cell, int index)
	{
		MortydexCell mortydexCell = (MortydexCell)cell;
		_selectedMortyID = mortydexCell.id;
		SwapMorty(mortydexCell.id);
		_mainMenu.SetBtnEnabled(0, true);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			SwapMorty();
			if (this.AboutEvent != null)
			{
				this.AboutEvent(_selectedMortyID);
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
