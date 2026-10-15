public class AttackDisplayInfo
{
	public string name { get; private set; }

	public string description { get; private set; }

	public eAttackCatagory catagory { get; private set; }

	public string element { get; private set; }

	public int remainingPP { get; private set; }

	public int maxPP { get; private set; }

	public string popUpValue1 { get; private set; }

	public string popUpValue2 { get; private set; }

	public AttackDisplayInfo(AttackData data, int remainingPP = 0)
	{
		name = data.name;
		description = data.description;
		catagory = AttackDefs.GetAttackCatagory(data);
		element = data.element;
		this.remainingPP = remainingPP;
		maxPP = data.pp;
		popUpValue1 = AttackDefs.GetPopUpValue1(data.id);
		popUpValue2 = AttackDefs.GetPopUpValue2(data.id);
	}

	public AttackDisplayInfo(string id, int remainingPP = 0)
	{
		name = MPAttackDefs.Name(id);
		description = MPAttackDefs.Description(id);
		catagory = MPAttackDefs.GetAttackCatagory(id);
		element = MPAttackDefs.ElementType(id);
		this.remainingPP = remainingPP;
		maxPP = MPAttackDefs.PP(id);
		popUpValue1 = MPAttackDefs.GetPopUpValue1(id);
		popUpValue2 = MPAttackDefs.GetPopUpValue2(id);
	}
}
