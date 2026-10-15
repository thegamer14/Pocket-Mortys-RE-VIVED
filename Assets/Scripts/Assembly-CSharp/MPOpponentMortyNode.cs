using UnityEngine;

public class MPOpponentMortyNode : MPNode
{
	protected MPNodeCharacterMoveModule _characterMoveModule;

	private GameObject faintedObj;

	public MPOpponentMortyData data { get; private set; }

	public override void SetUp()
	{
		base.SetUp();
		if (data == null)
		{
			data = new MPOpponentMortyData();
		}
		string assetId = GetAssetId();
		_characterMoveModule = SetUpCharacterMoveModule(assetId);
		faintedObj = base.transform.FindChild("Status/Fainted").gameObject;
		RefreshStatus();
	}

	public void SetUpWithData(MPOpponentMortyData mortyData, eDirection direction, float characterZDepth)
	{
		data = mortyData;
		initialDirection = direction;
		SetUp();
		_characterMoveModule.SetZDepth(characterZDepth);
	}

	public void RefreshStatus()
	{
		faintedObj.SetActive(data.isFainted);
	}

	protected override string GetAssetId()
	{
		return (data == null) ? null : MPMortyDefs.AssetID(data.id);
	}

	public override void SetEditorValues(eDirection initialDirection)
	{
		MPMortyDefs.SetUp();
		base.SetEditorValues(initialDirection);
	}
}
