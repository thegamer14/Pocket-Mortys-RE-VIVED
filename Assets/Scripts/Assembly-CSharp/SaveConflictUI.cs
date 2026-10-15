using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveConflictUI : UIBase
{
	public delegate void SaveConfilctUISelectedDelegate(CloudContainer cloudContainer);

	private const float _kScrollMenuPaddingTop = 0f;

	private const float _kScrollMenuPaddingBottom = 0f;

	private const float _kScrollMenuCellGap = -4f;

	private GameObject _loadingObj;

	private MenuDialogue _dialogue;

	private Text _loadingLabel;

	private ScrollMenu _scrollMenu;

	private List<CloudContainer> _containers;

	private CellBase _selectedCell;

	public event SaveConfilctUISelectedDelegate SaveSelectedEvent;

	protected override void LateAwake()
	{
		_loadingObj = base.transform.FindChild("Loading").gameObject;
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_loadingLabel = base.transform.FindChild("Loading/Label").GetComponent<Text>();
	}

	public void SetUp()
	{
		_scrollMenu.SetUp(1, false, 0f, 0f, -4f);
		_scrollMenu.AddHeaderCell(TextDefs.SAVE_SELECT_TITLE);
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.SAVE_SELECT_DIAL);
		_loadingLabel.text = TextDefs.SAVE_SELECT_LOADING;
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += CellSelectedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= CellSelectedHandler;
	}

	public void DisplayLoading()
	{
		_loadingObj.SetActive(true);
	}

	public void DisplayConflicts(List<CloudContainer> containers)
	{
		_containers = containers;
		CreateSaveDataCells();
		_loadingObj.SetActive(false);
	}

	public void DeselectSelectedCell()
	{
		_scrollMenu.SetCellUnselected(_selectedCell);
		_selectedCell = null;
	}

	private void CreateSaveDataCells()
	{
		_scrollMenu.RemoveAllCellsOfType(typeof(SaveDataCell));
		foreach (CloudContainer container in _containers)
		{
			AddSaveDataCell(container);
		}
	}

	private void AddSaveDataCell(CloudContainer cloudContainer)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "SaveDataCell"));
		SaveDataCell component = rectTransform.GetComponent<SaveDataCell>();
		component.SetUp(cloudContainer);
		_scrollMenu.AddCell(component);
	}

	private void CellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = cell;
		if (this.SaveSelectedEvent != null)
		{
			this.SaveSelectedEvent((cell as SaveDataCell).cloudContainer);
		}
	}
}
