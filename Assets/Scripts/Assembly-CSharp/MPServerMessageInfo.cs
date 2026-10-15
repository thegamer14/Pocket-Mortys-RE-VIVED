using System;

public class MPServerMessageInfo : MPStateInfo
{
	public Type nextState;

	public MPServerMessageInfo(Type nextState)
		: base(eMPStateOrigin.NONE)
	{
		this.nextState = nextState;
	}
}
