using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    // 该对象池的父节点(二级节点)
    public Transform poolFather;
    // 对象容器
    public Queue<GameObject> poolQueue;
    // 初始容量 -1代表无限
    public int maxCapacity = -1;
    public GameObjectPool(int capacity = -1)
    {
        if (capacity == -1) poolQueue = new Queue<GameObject>();
        else poolQueue = new Queue<GameObject>(capacity);
    }
    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="poolRootObj">对象池总节点（一级节点）</param>
    /// <param name="capacity">容量</param>
    public void Init(string name, Transform poolRoot, int capacity = -1)
    {
        // 创建父节点 并设置到对象池根节点下方
        if (poolFather == null) poolFather = new GameObject($"{name}Pool(max={capacity})").transform;
        if(poolFather.parent == null) poolFather.SetParent(poolRoot);
        maxCapacity = capacity;
    }
    /// <summary>
    /// 将对象放进对象池
    /// </summary>
    public bool PushObj(GameObject obj)
    {
        // 检测是不是超过容量
        if (maxCapacity != -1 && poolQueue.Count >= maxCapacity)
        {
            GameObject.Destroy(obj);
            return false;
        }
        // 对象进容器
        poolQueue.Enqueue(obj);
        // 设置父物体
        obj.transform.SetParent(poolFather);
        // 设置隐藏
        obj.SetActive(false);
        return true;
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="parent">设置该对象的父物体</param>
    /// <returns>要获取的对象</returns>
    public GameObject GetObj(Transform parent = null)
    {
        if (poolQueue.Count == 0) return null;
        GameObject obj = poolQueue.Dequeue();
        // 显示对象
        obj.SetActive(true);
        // 设置父物体 为null时不报错
        obj.transform.SetParent(parent);
        //移动到当前场景
        if (parent == null) UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        return obj;
    }
    /// <summary>
    /// 销毁对象池
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(poolFather.gameObject);
        maxCapacity = -1;
        poolQueue.Clear();
        poolFather = null;
    } 
}
