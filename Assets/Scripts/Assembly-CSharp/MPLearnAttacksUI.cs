using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPLearnAttacksUI : UIBase
{
	public delegate void LearnAttacksDelegate(Dictionary<string, SortedList<int, MPAttackData>> newAttacks);

	private BattleBackground _background;

	private AttackLearnNewBox _newAttackBox;

	private Transform _attackLearnContainer;

	private BattleMorty _battleMorty;

	private MenuDialogue _dialogue;

	private BattleAttackMenu _attackMenu;

	private MPAttacksToLearnData _attacksToLearnData;

	private int _atlMortyIndex;

	private int _atlAttackIndex;

	private string _currentMortyNetId;

	private string _currentMortyName;

	private Dictionary<string, SortedList<int, MPAttackData>> _newAttacks;

	public event LearnAttacksDelegate FinishedEvent;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background").GetComponent<BattleBackground>();
		_attackLearnContainer = base.transform.FindChild("AttackLearnContainer");
		_battleMorty = base.transform.FindChild("PlayerMorty").GetComponent<BattleMorty>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_attackMenu = base.transform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
	}

	public void SetUp(string worldId, MPAttacksToLearnData attacksToLearnData)
	{
		_attacksToLearnData = attacksToLearnData;
		_atlMortyIndex = 0;
		_newAttacks = new Dictionary<string, SortedList<int, MPAttackData>>();
		_background.SetBackground(MPWorldDefs.GetResourceId(worldId));
		_battleMorty.SetUp(eBattleOpponentSide.LEFT);
		_attackMenu.SetPositionOut();
		_dialogue.SetText(string.Empty);
		StartCoroutine(SetUpMorty(_atlMortyIndex));
	}

	protected override void AddEvents()
	{
		_attackMenu.BtnPressedEvent += AttackMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_attackMenu.BtnPressedEvent -= AttackMenuBtnPressedHandler;
	}

	public IEnumerator SetUpMorty(int index)
	{
		_atlMortyIndex = index;
		_currentMortyNetId = _attacksToLearnData.morties[_atlMortyIndex].Key;
		MPOpponentMortyData mortyData = App.net.player.data.GetMortyData(_currentMortyNetId);
		_currentMortyName = MPMortyDefs.Name(mortyData.id);
		_newAttacks.Add(_currentMortyNetId, new SortedList<int, MPAttackData>(mortyData.attacks));
		yield return StartCoroutine(_battleMorty.SetImage(MPMortyDefs.AssetID(mortyData.id)));
		StartCoroutine(_battleMorty.TweenInCo());
		yield return StartCoroutine(SetUpNextAttack(0));
	}

	public IEnumerator SetUpNextAttack(int index)
	{
		_atlAttackIndex = index;
		string attackId = _attacksToLearnData.morties[_atlMortyIndex].Value[_atlAttackIndex];
		eAttackCatagory attackCatagory = MPAttackDefs.GetAttackCatagory(attackId);
		RectTransform attackBoxTrans = UI.Instantiate(string.Format("UI/Menu/AttackLearnNewBox/AttackLearnNewBox{0}", attackCatagory), _attackLearnContainer);
		_newAttackBox = attackBoxTrans.GetComponent<AttackLearnNewBox>();
		_newAttackBox.SetUp(new AttackDisplayInfo(attackId), 200);
		_newAttackBox.SetPositionOut();
		yield return new WaitForEndOfFrame();
		StartCoroutine(_newAttackBox.TweenIn());
		if (_newAttacks[_currentMortyNetId].Count < 4)
		{
			_newAttacks[_currentMortyNetId].AddNext(new MPAttackData
			{
				id = attackId,
				PP = MPAttackDefs.PP(attackId)
			});
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_LEARNED, _currentMortyName, MPAttackDefs.Name(attackId)), 2f, true));
			StartCoroutine(FinishedLearningAttack());
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_1, _currentMortyName, MPAttackDefs.Name(attackId)), 2f, true));
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_WANTS_TO_LEARN_2, 2f, true));
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_3, MPAttackDefs.Name(attackId))));
			_attackMenu.SetInteractable(true);
			StartCoroutine(ShowAttackMenu(_newAttacks[_currentMortyNetId]));
		}
	}

	private IEnumerator ConfirmLearnAttack(int index)
	{
		string attackId = _attacksToLearnData.morties[_atlMortyIndex].Value[_atlAttackIndex];
		if (index != 4)
		{
			string forgetAttackID = _newAttacks[_currentMortyNetId][index].id;
			string forgetAttackName = MPAttackDefs.Name(forgetAttackID);
			_newAttacks[_currentMortyNetId].Remove(index);
			RefreshAttackMenu(_newAttacks[_currentMortyNetId]);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FORGOT_ATTACK, _currentMortyName, forgetAttackName), 2f, true));
			_newAttacks[_currentMortyNetId][index] = new MPAttackData
			{
				id = attackId,
				PP = MPAttackDefs.PP(attackId)
			};
			RefreshAttackMenu(_newAttacks[_currentMortyNetId]);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_LEARNED, _currentMortyName, MPAttackDefs.Name(attackId)), 2f, true));
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_DID_NOT_LEARN, _currentMortyName, MPAttackDefs.Name(attackId)), 2f, true));
		}
		StartCoroutine(FinishedLearningAttack());
	}

	private IEnumerator FinishedLearningAttack()
	{
		StartCoroutine(HideAttackMenu());
		yield return StartCoroutine(_newAttackBox.TweenOut(true));
		if (_attacksToLearnData.morties[_atlMortyIndex].Value.Count - 1 > _atlAttackIndex)
		{
			StartCoroutine(SetUpNextAttack(_atlAttackIndex + 1));
		}
		else if (_attacksToLearnData.morties.Count - 1 > _atlMortyIndex)
		{
			yield return StartCoroutine(_battleMorty.TweenOutCo());
			StartCoroutine(SetUpMorty(_atlMortyIndex + 1));
		}
		else if (this.FinishedEvent != null)
		{
			this.FinishedEvent(_newAttacks);
		}
	}

	private void SetMenusInteractable(bool isInteractable)
	{
		_attackMenu.SetInteractable(isInteractable);
	}

	protected virtual IEnumerator ShowAttackMenu(SortedList<int, MPAttackData> attacks)
	{
		RefreshAttackMenu(attacks);
		yield return StartCoroutine(_attackMenu.TweenIn());
	}

	private IEnumerator HideAttackMenu()
	{
		yield return StartCoroutine(_attackMenu.TweenOut());
	}

	private void RefreshAttackMenu(SortedList<int, MPAttackData> attacks)
	{
		for (int i = 0; i < 4; i++)
		{
			if (attacks.ContainsKey(i))
			{
				_attackMenu.SetBtnEnabled(i, true);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(attacks[i].id, attacks[i].PP));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	private void AttackMenuBtnPressedHandler(int index)
	{
		_attackMenu.SetInteractable(false);
		StartCoroutine(ConfirmLearnAttack(index));
	}
}
