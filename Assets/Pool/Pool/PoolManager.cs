using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PoolManager
{
    private static readonly PoolManager _instance = new PoolManager();
    public static PoolManager instance { get { return _instance; } }


    private Dictionary<string, GameObject> dicPrefab = new Dictionary<string, GameObject>();//通过poolName类辨别，判断是否已经设置过prefab
    private Dictionary<string, Pool> dicPool = new Dictionary<string, Pool>();//通过poolName类辨别，pool池子
    private Dictionary<Type, string> dicTypePoolName = new Dictionary<Type, string>();//传入继承PoolObject的类 返回poolName


    private PoolManager()
    {

    }

    /// <summary>
    /// 创建Pool并设置Pool的预制体（安全的，重复的key不添加,poolName不写为Pool_+prefab.name）
    /// </summary>
    public Pool CreatPool(GameObject prefab, bool isDontDestroyOnLoad = false, string poolName = null, int maxCapacity=-1)
    {
        if (poolName == null)
            poolName = string.Format("Pool_{0}", prefab.name);
        if (dicPrefab.ContainsKey(poolName))
            return dicPool.TryGet(poolName);

        //设置Pool的预制体
        dicPrefab.Add(poolName, prefab);

        PoolObject poolObject = prefab.GetComponent<PoolObject>();
        bool isInheritPoolObject = poolObject ? true : false;

        //创建Pool
        GameObject poolGo = new GameObject(poolName);
        Pool pool = poolGo.AddComponent<Pool>();
        pool.SetPoolName(poolName).SetPoolPrefab(prefab).SetDontDestroyOnLoad(isDontDestroyOnLoad).SetInheritPoolObject(isInheritPoolObject).SetPoolMaxCapacity(maxCapacity).StartPool();
        dicPool.Add(poolName, pool);

        //设置PoolObject
        if (isInheritPoolObject)
        {
            poolObject.poolName = poolName;
            poolObject.pool = pool;
            //Debug.Log("poolName" + poolObject.poolName + poolObject.pool.ToString());
            dicTypePoolName.Add(poolObject.GetType(), poolName);//经测试，这里存储的poolObject.GetType()为子类的Type
        }

        return pool;
    }


    public Pool GetPool<T>() where T : PoolObject
    {
        return GetPool(dicTypePoolName.TryGet(typeof(T)));
    }

    public Pool GetPool(string poolName)
    {
        return dicPool.TryGet(poolName); //未找到该poolName的Pool时 需要先CreatPool
    }


    public void RemovePoolByPoolName(string poolName)
    {
        if (dicPool.ContainsKey(poolName))
        {
            //获取放最前面 
            Pool pool = dicPool[poolName];
            Type type = null;

            //移除pool、prefab 即通过poolName注册的
            dicPrefab.Remove(poolName);
            dicPool.Remove(poolName);

            //移除Type和poolName键值对
            var enumerator = dicTypePoolName.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //Debug.Log("PoolObject池子中 Key：" + enumerator.Current.Key + " Value：" + enumerator.Current.Value.GetType());
                if (enumerator.Current.Value == poolName)
                {
                    type = enumerator.Current.Key;
                }
            }

            if (type != null)
                dicTypePoolName.Remove(type);

            Debug.Log("切换或销毁场景，清空该poolName在内存中的pool池子和prefab池子：" + poolName);
        }
        else
        {
            Debug.Log("还未生成过" + poolName + "就调用了RemovePool！");
        }
    }


}

