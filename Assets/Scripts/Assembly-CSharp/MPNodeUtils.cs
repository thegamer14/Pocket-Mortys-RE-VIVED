using UnityEngine;

public class MPNodeUtils
{
	private const float _kCollisionYPos = 10f;

	private const float _kCollisionYDistance = 20f;

	public const int KInteractionLayerMask = 256;

	public const int KSegmentLayerMask = 512;

	public const int KBlockersLayerMask = 1024;

	public const int KMPUserLayer = 11;

	public static bool IsCollidingInDirection(Vector3 position, eDirection colDirection, int ignoreLayers)
	{
		Vector3 direction = Coord.GetCoordFromDirection(colDirection).ToVector3();
		Vector3 origin = position + new Vector3(0f, 0.9f, 0f);
		if (Physics.Raycast(origin, direction, 1f, ignoreLayers))
		{
			return true;
		}
		Vector3 origin2 = new Vector3(origin.x + direction.x, 10f, origin.z + direction.z);
		if (!Physics.Raycast(origin2, Vector3.down, 20f, 512))
		{
			return true;
		}
		return false;
	}

	public static bool IsCollidingWithUser(Vector3 position, eDirection colDirection, int ignoreLayers)
	{
		Vector3 direction = Coord.GetCoordFromDirection(colDirection).ToVector3();
		Vector3 origin = position + new Vector3(0f, 0.9f, 0f);
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction, out hitInfo, 1f, ignoreLayers) && hitInfo.transform.gameObject.layer == 11)
		{
			return true;
		}
		return false;
	}

	public static MPNode GetCollidingWithInteractionPoint(Vector3 position, eDirection direction)
	{
		Vector3 direction2 = Coord.GetCoordFromDirection(direction).ToVector3();
		Vector3 origin = position + new Vector3(0f, 0.9f, 0f);
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction2, out hitInfo, 1f, 256) && !hitInfo.collider.isTrigger)
		{
			return hitInfo.transform.parent.gameObject.GetComponent<MPNode>();
		}
		return null;
	}

	public static float GetGroundCollisionValue(Vector3 position)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Vector3(position.x, 10f, position.z), Vector3.down, out hitInfo, 20f, 512))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	public static int GetCollidingDistanceInDirection(Vector3 position, eDirection colDirection, int distance, int ignoreLayers)
	{
		Vector3 direction = Coord.GetCoordFromDirection(colDirection).ToVector3();
		Vector3 origin = position + new Vector3(0f, 0.9f, 0f);
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction, out hitInfo, (float)distance * 1f, ignoreLayers))
		{
			return Mathf.FloorToInt(Vector3.Distance(hitInfo.point, position) / 1f);
		}
		return -1;
	}
}
