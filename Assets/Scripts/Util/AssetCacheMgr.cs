using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using Game.Util;
using System.Collections;

public class AssetCacheMgr
{
    private static ILoadAsset m_assetMgr;
    public static ILoadAsset AssetMgr
    {
        get { return m_assetMgr; }
        set { m_assetMgr = value; }
    }
    public struct SResourceRecord
    {
        public int nCreatedTimes;
        public int nDestroyTimes;
    }
    //private static Dictionary<int, KeyValuePair<string, Object>> m_gameObjectDic = new Dictionary<int, KeyValuePair<string, Object>>();
    private static Dictionary<int, String> m_resourceDic = new Dictionary<int, string>();
    private static Dictionary<int, string> m_gameObjectNameMapping = new Dictionary<int, string>();

    public static Dictionary<int, String> ResourceDic
    {
        get { return AssetCacheMgr.m_resourceDic; }
    }

    public static Dictionary<int, string> GameObjectNameMapping
    {
        get { return AssetCacheMgr.m_gameObjectNameMapping; }
    }

    #region GetInstance

    /// <summary>
    /// 加载本地实例。
    /// </summary>
    /// <param name="resourceName">模型名称</param>
    /// <returns>本地实例对象。</returns>
    public static Object GetLocalInstance(string resourceName)
    {
        UnityEngine.Object gameObject = m_assetMgr.LoadLocalInstance(resourceName);
        //m_gameObjectDic.Add(gameObject.GetInstanceID(), new KeyValuePair<string, Object>(resourceName, gameObject));
        return gameObject;
    }

    /// <summary>
    /// 获取资源实例。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="loaded">资源实例加载完成回调</param>
    public static void GetInstance(string resourceName, Action<String, int, Object> loaded)
    {
        m_assetMgr.LoadInstance(resourceName, (pref, guid, go) =>
        {
            if (guid != -1)
                m_gameObjectNameMapping.Add(guid, resourceName);
            if (loaded != null)
                loaded(pref, guid, go);
        });
    }

    /// <summary>
    /// 获取资源实例。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="duration">资源延迟消耗时间（单位：毫秒）</param>
    public static void GetInstance(string resourceName, uint duration)
    {
        GetInstance(resourceName, duration, null);
    }

    /// <summary>
    /// 获取资源实例。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="duration">资源延迟消耗时间（单位：毫秒）</param>
    /// <param name="loaded">资源实例加载完成回调</param>
    public static void GetInstance(string resourceName, uint duration, Action<String, int, Object> loaded)
    {
        m_assetMgr.LoadInstance(resourceName, (pref, guid, go) =>
        {
            if (loaded != null)
                loaded(pref, guid, go);
            TimerHeap.AddTimer(duration, 0, () =>
            {
                ReleaseInstance(go);
            });
        });
    }

    public static void GetInstances(string[] resourcesName, Action<Object[]> loaded)
    {
        if (resourcesName == null || resourcesName.Length == 0)
        {
            if (loaded != null)
                loaded(null);
            return;
        }
        UnityEngine.Object[] objs = new Object[resourcesName.Length];
        for (int i = 0; i < resourcesName.Length; i++)
        {
            int index = i;
            string pref = resourcesName[index];
            GetInstance(pref, (resource, guid, obj) =>
            {
                objs[index] = obj;
                if (index == resourcesName.Length - 1)
                {
                    if (loaded != null)
                        loaded(objs);
                }
            });
        }
    }

    /// <summary>
    /// 获取场景资源实例。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="loaded">资源实例加载完成回调</param>
    public static void GetSceneInstance(string resourceName, Action<String, int, Object> loaded, Action<float> progress)
    {
        m_assetMgr.LoadSceneInstance(resourceName, (pref, guid, go) =>
        {
            if (guid != -1)
                m_gameObjectNameMapping.Add(guid, resourceName);
            if (loaded != null)
                loaded(pref, guid, go);
        }, progress);
    }

    public static void GetInstanceAutoRelease(string resourceName, Action<String, int, Object> loaded)
    {
        GetInstance(resourceName, (resource, guid, go) =>
        {
            UnloadAssetbundle(resourceName);
            if (loaded != null)
                loaded(resource, guid, go);
        });
    }

    public static void GetInstancesAutoRelease(string[] resourcesName, Action<Object[]> loaded)
    {
        GetInstances(resourcesName, (gos) =>
        {
            UnloadAssetbundles(resourcesName);
            if (loaded != null)
                loaded(gos);
        });
    }

    public static void GetUIInstance(string resourceName, Action<String, int, Object> loaded, Action<float> progress = null)
    {
        //GetInstance(resourceName, loaded);
        m_assetMgr.LoadUIAsset(resourceName, (o) =>
        {
            Object go = null;
            var guid = -1;
            if (o)
            {
                go = GameObject.Instantiate(o);
                guid = go.GetInstanceID();
                m_gameObjectNameMapping.Add(guid, resourceName);
            }
            if (loaded != null)
                loaded(resourceName, guid, go);
        }, progress);
    }

    public static void GetNoCacheInstance(string resourceName, Action<String, int, Object> loaded)
    {
        m_assetMgr.LoadInstance(resourceName, loaded);
    }

    public static Object SynGetInstance(string resourceName)
    {
        return m_assetMgr.SynLoadInstance(resourceName);
    }

    public static GameObject SynGetInstance(Object resource)
    {
        return GameObject.Instantiate(resource) as GameObject;
    }

    #endregion

    #region GetResource

    /// <summary>
    /// 获取本地资源。
    /// </summary>
    /// <param name="resourceName">资源名称</param>
    /// <returns>本地资源对象。</returns>
    public static Object GetLocalResource(string resourceName)
    {
        return m_assetMgr.LoadLocalAsset(resourceName);
    }

    /// <summary>
    /// 获取资源对象。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="loaded">资源对象加载完成回调</param>
    public static void GetResource(string resourceName, Action<Object> loaded, Action<float> progress = null)
    {
        m_assetMgr.LoadAsset(resourceName, (obj) =>
        {
            if (obj)
            {
                int id = obj.GetInstanceID();
                if (!m_resourceDic.ContainsKey(id))
                    m_resourceDic.Add(id, resourceName);
            }
            if (loaded != null)
                loaded(obj);
        }, progress);
    }

    public static void GetResources(string[] resourcesName, Action<Object[]> loaded, Action<float> progress = null)
    {
        if (resourcesName == null || resourcesName.Length == 0)
        {
            if (loaded != null)
                loaded(null);
            return;
        }
        UnityEngine.Object[] objs = new Object[resourcesName.Length];
        for (int i = 0; i < resourcesName.Length; i++)
        {
            int index = i;
            string pref = resourcesName[index];
            Action<float> actionProgress = null;
            if (progress != null)
            {
                actionProgress = (pg) => { progress((pg + index) / resourcesName.Length); };
            }
            GetResource(pref, (obj) =>
            {
                objs[index] = obj;
                if (index == resourcesName.Length - 1)
                {
                    if (loaded != null)
                        loaded(objs);
                }
            }, actionProgress);
        }
    }

    public static void GetNoCacheResource(string resourceName, Action<Object> loaded, Action<float> progress = null)
    {
        m_assetMgr.LoadAsset(resourceName, loaded, progress);
    }

    /// <summary>
    /// 获取场景资源对象。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="loaded">资源对象加载完成回调</param>
    public static void GetSceneResource(string resourceName, Action<Object> loaded)
    {
        if (String.IsNullOrEmpty(resourceName))
        {
            if (loaded != null)
                loaded(null);
            return;
        }
        m_assetMgr.LoadSceneAsset(resourceName, (obj) =>
        {
            if (obj)
            {
                int id = obj.GetInstanceID();
                if (!m_resourceDic.ContainsKey(id))
                    m_resourceDic.Add(id, resourceName);
            }
            if (loaded != null)
                loaded(obj);
        });
    }

    public static void GetResourceAutoRelease(string resourceName, Action<Object> loaded)
    {
        m_assetMgr.LoadAsset(resourceName, (obj) =>
        {
            UnloadAssetbundle(resourceName);
            if (obj)
            {
                int id = obj.GetInstanceID();
                if (!m_resourceDic.ContainsKey(id))
                    m_resourceDic.Add(id, resourceName);
            }
            if (loaded != null)
                loaded(obj);
        });
    }

    public static void GetResourcesAutoRelease(string[] resourcesName, Action<Object[]> loaded, Action<float> progress = null)
    {
        GetResources(resourcesName, (gos) =>
        {
            UnloadAssetbundles(resourcesName);
            if (loaded != null)
                loaded(gos);
        }, progress);
    }

    public static void GetUIResource(string resourceName, Action<Object> loaded, Action<float> progress = null)
    {
        m_assetMgr.LoadUIAsset(resourceName, (obj) =>
        {
            if (obj)
            {
                int id = obj.GetInstanceID();
                if (!m_resourceDic.ContainsKey(id))
                    m_resourceDic.Add(id, resourceName);
            }
            if (loaded != null)
                loaded(obj);
        }, progress);
    }

    public static void GetUIResources(string[] resourcesName, Action<Object[]> loaded)
    {
        if (resourcesName == null || resourcesName.Length == 0)
        {
            if (loaded != null)
                loaded(null);
            return;
        }
        UnityEngine.Object[] objs = new Object[resourcesName.Length];
        for (int i = 0; i < resourcesName.Length; i++)
        {
            int index = i;
            string pref = resourcesName[index];
            GetUIResource(pref, (obj) =>
            {
                objs[index] = obj;
                if (index == resourcesName.Length - 1)
                {
                    if (loaded != null)
                        loaded(objs);
                }
            });
        }
    }

    public static void GetSecondUIResource(string resourceName, Action<Object> loaded, Action<float> progress = null)
    {
        m_assetMgr.SecondLoadUIAsset(resourceName, (obj) =>
        {
            if (obj)
            {
                int id = obj.GetInstanceID();
                if (!m_resourceDic.ContainsKey(id))
                    m_resourceDic.Add(id, resourceName);
            }
            if (loaded != null)
                loaded(obj);
        }, progress);
    }

    public static void GetSecondtUIResources(string[] resourcesName, Action<Object[]> loaded)
    {
        if (resourcesName == null || resourcesName.Length == 0)
        {
            if (loaded != null)
                loaded(null);
            return;
        }
        UnityEngine.Object[] objs = new Object[resourcesName.Length];
        for (int i = 0; i < resourcesName.Length; i++)
        {
            int index = i;
            string pref = resourcesName[index];
            GetSecondUIResource(pref, (obj) =>
            {
                objs[index] = obj;
                if (index == resourcesName.Length - 1)
                {
                    if (loaded != null)
                        loaded(objs);
                }
            });
        }
    }

    public static Object SynGetResource(string resourceName)
    {
        return m_assetMgr.SynLoadAsset(resourceName);
    }

    #endregion

    #region ReleaseInstance

    /// <summary>
    /// 释放本地实例资源。
    /// </summary>
    /// <param name="go">本地实例对象。</param>
    public static void ReleaseLocalInstance(Object go)
    {
        if (go)
        {
            GameObject.Destroy(go);
            //int guid = go.GetInstanceID();
            //ReleaseLocalInstance(guid);
        }
    }
    /// <summary>
    /// 销毁资源实例。
    /// </summary>
    /// <param name="go">实例对象</param>
    public static void ReleaseInstance(Object go, bool releaseAsset = true)
    {
        if (go)
        {
            int guid = go.GetInstanceID();
            GameObject.Destroy(go);
            if (m_gameObjectNameMapping.ContainsKey(guid))
            {
                if (releaseAsset)
                    m_assetMgr.Release(m_gameObjectNameMapping[guid]);
                m_gameObjectNameMapping.Remove(guid);
            }
            else
            {
				Debug.Log("go not in mapping: " + go.name);
            }
        }
    }

    public static void UnloadAsset(Object go, bool releaseAsset = true)
    {
        if (go)
        {
            int guid = go.GetInstanceID();
            if (m_gameObjectNameMapping.ContainsKey(guid))
            {
                if (releaseAsset)
                    m_assetMgr.UnloadAsset(m_gameObjectNameMapping[guid]);
                //m_gameObjectNameMapping.Remove(guid);
            }
            else
            {
				Debug.Log("go not in mapping: " + go.name);
            }
        }
    }

    public static void SynReleaseInstance(Object go)
    {
        GameObject.Destroy(go);
    }

    #endregion

    #region ReleaseResource

    /// <summary>
    /// 释放本地资源。
    /// </summary>
    /// <param name="go">本地资源对象。</param>
    public static void ReleaseLocalResource(Object go)
    {
        Resources.UnloadAsset(go);
    }

    /// <summary>
    /// 强行释放资源
    /// </summary>
    /// <param name="resourceName"></param>
    public static void ReleaseResourceImmediate(string resourceName)
    {
        m_assetMgr.Release(resourceName, true);
    }

    /// <summary>
    /// 强行释放资源
    /// </summary>
    /// <param name="obj"></param>
    public static void ReleaseResourceImmediate(Object obj)
    {
        if (!obj)
            return;
        int id = obj.GetInstanceID();
        if (m_resourceDic.ContainsKey(id))
        {
            string prefab = m_resourceDic[id];
            m_assetMgr.Release(prefab, true);
            m_resourceDic.Remove(id);
        }
    }

    public static void ReleaseResourcesImmediate(string[] resourcesName)
    {
        if (resourcesName != null)
            foreach (var item in resourcesName)
            {
                ReleaseResourceImmediate(item);
            }
    }
    /// <summary>
    /// 销毁资源对象。
    /// </summary>
    /// <param name="go">资源对象</param>
    public static void ReleaseResource(Object obj, bool releaseAsset = true)
    {
        if (!obj)
            return;
        int id = obj.GetInstanceID();
        if (m_resourceDic.ContainsKey(id))
        {
            string prefab = m_resourceDic[id];
            if (releaseAsset)
            {
                m_assetMgr.Release(prefab);
                m_resourceDic.Remove(id);
            }
        }
    }
    /// <summary>
    /// 强行释放资源
    /// </summary>
    /// <param name="在资源索引里可以找到"></param>
    public static void ReleaseResource(string resourceName, bool releaseAsset = true)
    {
        if (releaseAsset)
        {
            m_assetMgr.Release(resourceName);
        }
    }

    public static void ReleasesResource(string[] resourcesName, bool releaseAsset = true)
    {
        if (resourcesName != null)
            foreach (var item in resourcesName)
            {
                ReleaseResource(item, releaseAsset);
            }
    }

    public static void UnloadAssetbundle(string resourceName)
    {
        m_assetMgr.Release(resourceName, false);
    }

    public static void UnloadAssetbundles(string[] resourcesName)
    {
        if (resourcesName == null)
            return;
        foreach (var item in resourcesName)
        {
            m_assetMgr.Release(item, false);
        }
    }

    #endregion

    public static void ForceClear(HashSet<string> CityResources)
    {
        m_gameObjectNameMapping.Clear();
        m_assetMgr.ForceClear();
        m_resourceDic.Clear();
    }

    public void ClearLoadAssetTasks()
    {

    }
}

public interface ILoadAsset
{
    void LoadInstance(string prefab, Action<String, int, Object> loaded);
    void LoadInstance(string prefab, Action<String, int, Object> loaded, Action<float> progress);
    Object SynLoadInstance(string prefab);
    void LoadSceneInstance(string prefab, Action<String, int, Object> loaded, Action<float> progress);
    void LoadAsset(string prefab, Action<Object> loaded);
    void LoadAsset(string prefab, Action<Object> loaded, Action<float> progress);
    void LoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress);
    void SecondLoadAsset(string prefab, Action<Object> loaded, Action<float> progress);
    void SecondLoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress);
    Object SynLoadAsset(string prefab);
    void LoadSceneAsset(string prefab, Action<Object> loaded);
    Object LoadLocalInstance(string prefab);
    Object LoadLocalAsset(string prefab);
    void Release(string prefab);
    void Release(string prefab, Boolean releaseAsset);
    void ForceClear();
    void UnloadAsset(string prefab);
}