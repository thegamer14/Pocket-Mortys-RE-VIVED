using UnityEngine;

public class NodeTournamentLocked : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerDefs.DefeatedCouncilMembers >= 2)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
