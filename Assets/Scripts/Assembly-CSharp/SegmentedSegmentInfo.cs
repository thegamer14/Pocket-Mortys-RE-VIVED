public class SegmentedSegmentInfo
{
	public string name;

	public eSegmentType type;

	public eSegmentJointType jointType;

	public float rotation;

	public SegmentedSegmentInfo(string name, eSegmentType type, eSegmentJointType jointType, float rotation)
	{
		this.name = name;
		this.type = type;
		this.jointType = jointType;
		this.rotation = rotation;
	}

	public SegmentedSegmentInfo(eSegmentType type, eSegmentJointType jointType, float rotation)
	{
		name = null;
		this.type = type;
		this.jointType = jointType;
		this.rotation = rotation;
	}
}
