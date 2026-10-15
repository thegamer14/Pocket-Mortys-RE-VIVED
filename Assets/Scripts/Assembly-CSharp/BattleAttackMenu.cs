public class BattleAttackMenu : MenuBase
{
	public void SetUpBtn(int index, AttackDisplayInfo displayInfo, bool showRemainingPP = true)
	{
		((BattleAttackBtn)_btns[index]).SetUp(index, displayInfo, showRemainingPP);
	}
}
