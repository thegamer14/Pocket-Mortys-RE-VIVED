using UnityEngine;

public class LocalizedMaterialList : MonoBehaviour
{
	private Texture[] _enTextures;

	public Material[] materials;

	public void LoadComponents()
	{
		_enTextures = new Texture[materials.Length];
		for (int i = 0; i < materials.Length; i++)
		{
			_enTextures[i] = materials[i].mainTexture;
		}
	}

	public void Localize()
	{
		for (int i = 0; i < materials.Length; i++)
		{
			Material material = materials[i];
			Texture texture = _enTextures[i];
			Texture texture2 = Resources.Load<Texture>(LocalizationDefs.GetAssetTextureResource(texture.name));
			if (texture2 != null)
			{
				material.mainTexture = texture2;
			}
			else
			{
				material.mainTexture = texture;
			}
		}
	}
}
