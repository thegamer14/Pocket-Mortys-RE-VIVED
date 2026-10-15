using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MortyStatsBox : MonoBehaviour
{
	private const int _kStatsBoxInPositionX = 720;

	private const int _kStatsBoxOutPositionX = 1130;

	private const float _kStatsBoxInOutDuration = 0.6f;

	private Text _stat1Name;

	private Text _stat2Name;

	private Text _stat3Name;

	private Text _stat4Name;

	private Text _stat1Value;

	private Text _stat2Value;

	private Text _stat3Value;

	private Text _stat4Value;

	private void Awake()
	{
		_stat1Name = base.transform.FindChild("Stat1Name").GetComponent<Text>();
		_stat2Name = base.transform.FindChild("Stat2Name").GetComponent<Text>();
		_stat3Name = base.transform.FindChild("Stat3Name").GetComponent<Text>();
		_stat4Name = base.transform.FindChild("Stat4Name").GetComponent<Text>();
		_stat1Value = base.transform.FindChild("Stat1Value").GetComponent<Text>();
		_stat2Value = base.transform.FindChild("Stat2Value").GetComponent<Text>();
		_stat3Value = base.transform.FindChild("Stat3Value").GetComponent<Text>();
		_stat4Value = base.transform.FindChild("Stat4Value").GetComponent<Text>();
	}

	private void Start()
	{
		SetLabels();
	}

	private void SetLabels()
	{
		_stat1Name.text = TextDefs.MORTY_STAT_HP + ":";
		_stat2Name.text = TextDefs.MORTY_STAT_ATTACK + ":";
		_stat3Name.text = TextDefs.MORTY_STAT_DEFENCE + ":";
		_stat4Name.text = TextDefs.MORTY_STAT_SPEED + ":";
	}

	public void Set(int hp, int attack, int defence, int speed)
	{
		_stat1Value.text = string.Format("{0}", hp);
		_stat2Value.text = string.Format("{0}", attack);
		_stat3Value.text = string.Format("{0}", defence);
		_stat4Value.text = string.Format("{0}", speed);
	}

	public void Set(int hp, int hpDiffrence, int attack, int attackDiffrence, int defence, int defenceDiffrence, int speed, int speedDiffrence)
	{
		_stat1Value.text = ((hpDiffrence <= 0) ? string.Format("{0}", hp) : string.Format("{0} (+{1})", hp, hpDiffrence));
		_stat2Value.text = ((attackDiffrence <= 0) ? string.Format("{0}", attack) : string.Format("{0} (+{1})", attack, attackDiffrence));
		_stat3Value.text = ((defenceDiffrence <= 0) ? string.Format("{0}", defence) : string.Format("{0} (+{1})", defence, defenceDiffrence));
		_stat4Value.text = ((speedDiffrence <= 0) ? string.Format("{0}", speed) : string.Format("{0} (+{1})", speed, speedDiffrence));
	}

	public void SetPositionIn()
	{
		base.transform.localPosition = new Vector3(720f, base.transform.localPosition.y, base.transform.localPosition.z);
	}

	public void SetPositionOut()
	{
		base.transform.localPosition = new Vector3(1130f, base.transform.localPosition.y, base.transform.localPosition.z);
	}

	public IEnumerator TweenIn()
	{
		Tween tween = base.transform.DOLocalMoveX(720f, 0.6f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOut()
	{
		Tween tween = base.transform.DOLocalMoveX(1130f, 0.6f);
		yield return tween.WaitForCompletion();
	}
}
