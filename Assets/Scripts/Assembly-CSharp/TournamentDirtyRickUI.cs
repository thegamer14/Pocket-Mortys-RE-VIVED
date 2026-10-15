using System.Collections;
using UnityEngine.UI;

public class TournamentDirtyRickUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private CoinBar _coinBar;

	private Text _price;

	private int _currentCost;

	private int _coinValue;

	public event BagUIBtnDelegate BuyEvent;

	public event BagUIBtnDelegate BuyCompleteEvent;

	public event BagUIBtnDelegate BackEvent;

	public event BagUIBtnDelegate GetCoinsEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_coinBar = base.transform.FindChild("MenuBar/MainMenu/CoinBar").GetComponent<CoinBar>();
		_price = base.transform.FindChild("PriceImg/Price").GetComponent<Text>();
	}

	public void SetUp(int currentCost)
	{
		_currentCost = currentCost;
		SetCoins(PlayerDefs.CoinCount);
		SetLabels();
	}

	public override void SetLabels()
	{
		_coinBar.SetLabels();
		_mainMenu.SetBtnTitle(0, TextDefs.TOURNAMENT_DIRTY_RICK_BTN_BUY);
		_dialogue.SetText(string.Format(TextDefs.TOURNAMENT_DIRTY_RICK_DIAL, _currentCost));
		_price.text = _currentCost.ToString();
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_coinBar.GetMoreEvent += CoinBarGetMoreHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_coinBar.GetMoreEvent -= CoinBarGetMoreHandler;
	}

	private void SetCoins(int coins)
	{
		_coinValue = coins;
		_coinBar.SetCount(_coinValue);
	}

	public IEnumerator Buy(bool isSuccess)
	{
		if (isSuccess)
		{
			StartCoroutine(_mainMenu.TweenOut());
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.TOURNAMENT_DIRTY_RICK_DIAL_SUCCESS_1, -1f, true));
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.TOURNAMENT_DIRTY_RICK_DIAL_SUCCESS_2, -1f, true));
			if (this.BuyCompleteEvent != null)
			{
				this.BuyCompleteEvent();
			}
		}
		else
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_DIRTY_RICK_DIAL_FAILED);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BuyEvent != null)
			{
				this.BuyEvent();
			}
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}

	public void CoinBarGetMoreHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.GetCoinsEvent != null)
		{
			this.GetCoinsEvent();
		}
	}
}
