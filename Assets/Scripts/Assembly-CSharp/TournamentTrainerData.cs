using System;

[Serializable]
public class TournamentTrainerData : TrainerData
{
	public TournamentTrainerData(string id)
		: base(id)
	{
	}

	protected override void SetMorties()
	{
		morties = TournamentDefs.TrainerMorties(id);
	}

	protected override void SetItems()
	{
		items = TournamentDefs.TrainerItems(id);
	}
}
