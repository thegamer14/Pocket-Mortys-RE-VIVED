using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InvestmentUI : UIBase
{
	public delegate void SelectedInvestmentDelegate(int investmentIndex);

	public delegate void InvestmentUIDelegate();

	private const float _kScrollMenuPaddingTop = 0f;

	private const float _kScrollMenuPaddingBottom = 0f;

	private const float _kScrollMenuCellGap = 7f;

	private MenuBase _idleMenu;

	private MenuBase _resultMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private CoinBar _coinBar;

	private InvestmentCell _selectedCell;

	private int _coinValue;

	private Tween _coinTween;

	public int selectedIndex
	{
		get
		{
			return (!(_selectedCell != null)) ? (-1) : _selectedCell.investmentIndex;
		}
	}

	public event SelectedInvestmentDelegate BeginEvent;

	public event SelectedInvestmentDelegate ResultsEvent;

	public event InvestmentUIDelegate BackEvent;

	public event InvestmentUIDelegate GetMoreCoinEvent;

	protected override void LateAwake()
	{
		_idleMenu = base.transform.FindChild("MenuBar/IdleMenu").GetComponent<MenuBase>();
		_resultMenu = base.transform.FindChild("MenuBar/ResultMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_coinBar = base.transform.FindChild("MenuBar/CoinBar").GetComponent<CoinBar>();
	}

	public void SetUp()
	{
		_idleMenu.SetBtnEnabled(0, false);
		_resultMenu.SetPositionOut();
		SetCoins(PlayerDefs.CoinCount);
		AddInvestmentCells();
	}

	private void AddInvestmentCells()
	{
		_scrollMenu.SetUp(1, false, 0f, 0f, 7f);
		for (int i = 0; i < 3; i++)
		{
			InvestmentCell component = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "InvestmentCell")).GetComponent<InvestmentCell>();
			component.CellStateChangedEvent += CellStateChangedHandler;
			component.SetUp(i);
			_scrollMenu.AddCell(component);
		}
	}

	public override void SetLabels()
	{
		_idleMenu.SetBtnTitle(0, TextDefs.INVESTMENT_BTN_BEGIN);
		_resultMenu.SetBtnTitle(0, TextDefs.INVESTMENT_BTN_COLLECT);
		_dialogue.SetText(TextDefs.INVESTMENT_DIAL);
		_coinBar.SetLabels();
	}

	protected override void AddEvents()
	{
		_idleMenu.BtnPressedEvent += IdleMenuBtnHandler;
		_resultMenu.BtnPressedEvent += ResultsMenuBtnHandler;
		_scrollMenu.SelectedEvent += CellSelectedHandler;
		_scrollMenu.UnselectedEvent += CellUnselectedHandler;
		_coinBar.GetMoreEvent += CoinBarGetMoreHandler;
	}

	protected override void RemoveEvents()
	{
		_idleMenu.BtnPressedEvent -= IdleMenuBtnHandler;
		_resultMenu.BtnPressedEvent -= ResultsMenuBtnHandler;
		_scrollMenu.SelectedEvent -= CellSelectedHandler;
		_scrollMenu.UnselectedEvent -= CellUnselectedHandler;
		_coinBar.GetMoreEvent -= CoinBarGetMoreHandler;
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
			if (item is InvestmentCell)
			{
				(item as InvestmentCell).CellStateChangedEvent -= CellStateChangedHandler;
			}
		}
	}

	private void RefreshMenuForSelectedInvestment()
	{
		if (selectedIndex == -1)
		{
			StartCoroutine(ShowMenuWithTween(_idleMenu));
			_dialogue.TweenText(TextDefs.INVESTMENT_DIAL);
			return;
		}
		switch (InvestmentDefs.GetInvestmentState(selectedIndex))
		{
		case eInvestmentState.IDLE:
			StartCoroutine(ShowMenuWithTween(_idleMenu));
			_idleMenu.SetBtnEnabled(0, true);
			_dialogue.TweenText(string.Format(TextDefs.INVESTMENT_SELECTED_IDLE_DIAL, InvestmentDefs.GetInvestmentDeposit(selectedIndex), InvestmentDefs.GetInvestmentReturn(selectedIndex)));
			break;
		case eInvestmentState.PROGRESS:
			StartCoroutine(ShowMenuWithTween(_idleMenu));
			_idleMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.INVESTMENT_SELECTED_IN_PROGRESS_DIAL);
			break;
		case eInvestmentState.RESULT_AVAILABLE:
			StartCoroutine(ShowMenuWithTween(_resultMenu));
			_dialogue.TweenText(TextDefs.INVESTMENT_SELECTED_RESULTS_DIAL);
			break;
		}
	}

	private IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		menu.SetInteractable(false);
		StartCoroutine(_resultMenu.TweenOut());
		StartCoroutine(_idleMenu.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
			menu.SetInteractable(true);
		}
	}

	public void NotEnoughCoin()
	{
		_dialogue.TweenText(TextDefs.INVESTMENT_NOT_ENOUGH_DIAL);
	}

	public void InvestmentBegun()
	{
		TweenCoins(PlayerDefs.CoinCount);
	}

	public void InvestmentCollected(int investmentIndex)
	{
		_scrollMenu.SetCellUnselected(_selectedCell);
		TweenCoins(PlayerDefs.CoinCount);
		_dialogue.TweenText(string.Format(TextDefs.INVESTMENT_COLLECTED_DIAL, InvestmentDefs.GetInvestmentReturn(investmentIndex)));
	}

	private void SetCoins(int coins)
	{
		_coinValue = coins;
		_coinBar.SetCount(_coinValue);
	}

	private void TweenCoins(int coins)
	{
		_coinTween = DOTween.To(() => _coinValue, delegate(int x)
		{
			_coinValue = x;
		}, coins, 1f);
		_coinTween.OnUpdate(delegate
		{
			SetCoins(_coinValue);
		});
	}

	public void CellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (InvestmentCell)cell;
		RefreshMenuForSelectedInvestment();
	}

	public void CellUnselectedHandler(CellBase cell, int index)
	{
		_selectedCell = null;
		_idleMenu.SetBtnEnabled(0, false);
		RefreshMenuForSelectedInvestment();
	}

	private void CoinBarGetMoreHandler()
	{
		if (this.GetMoreCoinEvent != null)
		{
			this.GetMoreCoinEvent();
		}
	}

	private void IdleMenuBtnHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BeginEvent != null)
			{
				this.BeginEvent(_selectedCell.investmentIndex);
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
				this.ResultsEvent(_selectedCell.investmentIndex);
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

	private void CellStateChangedHandler(InvestmentCell cell)
	{
		if (cell == _selectedCell)
		{
			RefreshMenuForSelectedInvestment();
		}
	}
}
