using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
	public delegate void WorldControllerDelegate();

	public delegate void WorldControllerNodeStartDelegate(PlayerNode playerNode, AdvancedNode node, bool isTrigger);

	public delegate void WorldControllerNodeDelegate(PlayerNode playerNode, AdvancedNode node);

	public delegate void WorldControllerDialogueDelegate(DialogueData dialogueData);

	private const float _kCameraOffsetY = 20f;

	private const float _kCameraOffsetZ = -15f;

	private const float _kCameraRotationX = 52f;

	private GameObject _container;

	private string _worldID;

	private string _nodeSetID;

	private string _entranceID;

	protected int _segmentedWorldWidth;

	protected int _segmentedWorldDepth;

	protected int _worldAreaSize;

	private WorldCamera _cameraFollow;

	protected SegmentResourcePool _segmentResourcePool;

	protected List<Segment> _segments;

	private NodeResourcePool _nodeResourcePool;

	private AdvancedNode _spawnNode;

	protected PlayerNode _player;

	private GymNode _gymNode;

	private TrainerNode _gymLeader;

	protected Dictionary<ePlaceholderType, int> _nodeLimits;

	protected Dictionary<ePlaceholderType, int> _nodeCounts;

	protected List<string> _availableSignPostIDs;

	protected int _lastAvailableSignPostIDIndex;

	protected List<string> _usedTrainerIDs;

	protected List<string> _availableNPCIDs;

	protected string _availableRareMortyID;

	protected List<List<string>> _availableMortyIDs;

	private int _lastAvailableMortyIDIndex;

	protected List<string> _usedItemPartIDs;

	protected bool _itemCrateCreated;

	protected List<string> _availableQuestIDs;

	protected bool _alternateGachaMachine;

	[HideInInspector]
	public int itemCratesRemainingCount;

	[HideInInspector]
	public int trainersRemainingCount;

	[HideInInspector]
	public int gymLeadersRemainingCount;

	public event WorldControllerDelegate PlayerInteractAvailableEvent;

	public event WorldControllerDelegate PlayerInteractUnavailableEvent;

	public event WorldControllerNodeStartDelegate PlayerInteractStartEvent;

	public event WorldControllerNodeDelegate PlayerInteractEndEvent;

	public event WorldControllerDialogueDelegate DisplayDialogueEvent;

	private void Awake()
	{
		_container = new GameObject("Container");
		_container.transform.SetParent(base.transform);
		_segments = new List<Segment>();
		_usedItemPartIDs = new List<string>();
		_usedTrainerIDs = new List<string>();
		_cameraFollow = Camera.main.gameObject.AddComponent<WorldCamera>();
	}

	private void BaseSetUp(string worldID, string nodeSetID, string entranceID)
	{
		_segmentedWorldWidth = ((PlayerDefs.BadgeCount != 1) ? 3 : 2);
		_segmentedWorldDepth = 3;
		_worldAreaSize = _segmentedWorldWidth * _segmentedWorldDepth;
		_nodeLimits = WorldDefs.GetLimits(worldID, _worldAreaSize);
		_nodeCounts = WorldDefs.GetEmptyLimitDict();
		LoadResources(worldID, nodeSetID);
		_entranceID = entranceID;
	}

	public virtual void SetUp(string worldID, string nodeSetID, string entranceID)
	{
		BaseSetUp(worldID, nodeSetID, entranceID);
		_availableSignPostIDs = SignPostDefs.GetListOfRandomPooledIDs(true);
		_availableRareMortyID = ((!(UnityEngine.Random.value < 0.3f)) ? null : MortyDefs.GetRandomID(new RandomMortyParams
		{
			badgeLevelLock = true,
			rarityLock = eMortyRarityCheck.RARE
		}));
		_availableMortyIDs = MortyDefs.GetIDListsForWorld();
		_availableQuestIDs = PlayerDefs.GetAvailableQuestIDs();
		_availableNPCIDs = NPCDefs.GetListOfRandomNPCsForWorld(worldID, QuestDefs.ConvertGuestIDListToAssetIds(_availableQuestIDs));
		CreateSegments(worldID);
		CreateAllNodesFromAllSegments();
		RefreshAllTrainerNodeSights();
		if (_spawnNode != null)
		{
			string[] array = InteractionDefs.TransitionDirection(_entranceID).Split(':');
			eDirection oppositeDirection = HelperMethods.GetOppositeDirection((eDirection)Enum.Parse(typeof(eDirection), array[0]));
			int distance = int.Parse(array[1]);
			CreatePlayerNode(_spawnNode.transform.FindChild("InteractionPoint").position, oppositeDirection, distance);
			_spawnNode.InteractionStarted(eDirection.NONE, false);
			if (entranceID == "CitadelCourierFlapPortal")
			{
				_player.AlignMortysAroundPlayer();
			}
		}
	}

	public void SetUpFromSave(string worldID, string nodeSetID)
	{
		BaseSetUp(worldID, nodeSetID, WorldDefs.GetSavedSegmentedEntranceID());
		CreateSegmentsFromSaveData();
		CreateAllNodesFromAllSegmentSaveDatas();
		RemoveNodesFromSegmentPlaceholders();
		RefreshAllTrainerNodeSights();
		if (_spawnNode != null)
		{
			string[] array = InteractionDefs.TransitionDirection(_entranceID).Split(':');
			eDirection oppositeDirection = HelperMethods.GetOppositeDirection((eDirection)Enum.Parse(typeof(eDirection), array[0]));
			CreatePlayerNode(_spawnNode.transform.FindChild("InteractionPoint").position, oppositeDirection, int.Parse(array[1]));
			_spawnNode.InteractionStarted(eDirection.NONE, false);
		}
		else
		{
			CreatePlayerNode(WorldDefs.GetSavedSegmentedPlayerPosition(), WorldDefs.GetSavedSegmentedPlayerDirection(), -1, WorldDefs.GetSavedSegmentedPlayerMortyPositions(), WorldDefs.GetSavedSegmentedPlayerMortyDirections());
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(_cameraFollow);
	}

	public void DPadChanged(eDirection direction)
	{
		_player.DPadChanged(direction);
	}

	public void FaceBtnAPressed()
	{
		_player.FaceBtnAPressed();
	}

	public void LoadResources(string worldID, string nodeSetID = null)
	{
		_worldID = worldID;
		_segmentResourcePool = new SegmentResourcePool(_worldID);
		_nodeSetID = nodeSetID;
		_nodeResourcePool = new NodeResourcePool("World/Nodes/{0}/", _nodeSetID);
	}

	protected virtual void CreateSegments(string worldID)
	{
		if (WorldDefs.IsSegmented(worldID))
		{
			SegmentedWorldGenerator segmentedWorldGenerator = new SegmentedWorldGenerator();
			SegmentedSegmentInfo[,] array = segmentedWorldGenerator.Generate(_segmentedWorldWidth, _segmentedWorldDepth, true);
			for (int i = 0; i < array.GetLength(1); i++)
			{
				for (int j = 0; j < array.GetLength(0); j++)
				{
					CreateSegment(array[j, i], j, array.GetLength(1) - 1 - i, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
				}
			}
		}
		else
		{
			SegmentedSegmentInfo info = new SegmentedSegmentInfo(eSegmentType.DEFAULT, eSegmentJointType.BASE, 0f);
			CreateSegment(info, 0, 0, WorldDefs.SegmentWidth(worldID), WorldDefs.SegmentDepth(worldID));
		}
	}

	protected virtual void CreateSegmentsFromSaveData()
	{
		List<SegmentSaveData> savedSegmentedWorldSegments = WorldDefs.GetSavedSegmentedWorldSegments();
		for (int i = 0; i < savedSegmentedWorldSegments.Count; i++)
		{
			SegmentedSegmentInfo info = new SegmentedSegmentInfo(savedSegmentedWorldSegments[i].name, savedSegmentedWorldSegments[i].type, savedSegmentedWorldSegments[i].jointType, savedSegmentedWorldSegments[i].rotation);
			CreateSegment(info, new Vector3(savedSegmentedWorldSegments[i].positionX, 0f, savedSegmentedWorldSegments[i].positionZ));
		}
	}

	protected void CreateSegment(SegmentedSegmentInfo info, int x, int z, float width, float depth)
	{
		CreateSegment(info, new Vector3(width / 2f + (float)x * width, 0f, depth / 2f + (float)z * depth));
	}

	protected Segment CreateSegment(SegmentedSegmentInfo info, Vector3 position)
	{
		UnityEngine.Object obj = _segmentResourcePool.Get(info.type, info.jointType, info.name);
		GameObject gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
		gameObject.name = obj.name;
		Segment component = gameObject.GetComponent<Segment>();
		component.type = info.type;
		component.jointType = info.jointType;
		gameObject.transform.SetParent(_container.transform);
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = Quaternion.Euler(0f, info.rotation, 0f);
		component.SetUp(_segmentResourcePool.GetSegmentedMaterial());
		_segments.Add(component);
		return component;
	}

	protected void CreateAllNodesFromAllSegmentSaveDatas()
	{
		List<SegmentSaveData> savedSegmentedWorldSegments = WorldDefs.GetSavedSegmentedWorldSegments();
		for (int i = 0; i < savedSegmentedWorldSegments.Count; i++)
		{
			CreateAllBasicNodesFromPlaceholders(_segments[i].GetAllBasicNodePlaceholders());
			CreateAllNonBasicNodesFromSaveData(savedSegmentedWorldSegments[i].nonBasicNodeDatas, _segments[i]);
		}
	}

	private void CreateAllBasicNodesFromSaveData(List<NodeSaveData> nodeDatas, Segment segment)
	{
		for (int i = 0; i < nodeDatas.Count; i++)
		{
			CreateNodeOfType(nodeDatas[i], segment);
		}
	}

	private void CreateAllNonBasicNodesFromSaveData(List<NodeSaveData> nodeDatas, Segment segment)
	{
		for (int i = 0; i < nodeDatas.Count; i++)
		{
			Node node = CreateNodeOfType(nodeDatas[i], segment);
			if (node != null)
			{
				segment.nonBasicNodes.Add(node);
			}
		}
	}

	private Node CreateNodeOfType(NodeSaveData nodeSaveData, Segment segment)
	{
		Node node = null;
		switch (nodeSaveData.type)
		{
		default:
			return CreateNode(nodeSaveData.name, nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.ITEM:
			return CreateItemNode(nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.MORTY:
			return CreateMortyNode((MortyData)nodeSaveData.data, true, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.NPC:
			return CreateNPCNode((NPCData)nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.TRAINER:
			return CreateTrainerNode((TrainerData)nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.PORTAL:
			return CreatePortalNode(nodeSaveData.name, nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.GYM:
			return CreateGymNode(nodeSaveData.data, WorldDefs.GetSavedSegmentedGymLeaderData(), segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.QUEST_GIVER:
			return CreateQuestGiverNode((QuestGiverData)nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		case eNodeType.ITEM_CRATE:
			return CreateItemCrateNode(nodeSaveData.data, segment.nonBasicNodesHolder, nodeSaveData.position, nodeSaveData.initialDirection, nodeSaveData.applyRotation);
		}
	}

	protected virtual Node CreateNode(string resourceName, NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		UnityEngine.Object obj = _nodeResourcePool.Get(resourceName);
		if (obj == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
		Node component = gameObject.GetComponent<Node>();
		component.name = obj.name;
		component.transform.SetParent(parent, false);
		component.transform.localPosition = position;
		component.SetUp(data, initialDirection, applyRotation);
		return component;
	}

	private ItemNode CreateItemNode(NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		return (ItemNode)CreateNode("Item", data, parent, position, initialDirection, applyRotation);
	}

	private ItemCrateNode CreateItemCrateNode(NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		_itemCrateCreated = true;
		itemCratesRemainingCount++;
		return (ItemCrateNode)CreateNode("ItemCrate", data, parent, position, initialDirection, applyRotation);
	}

	private MortyNode CreateMortyNode(MortyData mortyData, bool isWonderer, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		MortyNode mortyNode = (MortyNode)CreateNode("Morty", mortyData, parent, position, initialDirection, applyRotation);
		if (isWonderer)
		{
			mortyNode.AddModuleAIWander();
		}
		return mortyNode;
	}

	private NPCNode CreateNPCNode(NPCData npcData, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		NPCNode nPCNode = (NPCNode)CreateNode("NPC", npcData, parent, position, initialDirection, applyRotation);
		if (NPCDefs.IsWanderer(nPCNode.data.id))
		{
			nPCNode.AddModuleAIWander();
		}
		return nPCNode;
	}

	private TrainerNode CreateTrainerNode(TrainerData trainerData, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		TrainerNode trainerNode = (TrainerNode)CreateNode("Trainer", trainerData, parent, position, initialDirection, applyRotation);
		trainerNode.AddMorties(_nodeResourcePool.Get("Morty"));
		trainerNode.ActivateSightInteractionPoint(trainerData.type != eTrainerType.GYM_LEADER && !trainerData.isDefeated);
		if (!trainerData.isDefeated)
		{
			if (trainerData.type == eTrainerType.GYM_LEADER)
			{
				gymLeadersRemainingCount++;
			}
			else
			{
				trainersRemainingCount++;
			}
		}
		return trainerNode;
	}

	private Node CreatePortalNode(string resourceName, NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		AdvancedNode advancedNode = (AdvancedNode)CreateNode(resourceName, data, parent, position, initialDirection, applyRotation);
		if (advancedNode.data.id.Equals(_entranceID))
		{
			_spawnNode = advancedNode;
		}
		return advancedNode;
	}

	protected Node CreateGymNode(NodeData data, TrainerData gymLeaderData, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		_gymNode = (GymNode)CreateNode("Gym", data, parent, position, initialDirection, applyRotation);
		if (gymLeaderData == null)
		{
			gymLeaderData = TrainerDefs.GetNewRandomGymLeaderData(PlayerDefs.lastGymLeaderIDs);
			PlayerDefs.SetLastGymLeaderID(gymLeaderData.id);
		}
		else if (gymLeaderData.isDefeated)
		{
			RefreshGymLeaderDefeated();
		}
		_gymLeader = CreateTrainerNode(gymLeaderData, _gymNode.transform.parent, _gymNode.GetGymLeaderPosition(), eNodeInitialDirection.FIXED_DOWN, false);
		return _gymNode;
	}

	private QuestGiverNode CreateQuestGiverNode(QuestGiverData questGiverData, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		QuestGiverNode questGiverNode = (QuestGiverNode)CreateNode("QuestGiver", questGiverData, parent, position, initialDirection, applyRotation);
		if (QuestDefs.IsWanderer(questGiverNode.data.id))
		{
			questGiverNode.AddModuleAIWander();
		}
		return questGiverNode;
	}

	private CraftTableNode CreateCraftTableNode(NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		return (CraftTableNode)CreateNode("CraftTable", data, parent, position, initialDirection, applyRotation);
	}

	private GachaMachineNode CreateGachaMachineNode(NodeData data, Transform parent, Vector3 position, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		return (GachaMachineNode)CreateNode("GachaMachine", data, parent, position, initialDirection, applyRotation);
	}

	private void CreatePlayerNode(Vector3 position, eDirection direction, int distance, Vector3[] mortyPositions = null, eDirection[] mortyDirections = null)
	{
		StartCoroutine(CreatePlayerNodeCo(position, direction, distance, mortyPositions, mortyDirections));
	}

	protected virtual IEnumerator CreatePlayerNodeCo(Vector3 position, eDirection direction, int distance, Vector3[] mortyPositions = null, eDirection[] mortyDirections = null)
	{
		_player = (PlayerNode)CreateNode("Player", new NodeData(PlayerDefs.AssetID), _container.transform, position, eNodeInitialDirection.DOWN, false);
		_cameraFollow.SetUp(_player.transform, new Vector3(0f, 20f, -15f), new Vector3(52f, 0f, 0f));
		if (WorldDefs.IsWorldLockedToBounds(_worldID))
		{
			_cameraFollow.SetLocked(WorldDefs.BoundsMin(_worldID), WorldDefs.BoundsMax(_worldID));
		}
		_player.allowRandomVOClips = true;
		_player.allowRandomVOClipsForSegmentedWorld = WorldDefs.IsSegmented(_worldID);
		if (distance == -1)
		{
			_player.SetDirection(direction);
			_player.SetUpMorties(_nodeResourcePool.Get("Morty"), mortyPositions, mortyDirections);
			_player.InteractAvailableEvent += PlayerInteractAvailableHandler;
			_player.InteractUnavailableEvent += PlayerInteractUnavailableHandler;
			_player.InteractStartEvent += PlayerInteractStartHandler;
			_player.InteractEndEvent += PlayerInteractEndHandler;
		}
		else
		{
			yield return StartCoroutine(_player.SetUpMortiesWithAnimation(_nodeResourcePool.Get("Morty"), direction));
			_player.InteractAvailableEvent += PlayerInteractAvailableHandler;
			_player.InteractUnavailableEvent += PlayerInteractUnavailableHandler;
			_player.InteractStartEvent += PlayerInteractStartHandler;
			_player.InteractEndEvent += PlayerInteractEndHandler;
			yield return StartCoroutine(_player.ForceMoveInDirectionFor(direction, distance));
		}
		if (_spawnNode is PortalNode)
		{
			if (WorldDefs.IsSegmented(_worldID))
			{
				StartCoroutine(((PortalNode)_spawnNode).Close(true));
				yield break;
			}
			yield return StartCoroutine(((PortalNode)_spawnNode).Close());
			yield return StartCoroutine(((PortalNode)_spawnNode).Open());
		}
	}

	public void RemovePlayerNode(eDirection direction, int distance)
	{
		StartCoroutine(RemovePlayerNodeCo(direction, distance));
	}

	private IEnumerator RemovePlayerNodeCo(eDirection direction, int distance)
	{
		_player.InteractAvailableEvent -= PlayerInteractAvailableHandler;
		_player.InteractUnavailableEvent -= PlayerInteractUnavailableHandler;
		_player.InteractStartEvent -= PlayerInteractStartHandler;
		_player.InteractEndEvent -= PlayerInteractEndHandler;
		yield return StartCoroutine(_player.ForceMoveInDirectionFor(direction, distance));
		yield return StartCoroutine(_player.RemoveMortiesWithAnimation());
	}

	public void RefreshAllTrainerNodeSights()
	{
		TrainerNode[] componentsInChildren = base.transform.GetComponentsInChildren<TrainerNode>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ActivateSightInteractionPoint(componentsInChildren[i].trainerData.type != eTrainerType.GYM_LEADER && !componentsInChildren[i].trainerData.isDefeated);
		}
	}

	protected void CreateAllNodesFromAllSegments()
	{
		_segments.Shuffle();
		int num = 0;
		for (int i = 0; i < _segments.Count; i++)
		{
			if (_segments[i].jointType != eSegmentJointType.EDGE && _segments[i].jointType != eSegmentJointType.EDGE_CORNER)
			{
				num++;
			}
			CreateAllBasicNodesFromPlaceholders(_segments[i].GetAllBasicNodePlaceholders());
			CreateAllNonBasicNodesFromPlaceholders(_segments[i].GetAllNonBasicNodePlaceholders(), _segments[i], num);
		}
	}

	private void CreateAllBasicNodesFromPlaceholders(PlaceholderNode[] placeholders)
	{
		for (int i = 0; i < placeholders.Length; i++)
		{
			CreateNodeOfTypeFromPlaceholder(placeholders[i]);
			UnityEngine.Object.Destroy(placeholders[i].gameObject);
		}
	}

	private void CreateAllNonBasicNodesFromPlaceholders(PlaceholderNode[] placeholders, Segment segmentToSaveTo, int worldAreaSegmentCount)
	{
		placeholders.Shuffle();
		for (int i = 0; i < placeholders.Length; i++)
		{
			UnityEngine.Object.Destroy(placeholders[i].gameObject);
			if (_nodeLimits.ContainsKey(placeholders[i].type))
			{
				int num = _nodeLimits[placeholders[i].type];
				int num2 = _nodeCounts[placeholders[i].type];
				if (Mathf.FloorToInt((float)num / (float)_worldAreaSize * (float)worldAreaSegmentCount) - num2 <= 0)
				{
					continue;
				}
			}
			Node node = CreateNodeOfTypeFromPlaceholder(placeholders[i]);
			if (node != null)
			{
				segmentToSaveTo.nonBasicNodes.Add(node);
				if (_nodeCounts.ContainsKey(placeholders[i].type))
				{
					_nodeCounts[placeholders[i].type]++;
				}
			}
		}
	}

	private Node CreateNodeOfTypeFromPlaceholder(PlaceholderNode placeholder)
	{
		Node node = null;
		switch (placeholder.type)
		{
		default:
			return CreateNodeFromPlaceholder(placeholder);
		case ePlaceholderType.ITEM_PART_OR_CRATE:
			return CreateItemPartOrCrateNodeFromPlaceholder(placeholder);
		case ePlaceholderType.MORTY:
			return CreateWildMortyNodeFromPlaceholder(placeholder);
		case ePlaceholderType.NPC:
			return CreateNPCNodeFromPlaceholders(placeholder);
		case ePlaceholderType.TRAINER:
			return CreateTrainerNodeFromPlaceholders(placeholder);
		case ePlaceholderType.PORTAL:
			return CreatePortalNodeFromPlaceholder(placeholder);
		case ePlaceholderType.SIGN_POST:
			return CreateSignPostNodeFromPlaceholder(placeholder);
		case ePlaceholderType.GYM:
			return CreateGymNodeFromPlaceholder(placeholder);
		case ePlaceholderType.GACHA_OR_CRAFT:
			return CreateGachaOrCraftMachineNodeFromPlaceholder(placeholder);
		case ePlaceholderType.QUEST_GIVER:
			return CreateQuestGiverNodeFromPlaceholder(placeholder);
		}
	}

	private Node CreateNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		return CreateNode(placeholder.name, placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	private Node CreateItemPartOrCrateNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		if (string.IsNullOrEmpty(placeholder.data.id))
		{
			if (!_itemCrateCreated)
			{
				return CreateItemCrateNode(placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, eNodeInitialDirection.FIXED_DOWN, true);
			}
			string randomBadgeLockedRewardID = ItemDefs.GetRandomBadgeLockedRewardID(eItemType.PART, true, true, _usedItemPartIDs);
			placeholder.data.id = randomBadgeLockedRewardID;
			if (!_usedItemPartIDs.Contains(randomBadgeLockedRewardID))
			{
				_usedItemPartIDs.Add(randomBadgeLockedRewardID);
			}
			return CreateItemNode(placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, eNodeInitialDirection.FIXED_DOWN, false);
		}
		return null;
	}

	private Node CreateWildMortyNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		int balancedMortyLevel = MortyDefs.GetBalancedMortyLevel(eBalanceType.WILD_MORTY);
		if (string.IsNullOrEmpty(placeholder.data.id))
		{
			if (!string.IsNullOrEmpty(_availableRareMortyID))
			{
				placeholder.data.id = _availableRareMortyID;
				_availableRareMortyID = null;
			}
			else
			{
				_lastAvailableMortyIDIndex++;
				_lastAvailableMortyIDIndex %= _availableMortyIDs.Count;
				int index = UnityEngine.Random.Range(0, _availableMortyIDs[_lastAvailableMortyIDIndex].Count);
				placeholder.data.id = _availableMortyIDs[_lastAvailableMortyIDIndex][index];
			}
		}
		if (MortyDefs.IsIDActive(placeholder.data.id))
		{
			MortyData newMortyData = MortyDefs.GetNewMortyData(placeholder.data.id, balancedMortyLevel, eIVSetUp.RANDOM, eEVSetUp.NONE);
			return CreateMortyNode(newMortyData, true, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
		}
		return null;
	}

	private Node CreateNPCNodeFromPlaceholders(PlaceholderNode placeholder)
	{
		if (string.IsNullOrEmpty(placeholder.data.id) && _availableNPCIDs.Count > 0)
		{
			string text = _availableNPCIDs.PickRandom();
			_availableNPCIDs.Remove(text);
			placeholder.data.id = text;
		}
		if (NPCDefs.IsIDActive(placeholder.data.id))
		{
			NPCData newNPCData = NPCDefs.GetNewNPCData(placeholder.data.id);
			return CreateNPCNode(newNPCData, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
		}
		return null;
	}

	private Node CreateTrainerNodeFromPlaceholders(PlaceholderNode placeholder)
	{
		TrainerData trainerData = null;
		if (string.IsNullOrEmpty(placeholder.data.id))
		{
			trainerData = TrainerDefs.GetNewRandomTrainerData(_usedTrainerIDs);
			_usedTrainerIDs.Add(trainerData.id);
		}
		if (trainerData == null)
		{
			if (!TrainerDefs.IsIDActive(placeholder.data.id))
			{
				return null;
			}
			trainerData = TrainerDefs.GetNewTrainerData(placeholder.data.id);
		}
		return CreateTrainerNode(trainerData, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	private Node CreatePortalNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		return CreatePortalNode(placeholder.name, placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	private Node CreateSignPostNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		if (string.IsNullOrEmpty(placeholder.data.id))
		{
			_lastAvailableSignPostIDIndex++;
			_lastAvailableSignPostIDIndex %= _availableSignPostIDs.Count;
			placeholder.data.id = _availableSignPostIDs[_lastAvailableSignPostIDIndex];
		}
		if (!SignPostDefs.IsIDActive(placeholder.data.id))
		{
			return null;
		}
		return CreateNode(placeholder.name, placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	protected virtual Node CreateGymNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		return CreateGymNode(placeholder.data, null, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	private Node CreateGachaOrCraftMachineNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		if (_alternateGachaMachine)
		{
			_alternateGachaMachine = false;
			return CreateCraftTableNode(placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
		}
		_alternateGachaMachine = true;
		return CreateGachaMachineNode(placeholder.data, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
	}

	private Node CreateQuestGiverNodeFromPlaceholder(PlaceholderNode placeholder)
	{
		if (!QuestDefs.IsIDActive(placeholder.data.id))
		{
			return null;
		}
		if (_availableQuestIDs.Contains(placeholder.data.id))
		{
			QuestGiverData questGiverData = new QuestGiverData(placeholder.data.id);
			return CreateQuestGiverNode(questGiverData, placeholder.transform.parent, placeholder.transform.localPosition, placeholder.initialDirection, placeholder.applyRotation);
		}
		return null;
	}

	private void RemoveNodesFromSegmentPlaceholders()
	{
		for (int i = 0; i < _segments.Count; i++)
		{
			PlaceholderNode[] allNodePlaceholders = _segments[i].GetAllNodePlaceholders();
			for (int j = 0; j < allNodePlaceholders.Length; j++)
			{
				UnityEngine.Object.Destroy(allNodePlaceholders[j].gameObject);
			}
		}
	}

	public void RefreshIntoCache()
	{
		_cameraFollow.SetAudioListenerEnabled(false);
	}

	public virtual void RefreshFromCache()
	{
		RefreshPlayerMorties();
		RefreshPlayerAsset();
		_cameraFollow.SetAudioListenerEnabled(true);
	}

	public virtual void RefreshPlayerMorties()
	{
		if (_player != null)
		{
			_player.SetUpMorties(_nodeResourcePool.Get("Morty"));
		}
	}

	public virtual void RefreshPlayerAsset()
	{
		if (_player != null)
		{
			_player.RefreshAsset();
		}
	}

	public virtual void RefreshGymLeaderDefeated()
	{
		if (!_gymNode.IsPortalOpen)
		{
			_gymNode.OpenPortal(_nodeResourcePool.Get("Portal"), (!_worldID.Equals("Tutorial")) ? "SegmentedWorldPortal" : "TutorialWorld");
		}
	}

	public void SaveWorld()
	{
		if (WorldDefs.IsSegmented(_worldID))
		{
			StartCoroutine(SaveWorldCo());
		}
	}

	public IEnumerator SaveWorldCo()
	{
		yield return new WaitForEndOfFrame();
		WorldDefs.SetSavedSegmentedWorldSegments(_worldID, _nodeSetID, _entranceID, _segments, _gymLeader.trainerData);
		WorldDefs.SetSavedSegmentedWorldPlayer(_player.transform.localPosition, _player.GetDirection(), _player.GetMortyPositions(), _player.GetMortyDirections());
		App.saveData.Save(eSaveDataType.SEGMENTED_WORLD);
	}

	public void ShowWorld(bool value)
	{
		if (_container != null)
		{
			_container.SetActive(value);
		}
	}

	public virtual void DisplayDialogue(string dialogueID)
	{
		if (this.DisplayDialogueEvent != null)
		{
			this.DisplayDialogueEvent(GetDialogueData(dialogueID));
		}
	}

	public virtual DialogueData GetDialogueData(string id)
	{
		return null;
	}

	private void PlayerInteractAvailableHandler(AdvancedNode node)
	{
		if (this.PlayerInteractAvailableEvent != null)
		{
			this.PlayerInteractAvailableEvent();
		}
	}

	private void PlayerInteractUnavailableHandler(AdvancedNode node)
	{
		if (this.PlayerInteractUnavailableEvent != null)
		{
			this.PlayerInteractUnavailableEvent();
		}
	}

	protected virtual void PlayerInteractStartHandler(AdvancedNode node, bool isTrigger)
	{
		if (!_player.isForcedWalking && this.PlayerInteractStartEvent != null)
		{
			this.PlayerInteractStartEvent(_player, node, isTrigger);
		}
	}

	private void PlayerInteractEndHandler(AdvancedNode node)
	{
		if (this.PlayerInteractEndEvent != null)
		{
			this.PlayerInteractEndEvent(_player, node);
		}
	}

	public void PlayerInteractSuccess(AdvancedNode node)
	{
		_player.SuccessfulNodeInteraction(node.type);
	}
}
