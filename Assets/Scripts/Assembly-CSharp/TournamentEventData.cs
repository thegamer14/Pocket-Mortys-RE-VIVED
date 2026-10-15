using System;

[Serializable]
public class TournamentEventData
{
	public bool isComplete;

	public bool isOpenWithCoupon;

	public int attempts;

	public TournamentEventData()
	{
		isComplete = false;
		isOpenWithCoupon = false;
		attempts = 0;
	}
}
