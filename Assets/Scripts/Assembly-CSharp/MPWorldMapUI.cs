using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPWorldMapUI : UIBase
{
	public delegate void UIDelegate();

	public UIDelegate BackEvent;

	private Image _background;

	private GameObject _iconTemplate;

	private Dictionary<string, Image> _userIcons;

	private MenuBase _mainMenu;

	private string _worldId;

	private MPWorldMapData _mapData;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background").GetComponent<Image>();
		_iconTemplate = base.transform.FindChild("UserIcons/IconTemplate").gameObject;
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
	}

	public void SetUp(string worldId, List<MPUserData> users)
	{
		_iconTemplate.SetActive(false);
		SetUpForWorld(worldId);
		AddAllUsers(users);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetUpForWorld(string worldId)
	{
		_worldId = worldId;
		_mapData = MPWorldDefs.GetMapWorldDataForId(worldId);
		_background.sprite = Resources.Load<Sprite>(string.Format("Multiplayer/UI/Sprites/MPWorldMap/{0}/Background", _worldId));
	}

	private void AddAllUsers(List<MPUserData> users)
	{
		_userIcons = new Dictionary<string, Image>();
		for (int i = 0; i < users.Count; i++)
		{
			AddUser(users[i]);
		}
	}

	public void AddUser(MPUserData user)
	{
		GameObject gameObject = Object.Instantiate(_iconTemplate, _iconTemplate.transform.parent);
		Image component = gameObject.GetComponent<Image>();
		gameObject.SetActive(true);
		component.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", user.assetId));
		component.SetNativeSize();
		_userIcons.Add(user.netId, component);
		SetUserPlacementData(user.netId, user.placementData);
		if (user.netId == App.net.player.data.netId)
		{
			component.rectTransform.SetAsLastSibling();
			TweenUserIcon(component);
		}
		else
		{
			component.rectTransform.SetAsFirstSibling();
		}
	}

	public void RemoveUser(MPUserData user)
	{
		if (_userIcons.ContainsKey(user.netId))
		{
			Object.Destroy(_userIcons[user.netId].gameObject);
			_userIcons.Remove(user.netId);
		}
	}

	public void SetUserPlacementData(string netId, MPNodePlacementData placementData)
	{
		if (_userIcons.ContainsKey(netId))
		{
			Vector2 mapPositionForPlacementData = GetMapPositionForPlacementData(placementData);
			_userIcons[netId].rectTransform.localPosition = mapPositionForPlacementData;
			_userIcons[netId].enabled = placementData.coord != Coord.zero;
		}
	}

	private Vector2 GetMapPositionForPlacementData(MPNodePlacementData placementData)
	{
		float num = (float)(placementData.coord.x - _mapData.sizeMin.x) / (float)(_mapData.sizeMax.x - _mapData.sizeMin.x) * 100f;
		float num2 = (float)(placementData.coord.z - _mapData.sizeMin.z) / (float)(_mapData.sizeMax.z - _mapData.sizeMin.z) * 100f;
		float num3 = _mapData.topRight - _mapData.topLeft;
		float num4 = _mapData.bottomRight - _mapData.bottomLeft;
		float num5 = _mapData.bottomLeft - (_mapData.bottomLeft - _mapData.topLeft) / 100f * num2;
		float num6 = num4 - (num4 - num3) / 100f * num2;
		float num7 = 2f * (1f - num2 * 0.01f);
		float x = num5 + num6 / 100f * num;
		float y = _mapData.bottom + ((_mapData.top - _mapData.bottom) / 100f + num7) * num2;
		return new Vector2(x, y);
	}

	private void TweenUserIcon(Image userIcon)
	{
		userIcon.rectTransform.DOScale(1.4f, 1f).SetLoops(-1, LoopType.Yoyo);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (BackEvent != null)
		{
			BackEvent();
		}
	}
}
