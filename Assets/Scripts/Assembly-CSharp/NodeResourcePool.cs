using System.Collections.Generic;
using UnityEngine;

public class NodeResourcePool
{
	private Dictionary<string, Object> _resourceDict;

	private string _path;

	public NodeResourcePool(string path, string setID)
	{
		_resourceDict = new Dictionary<string, Object>();
		_path = path;
		LoadResourcesForSet(setID);
	}

	public void LoadResourcesForSet(string setID)
	{
		_resourceDict.Clear();
		List<Object> list = new List<Object>();
		list.AddRange(Resources.LoadAll(string.Format(_path, "Default")));
		string path = string.Format(_path, setID);
		list.AddRange(Resources.LoadAll(path));
		for (int i = 0; i < list.Count; i++)
		{
			_resourceDict.Add(list[i].name, list[i]);
		}
	}

	public Object Get(string resourceName)
	{
		if (_resourceDict.ContainsKey(resourceName))
		{
			return _resourceDict[resourceName];
		}
		return null;
	}
}
