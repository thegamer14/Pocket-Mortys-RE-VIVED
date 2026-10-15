using System.Collections.Generic;
using UnityEngine;

public class MPWorldController : MonoBehaviour
{
	public delegate void MPWorldControllerDelegate();

	public delegate void MPWorldControllerInteractDelegate(MPUserNode playerNode, MPNode node);

	private const float _kCameraOffsetY = 20f;

	private const float _kCameraOffsetZ = -15f;

	private const float _kCameraRotationX = 52f;

	private Transform _container;

	private Transform _nodeContainer;

	private Transform _worldTrans;

	private WorldCamera _cameraFollow;

	private NodeResourcePool _nodeResourcePool;

	private SortedList<string, MPNode> _allNetNodes;

	private MPUserNode _playerNode;

	public event MPWorldControllerDelegate PlayerInteractAvailableEvent;

	public event MPWorldControllerDelegate PlayerInteractUnavailableEvent;

	public event MPWorldControllerInteractDelegate PlayerInteractStartRequestEvent;

	public event MPWorldControllerInteractDelegate PlayerInteractEndRequestEvent;

	private void Awake()
	{
		_container = new GameObject("Container").transform;
		_container.SetParent(base.transform);
		_nodeContainer = new GameObject("Other Nodes").transform;
		_nodeContainer.SetParent(_container);
		_cameraFollow = Camera.main.gameObject.AddComponent<WorldCamera>();
	}

	public void SetUp(string worldID)
	{
		_allNetNodes = new SortedList<string, MPNode>();
		_nodeResourcePool = new NodeResourcePool("Multiplayer/World/Nodes/{0}/", worldID);
		CreateWorld(worldID);
		SetUpClientNodes();
		SetUpRoomNodes();
		if (TestDefs.MP_DEBUG_SPAWN_POINTS)
		{
			DebugCreateSpawnPoints();
		}
	}

	public void SetUpClientNodes()
	{
		MPNode[] componentsInChildren = _worldTrans.FindChild("Nodes").GetComponentsInChildren<MPNode>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetUp();
		}
	}

	public void SetUpRoomNodes()
	{
		for (int i = 0; i < App.net.room.users.Count; i++)
		{
			if (App.net.room.users[i].type == eMPOpponentType.PLAYER)
			{
				CreatePlayerNode(App.net.room.users[i]);
			}
			else
			{
				CreateUserNode(App.net.room.users[i]);
			}
		}
		for (int j = 0; j < App.net.room.wildMorties.Count; j++)
		{
			CreateWildMortyNode(App.net.room.wildMorties[j]);
		}
		for (int k = 0; k < App.net.room.pickups.Count; k++)
		{
			CreatePickupNode(App.net.room.pickups[k]);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(_cameraFollow);
		RemoveNode(_playerNode);
	}

	public void WorldTouched(Vector2 viewportPoint)
	{
		float num = float.PositiveInfinity;
		MPUserNode mPUserNode = null;
		foreach (KeyValuePair<string, MPNode> allNetNode in _allNetNodes)
		{
			if (allNetNode.Value.interactionType == eMPNodeInteractionType.USER && allNetNode.Value != _playerNode)
			{
				float num2 = Vector2.Distance(viewportPoint, GetViewportPointForNode(allNetNode.Value));
				if (num2 < num && num2 < 0.2f)
				{
					num = num2;
					mPUserNode = allNetNode.Value as MPUserNode;
				}
			}
		}
		if (mPUserNode != null)
		{
			PlayerInteractStartRequestHandler(mPUserNode);
		}
	}

	public void DPadChanged(eDirection direction)
	{
		_playerNode.DPadChanged(direction);
	}

	public void FaceBtnAPressed()
	{
		_playerNode.FaceBtnAPressed();
	}

	private void CreateWorld(string worldId)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(string.Format("Multiplayer/World/Dimensions/{0}", MPWorldDefs.GetResourceId(worldId))));
		_worldTrans = gameObject.transform;
		_worldTrans.SetParent(_container, true);
	}

	public MPNode CreateNode(string resourceName, Coord coord, string netId = null)
	{
		Object obj = _nodeResourcePool.Get(resourceName);
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(obj) as GameObject;
		MPNode component = gameObject.GetComponent<MPNode>();
		component.name = obj.name;
		component.transform.SetParent(_nodeContainer, false);
		Vector3 vector = coord.ToVector3() * 1f + MPGameDefs.kGridToUnitOffset;
		vector.y = MPNodeUtils.GetGroundCollisionValue(vector);
		component.transform.localPosition = vector;
		if (!string.IsNullOrEmpty(netId))
		{
			if (_allNetNodes.ContainsKey(netId))
			{
				RemoveNode(_allNetNodes[netId]);
			}
			_allNetNodes.Add(netId, component);
		}
		return component;
	}

	public void RemoveNode(MPNode node)
	{
		if (!(node == null))
		{
			if (node == _playerNode)
			{
				_playerNode.InteractAvailableEvent -= PlayerInteractAvailableHandler;
				_playerNode.InteractUnavailableEvent -= PlayerInteractUnavailableHandler;
				_playerNode.InteractStartRequestEvent -= PlayerInteractStartRequestHandler;
				_playerNode.InteractEndRequestEvent -= PlayerInteractEndRequestHandler;
			}
			int num = _allNetNodes.IndexOfValue(node);
			if (num >= 0)
			{
				_allNetNodes.RemoveAt(num);
			}
			Object.Destroy(node.gameObject);
		}
	}

	public void RemoveNetNode(string netId)
	{
		if (_allNetNodes.ContainsKey(netId))
		{
			RemoveNode(_allNetNodes[netId]);
		}
	}

	public void UpdateNetNodeWithUdpPlacementData(string netId, MPNodePlacementData placementData)
	{
		if (_allNetNodes.ContainsKey(netId))
		{
			_allNetNodes[netId].UpdateWithUdpPlacementData(placementData);
		}
	}

	public Vector2 GetViewportPointForNode(MPNode node)
	{
		return Camera.main.WorldToViewportPoint(node.transform.position + node.touchPositionOffset);
	}

	public void CreateOpponentMortyNodes(MPOpponentNode opponentNode)
	{
		List<MPOpponentMortyData> allPartyMortyDatas = opponentNode.data.GetAllPartyMortyDatas();
		opponentNode.RemoveFollowingMortyNodes();
		Coord[] coordHistory = opponentNode.data.placementData.coordHistory;
		eDirection[] directionHistory = opponentNode.data.placementData.directionHistory;
		MPNode mPNode = opponentNode;
		Coord coord = opponentNode.data.placementData.coord;
		for (int i = 0; i < allPartyMortyDatas.Count; i++)
		{
			Coord coord2 = ((i >= coordHistory.Length) ? coord : coordHistory[i]);
			eDirection initialDirection = ((i >= directionHistory.Length) ? Coord.GetDirectionFromCoord(coord - coord2) : directionHistory[i]);
			MPOpponentMortyNode mPOpponentMortyNode = CreateOpponentMortyNode(allPartyMortyDatas[i], coord2, initialDirection, opponentNode.data.type);
			mPNode.AddFollowingNode(mPOpponentMortyNode);
			mPNode = mPOpponentMortyNode;
			coord = coord2;
			opponentNode.AddFollowingMortyNode(mPOpponentMortyNode);
		}
	}

	public void RefreshOpponentNodeStateWithData(MPOpponentData opponentData)
	{
		if (_allNetNodes.ContainsKey(opponentData.netId))
		{
			((MPOpponentNode)_allNetNodes[opponentData.netId]).RefreshState();
		}
	}

	public void RefreshOpponentWithModifiedData(MPOpponentData opponentData)
	{
		if (_allNetNodes.ContainsKey(opponentData.netId) && _allNetNodes[opponentData.netId] is MPUserNode)
		{
			MPUserNode mPUserNode = (MPUserNode)_allNetNodes[opponentData.netId];
			opponentData.placementData.directionHistory = mPUserNode.GetMortyDirections();
			CreateOpponentMortyNodes(mPUserNode);
			mPUserNode.RefreshLevel();
			mPUserNode.RefreshAvatarSprites();
		}
	}

	public MPUserNode CreateUserNode(MPUserData userData)
	{
		MPUserNode mPUserNode = (MPUserNode)CreateNode("MPUser", userData.placementData.coord, userData.netId);
		mPUserNode.SetUpWithData(userData, userData.placementData.direction);
		CreateOpponentMortyNodes(mPUserNode);
		return mPUserNode;
	}

	public void SendPlayerPlacementData(bool snap = false)
	{
		if (_playerNode != null)
		{
			App.net.room.nodeSender.SendPosition(_playerNode.moveModule.coord, _playerNode.moveModule.direction, snap);
		}
	}

	public void DisplayEmoteOnUser(string emoteId, string userId)
	{
		if (_allNetNodes.ContainsKey(userId) && _allNetNodes[userId] is MPUserNode)
		{
			((MPUserNode)_allNetNodes[userId]).DisplayEmote(emoteId);
		}
	}

	private void CreatePlayerNode(MPUserData playerData)
	{
		_playerNode = CreateUserNode(playerData);
		_cameraFollow.SetUp(_playerNode.transform, new Vector3(0f, 20f, -15f), new Vector3(52f, 0f, 0f));
		if (playerData.placementData.walkIn)
		{
			StartCoroutine(_playerNode.ForceMoveInDirectionFor(eDirection.DOWN, 5));
			playerData.placementData.walkIn = false;
		}
		_playerNode.InteractAvailableEvent += PlayerInteractAvailableHandler;
		_playerNode.InteractUnavailableEvent += PlayerInteractUnavailableHandler;
		_playerNode.InteractStartRequestEvent += PlayerInteractStartRequestHandler;
		_playerNode.InteractEndRequestEvent += PlayerInteractEndRequestHandler;
	}

	public MPWildMortyNode CreateWildMortyNode(MPOpponentData opponentData)
	{
		MPWildMortyNode mPWildMortyNode = (MPWildMortyNode)CreateNode("MPWildMorty", opponentData.placementData.coord, opponentData.netId);
		mPWildMortyNode.SetUpWithData(opponentData, opponentData.placementData.direction);
		return mPWildMortyNode;
	}

	public MPOpponentMortyNode CreateOpponentMortyNode(MPOpponentMortyData mortyData, Coord coord, eDirection initialDirection, eMPOpponentType followingType)
	{
		float characterZDepth = ((followingType != eMPOpponentType.PLAYER) ? 0.249f : 0.247f);
		MPOpponentMortyNode mPOpponentMortyNode = (MPOpponentMortyNode)CreateNode("MPOpponentMorty", coord);
		mPOpponentMortyNode.SetUpWithData(mortyData, initialDirection, characterZDepth);
		return mPOpponentMortyNode;
	}

	public MPPickupNode CreatePickupNode(MPPickupData pickupData)
	{
		MPPickupNode mPPickupNode = (MPPickupNode)CreateNode("MPPickup", pickupData.placementData.coord, pickupData.netId);
		mPPickupNode.SetUpWithData(pickupData, pickupData.placementData.direction);
		return mPPickupNode;
	}

	public void RefreshIntoCache()
	{
		_cameraFollow.SetAudioListenerEnabled(false);
		_playerNode.data.placementData = new MPNodePlacementData(_playerNode.GetCoord(), _playerNode.GetDirection(), _playerNode.GetMortyCoords(), _playerNode.GetMortyDirections());
	}

	public void RefreshFromCache()
	{
		_cameraFollow.SetAudioListenerEnabled(true);
		foreach (KeyValuePair<string, MPNode> allNetNode in _allNetNodes)
		{
			allNetNode.Value.ManualGroundCollisionUpdate();
		}
	}

	public void ShowWorld(bool value)
	{
		if (_container != null)
		{
			_container.gameObject.SetActive(value);
		}
	}

	private void PlayerInteractAvailableHandler()
	{
		if (this.PlayerInteractAvailableEvent != null)
		{
			this.PlayerInteractAvailableEvent();
		}
	}

	private void PlayerInteractUnavailableHandler()
	{
		if (this.PlayerInteractUnavailableEvent != null)
		{
			this.PlayerInteractUnavailableEvent();
		}
	}

	protected void PlayerInteractStartRequestHandler(MPNode node)
	{
		if (!_playerNode.isForcedMoving && this.PlayerInteractStartRequestEvent != null)
		{
			this.PlayerInteractStartRequestEvent(_playerNode, node);
		}
	}

	private void PlayerInteractEndRequestHandler(MPNode node)
	{
		if (!_playerNode.isForcedMoving && this.PlayerInteractEndRequestEvent != null)
		{
			this.PlayerInteractEndRequestEvent(_playerNode, node);
		}
	}

	private void DebugCreateSpawnPoints()
	{
		List<Vector3> wildMortySpawnCoords = MPWorldDefs.GetWildMortySpawnCoords(MPSharedStateInfo.worldInfo.worldId);
		List<Vector3> itemSpawnCoords = MPWorldDefs.GetItemSpawnCoords(MPSharedStateInfo.worldInfo.worldId);
		GameObject gameObject = new GameObject("Spawn Points");
		gameObject.transform.SetParent(_container);
		for (int i = 0; i < wildMortySpawnCoords.Count; i++)
		{
			Vector3 vector = wildMortySpawnCoords[i] * 1f + MPGameDefs.kGridToUnitOffset;
			GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			gameObject2.name = "Morty " + i;
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = new Vector3(vector.x, MPNodeUtils.GetGroundCollisionValue(wildMortySpawnCoords[i]), vector.z);
			gameObject2.GetComponent<MeshRenderer>().material.color = Color.red;
		}
		for (int j = 0; j < itemSpawnCoords.Count; j++)
		{
			Vector3 vector2 = itemSpawnCoords[j] * 1f + MPGameDefs.kGridToUnitOffset;
			GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			gameObject3.name = "Item " + j;
			gameObject3.transform.SetParent(gameObject.transform);
			gameObject3.transform.localPosition = new Vector3(vector2.x, MPNodeUtils.GetGroundCollisionValue(itemSpawnCoords[j]), vector2.z);
			gameObject3.GetComponent<MeshRenderer>().material.color = Color.blue;
		}
	}
}
