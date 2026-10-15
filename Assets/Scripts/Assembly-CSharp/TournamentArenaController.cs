using System.Collections;
using UnityEngine;

public class TournamentArenaController : MonoBehaviour
{
	public delegate void WorldControllerDelegate();

	public delegate void WorldControllerNodeStartDelegate(PlayerNode playerNode, AdvancedNode node, bool isTrigger);

	public delegate void WorldControllerNodeDelegate(PlayerNode playerNode, AdvancedNode node);

	private const float _kCameraOffsetY = 20f;

	private const float _kCameraOffsetZ = -15f;

	private const float _kCameraRotationX = 52f;

	private Object _mortyResource;

	private Transform _container;

	private WorldCamera _cameraFollow;

	private StoryCamera _storyCamera;

	protected PlayerNode _player;

	protected TournamentTrainerNode _trainer;

	protected NodeData _antagonistData;

	private ItemCrateNode _stashItemCrate;

	public event WorldControllerDelegate PlayerInteractAvailableEvent;

	public event WorldControllerDelegate PlayerInteractUnavailableEvent;

	public event WorldControllerNodeStartDelegate PlayerInteractStartEvent;

	public event WorldControllerNodeDelegate PlayerInteractEndEvent;

	private void Awake()
	{
		_container = base.transform.FindChild("Container");
		_container.transform.SetParent(base.transform);
	}

	public virtual void SetUp(string trainerID)
	{
		SetUpCamera();
		SetUpScreen(trainerID);
		SetUpRing(trainerID);
		SetUpStage(trainerID);
		SetUpPlayer();
		SetUpTrainer(trainerID);
		SetUpStashedItemCrate();
		_cameraFollow.SetUp(_player.transform, new Vector3(0f, 20f, -15f), new Vector3(52f, 0f, 0f));
		_cameraFollow.SetLocked(new Vector3(10.5f, -999f, -10.5f), new Vector3(10.5f, 999f, 999f));
	}

	private void OnDestroy()
	{
		Object.Destroy(_cameraFollow);
		RemovePlayer();
	}

	private void SetUpCamera()
	{
		_cameraFollow = Camera.main.gameObject.AddComponent<WorldCamera>();
		_storyCamera = Camera.main.gameObject.AddComponent<StoryCamera>();
	}

	public void FollowCameraEnabled(bool isEnable)
	{
		_cameraFollow.SetFollowEnabled(isEnable);
	}

	public IEnumerator MoveStoryCameraTo(Vector3 position, float duration)
	{
		FollowCameraEnabled(false);
		yield return StartCoroutine(_storyCamera.MoveTo(position, duration));
	}

	private void SetUpScreen(string trainerID)
	{
		string text = TournamentDefs.TrainerScreenAsset(trainerID);
		if (!string.IsNullOrEmpty(text))
		{
			Transform transform = _container.FindChild("Screen");
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			Material material = (Material)Resources.Load(string.Format("Tournament/Screen/{0}", text));
			component.material = material;
		}
	}

	private void SetUpRing(string trainerID)
	{
		string text = TournamentDefs.TrainerRingAsset(trainerID);
		if (!string.IsNullOrEmpty(text))
		{
			Transform transform = _container.FindChild("Meshs/ringMain");
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			Material material = (Material)Resources.Load(string.Format("Tournament/RingAssets/{0}", text));
			component.material = material;
		}
	}

	private void SetUpStage(string trainerID)
	{
		string value = TournamentDefs.TrainerStageAsset(trainerID);
		if (!string.IsNullOrEmpty(value))
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(string.Format("Tournament/StageAssets/{0}", TournamentDefs.TrainerStageAsset(trainerID))));
			gameObject.transform.parent = _container;
			gameObject.transform.localPosition = new Vector3(10.5f, 0.85f, 24.5f);
		}
	}

	private void SetUpPlayer()
	{
		_player = _container.FindChild("Player").GetComponent<PlayerNode>();
		_player.SetUp(new NodeData(PlayerDefs.AssetID), eNodeInitialDirection.UP, false);
		_mortyResource = Resources.Load(string.Format("World/Nodes/{0}/", "Default") + "Morty");
		_player.SetUpMorties(_mortyResource);
		_player.InteractAvailableEvent += PlayerInteractAvailableHandler;
		_player.InteractUnavailableEvent += PlayerInteractUnavailableHandler;
		_player.InteractStartEvent += PlayerInteractStartHandler;
		_player.InteractEndEvent += PlayerInteractEndHandler;
	}

	private void RemovePlayer()
	{
		_player.InteractAvailableEvent -= PlayerInteractAvailableHandler;
		_player.InteractUnavailableEvent -= PlayerInteractUnavailableHandler;
		_player.InteractStartEvent -= PlayerInteractStartHandler;
		_player.InteractEndEvent -= PlayerInteractEndHandler;
	}

	public IEnumerator ForceWalkPlayer(eDirection direction, int distance)
	{
		yield return StartCoroutine(_player.ForceMoveInDirectionFor(direction, distance));
	}

	public IEnumerator ForceWalkPlayerOut(bool moveLeft)
	{
		if (moveLeft)
		{
			yield return StartCoroutine(_player.ForceMoveInDirectionFor(eDirection.LEFT, 1));
		}
		yield return StartCoroutine(_player.ForceMoveInDirectionFor(eDirection.DOWN, 8));
	}

	private void SetUpTrainer(string trainerID)
	{
		TournamentTrainerData trainerData = TournamentDefs.GetTrainerData(trainerID);
		_trainer = _container.FindChild("TournamentTrainer").GetComponent<TournamentTrainerNode>();
		_trainer.SetUp(trainerData, eNodeInitialDirection.DOWN, false);
		_trainer.AddMorties(Resources.Load(string.Format("World/Nodes/{0}/", "Default") + "Morty"));
	}

	public void SetUpStashedItemCrate()
	{
		_stashItemCrate = _container.FindChild("StashItemCrate").GetComponent<ItemCrateNode>();
		if (!TournamentDefs.IsItemStashActive())
		{
			Object.Destroy(_stashItemCrate.gameObject);
		}
	}

	public void DPadChanged(eDirection direction)
	{
		_player.DPadChanged(direction);
	}

	public void FaceBtnAPressed()
	{
		_player.FaceBtnAPressed();
	}

	public void RefreshIntoCache()
	{
		_cameraFollow.SetAudioListenerEnabled(false);
	}

	public virtual void RefreshFromCache()
	{
		RefreshPlayerMorties();
		_cameraFollow.SetAudioListenerEnabled(true);
	}

	public virtual void RefreshPlayerMorties()
	{
		if (_player != null)
		{
			_player.SetUpMorties(_mortyResource);
		}
	}

	public void ShowWorld(bool value)
	{
		if (_container != null)
		{
			_container.gameObject.SetActive(value);
		}
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
		if (this.PlayerInteractStartEvent != null)
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
