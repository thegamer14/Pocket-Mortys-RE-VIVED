using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CourierFlapUI : UIBase
{
	public delegate void CourierFlapUIDelegate();

	private MenuDialogue dialogue;

	private Transform _flapTrans;

	private BattleBackground _background;

	public event CourierFlapUIDelegate FinishedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_flapTrans = base.transform.FindChild("CourierFlap");
		_background = base.transform.FindChild("Background").GetComponent<BattleBackground>();
	}

	public void SetUp()
	{
		_background.SetBackground(SharedStatInfo.worldInfo.worldID, SharedStatInfo.worldInfo.nodeSetID);
		_background.SetSpeed(-12f);
		_flapTrans.DOLocalMoveY(-70f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo)
			.SetRelative(true);
		StartCoroutine(PlayText());
	}

	public IEnumerator PlayText()
	{
		yield return StartCoroutine(dialogue.SetTextCo(TextDefs.COURIER_FLAP_DAIL_1, 5.5f, true));
		if (this.FinishedEvent != null)
		{
			this.FinishedEvent();
		}
	}
}
