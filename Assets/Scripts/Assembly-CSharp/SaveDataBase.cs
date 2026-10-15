using System;
using System.Runtime.Serialization;

[Serializable]
public class SaveDataBase : ISerializable
{
	private const string _kVersionIDKey = "VersionIDKey";

	public int versionID;

	protected virtual int _targetVersionID
	{
		get
		{
			return 1;
		}
	}

	public SaveDataBase()
	{
		SetDefaults();
	}

	public SaveDataBase(SerializationInfo info, StreamingContext context)
	{
		SetDefaults();
		DeserializeValues(info, context);
		UpdateToLatestVersion(info);
	}

	protected virtual void SetDefaults()
	{
		versionID = _targetVersionID;
	}

	protected virtual void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("VersionIDKey", versionID, typeof(int));
	}

	protected virtual void DeserializeValues(SerializationInfo info, StreamingContext context)
	{
		versionID = (int)info.GetValue("VersionIDKey", typeof(int));
	}

	private void UpdateToLatestVersion(SerializationInfo info)
	{
		while (versionID < _targetVersionID)
		{
			switch (versionID)
			{
			case 1:
				UpdateToVersion2(info);
				break;
			case 2:
				UpdateToVersion3(info);
				break;
			case 3:
				UpdateToVersion4(info);
				break;
			case 4:
				UpdateToVersion5(info);
				break;
			case 5:
				UpdateToVersion6(info);
				break;
			case 6:
				UpdateToVersion7(info);
				break;
			case 7:
				UpdateToVersion8(info);
				break;
			case 8:
				UpdateToVersion9(info);
				break;
			case 9:
				UpdateToVersion10(info);
				break;
			}
			versionID++;
		}
	}

	protected virtual void UpdateToVersion2(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion3(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion4(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion5(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion6(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion7(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion8(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion9(SerializationInfo info)
	{
	}

	protected virtual void UpdateToVersion10(SerializationInfo info)
	{
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializeValues(info, context);
	}
}
