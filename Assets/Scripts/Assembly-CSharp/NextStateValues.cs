public class NextStateValues
{
	public StateBase toState;

	public TransInfo transitionInInfo;

	public string sceneSwitch;

	public eStateAction action;

	public NextStateValues(StateBase toState, TransInfo transitionInInfo, string sceneSwitch, eStateAction action)
	{
		this.toState = toState;
		this.transitionInInfo = transitionInInfo;
		this.sceneSwitch = sceneSwitch;
		this.action = action;
	}
}
