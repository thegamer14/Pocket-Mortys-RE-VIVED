using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPDeckCell : CellBase
{
	public int deckIndex;

	private Text _deckTitleLabel;

	private Text _deckLabel;

	private GameObject _activeTab;

	private Text _activeLabel;

	protected override void Awake()
	{
		base.Awake();
		_deckTitleLabel = base.transform.Find("DeckTab/DeckTitleLabel").GetComponent<Text>();
		_deckLabel = base.transform.Find("DeckTab/DeckLabel").GetComponent<Text>();
		_activeTab = base.transform.Find("ActiveTab").gameObject;
		_activeLabel = base.transform.Find("ActiveTab/Label").GetComponent<Text>();
	}

	public void SetUp(int deckIndex)
	{
		base.SetUp(deckIndex.ToString());
		this.deckIndex = deckIndex;
		AddMortys();
		_deckTitleLabel.text = TextDefs.MORTY_STORAGE_DECKS_DECK;
		_activeLabel.text = TextDefs.MORTY_STORAGE_DECKS_ACTIVE;
		_deckLabel.text = ExtensionMethods.IntToLetters(deckIndex);
		SetActiveTab();
	}

	public void SetActiveTab()
	{
		_activeTab.SetActive(deckIndex == App.net.player.data.activeDeckIndex);
	}

	private void AddMortys()
	{
		List<MPOpponentMortyData> deckMortyDatas = App.net.player.data.GetDeckMortyDatas(deckIndex);
		for (int i = 0; i < 5; i++)
		{
			MPOpponentMortyData up = ((i >= deckMortyDatas.Count) ? null : deckMortyDatas[i]);
			base.transform.FindChild("Mortys/Morty" + i).GetComponent<MPDeckCellMorty>().SetUp(up);
		}
	}
}
