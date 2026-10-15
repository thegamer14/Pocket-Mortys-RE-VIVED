using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleOpponentAI : BattleOpponent
{
	private const float _kSwitchAttemptNurfIntelligence = 0.8f;

	private const int _kSwitchAttemptMovesRequired = 3;

	private MortyData _enemyMortyData;

	private Dictionary<MortyData, int> _enemyMoveCount;

	public eTrainerType trainerType;

	private float _intelligence;

	private int _tutorialMoveCount;

	private int _movesSinceSwitching;

	private bool _firstAttackBonusApplied;

	public BattleOpponentAI(eBattleOpponentType type, eBattleOpponentSide side, List<MortyData> mortyDatas, Dictionary<string, int> items)
		: base(type, side, null, null, mortyDatas, items)
	{
		TrainerSetUp(null, null, eTrainerType.NONE);
	}

	public BattleOpponentAI(eBattleOpponentType type, eBattleOpponentSide side, List<MortyData> mortyDatas, Dictionary<string, int> items, TrainerData trainerData)
		: base(type, side, null, null, mortyDatas, items)
	{
		TrainerSetUp(TrainerDefs.AssetID(trainerData.id), TrainerDefs.Name(trainerData.id), trainerData.type);
	}

	public BattleOpponentAI(eBattleOpponentType type, eBattleOpponentSide side, List<MortyData> mortyDatas, Dictionary<string, int> items, TournamentTrainerData trainerData)
		: base(type, side, null, null, mortyDatas, items)
	{
		TrainerSetUp(TournamentDefs.TrainerAssetID(trainerData.id), TournamentDefs.TrainerName(trainerData.id), eTrainerType.NONE, TournamentDefs.TrainerIntelligence(trainerData.id));
	}

	private void TrainerSetUp(string trainerAssetID, string trainerName, eTrainerType trainerType, float intelligence = -1f)
	{
		_enemyMoveCount = new Dictionary<MortyData, int>();
		_tutorialMoveCount = 0;
		_movesSinceSwitching = 3;
		assetID = trainerAssetID;
		name = trainerName;
		this.trainerType = trainerType;
		SetIntelligence(intelligence);
	}

	private void SetIntelligence(float forceValue = -1f)
	{
		if (forceValue > -1f)
		{
			_intelligence = forceValue;
		}
		else if (type == eBattleOpponentType.AI_TRAINER)
		{
			switch (trainerType)
			{
			default:
				_intelligence = 0.5f;
				break;
			case eTrainerType.GYM_LEADER:
				_intelligence = 0.75f;
				break;
			case eTrainerType.COUNCIL_MEMBER:
				_intelligence = 0.9f;
				break;
			}
		}
		else
		{
			_intelligence = 0.25f;
		}
	}

	public override void SetEnemyMorty(MortyData mortyData)
	{
		_enemyMortyData = mortyData;
		if (!_enemyMoveCount.ContainsKey(_enemyMortyData))
		{
			_enemyMoveCount.Add(_enemyMortyData, 0);
		}
	}

	public void RefreshMove()
	{
		_movesSinceSwitching++;
		move = null;
		if (SharedStatInfo.battleInfo.isTutorial)
		{
			RefreshTutorialMove();
		}
		else
		{
			if (move == null)
			{
				RefreshItemMove();
			}
			if (move == null && _intelligence > 0.5f)
			{
				RefreshSwitchMove();
			}
			if (move == null)
			{
				RefreshAttackMove(true);
			}
		}
		_enemyMoveCount[_enemyMortyData]++;
	}

	private void RefreshTutorialMove()
	{
		for (int i = 0; i < currentMortyData.attackIDs.Count; i++)
		{
			if (string.IsNullOrEmpty(currentMortyData.attackIDs[i]))
			{
				continue;
			}
			bool flag = false;
			AttackData data = AttackDefs.GetData(currentMortyData.attackIDs[i]);
			for (int j = 0; j < data.effectDatas.Length; j++)
			{
				if (data.effectDatas[j].type == eAttackEffectType.Hit)
				{
					flag = true;
				}
			}
			if ((_tutorialMoveCount == 0 && flag) || (_tutorialMoveCount > 0 && !flag))
			{
				move = new BattleAttackMove(data);
				_tutorialMoveCount++;
				return;
			}
		}
		move = new BattleAttackMove(AttackDefs.GetData("AttackStruggle"));
	}

	public void RefreshAttackMove(bool force = false)
	{
		List<KeyValuePair<string, float>> list = new List<KeyValuePair<string, float>>();
		for (int i = 0; i < currentMortyData.attackIDs.Count; i++)
		{
			if (!string.IsNullOrEmpty(currentMortyData.attackIDs[i]))
			{
				AttackData data = AttackDefs.GetData(currentMortyData.attackIDs[i]);
				float num = 0f;
				for (int j = 0; j < data.effectDatas.Length; j++)
				{
					num += GetAttackEffectValue(data.effectDatas[j], MortyDefs.GetElementTypeModifier(data.element, MortyDefs.ElementType(_enemyMortyData.id)));
				}
				num *= Random.Range(_intelligence, 1f);
				list.Add(new KeyValuePair<string, float>(currentMortyData.attackIDs[i], num));
				_firstAttackBonusApplied = false;
			}
		}
		if (list.Count > 0)
		{
			string key = list.OrderByDescending((KeyValuePair<string, float> e) => e.Value).First().Key;
			move = new BattleAttackMove(AttackDefs.GetData(key));
		}
		if (force && move == null)
		{
			move = new BattleAttackMove(AttackDefs.GetData(currentMortyData.attackIDs.PickRandom()));
		}
	}

	private float GetAttackEffectValue(AttackEffectData effectData, float elementModifier)
	{
		float num = 0f;
		bool applyBonus = false;
		switch (effectData.type)
		{
		case eAttackEffectType.Hit:
			num += GetAttackEffectValueHit(effectData, elementModifier);
			break;
		case eAttackEffectType.Stat:
			num += GetAttackEffectValueStat(effectData, elementModifier, out applyBonus);
			break;
		case eAttackEffectType.Poison:
			num += GetAttackEffectValuePoison(effectData, elementModifier, out applyBonus);
			break;
		case eAttackEffectType.Paralyse:
			num += GetAttackEffectValueParalyse(effectData, elementModifier, out applyBonus);
			break;
		case eAttackEffectType.Absorb:
			num += GetAttackEffectValueAbsorb(effectData, elementModifier);
			break;
		}
		if (!_firstAttackBonusApplied && applyBonus && _enemyMoveCount[_enemyMortyData] == 0)
		{
			num += Random.Range(0.2f, 0.4f);
			_firstAttackBonusApplied = true;
		}
		if (effectData.accuracy >= 0f)
		{
			num *= effectData.accuracy;
		}
		return num;
	}

	private float GetAttackEffectValueHit(AttackEffectData effectData, float elementModifier)
	{
		float num = 0f;
		if (!effectData.toSelf)
		{
			num += effectData.power * 0.01f * elementModifier;
		}
		return num;
	}

	private float GetAttackEffectValueStat(AttackEffectData effectData, float elementModifier, out bool applyBonus)
	{
		float num = 0f;
		applyBonus = false;
		if (currentMortyData.GetHPPercentage() > 0.25f && _enemyMortyData.GetHPPercentage() > 0.25f)
		{
			if (!effectData.toSelf)
			{
				if (effectData.power > 0f)
				{
					float num2 = Mathf.Max(0f, _enemyMortyData.GetStatModifierValue(effectData.stat));
					num -= Mathf.Sqrt(num2 + 1f) - 1f;
				}
				else if (effectData.power < 0f)
				{
					applyBonus = true;
					if (_enemyMortyData.GetStatModifierValue(effectData.stat) >= 0f)
					{
						num += 0.25f + (Mathf.Sqrt(Mathf.Abs(effectData.power)) - 1f);
					}
				}
			}
			else if (effectData.power > 0f)
			{
				applyBonus = true;
				if (currentMortyData.GetStatModifierValue(effectData.stat) <= 0f)
				{
					num += 0.25f + (Mathf.Sqrt(Mathf.Abs(effectData.power)) - 1f);
				}
			}
			else if (effectData.power < 0f)
			{
				float f = Mathf.Min(0f, currentMortyData.GetStatModifierValue(effectData.stat));
				num -= Mathf.Sqrt(Mathf.Abs(f) + 1f) - 1f;
			}
		}
		return num;
	}

	private float GetAttackEffectValuePoison(AttackEffectData effectData, float elementModifier, out bool applyBonus)
	{
		float num = 0f;
		applyBonus = false;
		if (!effectData.toSelf)
		{
			applyBonus = true;
			if (!_enemyMortyData.isPoisoned && _enemyMortyData.GetHPPercentage() > 0.5f)
			{
				num = _enemyMortyData.GetHPPercentage() * 0.4f;
			}
		}
		else if (!currentMortyData.isPoisoned)
		{
			num -= 0.4f;
		}
		return num;
	}

	private float GetAttackEffectValueParalyse(AttackEffectData effectData, float elementModifier, out bool applyBonus)
	{
		float num = 0f;
		applyBonus = false;
		if (!effectData.toSelf)
		{
			applyBonus = true;
			if (!_enemyMortyData.isParalysed && _enemyMortyData.GetHPPercentage() > 0.5f)
			{
				num += _enemyMortyData.GetHPPercentage() * 0.4f;
			}
		}
		else if (!currentMortyData.isParalysed)
		{
			num -= 0.4f;
		}
		return num;
	}

	private float GetAttackEffectValueAbsorb(AttackEffectData effectData, float elementModifier)
	{
		float num = GetAttackEffectValueHit(effectData, elementModifier);
		if (!effectData.toSelf && _enemyMortyData.GetHPPercentage() > 0.1f && currentMortyData.GetHPPercentage() < 0.9f)
		{
			num += num * effectData.percent;
		}
		return num;
	}

	public void RefreshItemMove(bool force = false)
	{
		if (items == null)
		{
			return;
		}
		if (items.ContainsKey("ItemHalzingerPure"))
		{
			RefreshItemReviveMove("ItemHalzingerPure");
		}
		else if (items.ContainsKey("ItemHalzinger"))
		{
			RefreshItemReviveMove("ItemHalzinger");
		}
		else if (!RefreshItemHPMove())
		{
			if (items.ContainsKey("ItemPoisonCure"))
			{
				RefreshItemCureMove("ItemPoisonCure");
			}
			else if (items.ContainsKey("ItemParalysisCure"))
			{
				RefreshItemCureMove("ItemParalysisCure");
			}
		}
		if (force && move == null && items.Count > 0)
		{
			string itemID = items.Keys.First();
			move = new BattleItemMove(itemID, currentMortyData);
		}
	}

	public void RefreshItemReviveMove(string id)
	{
		MortyData mortyData = mortyDatas.Where((MortyData e) => e.isFainted).FirstOrDefault();
		if (mortyData != null)
		{
			move = new BattleItemMove(id, mortyData);
		}
	}

	public bool RefreshItemHPMove()
	{
		List<MortyData> orderedMortyDatas = mortyDatas.OrderBy((MortyData e) => e.hp).ToList();
		for (int i = 0; i < orderedMortyDatas.Count; i++)
		{
			if (orderedMortyDatas[i].isFainted)
			{
				continue;
			}
			if (orderedMortyDatas[i].GetHPPercentage() > 0.3f)
			{
				return false;
			}
			IEnumerable<string> source = items.Keys.Where((string e) => ItemDefs.EffectType(e) == "RestoreHP" && ItemDefs.EffectValue(e) > 0f);
			string text = source.OrderBy((string e) => Mathf.Abs(1f - ItemDefs.EffectValue(e) / (float)(orderedMortyDatas[i].hpStat - orderedMortyDatas[i].hp))).FirstOrDefault();
			if (text == null)
			{
				continue;
			}
			float num = ItemDefs.EffectValue(text) / (float)(orderedMortyDatas[i].hpStat - orderedMortyDatas[i].hp);
			if (num > 3f)
			{
				if (mortyDatas.Where((MortyData e) => !e.isFainted).Count() > 1)
				{
					return false;
				}
			}
			else if (num < 0.75f)
			{
				if (items.ContainsKey("ItemFullRecover") && orderedMortyDatas[i].isAilment)
				{
					move = new BattleItemMove("ItemFullRecover", orderedMortyDatas[i]);
					return true;
				}
				if (items.ContainsKey("ItemPureSerum"))
				{
					move = new BattleItemMove("ItemPureSerum", orderedMortyDatas[i]);
					return true;
				}
			}
			if (num > 0.25f)
			{
				move = new BattleItemMove(text, orderedMortyDatas[i]);
				return true;
			}
			return false;
		}
		return false;
	}

	public void RefreshItemCureMove(string id)
	{
		for (int i = 0; i < mortyDatas.Count; i++)
		{
			if (id == "ItemPoisonCure" && mortyDatas[i].isPoisoned && (float)mortyDatas[i].hp > 0.4f)
			{
				move = new BattleItemMove(id, mortyDatas[i]);
			}
			if (id == "ItemParalysisCure" && mortyDatas[i].isParalysed && (float)mortyDatas[i].hp > 0.4f)
			{
				move = new BattleItemMove(id, mortyDatas[i]);
			}
		}
	}

	public void RefreshSwitchMove(bool force = false)
	{
		float elementTypeModifier = MortyDefs.GetElementTypeModifier(MortyDefs.ElementType(currentMortyData.id), MortyDefs.ElementType(_enemyMortyData.id));
		IOrderedEnumerable<MortyData> source = mortyDatas.OrderByDescending((MortyData e) => MortyDefs.GetElementTypeModifier(MortyDefs.ElementType(e.id), MortyDefs.ElementType(_enemyMortyData.id)));
		MortyData mortyData = null;
		if (elementTypeModifier < 1f)
		{
			mortyData = source.Where((MortyData e) => !e.isFainted && MortyDefs.GetElementTypeModifier(MortyDefs.ElementType(e.id), MortyDefs.ElementType(_enemyMortyData.id)) >= 1f).FirstOrDefault();
		}
		else if (elementTypeModifier == 1f)
		{
			mortyData = source.Where((MortyData e) => !e.isFainted && MortyDefs.GetElementTypeModifier(MortyDefs.ElementType(e.id), MortyDefs.ElementType(_enemyMortyData.id)) > 1f).FirstOrDefault();
		}
		if (mortyData != null && (force || (_movesSinceSwitching > 3 && Random.value < _intelligence * 0.8f)))
		{
			move = new BattleSwitchMove(mortyData);
			_movesSinceSwitching = 0;
		}
		if (force && move == null)
		{
			MortyData mortyData2 = mortyDatas.Find((MortyData e) => !e.isFainted && e != currentMortyData);
			if (mortyData2 != null)
			{
				move = new BattleSwitchMove(mortyData2);
			}
		}
	}
}
