using System.Collections.Generic;

public static class ConvertExt
{
	public static Dictionary<string, object> ToDictionary(object value)
	{
		try
		{
			return (Dictionary<string, object>)value;
		}
		catch
		{
			if (value == null)
			{
				return null;
			}
		}
		return null;
	}

	public static Dictionary<string, object>[] ToDictionaryArray(object value)
	{
		try
		{
			return (Dictionary<string, object>[])value;
		}
		catch
		{
			if (value == null)
			{
				return null;
			}
			if (value.GetType() == typeof(object[]))
			{
				return new Dictionary<string, object>[0];
			}
		}
		return null;
	}
}
