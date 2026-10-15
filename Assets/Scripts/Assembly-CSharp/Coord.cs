using UnityEngine;

public struct Coord
{
	public int x;

	public int z;

	public static Coord zero
	{
		get
		{
			return new Coord(0, 0);
		}
	}

	public Coord(int xz)
	{
		x = xz;
		z = xz;
	}

	public Coord(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Coord))
		{
			return false;
		}
		Coord coord = (Coord)obj;
		return x == coord.x && z == coord.z;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ z.GetHashCode();
	}

	public static bool operator ==(Coord self, Coord other)
	{
		return self.x == other.x && self.z == other.z;
	}

	public static bool operator !=(Coord self, Coord other)
	{
		return !(self == other);
	}

	public static Coord operator +(Coord self, Coord other)
	{
		self.x += other.x;
		self.z += other.z;
		return self;
	}

	public static Coord operator -(Coord self, Coord other)
	{
		self.x -= other.x;
		self.z -= other.z;
		return self;
	}

	public override string ToString()
	{
		return "Coord (x: " + x + ", z: " + z + ")";
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, 0f, z);
	}

	public static Coord GetCoordFromPosition(Vector3 position)
	{
		return new Coord(Mathf.FloorToInt(position.x / 1f), Mathf.FloorToInt(position.z / 1f));
	}

	public static Vector3 GetPositionFromCoord(Coord coord)
	{
		return new Vector3((float)coord.x * 1f + 0.5f, 0f, (float)coord.z * 1f + 0.5f);
	}

	public static Coord GetCoordFromDirection(eDirection direction)
	{
		switch (direction)
		{
		default:
			return zero;
		case eDirection.UP:
			return new Coord(0, 1);
		case eDirection.DOWN:
			return new Coord(0, -1);
		case eDirection.RIGHT:
			return new Coord(1, 0);
		case eDirection.LEFT:
			return new Coord(-1, 0);
		}
	}

	public static eDirection GetDirectionFromCoord(Coord coord)
	{
		if (Mathf.Abs(coord.x) > Mathf.Abs(coord.z))
		{
			if (coord.x > 0)
			{
				return eDirection.RIGHT;
			}
			if (coord.x < 0)
			{
				return eDirection.LEFT;
			}
		}
		else
		{
			if (coord.z > 0)
			{
				return eDirection.UP;
			}
			if (coord.z < 0)
			{
				return eDirection.DOWN;
			}
		}
		return eDirection.DOWN;
	}

	public static float GetAngleFromCoord(Coord coord1, Coord coord2)
	{
		return Mathf.Atan2(coord1.z - coord2.z, coord2.x - coord1.x) * 57.29578f;
	}

	public bool IsEdgeAdjacent(Coord testCoord)
	{
		if (x != testCoord.x && z != testCoord.z)
		{
			return false;
		}
		return Mathf.Abs(testCoord.x - x) == 1 || Mathf.Abs(testCoord.z - z) == 1;
	}
}
