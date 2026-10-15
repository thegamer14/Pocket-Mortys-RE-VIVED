using UnityEngine;

public class MPNodePlayerBroadcastModule : MonoBehaviour
{
	private const float _kForceSendInterval = 2f;

	private MPNodeMoveModule _moveModule;

	private MPNodePlayerInputModule _playerInputModule;

	private Coord _lastCoord;

	private eDirection _lastDirection;

	private float _forceSendTimer = 2f;

	private void Start()
	{
		_moveModule = GetComponent<MPNodeMoveModule>();
		_playerInputModule = GetComponent<MPNodePlayerInputModule>();
	}

	private void Update()
	{
		if (!(App.net.room.nodeSender == null) && (_moveModule.coord != _lastCoord || (!_playerInputModule.isChangingDirection && _moveModule.direction != _lastDirection) || _forceSendTimer <= 0f))
		{
			App.net.room.nodeSender.SendPosition(_moveModule.coord, _moveModule.direction);
			_lastCoord = _moveModule.coord;
			_lastDirection = _moveModule.direction;
			_forceSendTimer = 2f;
		}
	}
}
