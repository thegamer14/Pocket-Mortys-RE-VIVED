using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UILayer : MonoBehaviour
{
	private Image _transitionFadeImage;

	private Image _backgroundFadeImage;

	private GameObject _loadingScreen;

	private Text _loadingScreenTitle;

	private Sequence _flashSeq;

	private Coroutine _transitionFadeCo;

	private Coroutine _backgroundFadeCo;

	public RectTransform uiContainerRectTrans { get; private set; }

	public float transitionAlpha
	{
		get
		{
			return _transitionFadeImage.color.a;
		}
		private set
		{
			value = Mathf.Clamp01(value);
			_transitionFadeImage.color = new Color(_transitionFadeImage.color.r, _transitionFadeImage.color.g, _transitionFadeImage.color.b, value);
			if (value == 0f && _transitionFadeImage.gameObject.activeSelf)
			{
				_transitionFadeImage.gameObject.SetActive(false);
			}
			else if (value > 0f && !_transitionFadeImage.gameObject.activeSelf)
			{
				_transitionFadeImage.gameObject.SetActive(true);
			}
		}
	}

	private float _backgroundAlpha
	{
		get
		{
			return _backgroundFadeImage.color.a;
		}
		set
		{
			value = Mathf.Clamp01(value);
			_backgroundFadeImage.color = new Color(_backgroundFadeImage.color.r, _backgroundFadeImage.color.g, _backgroundFadeImage.color.b, value);
			if (value == 0f && _backgroundFadeImage.gameObject.activeSelf)
			{
				_backgroundFadeImage.gameObject.SetActive(false);
			}
			else if (value > 0f && !_backgroundFadeImage.gameObject.activeSelf)
			{
				_backgroundFadeImage.gameObject.SetActive(true);
			}
		}
	}

	private void Awake()
	{
		uiContainerRectTrans = base.transform.Find("UIContainer").GetComponent<RectTransform>();
		_transitionFadeImage = base.transform.Find("TransitionFade").GetComponent<Image>();
		_backgroundFadeImage = base.transform.Find("BackgroundFade").GetComponent<Image>();
		_loadingScreen = base.transform.Find("LoadingScreen").gameObject;
		_loadingScreenTitle = _loadingScreen.transform.FindChild("Label").GetComponent<Text>();
		Reset();
	}

	public void Reset()
	{
		transitionAlpha = 0f;
		_backgroundAlpha = 0f;
		ShowLoadingScreen(false);
	}

	public void DestroySelf()
	{
		StopTransitionFadeCo();
		Object.Destroy(base.gameObject);
	}

	public void SetTransitionFade(Color? color)
	{
		if (color.HasValue)
		{
			_transitionFadeImage.gameObject.SetActive(true);
			_transitionFadeImage.color = new Color(color.Value.r, color.Value.g, color.Value.b, 1f);
		}
		else
		{
			_transitionFadeImage.gameObject.SetActive(false);
		}
	}

	public IEnumerator TransitionFade(eTransitionDirection transitionDirection, FadeTransInfo info)
	{
		StopTransitionFadeCo();
		StopFlash();
		_transitionFadeImage.color = info.color;
		_transitionFadeCo = StartCoroutine(TransitionFadeCo(transitionDirection, info));
		yield return _transitionFadeCo;
	}

	private IEnumerator TransitionFadeCo(eTransitionDirection transitionDirection, FadeTransInfo info)
	{
		transitionAlpha = ((transitionDirection != eTransitionDirection.OUT) ? info.to : info.from);
		float toValue = ((transitionDirection != eTransitionDirection.OUT) ? info.from : info.to);
		float timeElapsed = 0f;
		float startAlpha = transitionAlpha;
		while (timeElapsed < info.duration)
		{
			timeElapsed += TimeIndependant.deltaTime;
			float p = Mathf.Clamp01(timeElapsed / info.duration);
			transitionAlpha = startAlpha + (toValue - startAlpha) * p;
			yield return null;
		}
		transitionAlpha = toValue;
	}

	private void StopTransitionFadeCo()
	{
		if (_transitionFadeCo != null)
		{
			StopCoroutine(_transitionFadeCo);
			_transitionFadeCo = null;
		}
	}

	public void BackgroundFade(eTransitionDirection transitionDirection, Color color, float duration)
	{
		StopBackgroundFadeCo();
		_backgroundFadeCo = StartCoroutine(BackgroundFadeCo(transitionDirection, color, duration));
	}

	private IEnumerator BackgroundFadeCo(eTransitionDirection transitionDirection, Color color, float duration)
	{
		_backgroundFadeImage.color = color;
		_backgroundAlpha = ((transitionDirection != eTransitionDirection.IN) ? color.a : 0f);
		float toValue = ((transitionDirection != eTransitionDirection.IN) ? 0f : color.a);
		float timeElapsed = 0f;
		float startAlpha = _backgroundAlpha;
		while (timeElapsed < duration)
		{
			timeElapsed += TimeIndependant.deltaTime;
			float p = Mathf.Clamp01(timeElapsed / duration);
			_backgroundAlpha = startAlpha + (toValue - startAlpha) * p;
			yield return null;
		}
		_backgroundAlpha = toValue;
	}

	private void StopBackgroundFadeCo()
	{
		if (_backgroundFadeCo != null)
		{
			StopCoroutine(_backgroundFadeCo);
			_backgroundFadeCo = null;
		}
	}

	public void ShowLoadingScreen(bool isShow)
	{
		_loadingScreen.SetActive(isShow);
		_loadingScreenTitle.text = TextDefs.INTERSTITIAL_LOADING;
	}

	public void Flash(float outDuration)
	{
		StopFlash();
		_transitionFadeImage.gameObject.SetActive(true);
		_transitionFadeImage.color = Color.white;
		_flashSeq = DOTween.Sequence();
		_flashSeq.AppendInterval(0.1f);
		if (outDuration > 0f)
		{
			_flashSeq.Append(_transitionFadeImage.DOColor(new Color(1f, 1f, 1f, 0f), outDuration));
		}
		_flashSeq.OnComplete(StopFlash);
		_flashSeq.Play();
	}

	public void StopFlash()
	{
		if (_flashSeq != null)
		{
			_transitionFadeImage.gameObject.SetActive(false);
			_flashSeq.Kill();
			_flashSeq = null;
		}
	}
}
