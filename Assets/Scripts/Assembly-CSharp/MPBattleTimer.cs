using UnityEngine;
using UnityEngine.UI;

public class MPBattleTimer : MonoBehaviour
{
	private const float _kHandRotationSpeed = -350f;

	private Image _fill;

	private Image _hand;

	private Vector3 _startPosition;

	private MPBattleTimerData data;

	private void Awake()
	{
		_fill = base.transform.FindChild("Fill").GetComponent<Image>();
		_hand = base.transform.FindChild("Hand").GetComponent<Image>();
		App.net.battle.MoveTimerStartedEvent += NetBattleMoveTimerStartedHandler;
	}

	private void Start()
	{
		_startPosition = base.transform.localPosition;
		data = App.net.battle.timerData;
		Show(data != null && !App.net.battle.isTimerExpiredOrSkipped);
	}

	private void OnDestroy()
	{
		App.net.battle.MoveTimerStartedEvent -= NetBattleMoveTimerStartedHandler;
	}

	private void Update()
	{
		if (data != null && !App.net.battle.isTimerExpiredOrSkipped)
		{
			SetFill(data.time, data.timeTotal);
			RotateHand();
			if (data.time <= data.timeTotal / 3f)
			{
				Vibrate();
			}
		}
	}

	public void Show(bool isShow)
	{
		if (isShow)
		{
			base.gameObject.SetActive(true);
			if (data != null)
			{
				SetFill(data.time, data.timeTotal);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void SetFill(float time, float timeTotal)
	{
		_fill.fillAmount = 1f - time / timeTotal;
	}

	public void RotateHand()
	{
		_hand.transform.eulerAngles += new Vector3(0f, 0f, -350f) * Time.deltaTime;
	}

	private void Vibrate()
	{
		float num = Mathf.Sin(Time.time * 40f) * 3f;
		base.transform.localPosition = _startPosition + new Vector3(num, num, 0f);
	}

	private void NetBattleMoveTimerStartedHandler(MPBattleTimerData data)
	{
		this.data = data;
		Show(data != null && !App.net.battle.isTimerExpiredOrSkipped);
	}
}
