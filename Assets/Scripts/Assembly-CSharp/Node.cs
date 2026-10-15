using UnityEngine;

public class Node : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private eNodeInitialDirection _initialDirection;

	[HideInInspector]
	[SerializeField]
	private bool _applyRotation;

	public eNodeInitialDirection initialDirection
	{
		get
		{
			return _initialDirection;
		}
		private set
		{
			_initialDirection = value;
		}
	}

	public bool applyRotation
	{
		get
		{
			return _applyRotation;
		}
		private set
		{
			_applyRotation = value;
		}
	}

	public virtual void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		SetInitialDirection(initialDirection);
		SetApplyRotation(applyRotation);
	}

	public virtual void SetDirection(eDirection value)
	{
		if (applyRotation)
		{
			base.transform.eulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(value) - 90f, 0f);
		}
		else
		{
			base.transform.eulerAngles = Vector3.zero;
		}
	}

	public virtual void SetInitialDirection(eNodeInitialDirection value)
	{
		initialDirection = value;
		switch (initialDirection)
		{
		case eNodeInitialDirection.FIXED_DOWN:
			SetDirection(eDirection.DOWN);
			break;
		case eNodeInitialDirection.UP:
			base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(eDirection.UP) - 90f, 0f);
			SetDirection(HelperMethods.GetDirectionFromAngle(base.transform.eulerAngles.y + 90f));
			break;
		case eNodeInitialDirection.DOWN:
			base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(eDirection.DOWN) - 90f, 0f);
			SetDirection(HelperMethods.GetDirectionFromAngle(base.transform.eulerAngles.y + 90f));
			break;
		case eNodeInitialDirection.RIGHT:
			base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(eDirection.RIGHT) - 90f, 0f);
			SetDirection(HelperMethods.GetDirectionFromAngle(base.transform.eulerAngles.y + 90f));
			break;
		case eNodeInitialDirection.LEFT:
			base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(eDirection.LEFT) - 90f, 0f);
			SetDirection(HelperMethods.GetDirectionFromAngle(base.transform.eulerAngles.y + 90f));
			break;
		case eNodeInitialDirection.RANDOM:
			SetDirection(HelperMethods.GetRandomDirection());
			break;
		}
	}

	public virtual void SetApplyRotation(bool value)
	{
		applyRotation = value;
		SetInitialDirection(initialDirection);
	}
}
