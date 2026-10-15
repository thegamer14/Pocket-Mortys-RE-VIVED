using DG.Tweening;
using UnityEngine;

public class MPPickupNode : MPNode
{
	private SpriteRenderer _singleSprite;

	private SpriteRenderer _singleShadow;

	private Transform _multipleMeshTrans;

	public MPPickupData data { get; private set; }

	public override void SetUp()
	{
		base.SetUp();
		if (data == null)
		{
			data = new MPPickupData();
		}
		_singleSprite = base.transform.FindChild("Single/Sprite").GetComponent<SpriteRenderer>();
		_singleShadow = base.transform.FindChild("Single/Shadow").GetComponent<SpriteRenderer>();
		_multipleMeshTrans = base.transform.FindChild("Multiple/Mesh");
		_sfxInteract = "Audio/SFX/PickupItem";
		_sfxInteractVolume = 1f;
		StartTween();
	}

	public void SetUpWithData(MPPickupData itemData, eDirection direction)
	{
		data = itemData;
		initialDirection = direction;
		SetUp();
		if (data.rewardDatas.Length > 1)
		{
			base.transform.FindChild("Single").gameObject.SetActive(false);
		}
		else
		{
			base.transform.FindChild("Multiple").gameObject.SetActive(false);
		}
		_singleSprite.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", GetAssetId()));
	}

	protected override string GetAssetId()
	{
		if (data != null && data.rewardDatas.Length == 1)
		{
			return MPItemDefs.AssetID(data.rewardDatas[0].id);
		}
		return null;
	}

	private void StartTween()
	{
		if (data != null && data.rewardDatas.Length == 1)
		{
			_singleSprite.transform.DOLocalMoveY(-0.4f, 0.6f).SetRelative().SetEase(Ease.InQuad)
				.SetLoops(-1, LoopType.Yoyo);
			_singleSprite.transform.DOScaleY(0.95f, 0.6f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
			_singleShadow.transform.DOScale(1.3f, 0.6f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			_multipleMeshTrans.DOLocalRotate(new Vector3(0f, 120f, 0f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
		}
	}
}
