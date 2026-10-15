using System;
using UnityEngine.UI;

public class SaveDataCell : CellBase
{
	public CloudContainer cloudContainer;

	private Text _modifiedLabel;

	private Text _coinsLabel;

	private Text _couponsLabel;

	private Text _badgesLabel;

	private Text _mortysLabel;

	protected override void Awake()
	{
		base.Awake();
		_modifiedLabel = base.transform.FindChild("Modified").GetComponent<Text>();
		_coinsLabel = base.transform.FindChild("Coins").GetComponent<Text>();
		_couponsLabel = base.transform.FindChild("Coupons").GetComponent<Text>();
		_badgesLabel = base.transform.FindChild("Badges").GetComponent<Text>();
		_mortysLabel = base.transform.FindChild("Mortys").GetComponent<Text>();
	}

	public void SetUp(CloudContainer cloudContainer)
	{
		base.SetUp(cloudContainer.lastOpenedTimestamp.ToString());
		this.cloudContainer = cloudContainer;
		int num = ((cloudContainer.player != null && cloudContainer.player.badges != null) ? cloudContainer.player.badges.Count : 0);
		int num2 = ((cloudContainer.player != null) ? cloudContainer.player.coins : 0);
		int num3 = ((cloudContainer.player != null) ? cloudContainer.player.coupons : 0);
		string arg = ((cloudContainer.lastModifiedTimestamp <= 0) ? "-" : new DateTime(cloudContainer.lastModifiedTimestamp).ToLocalTime().ToString());
		int num4 = 0;
		if (cloudContainer.player != null)
		{
			if (cloudContainer.player.partyMorties != null)
			{
				num4 += cloudContainer.player.partyMorties.Count;
			}
			if (cloudContainer.player.storageMorties != null)
			{
				num4 += cloudContainer.player.storageMorties.Count;
			}
		}
		_modifiedLabel.text = string.Format(TextDefs.SAVE_CELL_MODIFIED, arg);
		_coinsLabel.text = string.Format(TextDefs.SAVE_CELL_SCHMECKLES, num2);
		_couponsLabel.text = string.Format(TextDefs.SAVE_CELL_COUPONS, num3);
		_badgesLabel.text = string.Format(TextDefs.SAVE_CELL_BADGES, num);
		_mortysLabel.text = string.Format(TextDefs.SAVE_CELL_MORTYS, num4);
	}
}
