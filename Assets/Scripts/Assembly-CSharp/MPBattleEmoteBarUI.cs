using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPBattleEmoteBarUI : MonoBehaviour
{
	public delegate void EmoteSelectedDelegate(string emoteId);

	public delegate void EmoteBarDelegate();

	private Transform _emotesTran;

	private RectTransform _thisRectTrans;

	private bool _isOn;

	private bool _isAnimatingOn;

	private bool _isAnimatingOff;

	public event EmoteSelectedDelegate EmoteSelectedEvent;

	public event EmoteBarDelegate AnimateOnBeginEvent;

	public event EmoteBarDelegate AnimateOffCompleteEvent;

	private void Awake()
	{
		_emotesTran = base.transform.FindChild("Emotes");
		_thisRectTrans = base.transform as RectTransform;
		PopulateEmotes();
	}

	private void PopulateEmotes()
	{
		string[] p2pEmoteIds = MPEmoteDefs.p2pEmoteIds;
		foreach (string arg in p2pEmoteIds)
		{
			Button btn = UI.Instantiate(string.Format("UI/MPBattle/Emotes/{0}", arg), _emotesTran).GetComponent<Button>();
			btn.gameObject.name = arg;
			btn.onClick.AddListener(delegate
			{
				EmoteSelectedHandler(btn.gameObject);
			});
		}
	}

	public IEnumerator AnimateOnCo()
	{
		if (!_isAnimatingOn && !_isAnimatingOff && !_isOn)
		{
			if (this.AnimateOnBeginEvent != null)
			{
				this.AnimateOnBeginEvent();
			}
			_thisRectTrans.localScale = Vector3.zero;
			base.gameObject.SetActive(true);
			_thisRectTrans.DOKill();
			_isAnimatingOn = true;
			yield return new WaitForEndOfFrame();
			yield return _thisRectTrans.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
			_isOn = true;
			_isAnimatingOn = false;
		}
	}

	public IEnumerator AnimateOffCo()
	{
		if (!_isAnimatingOff && _isOn)
		{
			_thisRectTrans.DOKill();
			_isAnimatingOff = true;
			yield return _thisRectTrans.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
			_isAnimatingOff = false;
			_isOn = false;
			if (this.AnimateOffCompleteEvent != null)
			{
				this.AnimateOffCompleteEvent();
			}
		}
	}

	public void CloseButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		StartCoroutine(AnimateOffCo());
	}

	public void EmoteSelectedHandler(GameObject btn)
	{
		if (!_isAnimatingOn && !_isAnimatingOff)
		{
			App.sound.PlaySFX("Audio/SFX/GenericButton");
			StartCoroutine(AnimateOffCo());
			if (this.EmoteSelectedEvent != null)
			{
				this.EmoteSelectedEvent(btn.name);
			}
		}
	}
}
