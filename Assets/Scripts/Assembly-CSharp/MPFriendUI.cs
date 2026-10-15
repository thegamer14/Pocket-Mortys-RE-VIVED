using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MPFriendUI : UIBase
{
	public delegate void SelectedCellDelegate(MPFriendData selectedFriendData);

	public delegate void BtnWithDataDelegate(MPFriendData data);

	public delegate void BtnDelegate();

	private const float _kScrollMenuPaddingTop = 85f;

	private const float _kScrollMenuPaddingBottom = 100f;

	private const float _kScrollMenuCellGap = -4f;

	private const int _KScrollMenuMinFriendCells = 3;

	private GameObject _waitingWheel;

	private MenuBase _mainMenu;

	private MenuBase _friendMenu;

	private MenuBase _friendRequestMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private MPFriendRequestMortyCell _requestMortyCell;

	private CellBase _selectedCell;

	private MPFriendData _selectedCellData;

	public event SelectedCellDelegate SelectedCellDataEvent;

	public event BtnWithDataDelegate BattleEvent;

	public event BtnWithDataDelegate DonateEvent;

	public event BtnWithDataDelegate AcceptEvent;

	public event BtnWithDataDelegate DenyEvent;

	public event BtnWithDataDelegate RemoveEvent;

	public event BtnDelegate AddFriendEvent;

	public event BtnDelegate RequestMortyEvent;

	public event BtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_waitingWheel = base.transform.FindChild("WaitingWheel").gameObject;
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_friendMenu = base.transform.FindChild("MenuBar/FriendMenu").GetComponent<MenuBase>();
		_friendRequestMenu = base.transform.FindChild("MenuBar/FriendRequestMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		_scrollMenu.SetUp(1, false, 85f, 100f, -4f);
		ShowMenu(null);
		_dialogue.SetText(MPTextDefs.MP_FRIEND_LOADING_DIALOGUE);
	}

	public override void SetLabels()
	{
		_friendMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_BATTLE_BTN);
		_friendMenu.SetBtnTitle(1, MPTextDefs.MP_FRIEND_DONATE_BTN);
		_friendRequestMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_ACCEPT_BTN);
		_friendRequestMenu.SetBtnTitle(1, MPTextDefs.MP_FRIEND_DENY_BTN);
		base.transform.FindChild("MenuBar/MainMenu/AddFriendBar/Background/Title").GetComponent<Text>().text = MPTextDefs.MP_FRIEND_ADD_FRIEND_BTN;
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += ScrollCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_friendMenu.BtnPressedEvent += FriendBtnPressedHandler;
		_friendRequestMenu.BtnPressedEvent += FriendRequestBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= ScrollCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_friendMenu.BtnPressedEvent -= FriendBtnPressedHandler;
		_friendRequestMenu.BtnPressedEvent -= FriendRequestBtnPressedHandler;
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (_waitingWheel.activeSelf)
		{
			_waitingWheel.transform.Rotate(new Vector3(0f, 0f, -400f) * deltaTime);
		}
	}

	public void SetScrollMenu(List<MPFriendData> friendRequests, List<MPFriendData> friends, string mortyRequestId, float mortyRequestCountdown, MPFriendData mortyDonatedUser)
	{
		int count = _scrollMenu.count;
		ClearScrollMenu();
		CreateRequestMortyCell(mortyRequestId, mortyRequestCountdown, mortyDonatedUser);
		CreateFriendRequestCells(friendRequests);
		CreateFriendCells(friends);
		CellBase cellBase = null;
		if (_selectedCellData != null)
		{
			cellBase = _scrollMenu.GetCellForID(_selectedCellData.userId);
		}
		else if (MPSharedStateInfo.friendInfo.selectedFriendData != null)
		{
			cellBase = _scrollMenu.GetCellForID(MPSharedStateInfo.friendInfo.selectedFriendData.userId);
		}
		if (cellBase != null)
		{
			if (_selectedCellData == null && MPSharedStateInfo.friendInfo.selectedFriendData != null)
			{
				_scrollMenu.CenterToCell(cellBase);
			}
			_scrollMenu.TrySetCellSelected(cellBase);
			return;
		}
		if (count == 0 || _selectedCellData != null)
		{
			if (friends.Count > 0)
			{
				_dialogue.TweenText(MPTextDefs.MP_FRIEND_DEFAULT_DIALOGUE);
			}
			else
			{
				_dialogue.TweenText(MPTextDefs.MP_FRIEND_NO_FRIENDS_DIALOGUE);
			}
		}
		_selectedCell = null;
		SetSelectedCellData(null);
	}

	private void CreateRequestMortyCell(string mortyRequestId, float mortyRequestCountdown, MPFriendData mortyDonatedUser)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPFriendRequestMortyCell"));
		_requestMortyCell = rectTransform.GetComponent<MPFriendRequestMortyCell>();
		_requestMortyCell.SetUp(mortyRequestId, mortyRequestCountdown, mortyDonatedUser);
		_scrollMenu.AddCell(_requestMortyCell);
	}

	private void CreateFriendCells(List<MPFriendData> friends)
	{
		List<MPFriendData> list = ((friends == null) ? new List<MPFriendData>() : new List<MPFriendData>(from e in friends
			orderby !e.isPending descending, !e.isOnline, e.username
			select e));
		_waitingWheel.SetActive(false);
		_scrollMenu.AddHeaderCell(MPTextDefs.MP_FRIEND_CELL_HEADER);
		int num = Mathf.Max(3, list.Count);
		for (int num2 = 0; num2 < num; num2++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPFriendCell"));
			MPFriendCell component = rectTransform.GetComponent<MPFriendCell>();
			component.SetUp((num2 >= list.Count) ? null : list[num2]);
			component.RemoveEvent += ScrollCellRemoveHandler;
			_scrollMenu.AddCell(component);
		}
	}

	private void CreateFriendRequestCells(List<MPFriendData> friendRequests)
	{
		if (friendRequests != null && friendRequests.Count != 0)
		{
			_scrollMenu.AddHeaderCell(MPTextDefs.MP_FRIEND_REQUEST_CELL_HEADER);
			for (int i = 0; i < friendRequests.Count; i++)
			{
				RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPFriendRequestCell"));
				MPFriendRequestCell component = rectTransform.GetComponent<MPFriendRequestCell>();
				component.SetUp(friendRequests[i]);
				_scrollMenu.AddCell(component);
			}
		}
	}

	private void ClearScrollMenu()
	{
		for (int i = 0; i < _scrollMenu.count; i++)
		{
			CellBase cellAtIndex = _scrollMenu.GetCellAtIndex(i);
			if (cellAtIndex is MPFriendCell)
			{
				MPFriendCell mPFriendCell = (MPFriendCell)cellAtIndex;
				mPFriendCell.RemoveEvent -= ScrollCellRemoveHandler;
			}
		}
		_scrollMenu.RemoveAllCells();
	}

	private void SetSelectedCellData(MPFriendData selectedFriendData)
	{
		_selectedCellData = selectedFriendData;
		if (this.SelectedCellDataEvent != null)
		{
			this.SelectedCellDataEvent(_selectedCellData);
		}
	}

	public void SetRequestMortyCellCountdown(float countdown)
	{
		_requestMortyCell.SetCountdown(countdown);
	}

	public void DisplayRefreshDataError(eFriendRefreshDataError error)
	{
		ClearScrollMenu();
		CreateFriendCells(null);
		_dialogue.TweenText(MPTextDefs.MP_FRIEND_LOADING_FAILED_DIALOGUE);
	}

	public void DisplayAcceptRequestError(eFriendRespondToRequestError error)
	{
		if (error != eFriendRespondToRequestError.FRIEND_LIMIT_SELF)
		{
			_dialogue.TweenText(MPTextDefs.MP_FRIEND_REQUEST_ACCEPT_FAILED_DIALOGUE);
		}
		else
		{
			_dialogue.TweenText(MPTextDefs.MP_FRIEND_REQUEST_ACCEPT_LIMIT_DIALOGUE);
		}
	}

	public void DisplayDenyRequestError(eFriendRespondToRequestError error)
	{
		_dialogue.TweenText(MPTextDefs.MP_FRIEND_REQUEST_DENY_FAILED_DIALOGUE);
	}

	public void DisplayRemoveRequestError(MPFriendData data, eFriendRemoveError error)
	{
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_REMOVE_FAILED_DIALOGUE, data.username));
	}

	public void ShowMenu(MenuBase menu)
	{
		_friendMenu.SetPositionOut();
		_friendRequestMenu.SetPositionOut();
		if (menu != null)
		{
			menu.SetPositionIn();
		}
	}

	public IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		_scrollMenu.SetInteractable(false);
		StartCoroutine(_friendMenu.TweenOut());
		StartCoroutine(_friendRequestMenu.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
		}
		_scrollMenu.SetInteractable(true);
	}

	private void ScrollCellSelectedHandler(CellBase cell, int index)
	{
		string text = ((_selectedCellData == null) ? null : _selectedCellData.userId);
		if (cell is MPFriendRequestMortyCell)
		{
			if (App.net.friend.mortyRequestCountdown > 0f)
			{
				_dialogue.TweenText(MPTextDefs.MP_FRIEND_MORTY_REQUEST_COUNTDOWN_DIALOGUE);
			}
			else if (this.RequestMortyEvent != null)
			{
				this.RequestMortyEvent();
				SetSelectedCellData(null);
			}
		}
		else if (cell is MPFriendRequestCell)
		{
			MPFriendData data = ((MPFriendRequestCell)cell).data;
			StartCoroutine(ShowMenuWithTween(_friendRequestMenu));
			if (_selectedCellData != data)
			{
				SetSelectedCellData(data);
				if (text != data.userId)
				{
					_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_REQUEST_SELECTED_DIALOGUE, _selectedCellData.username));
				}
			}
		}
		else if (cell is MPFriendCell)
		{
			MPFriendData data2 = ((MPFriendCell)cell).data;
			StartCoroutine(ShowMenuWithTween(_friendMenu));
			if (_selectedCellData != data2)
			{
				SetSelectedCellData(data2);
				if (text != data2.userId || _selectedCell is MPFriendRequestCell)
				{
					_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_SELECTED_DIALOGUE, _selectedCellData.username));
				}
				_friendMenu.SetBtnEnabled(0, _selectedCellData.IsAvailableForBattle());
				_friendMenu.SetBtnEnabled(1, _selectedCellData.IsAvailableForDonating());
			}
		}
		if (cell.retainSelection)
		{
			_selectedCell = cell;
		}
	}

	public void ScrollCellRemoveHandler(MPFriendCell cell)
	{
		if (this.RemoveEvent != null)
		{
			this.RemoveEvent(cell.data);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.AddFriendEvent != null)
			{
				this.AddFriendEvent();
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

	public void FriendBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BattleEvent != null)
			{
				this.BattleEvent(_selectedCellData);
			}
			break;
		case 1:
			if (App.net.player.data.GetAllMortyDatasOfId(_selectedCellData.requestingMortyId).Count <= 0)
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_DONATE_MORTY_NONE_DIALOGUE, MPMortyDefs.Name(_selectedCellData.requestingMortyId)));
			}
			else if (this.DonateEvent != null)
			{
				this.DonateEvent(_selectedCellData);
			}
			break;
		}
	}

	public void FriendRequestBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.AcceptEvent != null)
			{
				this.AcceptEvent(_selectedCellData);
			}
			break;
		case 1:
			if (this.DenyEvent != null)
			{
				this.DenyEvent(_selectedCellData);
			}
			break;
		}
	}
}
