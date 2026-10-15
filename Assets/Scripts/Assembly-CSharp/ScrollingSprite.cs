using UnityEngine;

public class ScrollingSprite : MonoBehaviour
{
	public Vector3 speed;

	public Vector3 speedRan;

	public Vector3 limitsMin;

	public Vector3 limitsMax;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
		speedRan = new Vector3(Random.value * speedRan.x, Random.value * speedRan.y, Random.value * speedRan.z);
	}

	private void Update()
	{
		_transform.localPosition += speed * Time.deltaTime;
		if (_transform.localPosition.x < limitsMin.x)
		{
			_transform.localPosition = new Vector3(limitsMax.x, _transform.localPosition.y, _transform.localPosition.z);
		}
		else if (_transform.localPosition.x > limitsMax.x)
		{
			_transform.localPosition = new Vector3(limitsMin.x, _transform.localPosition.y, _transform.localPosition.z);
		}
		if (_transform.localPosition.y < limitsMin.y)
		{
			_transform.localPosition = new Vector3(_transform.localPosition.x, limitsMax.y, _transform.localPosition.z);
		}
		else if (_transform.localPosition.y > limitsMax.y)
		{
			_transform.localPosition = new Vector3(_transform.localPosition.x, limitsMin.y, _transform.localPosition.z);
		}
		if (_transform.localPosition.z < limitsMin.z)
		{
			_transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, limitsMax.z);
		}
		else if (_transform.localPosition.z > limitsMax.z)
		{
			_transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, limitsMin.z);
		}
	}
}
