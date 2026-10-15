using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrainerNode : AdvancedNode
{
	private BoxCollider _interactionPointSight;

	private Transform _battleIcon;

	private List<MortyNode> _followingMorties;

	public TrainerData trainerData
	{
		get
		{
			return (TrainerData)_data;
		}
		private set
		{
			_data = value;
		}
	}

	public Vector3 setUpPosition { get; private set; }

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		_interactionPointSight = base.transform.FindChild("InteractionPointSight").GetComponent<BoxCollider>();
		_battleIcon = base.transform.FindChild("BattleIcon");
		base.SetUp(data, initialDirection, applyRotation);
		_followingMorties = new List<MortyNode>();
		if (Application.isPlaying)
		{
			_battleIcon.gameObject.SetActive(false);
			setUpPosition = base.transform.localPosition;
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			if (_followingMorties[i] != null)
			{
				Object.Destroy(_followingMorties[i].gameObject);
			}
		}
	}

	public override void SetDirection(eDirection value)
	{
		base.SetDirection(value);
		if (Application.isPlaying)
		{
			_interactionPointSight.transform.eulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(value) - 90f, 0f);
		}
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = TrainerDefs.AssetID(data.id);
		}
	}

	public void SetNewAssetID(string ID)
	{
		_assetID = ID;
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetUpSprites(_assetID);
			_moveAnimatorModule.SetDirection(GetDirection());
		}
	}

	public void AddMorties(Object resource)
	{
		for (int i = 0; i < trainerData.morties.Count; i++)
		{
			MortyNode mortyNode = AddMorty(resource, trainerData.morties[i], GetMortyPosition(i).ToVector3());
			if (trainerData.type == eTrainerType.GYM_LEADER)
			{
				mortyNode.SetDirection(eDirection.DOWN);
			}
			else
			{
				mortyNode.SetDirection(HelperMethods.GetRandomDirection(HelperMethods.GetOppositeDirection(GetDirection())));
			}
		}
	}

	private MortyNode AddMorty(Object resource, MortyData data, Vector3 position)
	{
		GameObject gameObject = Object.Instantiate(resource) as GameObject;
		MortyNode component = gameObject.GetComponent<MortyNode>();
		component.transform.parent = base.transform.parent;
		component.transform.position = base.transform.position + position;
		component.SetUp(data, eNodeInitialDirection.FIXED_DOWN, false);
		component.ActivateInteractionPoint(false);
		component.AddModuleAIWander(4f, 6f, true);
		_followingMorties.Add(component);
		return component;
	}

	private Coord GetMortyPosition(int index)
	{
		switch (index)
		{
		default:
			return Coord.zero;
		case 0:
			return Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(GetDirection()));
		case 1:
			return Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(GetDirection())) + Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(GetDirection()));
		case 2:
			return Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(GetDirection())) + Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(GetDirection(), false));
		case 3:
			return Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(GetDirection()));
		case 4:
			return Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(GetDirection(), false));
		}
	}

	public void RefreshStatus()
	{
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			_followingMorties[i].RefreshStatus();
		}
		ActivateSightInteractionPoint(trainerData.type != eTrainerType.GYM_LEADER && !trainerData.isDefeated);
	}

	public IEnumerator TweenAttackIcon()
	{
		App.sound.PlaySFX("Audio/SFX/TrainerAlert");
		_battleIcon.gameObject.SetActive(true);
		_battleIcon.transform.localScale = Vector3.zero;
		yield return _battleIcon.DOScale(1f, 0.2f).SetEase(Ease.Linear).WaitForCompletion();
		yield return _battleIcon.DOPunchScale(Vector3.one * 0.4f, 0.2f).WaitForCompletion();
		yield return new WaitForSeconds(1f);
		yield return _battleIcon.DOScale(0f, 0.4f).WaitForCompletion();
	}

	public void ActivateSightInteractionPoint(bool activate)
	{
		if (activate && !TestDefs.TRAINERS_DISABLE_SIGHT)
		{
			_interactionPointSight.gameObject.SetActive(true);
			int collidingDistanceInDirection = base.moveModule.GetCollidingDistanceInDirection(GetDirection(), 5);
			int num = ((collidingDistanceInDirection <= -1) ? 5 : (collidingDistanceInDirection + 1));
			_interactionPointSight.size = new Vector3(_interactionPointSight.size.x, _interactionPointSight.size.y, 0f - ((float)num - 1.1f));
			_interactionPointSight.center = new Vector3(_interactionPointSight.center.x, _interactionPointSight.center.y, 0f - (float)num / 2f);
		}
		else
		{
			_interactionPointSight.gameObject.SetActive(false);
		}
	}
}
