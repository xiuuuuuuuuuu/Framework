using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// 继承 继承Mono的自动挂载式的单例模式基类 的子类
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{   
    //声明一个事件用于存储“不继承Mono的Updata函数”
    private event UnityAction updateEvent;
    //声明一个事件用于存储“不继承Mono的FixedUpdata函数”
    private event UnityAction fixedUpdateEvent;
    //声明一个事件用于存储“不继承Mono的LateUpdata函数”
    private event UnityAction lateUpdateEvent;

    /// <summary>
    /// 添加Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }

    /// <summary>
    /// 移除Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }

    /// <summary>
    /// 添加FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent += updateFun;
    }
    /// <summary>
    /// 移除FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent -= updateFun;
    }

    /// <summary>
    /// 添加LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent -= updateFun;
    }


    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}
