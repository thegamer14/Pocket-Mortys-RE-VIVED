using UnityEngine;

public class NodeTournamentOpen : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerDefs.DefeatedCouncilMembers < 2)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
