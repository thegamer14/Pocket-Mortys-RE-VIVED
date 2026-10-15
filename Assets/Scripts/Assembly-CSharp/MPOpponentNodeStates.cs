using DG.Tweening;
using UnityEngine;

public class MPOpponentNodeStates : MonoBehaviour
{
	private GameObject _stateBattleObj;

	private GameObject _stateOccupiedObj;

	private void Awake()
	{
		_stateBattleObj = base.transform.FindChild("Battle").gameObject;
		_stateOccupiedObj = base.transform.FindChild("Occupied").gameObject;
		Tween t = _stateOccupiedObj.transform.DORotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.LocalAxisAdd);
		t.SetLoops(-1, LoopType.Restart);
		t.SetEase(Ease.Linear);
	}

	public void SetState(eMPOpponentState state, bool isPlayer)
	{
		switch (state)
		{
		default:
			_stateBattleObj.SetActive(false);
			_stateOccupiedObj.SetActive(false);
			break;
		case eMPOpponentState.BATTLE:
			_stateBattleObj.SetActive(true);
			_stateOccupiedObj.SetActive(false);
			break;
		case eMPOpponentState.OCCUPIED:
			_stateBattleObj.SetActive(false);
			_stateOccupiedObj.SetActive(!isPlayer);
			break;
		}
	}
}
