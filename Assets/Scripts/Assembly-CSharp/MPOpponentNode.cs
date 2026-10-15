using System.Collections.Generic;
using UnityEngine;

public class MPOpponentNode : MPNode
{
	protected MPNodeCharacterMoveModule _characterMoveModule;

	protected List<MPOpponentMortyNode> _followingMortyNodes;

	private MPOpponentNodeStates _states;

	public MPOpponentData data { get; private set; }

	public virtual void Awake()
	{
		_states = base.transform.FindChild("States").GetComponent<MPOpponentNodeStates>();
	}

	public override void SetUp()
	{
		base.SetUp();
		if (data == null)
		{
			data = new MPOpponentData();
		}
		_followingMortyNodes = new List<MPOpponentMortyNode>();
		touchPositionOffset = new Vector3(0f, 1f, 0f);
		RefreshAvatarSprites();
	}

	public virtual void SetUpWithData(MPOpponentData opponentData, eDirection direction)
	{
		data = opponentData;
		initialDirection = direction;
		SetUp();
		RefreshState();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		RemoveFollowingMortyNodes();
	}

	public override void ManualGroundCollisionUpdate()
	{
		base.ManualGroundCollisionUpdate();
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			_followingMortyNodes[i].ManualGroundCollisionUpdate();
		}
	}

	public void AddFollowingMortyNode(MPOpponentMortyNode mortyNode)
	{
		_followingMortyNodes.Add(mortyNode);
		eMPOpponentState state = data.state;
		if (state == eMPOpponentState.BATTLE)
		{
			mortyNode.gameObject.SetActive(false);
		}
	}

	public void RemoveFollowingMortyNodes()
	{
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			Object.Destroy(_followingMortyNodes[i].gameObject);
		}
		_followingMortyNodes.Clear();
	}

	private void ShowMorties(bool isShow)
	{
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			_followingMortyNodes[i].gameObject.SetActive(isShow);
		}
	}

	public Coord[] GetMortyCoords()
	{
		Coord[] array = new Coord[_followingMortyNodes.Count];
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			array[i] = _followingMortyNodes[i].GetCoord();
		}
		return array;
	}

	public eDirection[] GetMortyDirections()
	{
		eDirection[] array = new eDirection[_followingMortyNodes.Count];
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			array[i] = _followingMortyNodes[i].GetDirection();
		}
		return array;
	}

	public MPOpponentMortyNode[] GetFollowingMorties()
	{
		if (_followingMortyNodes != null)
		{
			return _followingMortyNodes.ToArray();
		}
		return null;
	}

	public void RefreshAvatarSprites()
	{
		_characterMoveModule = SetUpCharacterMoveModule(data.assetId);
	}

	public void RefreshState()
	{
		_states.SetState(data.state, data.type == eMPOpponentType.PLAYER);
		switch (data.state)
		{
		default:
			_characterMoveModule.ShowSprite(true);
			ShowMorties(true);
			break;
		case eMPOpponentState.BATTLE:
			_characterMoveModule.ShowSprite(false);
			ShowMorties(false);
			break;
		case eMPOpponentState.OCCUPIED:
			_characterMoveModule.ShowSprite(true);
			ShowMorties(true);
			break;
		}
	}

	public override void UpdateWithUdpPlacementData(MPNodePlacementData placementData)
	{
		base.UpdateWithUdpPlacementData(placementData);
		placementData.directionHistory = GetMortyDirections();
		data.placementData = placementData;
	}

	public override void UDPPlacementSnapFollowingNodesHandler(Coord[] coords)
	{
		base.UDPPlacementSnapFollowingNodesHandler(coords);
		if (coords == null)
		{
			foreach (MPOpponentMortyNode followingMortyNode in _followingMortyNodes)
			{
				followingMortyNode.moveModule.SetPosition(moveModule.coord);
			}
			return;
		}
		Coord coord = moveModule.coord;
		for (int i = 0; i < _followingMortyNodes.Count; i++)
		{
			Coord coord2 = ((i >= coords.Length) ? coord : coords[i]);
			_followingMortyNodes[i].moveModule.SetPosition(coord2);
			_followingMortyNodes[i].moveModule.SetDirection(coord);
			coord = coord2;
		}
	}
}
