using UnityEngine;

public class TiledTextureScroll : MonoBehaviour
{
	private Material _material;

	private Vector2 _offset;

	public float _xScrollSpeed = 1f;

	public float _yScrollSpeed = 1f;

	private void Awake()
	{
		_material = GetComponent<MeshRenderer>().material;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		_offset = new Vector2(MathExt.Wrap(_offset.x + _xScrollSpeed * Time.deltaTime, 0f, 1f), MathExt.Wrap(_offset.y + _yScrollSpeed * Time.deltaTime, 0f, 1f));
		_material.mainTextureOffset = _offset;
	}
}
