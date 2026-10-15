using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPRewardUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	public delegate void BagUITwitterBtnDelegate();

	private float _kBackgroundRotationSpeed = -10f;

	private RectTransform _backgroundDefault;

	private RectTransform _backgroundPlayerLevel;

	private RectTransform _coinReward;

	private RectTransform _mortyReward;

	private RectTransform _itemReward;

	private RectTransform _couponReward;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private eMPRewardConfirmation _lastConfirmation;

	public event BagUIBtnDelegate ContinueEvent;

	public event BagUITwitterBtnDelegate TweetEvent;

	protected override void LateAwake()
	{
		_backgroundDefault = base.transform.FindChild("BackgroundDefault").GetComponent<RectTransform>();
		_backgroundPlayerLevel = base.transform.FindChild("BackgroundPlayerLevel").GetComponent<RectTransform>();
		_coinReward = base.transform.FindChild("CoinReward").GetComponent<RectTransform>();
		_mortyReward = base.transform.FindChild("MortyReward").GetComponent<RectTransform>();
		_itemReward = base.transform.FindChild("ItemReward").GetComponent<RectTransform>();
		_couponReward = base.transform.FindChild("CouponReward").GetComponent<RectTransform>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(MPRewardData rewardData, eMPRewardConfirmation confirmation, bool playerLevelBackground)
	{
		_coinReward.gameObject.SetActive(false);
		_mortyReward.gameObject.SetActive(false);
		_itemReward.gameObject.SetActive(false);
		_couponReward.gameObject.SetActive(false);
		_mainMenu.SetPositionOut();
		DisplayReward(rewardData);
		_backgroundDefault.gameObject.SetActive(!playerLevelBackground);
		_backgroundPlayerLevel.gameObject.SetActive(playerLevelBackground);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.CRAFT_BTN_TWEET);
		_mainMenu.SetBtnTitle(1, TextDefs.REWARD_CONTINUE_BTN_USE);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public override void ManualUpdate(float deltaTime)
	{
		_backgroundDefault.Rotate(0f, 0f, _kBackgroundRotationSpeed * deltaTime);
	}

	private void DisplayReward(MPRewardData rewardData)
	{
		switch (rewardData.type)
		{
		case eMPRewardType.COIN:
			DisplayRewardCoin(rewardData);
			break;
		case eMPRewardType.COUPON:
			DisplayRewardCoupon(rewardData);
			break;
		case eMPRewardType.MORTY:
			DisplayRewardMorty(rewardData);
			break;
		case eMPRewardType.ITEM:
			DisplayRewardItem(rewardData);
			break;
		}
	}

	private void DisplayRewardCoin(MPRewardData rewardData)
	{
		_coinReward.gameObject.SetActive(true);
		_coinReward.FindChild("Label").GetComponent<Text>().text = rewardData.amount.ToString();
		TweenPunch(_coinReward);
	}

	private void DisplayRewardCoupon(MPRewardData rewardData)
	{
		_couponReward.gameObject.SetActive(true);
		TweenPunch(_couponReward);
	}

	private void DisplayRewardMorty(MPRewardData rewardData)
	{
		_mortyReward.gameObject.SetActive(true);
		Image component = _mortyReward.FindChild("Icon").GetComponent<Image>();
		component.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(rewardData.id), "front"));
		component.SetNativeSize();
		TweenPunch(_mortyReward);
	}

	private void DisplayRewardItem(MPRewardData rewardData)
	{
		_itemReward.gameObject.SetActive(true);
		Image component = _itemReward.FindChild("Icon").GetComponent<Image>();
		component.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPItemDefs.AssetID(rewardData.id), "front"));
		component.SetNativeSize();
		TweenPunch(_itemReward);
	}

	public IEnumerator ConfirmReward(MPRewardData rewardData, eMPRewardConfirmation confirmation, string analyticId)
	{
		switch (confirmation)
		{
		case eMPRewardConfirmation.SUCCESS:
		{
			string outcomeText = GetOutcomeText(rewardData);
			string dialogueText = null;
			if (rewardData.type == eMPRewardType.COIN && rewardData.amountReceived > 0)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, analyticId, rewardData.amountReceived);
			}
			else if (rewardData.substitute != null && rewardData.substitute.type == eMPRewardType.COIN && rewardData.substitute.amountReceived > 0)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, analyticId + "_sub", rewardData.substitute.amountReceived);
			}
			switch (rewardData.type)
			{
			case eMPRewardType.COIN:
				App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
				dialogueText = string.Format(MPTextDefs.MP_REWARD_TYPE_COIN, rewardData.amount);
				break;
			case eMPRewardType.COUPON:
				App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
				dialogueText = string.Format(TextDefs.REWARD_TYPE_ITEM, rewardData.amount, MPItemDefs.Name("ItemCouponMP"));
				break;
			case eMPRewardType.MORTY:
				App.sound.PlaySFX("Audio/SFX/SpecialReward");
				dialogueText = string.Format(TextDefs.REWARD_TYPE_MORTY, rewardData.amount, MPMortyDefs.Name(rewardData.id));
				break;
			case eMPRewardType.ITEM:
				App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
				dialogueText = string.Format(TextDefs.REWARD_TYPE_ITEM, rewardData.amount, MPItemDefs.Name(rewardData.id));
				break;
			}
			float dialogueTextReadDelay = ((outcomeText == null) ? 0f : 2f);
			bool dialogueTextIsSkippable = outcomeText != null;
			if (_lastConfirmation == eMPRewardConfirmation.WAITING)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, dialogueTextReadDelay, dialogueTextIsSkippable));
			}
			else
			{
				yield return StartCoroutine(_dialogue.SetTextCo(dialogueText, dialogueTextReadDelay, dialogueTextIsSkippable));
			}
			if (_lastConfirmation == eMPRewardConfirmation.WAITING || outcomeText != null)
			{
				StartCoroutine(_mainMenu.TweenIn());
			}
			else
			{
				_mainMenu.SetPositionIn();
			}
			if (outcomeText != null)
			{
				_dialogue.TweenText(outcomeText);
			}
			break;
		}
		case eMPRewardConfirmation.FAIL:
			StartCoroutine(_mainMenu.TweenIn());
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(MPTextDefs.MP_REWARD_CONFIRMATION_FAIL);
			break;
		default:
			_mainMenu.SetPositionOut();
			_dialogue.SetText(MPTextDefs.MP_REWARD_CONFIRMATION_WAITING);
			break;
		}
		_lastConfirmation = confirmation;
	}

	public string GetOutcomeText(MPRewardData rewardData)
	{
		switch (rewardData.type)
		{
		case eMPRewardType.COIN:
			if (rewardData.amount != rewardData.amountReceived)
			{
				return MPTextDefs.MP_COINS_MAX;
			}
			break;
		case eMPRewardType.COUPON:
			if (rewardData.amount != rewardData.amountReceived)
			{
				return TextDefs.COUPONS_MAX;
			}
			break;
		case eMPRewardType.MORTY:
			if (rewardData.isMortyLimit)
			{
				return string.Format(TextDefs.MORTY_CAPTURED_PARTY_AND_STORAGE_FULL, MPMortyDefs.Name(rewardData.id));
			}
			if (rewardData.isAddedToDeck)
			{
				return string.Format(TextDefs.MORTY_CAPTURED_PARTY, MPMortyDefs.Name(rewardData.id));
			}
			return string.Format(TextDefs.MORTY_CAPTURED_STORAGE, MPMortyDefs.Name(rewardData.id));
		case eMPRewardType.ITEM:
			if (rewardData.substitute != null)
			{
				return string.Format(MPTextDefs.MP_ITEM_REWARD_NO_SPACE_BUT_COINS, rewardData.substitute.amount);
			}
			if (rewardData.amount != rewardData.amountReceived)
			{
				return string.Format(TextDefs.ITEM_OBTAIN_NO_SPACE, MPItemDefs.Name(rewardData.id));
			}
			break;
		}
		return null;
	}

	public void TweenPunch(RectTransform rectTrans)
	{
		rectTrans.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.4f);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.TweetEvent != null)
			{
				this.TweetEvent();
			}
			break;
		case 1:
			if (this.ContinueEvent != null)
			{
				this.ContinueEvent();
			}
			break;
		}
	}
}
