using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCellMorty : MonoBehaviour
{
	private MortyData _mortyData;

	private GameObject _emptyObj;

	private GameObject _populatedObj;

	private MortyElementImage _elementImg;

	private GameObject _bootcampIconObj;

	private Text _outLabel;

	private Image _mortyIconImage;

	private MortyLevelBar _levelBar;

	private void Awake()
	{
		_emptyObj = base.transform.FindChild("Empty").gameObject;
		_populatedObj = base.transform.FindChild("Populated").gameObject;
		_elementImg = _populatedObj.transform.FindChild("Icons/Element").GetComponent<MortyElementImage>();
		_bootcampIconObj = _populatedObj.transform.FindChild("Icons/Bootcamp").gameObject;
		_outLabel = _populatedObj.transform.FindChild("Icons/OutLabel").GetComponent<Text>();
		_mortyIconImage = _populatedObj.transform.FindChild("MortyIcon").GetComponent<Image>();
		_levelBar = _populatedObj.transform.FindChild("LevelBar").GetComponent<MortyLevelBar>();
		AddEvents();
	}

	public void SetUp(MortyData mortyData)
	{
		_mortyData = mortyData;
		if (_mortyData == null)
		{
			_emptyObj.SetActive(true);
			_populatedObj.SetActive(false);
			return;
		}
		_emptyObj.SetActive(false);
		_populatedObj.SetActive(true);
		_elementImg.Set(MortyDefs.ElementType(_mortyData.id));
		_mortyIconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MortyDefs.AssetID(_mortyData.id)));
		_mortyIconImage.SetNativeSize();
		_levelBar.SetLevel(_mortyData.level);
		_bootcampIconObj.SetActive(BootcampDefs.IsMortyOnBootcamp(_mortyData));
		_outLabel.gameObject.SetActive(PlayerDefs.IsMoryInParty(_mortyData));
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	protected void AddEvents()
	{
		BootcampDefs.BootcampStateChangedEvent += BootampStateChangedHandler;
		PlayerDefs.PartyMortiesChangedEvent += PartyMortiesChangedHandler;
	}

	protected void RemoveEvents()
	{
		BootcampDefs.BootcampStateChangedEvent -= BootampStateChangedHandler;
		PlayerDefs.PartyMortiesChangedEvent -= PartyMortiesChangedHandler;
	}

	protected void PartyMortiesChangedHandler(List<MortyData> partyMorties)
	{
		if (_mortyData != null)
		{
			_outLabel.gameObject.SetActive(partyMorties.Contains(_mortyData));
		}
	}

	protected void BootampStateChangedHandler(int bootcampIndex, eBootcampState state)
	{
		if (_mortyData != null)
		{
			_bootcampIconObj.SetActive(BootcampDefs.IsMortyOnBootcamp(_mortyData));
		}
	}
}
