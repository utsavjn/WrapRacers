using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Game.Util;

public class LoadResources : MonoBehaviour, ILoadAsset
{
	private Dictionary<String, String> m_filesDic = new Dictionary<string, string>();
	private Dictionary<string, ResourceInfo> m_resourcesDic = new Dictionary<string, ResourceInfo>();
	private String m_resourcePath;
	private static LoadResources m_instance;
	public static LoadResources Instance
	{
		get { return m_instance; }
	}

	void Awake()
	{
		DontDestroyOnLoad(this);
		m_instance = this;
		AssetCacheMgr.AssetMgr = this;
		m_instance.m_filesDic.Clear();

		if (Application.platform == RuntimePlatform.WindowsPlayer)
			m_instance.LoadPathFile ();
		else {
			m_instance.m_resourcePath = Application.dataPath + "/Resources/";
			m_instance.GetFileInfo (new DirectoryInfo (m_instance.m_resourcePath));
			m_instance.BuildPathFile ();
		}
	}

	public void LoadInstance(string prefab, Action<string, int, Object> loaded)
	{
		LoadInstance(prefab, loaded, null);
	}

	public void LoadInstance(string prefab, Action<string, int, Object> loaded, Action<float> progress)
	{
		if (prefab.EndsWith(".unity"))
		{
			if (loaded != null)
				loaded(prefab, 0, null);
		}
		else
		{
			LoadAsset(prefab, (obj) =>
				{
					Object go = null;
					int guid = -1;
					if (obj != null)
					{
						go = Instantiate(obj);
						if (go != null)
						{
							GameObject gameObject = go as GameObject;
							if (gameObject != null)
							{
								guid = go.GetInstanceID();
							}
							else
							{
								Debug.LogError("Convert fail, gameObject is null : prefab = " + prefab);
							}
						}
						else
						{
							Debug.LogError("Instantiate fail, object is null : prefab = " + prefab);
						}
					}
					else
					{
						Debug.LogError("Null object: " + prefab);
						go = null;
						guid = -1;
					}
					if (loaded != null)
						loaded(prefab, guid, go);
				});
		}
	}

	public void LoadSceneInstance(string prefab, Action<string, int, Object> loaded, Action<float> progress)
	{
		SynLoadInstance(prefab, loaded);
	}

	public void Release(string prefab)
	{
		Release(prefab, false);
	}

	public void Release(string prefab, bool releaseAsset)
	{
		if (String.IsNullOrEmpty(prefab))
		{
			Debug.LogError("null prefab");
			return;
		}
		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(prefab, out resourceInfo);
	}

	public void ForceClear()
	{
		foreach (KeyValuePair<string, ResourceInfo> v in m_resourcesDic)
		{
			v.Value.GameObject = null;
		}
		m_resourcesDic.Clear();
	}

	public void ForceRelease(string strName)
	{
		if (String.IsNullOrEmpty(strName))
		{
			return;
		}
		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(strName, out resourceInfo);
		if (flag)
		{
			if (resourceInfo.IsGameObjectLoaded)
			{
				m_resourcesDic.Remove(strName);
			}
		}
		else
		{
			Debug.LogError(strName + " is not under my control");
		}
	}

	public void LoadAsset(string prefab, Action<Object> loaded)
	{
		LoadAsset(prefab, loaded, null);
	}

	public void LoadAsset(string prefab, Action<Object> loaded, Action<float> progress)
	{
		if (string.IsNullOrEmpty(prefab))
		{
			Debug.LogError("null prefab name.");
			if (loaded != null)
				loaded(null);
			return;
		}

		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(prefab, out resourceInfo);

		if (!flag)
		{
			resourceInfo = new ResourceInfo();
			if (m_filesDic.ContainsKey(prefab))
				resourceInfo.Path = m_filesDic[prefab];
			else
			{
				Debug.LogError("prefab not exist: " + prefab);
				if (loaded != null)
					loaded(null);
				return;
			}

			m_resourcesDic.Add(prefab, resourceInfo);
		}

		StartCoroutine(resourceInfo.GetGameObject(loaded));
	}

	public void LoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress)
	{
		LoadAsset(prefab, loaded);
	}

	public void SecondLoadAsset(string prefab, Action<Object> loaded, Action<float> progress)
	{
		LoadAsset(prefab, loaded, progress);
	}

	public void SecondLoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress)
	{
		LoadUIAsset(prefab, loaded, progress);
	}

	public void LoadSceneAsset(string prefab, Action<Object> loaded)
	{
		SynLoadAsset(prefab, loaded);
	}

	public Object LoadLocalInstance(string prefab)
	{
		return Instantiate(LoadLocalAsset(prefab));
	}

	public Object LoadLocalAsset(string prefab)
	{
		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(prefab, out resourceInfo);
		if (!flag)
		{
			resourceInfo = new ResourceInfo();
			resourceInfo.Path = prefab;
			m_resourcesDic.Add(prefab, resourceInfo);
		}

		return resourceInfo.GameObject;
	}

	public void SynLoadInstance(string prefab, Action<string, int, Object> loaded)
	{
		if (prefab.EndsWith(".unity"))
		{
			if (loaded != null)
				loaded(prefab, 0, null);
		}
		else
		{
			SynLoadAsset(prefab, (obj) =>
				{
					Object go;
					int guid;
					if (obj != null)
					{
						go = Instantiate(obj);
						guid = go.GetInstanceID();
					}
					else
					{
						Debug.LogError("Null object: " + prefab);
						go = null;
						guid = -1;
					}
					if (loaded != null)
						loaded(prefab, guid, go);
				});
		}
	}

	public void SynLoadAsset(string prefab, Action<Object> loaded)
	{
		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(prefab, out resourceInfo);
		if (!flag)
		{
			resourceInfo = new ResourceInfo();
			if (m_filesDic.ContainsKey(prefab))
				resourceInfo.Path = m_filesDic[prefab];
			else
			{
				Debug.LogError("prefab not exist: " + prefab);
				if (loaded != null)
					loaded(null);
				return;
			}

			m_resourcesDic.Add(prefab, resourceInfo);
		}

		if (loaded != null)
			loaded(resourceInfo.GameObject);
	}

	private void BuildPathFile()
	{
		var root = new System.Security.SecurityElement("root");
		foreach (var item in m_instance.m_filesDic)
		{
			root.AddChild(new System.Security.SecurityElement("k", item.Key));
			root.AddChild(new System.Security.SecurityElement("v", item.Value));
		}
		XMLParser.SaveText(m_instance.m_resourcePath + "resourceInfo.xml", root.ToString());
	}

	private void LoadPathFile()
	{
		var info = Resources.Load("resourceInfo") as TextAsset;
		var xml = XMLParser.LoadXML(info.text);
		for (int i = 0; i < xml.Children.Count; i += 2)
		{
			var key = xml.Children[i] as System.Security.SecurityElement;
			var value = xml.Children[i + 1] as System.Security.SecurityElement;
			m_instance.m_filesDic.Add(key.Text, value.Text);
		}
	}

	private void GetFileInfo(DirectoryInfo info)
	{
		var ds = info.GetDirectories().Where(t => t.Name.StartsWith(".") == false);
		var fs = info.GetFiles();
		foreach (var item in ds)
		{
			GetFileInfo(item);
		}
		foreach (var item in fs)
		{
			var key = item.Name;

			if (!IsResource(item.FullName.Replace('\\', '/')))
				continue;
			if (!m_instance.m_filesDic.ContainsKey(key))
			{
				var value = item.FullName.Replace('\\', '/').Replace(m_resourcePath, "");

				if (value.Equals ("unity default resources") || value.Equals("unity_builtin_extra"))
					continue;

				value = Utils.GetFilePathWithoutExtention(value);
				m_instance.m_filesDic.Add(key, value);
			}
		}
	}

	private bool IsResource(string resource)
	{
		var filter = new List<String>() { ".meta", ".xml", ".dds", ".unity" };
		foreach (var item in filter)
		{
			if (resource.EndsWith(item, StringComparison.OrdinalIgnoreCase))
				return false;
		}
		if (resource.EndsWith(".png", StringComparison.OrdinalIgnoreCase) && !resource.Contains("/Images/") && !resource.Contains("/Textures/"))
			return false;
		return true;
	}

	public Object SynLoadInstance(string prefab)
	{
		var obj = SynLoadAsset(prefab);
		if (obj)
			return GameObject.Instantiate(obj);
		else
			return null;
	}

	public Object SynLoadAsset(string prefab)
	{
		if (string.IsNullOrEmpty(prefab))
		{
			Debug.LogError("null prefab name.");
			return null;
		}
		ResourceInfo resourceInfo;
		var flag = m_resourcesDic.TryGetValue(prefab, out resourceInfo);
		if (!flag)
		{
			resourceInfo = new ResourceInfo();
			if (m_filesDic.ContainsKey(prefab))
				resourceInfo.Path = m_filesDic[prefab];
			else
			{
				Debug.LogError("prefab not exist: " + prefab);
				return null;
			}

			m_resourcesDic.Add(prefab, resourceInfo);
		}

		return resourceInfo.GameObject;
	}

	public void UnloadAsset(string prefab)
	{
	}
}

public class ResourceInfo
{
	private Object m_gameObject;
	public string Path { get; set; }
	public Boolean IsGameObjectLoaded { get { return m_gameObject != null; } }
	public Object GameObject
	{
		get
		{
			if (m_gameObject == null)
			{
				try
				{
					m_gameObject = Resources.Load(Path);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			return m_gameObject;
		}
		set
		{
			m_gameObject = value;
		}
	}

	public System.Collections.IEnumerator GetGameObject(Action<Object> action)
	{
		while (true)
		{
			if (!m_gameObject)
			{
				yield return null;
				try
				{
					m_gameObject = Resources.Load(Path);
					if (m_gameObject == null)
						Debug.LogWarning("null gameobject: " + Path);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				yield return null;
			}

			if (action != null)
				action(m_gameObject);
			yield break;
		}
	}
}