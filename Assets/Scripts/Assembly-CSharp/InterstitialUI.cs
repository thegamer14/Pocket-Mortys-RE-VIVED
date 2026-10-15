using UnityEngine.UI;

public class InterstitialUI : UIBase
{
	private Text _loadingLabel;

	protected override void LateAwake()
	{
		_loadingLabel = base.transform.FindChild("Label").GetComponent<Text>();
	}

	public override void SetLabels()
	{
		base.SetLabels();
		_loadingLabel.text = TextDefs.INTERSTITIAL_LOADING;
	}
}
