using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : BaseManager<PoolManager>
{
    // 对象池根节点(一级节点)
    private Transform poolRoot;
    // GameObject 对象池字典 key为GameObject对象的名字
    public Dictionary<string, GameObjectPool> gameObjectPoolDic = new();
    // 普通对象池字典 key为Object对象的FullName
    public Dictionary<string, ObjectPool> objectPoolDic = new();
    //初始化对象池系统
    public void Init()
    {
        //创建对象池根节点
        poolRoot = new GameObject("PoolRoot").transform;
    }
    
    #region GameObject对象池初始化相关
    /// <summary>
    /// 初始化GameObject对象池并设置容量
    /// </summary>
    /// <param name="keyName">资源名称</param>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    /// <param name="prefab">填写默认容量时预先放入的对象</param>
    public void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null, int defaultQuantity = 0)
    {
        if (defaultQuantity > maxCapacity && maxCapacity != -1)
        {
            Debug.Log("对象池系统出现错误：默认容量超出最大容量限制");
            return;
        }
        //设置的对象池已经存在
        if (gameObjectPoolDic.TryGetValue(keyName, out GameObjectPool pool))
        {
            //更新容量限制
            pool.maxCapacity = maxCapacity;
            //更新父节点名称
            pool.poolFather.name = $"{keyName}Pool(max={maxCapacity})";
            //底层Queue自动扩容这里不管

            //在指定默认容量和默认对象时才有意义
            if (defaultQuantity > 0)
            {
                if (prefab != null)
                {
                    int nowCapacity = pool.poolQueue.Count;
                    // 生成差值容量个数的物体放入对象池
                    for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                    {
                        GameObject go = GameObject.Instantiate(prefab);
                        go.name = prefab.name;
                        pool.PushObj(go);
                    }
                }
                else Debug.Log("对象池系统出现错误：默认对象未指定");
            }
        }
        //设置的对象池不存在
        else
        {
            //创建对象池
            pool = CreateGameObjectPool(keyName, maxCapacity);

            //在指定默认容量和默认对象时才有意义
            if (defaultQuantity != 0)
            {
                if (prefab != null)
                {
                    // 生成容量个数的物体放入对象池
                    for (int i = 0; i < defaultQuantity; i++)
                    {
                        GameObject go = GameObject.Instantiate(prefab);
                        go.name = prefab.name;
                        pool.PushObj(go);
                    }
                }
                else Debug.Log("对象池系统出现错误：默认对象未指定");
            }
        }
    }
    public void InitGameObjectPool(GameObject prefab, int maxCapacity = -1, int defaultQuantity = 0)
    {
        InitGameObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
    }

    /// <summary>
    /// 创建一个GameObject对象池类并记录到字典中
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="maxCapacity">最大容量 -1代表无限</param>
    /// <returns>GameObject对象池类</returns>
    private GameObjectPool CreateGameObjectPool(string name, int maxCapacity = -1)
    {
        GameObjectPool gameObjectPool = new GameObjectPool(maxCapacity);
        //对拿到的gameObjectPool副本进行初始化（覆盖之前的数据）
        gameObjectPool.Init(name, poolRoot, maxCapacity);
        //如果字典中没有记录该对象池 就记录到字典中
        if (!gameObjectPoolDic.ContainsKey(name))
        {
            gameObjectPoolDic.Add(name, gameObjectPool);
        }
        return gameObjectPool;
    }
    #endregion

    #region GameObject对象池操作相关
    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="keyName">用于字典查询的键名</param>
    /// <param name="parent">设置对象的父物体</param>
    /// <returns>想要获取的对象</returns>
    public GameObject GetGameObject(string keyName, Transform parent = null)
    {
        GameObject obj = null;
        // 检查字典是否有记录该对象池 并且对象池中有对象
        if (gameObjectPoolDic.TryGetValue(keyName, out GameObjectPool pool) && pool.poolQueue.Count > 0)
        {
            obj = pool.GetObj(parent);
            return obj;
        }
        Debug.LogWarning("对象池系统警告：获取对象失败，对象池不存在或对象池中没有可用对象，KeyName：" + keyName);
        return null;
        //TODO:后续可以考虑将最久使用的对象返回 
    }
    //重载：获取对象并返回组件
    public T GetGameObject<T>(string keyName, Transform parent = null) where T : Component
    {
        GameObject go = GetGameObject(keyName, parent);
        if (go != null)
        {
            if (go.TryGetComponent<T>(out T component)) return component;
            else
            {
                Debug.LogWarning($"未找到该组件:{typeof(T).Name}");
                return null;
            }
        } 
        else return null;
    }
    public void PushGameObject(GameObject gameObject)
    {
        PushGameObject(gameObject.name, gameObject);
    }
    public bool PushGameObject(string keyName, GameObject gameObject)
    {
        // 检查字典是否有记录该对象池
        if (gameObjectPoolDic.TryGetValue(keyName, out GameObjectPool pool))
        {
            return pool.PushObj(gameObject);
        }
        else// 不存在则创建对象池后再放入对象
        {
            pool = CreateGameObjectPool(keyName);
            return pool.PushObj(gameObject);
        }
    }
    /// <summary>
    /// 清除指定GameObject对象池
    /// </summary>
    /// <param name="keyName"></param>
    public void ClearGameObjectPool(string keyName)
    {
        if (gameObjectPoolDic.TryGetValue(keyName, out GameObjectPool gameObjectPool))
        {
            gameObjectPool.Destroy();
            gameObjectPoolDic.Remove(keyName);
        }
    }
    /// <summary>
    /// 清除所有GameObject对象池
    /// </summary>
    public void ClearAllGameObjectPool()
    {
        foreach (var gameObjectPool in gameObjectPoolDic.Values)
        {
            gameObjectPool.Destroy();
        }
        gameObjectPoolDic.Clear();
    }
    #endregion

    #region 普通Object对象池初始化相关
    /// <summary>
    /// 初始化对象池并设置容量
    /// </summary>
    /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
    /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
    public void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        //设置的对象池已经存在
        if (objectPoolDic.TryGetValue(keyName, out ObjectPool pool))
        {
            //更新容量限制
            pool.maxCapacity = maxCapacity;
            //底层Queue自动扩容这里不管

            //在指定默认容量时才有意义
            if (defaultQuantity != 0)
            {
                int nowCapacity = pool.poolQueue.Count;
                // 生成差值容量个数的物体放入对象池
                for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                {
                    T obj = new T();
                    PushObject(obj, keyName);
                }
            }
        }
        //设置的对象池不存在
        else
        {
            //创建对象池
            pool = CreateObjectPool(keyName, maxCapacity);

            //在指定默认容量和默认对象时才有意义
            if (defaultQuantity != 0)
            {
                // 生成容量个数的物体放入对象池
                for (int i = 0; i < defaultQuantity; i++)
                {
                    T obj = new T();
                    PushObject(obj, keyName);
                }
            }
        }
    }
    public void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
    {
        InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
    }
    public void InitObjectPool(string keyName, int maxCapacity = -1)
    {
        //设置的对象池已经存在
        if (objectPoolDic.TryGetValue(keyName, out ObjectPool pool))
        {
            //更新容量限制
            pool.maxCapacity = maxCapacity;
            //底层Queue自动扩容这里不管
        }
        //设置的对象池不存在
        else
        {
            //创建对象池
            CreateObjectPool(keyName, maxCapacity);
        }
    }
    public void InitObjectPool(System.Type type, int maxCapacity = -1)
    {
        InitObjectPool(type.FullName, maxCapacity);
    }
    /// <summary>
    /// 创建一条新的对象池数据
    /// </summary>
    private ObjectPool CreateObjectPool(string objectPoolName, int capacity = -1)
    {
        ObjectPool pool = new ObjectPool(capacity);
        //对拿到的pool副本进行初始化（覆盖之前的数据）
        pool.maxCapacity = capacity;
        objectPoolDic.Add(objectPoolName, pool);
        return pool;
    }
    #endregion
    #region 普通Object对象池操作相关
    public object GetObject(string keyName)
    {
        object obj = null;
        if (objectPoolDic.TryGetValue(keyName, out ObjectPool objectPool) && objectPool.poolQueue.Count > 0)
        {
            obj = objectPoolDic[keyName].GetObj();
        }
        return obj;
    }
    public object GetObject(System.Type type)
    {
        return GetObject(type.FullName);
    }

    public T GetObject<T>() where T : class
    {
        return (T)GetObject(typeof(T));
    }

    public T GetObject<T>(string keyName) where T : class
    {
        return (T)GetObject(keyName);
    }

    public bool PushObject(object obj)
    {
        return PushObject(obj, obj.GetType().FullName);
    }
    public bool PushObject(object obj, string keyName)
    {
        if (objectPoolDic.TryGetValue(keyName, out ObjectPool pool) == false)
        {
            pool = CreateObjectPool(keyName);
        }
        return pool.PushObj(obj);
    }

    public void ClearAllObjectPool()
    {
        foreach (var objectPool in objectPoolDic.Values)
        {
            objectPool.Destroy();
        }
        objectPoolDic.Clear();
    }
    public void ClearObjectPool<T>()
    {
        ClearObjectPool(typeof(T).FullName);
    }
    public void ClearObjectPool(System.Type type)
    {
        ClearObjectPool(type.FullName);
    }
    public void ClearObjectPool(string keyName)
    {
        if (objectPoolDic.TryGetValue(keyName, out ObjectPool objectPool))
        {
            objectPool.Destroy();
            objectPoolDic.Remove(keyName);
        }
    }
    #endregion
}
