using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MPFriendAddUI : UIBase
{
	public delegate void SearchDelegate(string username);

	public delegate void SendDelegate(string userId);

	public delegate void InviteDelegate();

	private GameObject _searchBox;

	private Text _title;

	private InputField _inputField;

	private HeaderCell _headerCell;

	private MPFriendRequestCell _requestCell;

	private MenuBase _searchMenu;

	private MenuBase _sendMenu;

	private MenuBase _inviteMenu;

	private MenuDialogue _dialogue;

	public event SearchDelegate SearchForUserEvent;

	public event SendDelegate SendFriendRequestEvent;

	public event InviteDelegate InviteEvent;

	protected override void LateAwake()
	{
		_searchBox = base.transform.FindChild("SearchBox").gameObject;
		_title = base.transform.FindChild("SearchBox/InputTitle").GetComponent<Text>();
		_inputField = base.transform.FindChild("SearchBox/UsernameInput").GetComponent<InputField>();
		_headerCell = base.transform.FindChild("HeaderCell").GetComponent<HeaderCell>();
		_requestCell = base.transform.FindChild("RequestCell").GetComponent<MPFriendRequestCell>();
		_searchMenu = base.transform.FindChild("MenuBar/SearchMenu").GetComponent<MenuBase>();
		_sendMenu = base.transform.FindChild("MenuBar/SendMenu").GetComponent<MenuBase>();
		_inviteMenu = base.transform.FindChild("MenuBar/InviteMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		_dialogue.SetText(MPTextDefs.MP_FRIEND_ADD_SEARCH_DIALOGUE);
		ShowMenu(_searchMenu);
		_requestCell.SetUp(null);
		_headerCell.gameObject.SetActive(false);
		_requestCell.gameObject.SetActive(false);
	}

	public override void SetLabels()
	{
		_headerCell.SetUp(MPTextDefs.MP_FRIEND_ADD_REQUEST_CELL_DESCRIPTION);
		_searchMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_ADD_SEARCH_BTN);
		_sendMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_ADD_SEND_BTN);
		_inviteMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_ADD_INVITE_BTN);
		_title.text = MPTextDefs.MP_FRIEND_ADD_TITLE;
	}

	protected override void AddEvents()
	{
		_searchMenu.BtnPressedEvent += SearchMenuBtnPressedHandler;
		_sendMenu.BtnPressedEvent += SendMenuBtnPressedHandler;
		_inviteMenu.BtnPressedEvent += InviteMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_searchMenu.BtnPressedEvent -= SearchMenuBtnPressedHandler;
		_sendMenu.BtnPressedEvent -= SendMenuBtnPressedHandler;
		_inviteMenu.BtnPressedEvent -= InviteMenuBtnPressedHandler;
	}

	private void SetStateWaitingForSearch()
	{
		_dialogue.TweenText(MPTextDefs.MP_FRIEND_ADD_SEARCH_WAITING_DIALOGUE);
		StartCoroutine(ShowMenuWithTween(null));
		if (this.SearchForUserEvent != null)
		{
			this.SearchForUserEvent(_inputField.text);
		}
	}

	private void SetStateSearchSuccess(MPFriendData friendData)
	{
		_searchBox.gameObject.SetActive(false);
		_headerCell.gameObject.SetActive(true);
		_requestCell.gameObject.SetActive(true);
		_requestCell.SetUp(friendData);
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_DIALOGUE, friendData.username));
		StartCoroutine(ShowMenuWithTween(_sendMenu));
	}

	private void SetStateSearchFailed(eFriendSearchForUserError error)
	{
		if (error != eFriendSearchForUserError.NONE)
		{
			_dialogue.TweenText(MPTextDefs.MP_FRIEND_ADD_SEARCH_FAILED_DIALOGUE);
		}
		else
		{
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_INVALID_USERNAME_DIALOGUE, _inputField.text));
		}
		StartCoroutine(ShowMenuWithTween(_searchMenu));
	}

	private void SetStateWaitingForAddFriend()
	{
		_dialogue.TweenText(MPTextDefs.MP_FRIEND_ADD_SEND_WAITING_DIALOGUE);
		StartCoroutine(ShowMenuWithTween(null));
		if (this.SendFriendRequestEvent != null)
		{
			this.SendFriendRequestEvent(_requestCell.data.userId);
		}
	}

	private void SetStateAddFriendSuccess()
	{
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_SUCCESS_DIALOGUE, _requestCell.data.username));
		_requestCell.SetUp(null);
		_searchBox.gameObject.SetActive(true);
		_headerCell.gameObject.SetActive(false);
		_requestCell.gameObject.SetActive(false);
		StartCoroutine(ShowMenuWithTween(_searchMenu));
		_inputField.text = string.Empty;
	}

	private void SetStateAddFriendFailed(eFriendSendRequestError error)
	{
		switch (error)
		{
		default:
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_FAILED_DIALOGUE, _requestCell.data.username));
			break;
		case eFriendSendRequestError.FRIEND_DUPLICATE:
			if (App.net.friend.friends.Find((MPFriendData e) => e.username == _requestCell.data.username) != null)
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_ALREADY_FRIENDS_DIALOGUE, _requestCell.data.username));
			}
			else if (App.net.friend.friendRequests.Find((MPFriendData e) => e.username == _requestCell.data.username) != null)
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_ALREADY_RECEIVED_DIALOGUE, _requestCell.data.username));
			}
			else
			{
				_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_ALREADY_SENT_DIALOGUE, _requestCell.data.username));
			}
			break;
		case eFriendSendRequestError.FRIEND_LIMIT_SELF:
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_LIMIT_MINE_DIALOGUE, _requestCell.data.username));
			break;
		case eFriendSendRequestError.FRIEND_LIMIT_OTHER:
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_ADD_SEND_LIMIT_OTHER_DIALOGUE, _requestCell.data.username));
			break;
		}
		StartCoroutine(ShowMenuWithTween(_sendMenu));
	}

	private void SetStateBacktoSearch()
	{
		_requestCell.SetUp(null);
		_searchBox.gameObject.SetActive(true);
		_headerCell.gameObject.SetActive(false);
		_requestCell.gameObject.SetActive(false);
		_dialogue.SetText(MPTextDefs.MP_FRIEND_ADD_SEARCH_DIALOGUE);
		StartCoroutine(ShowMenuWithTween(_searchMenu));
	}

	public void ShowMenu(MenuBase menu)
	{
		_searchMenu.SetPositionOut();
		_sendMenu.SetPositionOut();
		if (menu != null)
		{
			menu.SetPositionIn();
		}
	}

	public IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		StartCoroutine(_searchMenu.TweenOut());
		StartCoroutine(_sendMenu.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
		}
	}

	public void SearchForUserComplete(MPFriendData friendData, eFriendSearchForUserError error)
	{
		if (error == eFriendSearchForUserError.NONE && friendData != null)
		{
			SetStateSearchSuccess(friendData);
		}
		else
		{
			SetStateSearchFailed(error);
		}
	}

	public void SendFriendRequestComplete(eFriendSendRequestError error)
	{
		if (error == eFriendSendRequestError.NONE)
		{
			SetStateAddFriendSuccess();
		}
		else
		{
			SetStateAddFriendFailed(error);
		}
	}

	public void SearchMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			SetStateWaitingForSearch();
			break;
		case 1:
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			break;
		}
	}

	public void SendMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			SetStateWaitingForAddFriend();
			break;
		case 1:
			SetStateBacktoSearch();
			break;
		}
	}

	public void InviteMenuBtnPressedHandler(int index)
	{
		if (index == 0 && this.InviteEvent != null)
		{
			this.InviteEvent();
		}
	}
}
