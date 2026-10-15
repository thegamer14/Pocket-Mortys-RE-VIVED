using DG.Tweening;
using UnityEngine;

public class MPPortalNode : MPNode
{
	private const int _kRotationSpeed1 = 120;

	private const int _kRotationSpeed2 = 160;

	private const int _kRotationSpeed3 = 200;

	private const int _kRotationSpeed4 = 220;

	private Transform _parts;

	private Transform _part1;

	private Transform _part2;

	private Transform _part3;

	private Transform _part4;

	public override void SetUp()
	{
		base.SetUp();
		_parts = base.transform.FindChild("Parts");
		_part1 = _parts.FindChild("Part1");
		_part2 = _parts.FindChild("Part2");
		_part3 = _parts.FindChild("Part3");
		_part4 = _parts.FindChild("Part4");
	}

	private void Update()
	{
		_part1.Rotate(0f, 120f * Time.deltaTime, 0f);
		_part2.Rotate(0f, 160f * Time.deltaTime, 0f);
		_part3.Rotate(0f, 200f * Time.deltaTime, 0f);
		_part4.Rotate(0f, 220f * Time.deltaTime, 0f);
	}

	public void ShakePortal()
	{
		_parts.DOShakeScale(3f, 0.2f, 3, 0f);
	}
}
