using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WorldHudObjectiveUI : MonoBehaviour
{
	private Image _icon;

	private Image _tick;

	private void Awake()
	{
		_icon = base.transform.FindChild("Icon").GetComponent<Image>();
		_tick = base.transform.FindChild("Tick").GetComponent<Image>();
	}

	public void SetUp(bool isComplete)
	{
		_tick.gameObject.SetActive(isComplete);
	}

	public void TweenTicked()
	{
		StartCoroutine(TweenTickedCo());
	}

	public IEnumerator TweenTickedCo()
	{
		App.sound.PlaySFX("Audio/SFX/ObjectiveComplete");
		yield return _icon.transform.DOPunchScale(Vector3.one * 0.7f, 1f, 1, 0f).WaitForPosition(0.5f);
		_tick.gameObject.SetActive(true);
		_tick.transform.localScale = Vector3.one * 6f;
		_tick.color = new Color(1f, 1f, 1f, 0f);
		_tick.transform.DOScale(1f, 0.7f);
		_tick.DOColor(Color.white, 0.5f);
	}
}
