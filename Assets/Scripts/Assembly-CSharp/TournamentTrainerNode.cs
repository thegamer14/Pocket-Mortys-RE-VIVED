using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TournamentTrainerNode : AdvancedNode
{
	private Transform _battleIcon;

	private List<MortyNode> _followingMorties;

	public TournamentTrainerData trainerData
	{
		get
		{
			return (TournamentTrainerData)_data;
		}
		private set
		{
			_data = value;
		}
	}

	public Vector3 setUpPosition { get; private set; }

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
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
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = TournamentDefs.TrainerAssetID(data.id);
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
			mortyNode.SetDirection(eDirection.DOWN);
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
			return new Coord(0, 2);
		case 1:
			return new Coord(-1, 1);
		case 2:
			return new Coord(1, 1);
		case 3:
			return new Coord(-2, 0);
		case 4:
			return new Coord(2, 0);
		}
	}

	public void RefreshStatus()
	{
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			_followingMorties[i].RefreshStatus();
		}
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
}
