using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // 对象容器
    public Queue<object> poolQueue;
    // 容量限制 -1代表无限
    public int maxCapacity = -1;
    public ObjectPool(int capacity = -1)
    {
        maxCapacity = capacity;
        if (maxCapacity == -1) poolQueue = new Queue<object>();
        else poolQueue = new Queue<object>(capacity);
    }
    /// <summary>
    /// 将对象放进对象池
    /// </summary>
    public bool PushObj(object obj)
    {
        // 检测是不是超过容量 超过了就不放进去了
        if (maxCapacity != -1 && poolQueue.Count >= maxCapacity) return false;
        poolQueue.Enqueue(obj);
        return true;
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <returns></returns>
    public object GetObj() => poolQueue.Dequeue();

    public void Destroy()
    {
        poolQueue.Clear();
        maxCapacity = -1;
    }    
}
