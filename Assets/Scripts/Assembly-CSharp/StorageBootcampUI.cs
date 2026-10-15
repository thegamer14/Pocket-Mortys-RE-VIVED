using System.Collections;
using System.Collections.Generic;

public class StorageBootcampUI : UIBase
{
	public delegate void StorageBootcampBtnDelgate(int selectedBootcampIndex = 0);

	private const float _kScrollMenuPaddingTop = 0f;

	private const float _kScrollMenuPaddingBottom = 0f;

	private const float _kScrollMenuCellGap = 7f;

	private MenuBase _idleMenu;

	private MenuBase _inProgressMenu;

	private MenuBase _resultMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private CouponBar _couponBar;

	private BootcampCell _selectedCell;

	public int selectedBootcampIndex
	{
		get
		{
			return (!(_selectedCell != null)) ? (-1) : _selectedCell.bootcampIndex;
		}
	}

	public event StorageBootcampBtnDelgate BeginEvent;

	public event StorageBootcampBtnDelgate EditEvent;

	public event StorageBootcampBtnDelgate ExpediteEvent;

	public event StorageBootcampBtnDelgate ResultsEvent;

	public event StorageBootcampBtnDelgate BackEvent;

	public event StorageBootcampBtnDelgate BuyCouponsBtnEvent;

	protected override void LateAwake()
	{
		_idleMenu = base.transform.FindChild("MenuBar/IdleMenu").GetComponent<MenuBase>();
		_inProgressMenu = base.transform.FindChild("MenuBar/InProgressMenu").GetComponent<MenuBase>();
		_resultMenu = base.transform.FindChild("MenuBar/ResultMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_couponBar = base.transform.FindChild("MenuBar/InProgressMenu/CouponBar").GetComponent<CouponBar>();
	}

	public void SetUp()
	{
		_idleMenu.SetBtnEnabled(0, false);
		_idleMenu.SetBtnEnabled(1, false);
		_inProgressMenu.SetPositionOut();
		_resultMenu.SetPositionOut();
		AddBootcampCells();
	}

	private void AddBootcampCells()
	{
		_scrollMenu.SetUp(1, false, 0f, 0f, 7f);
		for (int i = 0; i < BootcampDefs.BootcampCount(); i++)
		{
			BootcampCell component = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "BootcampCell")).GetComponent<BootcampCell>();
			component.CellStateChangedEvent += CellStateChangedHandler;
			component.SetUp(i);
			_scrollMenu.AddCell(component);
			if (component.bootcampIndex == SharedStatInfo.storageBootcampInfo.selectedBootcampIndex)
			{
				_scrollMenu.TrySetCellSelected(component);
			}
		}
	}

	public override void SetLabels()
	{
		_idleMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_BEGIN);
		_idleMenu.SetBtnTitle(1, TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_EDIT);
		_resultMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_RESULTS);
		_inProgressMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_EXPEDITE);
		_couponBar.SetLabels();
		_couponBar.SetCount(PlayerDefs.CouponCount);
		_dialogue.SetText(TextDefs.MORTY_STORAGE_BOOTCAMP_DIAL);
	}

	public void DisplayDialogue(string text)
	{
		_dialogue.TweenText(text);
	}

	protected override void AddEvents()
	{
		_idleMenu.BtnPressedEvent += IdleMenuBtnHandler;
		_inProgressMenu.BtnPressedEvent += InProgressMenuBtnHandler;
		_couponBar.GetMoreEvent += CouponGetMoreHandler;
		_resultMenu.BtnPressedEvent += ResultsMenuBtnHandler;
		_scrollMenu.SelectedEvent += BootcampCellSelectedHandler;
		_scrollMenu.UnselectedEvent += BootcampCellUnselectedHandler;
		PlayerDefs.CouponCountChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		_idleMenu.BtnPressedEvent -= IdleMenuBtnHandler;
		_inProgressMenu.BtnPressedEvent -= InProgressMenuBtnHandler;
		_couponBar.GetMoreEvent -= CouponGetMoreHandler;
		_resultMenu.BtnPressedEvent -= ResultsMenuBtnHandler;
		_scrollMenu.SelectedEvent -= BootcampCellSelectedHandler;
		_scrollMenu.UnselectedEvent -= BootcampCellUnselectedHandler;
		PlayerDefs.CouponCountChangedEvent -= CouponCountChangedHandler;
		RemoveCellHandlers();
	}

	private void RemoveCellHandlers()
	{
		if (_scrollMenu == null)
		{
			return;
		}
		List<CellBase> cellListCopy = _scrollMenu.GetCellListCopy();
		foreach (CellBase item in cellListCopy)
		{
			if (item is BootcampCell)
			{
				(item as BootcampCell).CellStateChangedEvent -= CellStateChangedHandler;
			}
		}
	}

	private void RefreshMenuForSelectedBootcamp()
	{
		switch (BootcampDefs.GetBootcampState(selectedBootcampIndex))
		{
		case eBootcampState.IDLE:
			StartCoroutine(ShowMenuWithTween(_idleMenu));
			_idleMenu.SetBtnEnabled(0, true);
			_idleMenu.SetBtnEnabled(1, true);
			_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_BOOTCAMP_DIAL_SELECTED_IDLE, BootcampDefs.GetBootcampLevel(selectedBootcampIndex)));
			break;
		case eBootcampState.PROGRESS:
			StartCoroutine(ShowMenuWithTween(_inProgressMenu));
			_dialogue.TweenText(TextDefs.MORTY_STORAGE_BOOTCAMP_DIAL_SELECTED_IN_PROGRESS);
			break;
		case eBootcampState.RESULT_AVAILABLE:
			StartCoroutine(ShowMenuWithTween(_resultMenu));
			_dialogue.TweenText(TextDefs.MORTY_STORAGE_BOOTCAMP_DIAL_SELECTED_RESULTS);
			break;
		}
	}

	private IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		menu.SetInteractable(false);
		StartCoroutine(_resultMenu.TweenOut());
		StartCoroutine(_idleMenu.TweenOut());
		StartCoroutine(_inProgressMenu.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
			menu.SetInteractable(true);
		}
	}

	public void BootcampCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (BootcampCell)cell;
		RefreshMenuForSelectedBootcamp();
	}

	public void BootcampCellUnselectedHandler(CellBase cell, int index)
	{
		_selectedCell = null;
		_idleMenu.SetBtnEnabled(0, false);
		_idleMenu.SetBtnEnabled(1, false);
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_BOOTCAMP_DIAL);
	}

	private void IdleMenuBtnHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BeginEvent != null)
			{
				this.BeginEvent(_selectedCell.bootcampIndex);
			}
			break;
		case 1:
			if (this.EditEvent != null)
			{
				this.EditEvent(_selectedCell.bootcampIndex);
			}
			break;
		case 2:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}

	private void InProgressMenuBtnHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BeginEvent != null)
			{
				this.ExpediteEvent(_selectedCell.bootcampIndex);
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

	private void ResultsMenuBtnHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.ResultsEvent != null)
			{
				this.ResultsEvent(_selectedCell.bootcampIndex);
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

	private void CouponGetMoreHandler()
	{
		if (this.BuyCouponsBtnEvent != null)
		{
			this.BuyCouponsBtnEvent(_selectedCell.bootcampIndex);
		}
	}

	private void CouponCountChangedHandler(int count)
	{
		_couponBar.SetCount(PlayerDefs.CouponCount);
	}

	private void CellStateChangedHandler(BootcampCell cell)
	{
		if (cell == _selectedCell)
		{
			RefreshMenuForSelectedBootcamp();
		}
	}
}
