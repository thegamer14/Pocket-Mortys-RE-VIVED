using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashUI : UIBase
{
	public delegate void SplashDelegate();

	private const int _kFinishedDelay = 2;

	private const int _kSupportButtonTrigger = 5;

	private Image _asgLogo;

	private Image _bpsLogo;

	private int _supportButtonCounter;

	public event SplashDelegate FinishedEvent;

	protected override void LateAwake()
	{
		_asgLogo = base.transform.FindChild("ASGLogo").GetComponent<Image>();
		_bpsLogo = base.transform.FindChild("BPSLogo").GetComponent<Image>();
	}

	public void SetUp(bool showASG)
	{
		if (showASG)
		{
			_bpsLogo.gameObject.SetActive(false);
		}
		else
		{
			_asgLogo.gameObject.SetActive(false);
		}
		StartCoroutine(CountdownToFinished());
	}

	private IEnumerator CountdownToFinished()
	{
		yield return new WaitForSeconds(2f);
		if (this.FinishedEvent != null)
		{
			this.FinishedEvent();
		}
	}

	public void SupportButtonHandler()
	{
		_supportButtonCounter++;
		if (_supportButtonCounter == 5)
		{
			App.saveData.EmailCloudContainerToSupport();
		}
	}
}
