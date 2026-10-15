using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageCombineUI : UIBase
{
	public delegate void PartyUIMortyDelegate(MortyData mortyData1, MortyData mortyData2);

	public delegate void PartyUIBtnDelegate();

	private const int _kMortyMenuMinimumCellCount = 5;

	private const float _kMortyMenuPaddingTop = 85f;

	private const float _kMortyMenuPaddingBottom = 100f;

	private const float _kMortyMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private StorageCombineChamber _chamber;

	private CanvasGroup _overlayGroup;

	private Animator _animator;

	private ScrollMenu _mortyMenu;

	private List<MortyCombineCell> _selectedCells;

	private List<MortyData> newlyCombinedMortyDatas;

	public event PartyUIMortyDelegate CombineEvent;

	public event PartyUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_chamber = base.transform.FindChild("Chamber").GetComponent<StorageCombineChamber>();
		_overlayGroup = base.transform.FindChild("Overlay").GetComponent<CanvasGroup>();
		_animator = GetComponent<Animator>();
		_selectedCells = new List<MortyCombineCell>();
		newlyCombinedMortyDatas = new List<MortyData>();
		_animator.enabled = false;
	}

	public void SetUp()
	{
		_mainMenu.SetBtnEnabled(0, false);
		_mortyMenu.SetUp(2, true, 85f, 100f, -4f);
		CreateMortyCells();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_COMBINE_BTN_COMBINE);
		_dialogue.SetText(TextDefs.MORTY_STORAGE_COMBINE_DIAL_SELECTED_0);
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += MortyCellSelectedHandler;
		_mortyMenu.UnselectedEvent += MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= MortyCellSelectedHandler;
		_mortyMenu.UnselectedEvent -= MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public override void ActivateInput(bool isActive)
	{
		base.ActivateInput(isActive);
		_overlayGroup.blocksRaycasts = !isActive;
	}

	private void CreateMortyCells()
	{
		List<CellBase> cellListCopy = _mortyMenu.GetCellListCopy();
		for (int i = 0; i < cellListCopy.Count; i++)
		{
			_mortyMenu.RemoveCell(cellListCopy[i]);
		}
		List<MortyData> allPlayerMorties = PlayerDefs.GetAllPlayerMorties();
		int num = Mathf.Max(5, allPlayerMorties.Count);
		allPlayerMorties = (from e in allPlayerMorties
			orderby e.level
			group e by e.id).OrderByDescending(Enumerable.Count).SelectMany((IGrouping<string, MortyData> e) => e).ToList();
		for (int num2 = 0; num2 < num; num2++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyCombineCell"));
			MortyCombineCell component = rectTransform.GetComponent<MortyCombineCell>();
			MortyData mortyData = ((num2 >= allPlayerMorties.Count) ? null : allPlayerMorties[num2]);
			component.SetUp(mortyData, newlyCombinedMortyDatas.Contains(mortyData));
			_mortyMenu.AddCell(component);
		}
	}

	private void RefreshMortyCellsStates()
	{
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			MortyCombineCell mortyCombineCell = (MortyCombineCell)_mortyMenu.GetCellAtIndex(i);
			if (mortyCombineCell.state == eScrollMenuCellState.UNUSED)
			{
				continue;
			}
			if (_selectedCells.Count == 0)
			{
				mortyCombineCell.SetEnabled(true);
				_mainMenu.SetBtnEnabled(0, false);
			}
			else if (_selectedCells.Count == 1)
			{
				if (mortyCombineCell.data.id.Equals(_selectedCells[0].data.id))
				{
					if (!_selectedCells.Contains(mortyCombineCell))
					{
						mortyCombineCell.SetEnabled(true);
					}
					_mainMenu.SetBtnEnabled(0, false);
				}
				else
				{
					mortyCombineCell.SetEnabled(false);
				}
			}
			else if (_selectedCells.Count == 2)
			{
				if (!_selectedCells.Contains(mortyCombineCell))
				{
					mortyCombineCell.SetEnabled(false);
				}
				_mainMenu.SetBtnEnabled(0, true);
			}
		}
	}

	private MortyCombineCell GetCellForData(MortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			MortyCombineCell mortyCombineCell = (MortyCombineCell)_mortyMenu.GetCellAtIndex(i);
			if (mortyCombineCell.data == mortyData)
			{
				return mortyCombineCell;
			}
		}
		return null;
	}

	public void RefreshDialogue(string mortyID)
	{
		if (_selectedCells.Count == 0)
		{
			_dialogue.TweenText(TextDefs.MORTY_STORAGE_COMBINE_DIAL_SELECTED_0);
		}
		else if (_selectedCells.Count == 1)
		{
			if (PlayerDefs.GetMortyCombineCountForID(mortyID) > 0)
			{
				_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_COMBINE_DIAL_SELECTED_1, MortyDefs.Name(mortyID)));
			}
			else
			{
				_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_COMBINE_DIAL_SELECTED_1_FAIL, MortyDefs.Name(mortyID)));
			}
		}
		else if (_selectedCells.Count == 2)
		{
			_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_COMBINE_DIAL_SELECTED_2, MortyDefs.Name(mortyID)));
		}
	}

	private void TryCombineDatas()
	{
		if (this.CombineEvent != null)
		{
			this.CombineEvent(_selectedCells[0].data, _selectedCells[1].data);
		}
	}

	public void CombineDatasComplete(MortyData combinedMortyData, string resultsText)
	{
		newlyCombinedMortyDatas.Add(combinedMortyData);
		StartCoroutine(_mainMenu.TweenOut());
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_COMBINE_DIAL_COMBINING);
		StartCoroutine(AnimateCombine(resultsText));
	}

	public void CombineDatasFailed(string resultsText)
	{
		_dialogue.TweenText(resultsText);
	}

	private IEnumerator AnimateCombine(string resultsText)
	{
		_animator.enabled = true;
		ActivateInput(false);
		App.sound.PlaySFX("Audio/SFX/CombineMorties");
		yield return StartCoroutine(_animator.PlayAndWait("Combine", -1, 0f));
		yield return new WaitForSeconds(2f);
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(resultsText);
		_animator.enabled = false;
		ActivateInput(true);
	}

	public void AnimatorCombineFlashedHandler()
	{
		MortyData mortyData = newlyCombinedMortyDatas[newlyCombinedMortyDatas.Count - 1];
		CreateMortyCells();
		CellBase cellForData = GetCellForData(mortyData);
		_mortyMenu.TweenToCell(cellForData);
		_chamber.SetCombined(mortyData);
		_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_COMBINE_DIAL_COMBINED, MortyDefs.Name(mortyData.id)));
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		MortyCombineCell mortyCombineCell = (MortyCombineCell)cell;
		_selectedCells.Add(mortyCombineCell);
		_chamber.SelectMorty(mortyCombineCell);
		RefreshMortyCellsStates();
		RefreshDialogue(mortyCombineCell.data.id);
	}

	public void MortyCellUnselectedHandler(CellBase cell, int index)
	{
		MortyCombineCell mortyCombineCell = (MortyCombineCell)cell;
		_selectedCells.Remove(mortyCombineCell);
		_chamber.UnselectMorty(mortyCombineCell);
		RefreshMortyCellsStates();
		RefreshDialogue(mortyCombineCell.data.id);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			TryCombineDatas();
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
