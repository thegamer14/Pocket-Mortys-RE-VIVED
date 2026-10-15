using UnityEngine;
using UnityEngine.UI;

public class MPDeckCellMorty : MonoBehaviour
{
	private MPOpponentMortyData _mortyData;

	private GameObject _emptyObj;

	private GameObject _populatedObj;

	private MortyElementImage _elementImg;

	private Text _outLabel;

	private Image _mortyIconImage;

	private MortyLevelBar _levelBar;

	private void Awake()
	{
		_emptyObj = base.transform.FindChild("Empty").gameObject;
		_populatedObj = base.transform.FindChild("Populated").gameObject;
		_elementImg = _populatedObj.transform.FindChild("Icons/Element").GetComponent<MortyElementImage>();
		_outLabel = _populatedObj.transform.FindChild("Icons/OutLabel").GetComponent<Text>();
		_mortyIconImage = _populatedObj.transform.FindChild("MortyIcon").GetComponent<Image>();
		_levelBar = _populatedObj.transform.FindChild("LevelBar").GetComponent<MortyLevelBar>();
		AddEvents();
	}

	public void SetUp(MPOpponentMortyData mortyData)
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
		_elementImg.Set(MPMortyDefs.ElementType(_mortyData.id));
		_mortyIconImage.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(_mortyData.id)));
		_mortyIconImage.SetNativeSize();
		_levelBar.SetLevel(_mortyData.level);
		_outLabel.gameObject.SetActive(App.net.player.data.IsMortyInParty(_mortyData));
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	protected void AddEvents()
	{
		App.net.player.MortiesChangedEvent += NetPlayerMortiesChangedHandler;
	}

	protected void RemoveEvents()
	{
		App.net.player.MortiesChangedEvent -= NetPlayerMortiesChangedHandler;
	}

	protected void NetPlayerMortiesChangedHandler()
	{
		if (_mortyData != null)
		{
			_outLabel.gameObject.SetActive(App.net.player.data.IsMortyInParty(_mortyData));
		}
	}
}
