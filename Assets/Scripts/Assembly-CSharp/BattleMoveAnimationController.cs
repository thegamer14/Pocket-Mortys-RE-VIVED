using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMoveAnimationController : MonoBehaviour
{
	private Dictionary<string, Object> _resources;

	private BattleMorty _rightMorty;

	private Transform _rightMortyBarTrans;

	private BattleMorty _leftMorty;

	private Transform _leftMortyBarTrans;

	public void SetUp(BattleMorty leftMorty, Transform leftMortyBarTrans, BattleMorty rightMorty, Transform rightMortyBarTrans)
	{
		_resources = new Dictionary<string, Object>();
		Object[] array = Resources.LoadAll<Object>("UI/Battle/MoveAnimations/");
		for (int i = 0; i < array.Length; i++)
		{
			_resources.Add(array[i].name, array[i]);
		}
		_leftMorty = leftMorty;
		_leftMortyBarTrans = leftMortyBarTrans;
		_rightMorty = rightMorty;
		_rightMortyBarTrans = rightMortyBarTrans;
	}

	public IEnumerator PlayAnimation(string id, eBattleOpponentSide side)
	{
		if (!string.IsNullOrEmpty(id))
		{
			BattleMorty performingMorty = ((side != eBattleOpponentSide.LEFT) ? _leftMorty : _rightMorty);
			BattleMorty receivingMorty = ((side != eBattleOpponentSide.LEFT) ? _rightMorty : _leftMorty);
			Transform receivingMortyBarTrans = ((side != eBattleOpponentSide.LEFT) ? _rightMortyBarTrans : _leftMortyBarTrans);
			string directionAppend = ((side != eBattleOpponentSide.LEFT) ? "Right" : "Left");
			RectTransform animationTrans = UI.Instantiate(_resources[id + directionAppend], base.transform);
			BattleMoveAnimation animation = animationTrans.GetComponent<BattleMoveAnimation>();
			animation.SetUp(performingMorty, receivingMorty, receivingMortyBarTrans);
			yield return StartCoroutine(animation.Play());
			animation.MoveMortyOutOfAnimation();
			Object.Destroy(animationTrans.gameObject);
		}
	}
}
