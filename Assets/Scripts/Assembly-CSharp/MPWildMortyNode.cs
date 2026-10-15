using System.Collections.Generic;
using UnityEngine;

public class MPWildMortyNode : MPOpponentNode
{
	public const float moveCountdownMin = 0.5f;

	public const float moveCountdownMax = 3f;

	public const int boundsX = 1;

	public const int boundsZ = 1;

	private Coord _startingCoord;

	private eDirection _lastDirection;

	private float _moveCountdown;

	public override void SetUp()
	{
		base.SetUp();
		_startingCoord = moveModule.coord;
		_moveCountdown = 0f;
	}

	public override void SetUpWithData(MPOpponentData opponentData, eDirection direction)
	{
		base.SetUpWithData(opponentData, direction);
	}

	private void Update()
	{
		_moveCountdown -= Time.deltaTime;
		if (_moveCountdown <= 0f && base.data.state == eMPOpponentState.WORLD)
		{
			if (Random.Range(0, 3) == 0)
			{
				SetDirectionRandom(_lastDirection);
			}
			else
			{
				MoveRandom();
			}
			_moveCountdown = Random.Range(0.5f, 3f);
		}
	}

	private void SetDirectionRandom(eDirection excluding = eDirection.NONE)
	{
		_lastDirection = HelperMethods.GetRandomDirection(excluding);
		moveModule.SetDirection(_lastDirection);
	}

	private void MoveRandom()
	{
		List<eDirection> directionsWithinBounds = GetDirectionsWithinBounds();
		while (directionsWithinBounds.Count > 0)
		{
			eDirection eDirection2 = directionsWithinBounds.PickRandom();
			if (!MPNodeUtils.IsCollidingInDirection(base.transform.localPosition, eDirection2, moveModule.collisionIgnoreLayers))
			{
				moveModule.MoveToNextCoord(eDirection2);
				_lastDirection = eDirection2;
				break;
			}
			directionsWithinBounds.Remove(eDirection2);
		}
	}

	private List<eDirection> GetDirectionsWithinBounds()
	{
		List<eDirection> list = new List<eDirection>();
		Coord coord = moveModule.coord - _startingCoord;
		if (coord.x < 1)
		{
			list.Add(eDirection.RIGHT);
		}
		if (coord.x > -1)
		{
			list.Add(eDirection.LEFT);
		}
		if (coord.z < 1)
		{
			list.Add(eDirection.UP);
		}
		if (coord.z > -1)
		{
			list.Add(eDirection.DOWN);
		}
		return list;
	}
}
