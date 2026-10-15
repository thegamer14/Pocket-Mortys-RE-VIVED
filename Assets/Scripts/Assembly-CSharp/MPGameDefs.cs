using UnityEngine;

public class MPGameDefs
{
	public const string kUsernameLegalChars = "^[a-zA-Z0-9-_]*$";

	public const float kCoordToUnit = 1f;

	public const float kCoordToUnitHalf = 0.5f;

	public static Vector3 kGridToUnitOffset = new Vector3(0.5f, 0f, 0.5f);

	public const int kNumRegistrationMorties = 3;

	public static void SetUpDefs()
	{
		MPPlayerAvatarDefs.SetUp();
		MPMortyDefs.SetUp();
		MPAttackDefs.SetUp();
		MPItemDefs.SetUp();
		MPRecipeDefs.SetUp();
		MPNPCDefs.SetUp();
		MPSignPostDefs.SetUp();
		MPWorldDefs.SetUp();
	}

	public static void RemoveDefs()
	{
		MPPlayerAvatarDefs.Remove();
		MPMortyDefs.Remove();
		MPAttackDefs.Remove();
		MPItemDefs.Remove();
		MPRecipeDefs.Remove();
		MPNPCDefs.Remove();
		MPSignPostDefs.Remove();
		MPWorldDefs.Remove();
	}

	public static float GetPlayerXPTarget(int level)
	{
		int num = level + 2;
		return Mathf.FloorToInt(num * num * num);
	}
}
