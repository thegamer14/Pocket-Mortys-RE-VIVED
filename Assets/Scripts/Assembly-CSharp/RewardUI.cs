using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	public delegate void BagUITwitterBtnDelegate(RewardData rewardData);

	private float _kBackgroundRotationSpeed = -10f;

	private RectTransform _background;

	private RectTransform _badgeReward;

	private RectTransform _portalGunReward;

	private RectTransform _coinReward;

	private RectTransform _mortyReward;

	private RectTransform _itemReward;

	private RectTransform _couponReward;

	private RectTransform _trophyReward;

	private RectTransform _tournamentTicketReward;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private RewardData _rewardData;

	public event BagUIBtnDelegate ContinueEvent;

	public event BagUITwitterBtnDelegate TweetEvent;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background").GetComponent<RectTransform>();
		_badgeReward = base.transform.FindChild("BadgeReward").GetComponent<RectTransform>();
		_portalGunReward = base.transform.FindChild("PortalGunReward").GetComponent<RectTransform>();
		_coinReward = base.transform.FindChild("CoinReward").GetComponent<RectTransform>();
		_mortyReward = base.transform.FindChild("MortyReward").GetComponent<RectTransform>();
		_itemReward = base.transform.FindChild("ItemReward").GetComponent<RectTransform>();
		_couponReward = base.transform.FindChild("CouponReward").GetComponent<RectTransform>();
		_trophyReward = base.transform.FindChild("TrophyReward").GetComponent<RectTransform>();
		_tournamentTicketReward = base.transform.FindChild("TournamentTicketReward").GetComponent<RectTransform>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(RewardData rewardData, string resultText)
	{
		_rewardData = rewardData;
		if (resultText == null)
		{
			_mainMenu.SetPositionIn();
		}
		else
		{
			_mainMenu.SetPositionOut();
		}
		_badgeReward.gameObject.SetActive(false);
		_portalGunReward.gameObject.SetActive(false);
		_coinReward.gameObject.SetActive(false);
		_mortyReward.gameObject.SetActive(false);
		_itemReward.gameObject.SetActive(false);
		_couponReward.gameObject.SetActive(false);
		_trophyReward.gameObject.SetActive(false);
		_tournamentTicketReward.gameObject.SetActive(false);
		StartCoroutine(GiveReward(rewardData, resultText));
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
		_background.Rotate(0f, 0f, _kBackgroundRotationSpeed * deltaTime);
	}

	private IEnumerator GiveReward(RewardData rewardData, string resultText)
	{
		switch (rewardData.type)
		{
		case eRewardType.BADGE:
			App.sound.PlaySFX("Audio/SFX/SpecialReward");
			yield return StartCoroutine(ShowBadgeReward(rewardData, resultText != null));
			break;
		case eRewardType.COUNCIL_BADGE:
			App.sound.PlaySFX("Audio/SFX/SpecialReward");
			yield return StartCoroutine(ShowCouncilBadgeReward(rewardData, resultText != null));
			break;
		case eRewardType.PORTAL_GUN:
			App.sound.PlaySFX("Audio/SFX/SpecialReward");
			yield return StartCoroutine(ShowPortalGunReward(rewardData, resultText != null));
			break;
		case eRewardType.COIN:
			App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
			yield return StartCoroutine(ShowCoinReward(rewardData, resultText != null));
			break;
		case eRewardType.MORTY:
			App.sound.PlaySFX("Audio/SFX/SpecialReward");
			yield return StartCoroutine(ShowMortyReward(rewardData, resultText != null));
			break;
		case eRewardType.ITEM:
			App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
			if (rewardData.id == "ItemCoupon")
			{
				yield return StartCoroutine(ShowCouponReward(rewardData, resultText != null));
			}
			else
			{
				yield return StartCoroutine(ShowItemReward(rewardData, resultText != null));
			}
			break;
		case eRewardType.TROPHY:
			App.sound.PlaySFX("Audio/SFX/SpecialReward");
			yield return StartCoroutine(ShowTrophyReward(rewardData, resultText != null));
			break;
		case eRewardType.TOURNAMENT_TICKET:
			App.sound.PlaySFX("Audio/SFX/Reward", 0.75f);
			yield return StartCoroutine(ShowTournamentTicketReward(rewardData, resultText != null));
			break;
		}
		if (resultText != null)
		{
			_dialogue.TweenText(resultText);
			StartCoroutine(_mainMenu.TweenIn());
		}
	}

	private IEnumerator ShowBadgeReward(RewardData rewardData, bool isNextPage)
	{
		_badgeReward.gameObject.SetActive(true);
		Image icon = _badgeReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Badges/{0}Large", "GymLeader/" + rewardData.value));
		icon.SetNativeSize();
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.REWARD_TYPE_BADGE, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(TextDefs.REWARD_TYPE_BADGE);
		}
		TweenPunch(_badgeReward);
	}

	private IEnumerator ShowCouncilBadgeReward(RewardData rewardData, bool isNextPage)
	{
		_badgeReward.gameObject.SetActive(true);
		Image icon = _badgeReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Badges/{0}Large", "CouncilMember/" + rewardData.value));
		icon.SetNativeSize();
		TweenPunch(_badgeReward);
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.REWARD_TYPE_COUNCIL_BADGE, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(TextDefs.REWARD_TYPE_COUNCIL_BADGE);
		}
	}

	private IEnumerator ShowPortalGunReward(RewardData rewardData, bool isNextPage)
	{
		_portalGunReward.gameObject.SetActive(true);
		TweenPunch(_portalGunReward);
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.REWARD_TYPE_PORTAL_GUN, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(TextDefs.REWARD_TYPE_PORTAL_GUN);
		}
	}

	private IEnumerator ShowCoinReward(RewardData rewardData, bool isNextPage)
	{
		_coinReward.gameObject.SetActive(true);
		_coinReward.FindChild("Label").GetComponent<Text>().text = rewardData.value.ToString();
		TweenPunch(_coinReward);
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.REWARD_TYPE_COIN, rewardData.value), 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(string.Format(TextDefs.REWARD_TYPE_COIN, rewardData.value));
		}
	}

	private IEnumerator ShowMortyReward(RewardData rewardData, bool isNextPage)
	{
		_mortyReward.gameObject.SetActive(true);
		Image icon = _mortyReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MortyDefs.AssetID(rewardData.id), "front"));
		icon.SetNativeSize();
		TweenPunch(_mortyReward);
		string dialogueText = string.Format(TextDefs.REWARD_TYPE_MORTY, rewardData.value, MortyDefs.Name(rewardData.id));
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(dialogueText);
		}
	}

	private IEnumerator ShowItemReward(RewardData rewardData, bool isNextPage)
	{
		_itemReward.gameObject.SetActive(true);
		Image icon = _itemReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", ItemDefs.AssetID(rewardData.id), "front"));
		icon.SetNativeSize();
		TweenPunch(_itemReward);
		string dialogueText = string.Format(TextDefs.REWARD_TYPE_ITEM, rewardData.value, ItemDefs.Name(rewardData.id));
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(dialogueText);
		}
	}

	private IEnumerator ShowCouponReward(RewardData rewardData, bool isNextPage)
	{
		_couponReward.gameObject.SetActive(true);
		TweenPunch(_couponReward);
		string dialogueText = string.Format(TextDefs.REWARD_TYPE_ITEM, rewardData.value, ItemDefs.Name(rewardData.id));
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(dialogueText);
		}
	}

	private IEnumerator ShowTrophyReward(RewardData rewardData, bool isNextPage)
	{
		_trophyReward.gameObject.SetActive(true);
		Image icon = _trophyReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Trophies/{0}_large", rewardData.id));
		icon.SetNativeSize();
		string dialogueText = string.Format(TextDefs.REWARD_TYPE_TROPHY, TournamentDefs.TournamentName(rewardData.id));
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(dialogueText);
		}
		TweenPunch(_trophyReward);
	}

	private IEnumerator ShowTournamentTicketReward(RewardData rewardData, bool isNextPage)
	{
		_tournamentTicketReward.gameObject.SetActive(true);
		Image icon = _tournamentTicketReward.FindChild("Icon").GetComponent<Image>();
		icon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", "ItemTournamentTicket", "front"));
		icon.SetNativeSize();
		TweenPunch(_tournamentTicketReward);
		string dialogueText = TextDefs.REWARD_TYPE_TOURNAMENT_TICKET;
		if (isNextPage)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(dialogueText, 2f, isNextPage));
		}
		else
		{
			_dialogue.SetText(dialogueText);
		}
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
				this.TweetEvent(_rewardData);
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
