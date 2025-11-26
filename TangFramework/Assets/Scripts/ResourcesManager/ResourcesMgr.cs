using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源信息基类
/// 主要用于里式替换原则 父类容器装子类对象
/// </summary>
public abstract class ResInfoBase
{
    // 资源引用计数
    public int refCount;
}
/// <summary>
/// 资源信息类
/// 用于存储资源的相关信息，异步加载委托信息 异步加载协程等
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    //资源
    public T asset;
    //主要用于异步加载结束后 传递资源到外部的委托
    public UnityAction<T> callback;
    //用于存储异步加载时开启的协程
    public Coroutine coroutine;
    //引用计数为0时是否马上移除
    public bool isDel;

    #region 计数操作
    public void AddRefCount() => ++refCount;
    public void SubRefCount()
    {
        --refCount;
        if (refCount < 0) Debug.LogWarning("资源引用计数小于0,请检查使用和卸载是否配对执行");
    }
    #endregion
}

/// <summary>
/// 资源管理器
/// </summary>
public class ResourcesMgr : BaseManager<ResourcesMgr>
{
    //用于存储加载过或者加载中的资源的容器
    private Dictionary<string, ResInfoBase> resDic = new();
    
    #region 同步加载资源相关
    //同步加载资源
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = $"{typeof(T).Name}_{path}";
        ResInfo<T> resInfo;
        //如果字典中没有该资源
        if (!resDic.ContainsKey(resName))
        {
            //直接同步加载 并记录资源信息到字典中 方便下次直接获取
            T res = Resources.Load<T>(path);
            resInfo = new() { asset = res };
            //引用计数增加
            resInfo.AddRefCount();
            //将资源记录添加到字典中
            resDic.Add(resName, resInfo);
            return res;
        }
        else
        {
            //如果字典中有该资源 则直接从字典中取出资源信息
            resInfo = resDic[resName] as ResInfo<T>;
            if(resInfo == null)
            {
                Debug.LogError($"资源管理器中存储的资源类型与请求的资源类型不匹配，请检查是否混用Load和LoadAsync去加载同类型同名资源，路径：{path}");
                return null;
            }
            //引用计数增加
            resInfo.AddRefCount();
            //如果资源已经加载完了 则直接返回
            if (resInfo.asset != null) return resInfo.asset;
            else
            {
                //如果资源还没有加载完 则停止异步加载
                MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //直接同步加载
                T res = Resources.Load<T>(path);
                //记录：为资源信息赋值
                resInfo.asset = res;
                //把那些加载结束后需要调用的委托函数去执行了
                resInfo.callback?.Invoke(res);
                //加载完毕后 这些引用就可以清空
                resInfo.callback = null;
                resInfo.coroutine = null;
                //并使用
                return res;
            }
        }
    }
    #endregion

    #region 异步加载资源相关
    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="callback">加载结束后的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
    {
        //资源唯一ID 资源类型_路径
        string resName = $"{typeof(T).Name}_{path}";

        ResInfo<T> resInfo;
        //通过资源唯一ID去字典中查找资源
        if (!resDic.ContainsKey(resName))
        {
            //如果字典中没有该资源 则创建一个新的资源信息对象
            resInfo = new();
            //引用计数增加
            resInfo.AddRefCount();
            //将资源记录添加到字典中（此时资源还没有加载成功）
            resDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载结束后会调用
            resInfo.callback += callback;
            //开启协程去异步加载资源 并记录协程（用于之后可能的 停止）
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync());
        }
        else
        {
            //从字典中取出资源信息
            resInfo = resDic[resName] as ResInfo<T>;
            //引用计数增加
            resInfo.AddRefCount();
            //如果资源还没有加载完
            //意味着 还在进行异步加载
            if (resInfo.asset == null) resInfo.callback += callback;
            else callback?.Invoke(resInfo.asset);
        }
        //局部函数
        IEnumerator ReallyLoadAsync()
        {
            //异步加载资源
            ResourceRequest request = Resources.LoadAsync<T>(path);
            //等待资源加载结束后 才会执行yield return后面的代码
            yield return request;
            //如果字典中有该资源,逻辑上应该是有的，这只是为了安全性检查所以套了个if
            if (resDic.ContainsKey(resName))
            {
                //为asset赋值，记录加载完成的资源
                resInfo.asset = request.asset as T;
                //如果资源引用计数为0 就卸载该资源
                if (resInfo.refCount == 0) UnloadAsset<T>(path, resInfo.isDel,null,false);
                else
                {
                    //将加载完成的资源传递出去
                    resInfo.callback?.Invoke(resInfo.asset);
                    //加载完毕后 这些引用就可以清空 
                    //避免引用的占用可能带来的潜在的内存泄漏问题
                    resInfo.callback = null;
                    resInfo.coroutine = null;
                }

            }
        }
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">加载结束后的回调函数</param>
    [Obsolete("注意：建议使用泛型异步加载函数 LoadAsync<T>,如果实在要用Type加载,一定不能和泛型加载混用去加载同类型同名资源")]
    public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callback)
    {
        //资源唯一ID 资源类型_路径
        string resName = $"{type.Name}_{path}";

        ResInfo<UnityEngine.Object> resInfo;
        //通过资源唯一ID去字典中查找资源
        if (!resDic.ContainsKey(resName))
        {
            //如果字典中没有该资源 则创建一个新的资源信息对象
            resInfo = new();
            //引用计数增加
            resInfo.AddRefCount();
            //将资源记录添加到字典中（此时资源还没有加载成功）
            resDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载结束后会调用
            resInfo.callback += callback;
            //开启协程去异步加载资源 并记录协程（用于之后可能的 停止）
            resInfo.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync());
        }
        else
        {
            //从字典中取出资源信息
            resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数增加
            resInfo.AddRefCount();
            //如果资源还没有加载完
            //意味着 还在进行异步加载
            if (resInfo.asset == null) resInfo.callback += callback;
            else callback?.Invoke(resInfo.asset);
        }
        //局部函数
        IEnumerator ReallyLoadAsync()
        {
            //异步加载资源
            ResourceRequest request = Resources.LoadAsync(path, type);
            //等待资源加载结束后 才会执行yield return后面的代码
            yield return request;
            //如果字典中有该资源
            if (resDic.ContainsKey(resName))
            {
                //为asset赋值，记录加载完成的资源
                resInfo.asset = request.asset;
                //如果资源引用计数为0 就卸载该资源
                if (resInfo.refCount == 0) UnloadAsset(path, type, resInfo.isDel, null, false);
                else
                {
                    //将加载完成的资源传递出去
                    resInfo.callback?.Invoke(resInfo.asset);
                    //加载完毕后 这些引用就可以清空 
                    //避免引用的占用可能带来的潜在的内存泄漏问题
                    resInfo.callback = null;
                    resInfo.coroutine = null;
                }
            }
        }
    }
    #endregion
    #region 卸载资源相关
    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="isDel">是否立即卸载</param>
    /// <param name="callback">回调函数</param>
    /// <param name="isSub">是否增加引用计数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callback = null,bool isSub = true)
    {
        string resName = $"{typeof(T).Name}_{path}";
        //如果字典中有该资源
        if (resDic.ContainsKey(resName))
        {
            //从字典中取出资源信息
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //引用计数-1
            if (isSub) resInfo.SubRefCount();
            //记录 引用计数为0时 是否马上移除 标签
            resInfo.isDel = isDel;
            //如果资源已经加载完了且引用计数为0
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                //从字典中移除该资源信息
                resDic.Remove(resName);
                //通过api卸载资源
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
                Debug.Log($"资源 {path} 已经被卸载");
            }
            else if (resInfo.asset == null)//资源正在异步加载中
            {
                //当异步加载不想使用时，我们应该移除它的回调记录，而不是直接去卸载资源
                if (callback != null) resInfo.callback -= callback;
            }
        }
        else
        {
            Debug.LogWarning($"资源 {path} 不存在，无法卸载");
        }
    }
    /// <summary>
    /// 为Type专门设计的卸载资源函数重载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<UnityEngine.Object> callback = null,bool isSub = true)
    {
        string resName = $"{type.Name}_{path}";
        //如果字典中有该资源
        if (resDic.ContainsKey(resName))
        {
            //从字典中取出资源信息
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数-1
            if (isSub) resInfo.SubRefCount();
            //记录 引用计数为0时 是否马上移除 标签
            resInfo.isDel = isDel;
            //如果资源已经加载完了且引用计数为0
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                //从字典中移除该资源信息
                resDic.Remove(resName);
                //通过api卸载资源
                Resources.UnloadAsset(resInfo.asset);
                Debug.Log($"资源 {path} 已经被卸载");
            }
            else if (resInfo.asset == null)//资源正在异步加载中
            {
                //当异步加载不想使用时，我们应该移除它的回调记录，而不是直接去卸载资源
                if (callback != null) resInfo.callback -= callback;
            }
        }
        else
        {
            Debug.LogWarning($"资源 {path} 不存在，无法卸载");
        }
    }

    /// <summary>
    /// 异步卸载未使用的资源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callback = null)
    {
        // 卸载未使用的资源
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets());
        IEnumerator ReallyUnloadUnusedAssets()
        {
            //在真正移除不使用的资源之前 应该把 之前记录的那些引用计数为0且没有被记录的资源 移除掉
            List<string> list = new();
            //记录引用计数为0且没有被记录的资源
            foreach (string path in resDic.Keys)
            {
                if (resDic[path].refCount == 0) list.Add(path);
            }
            //移除掉这些资源（foreach不能边记录边移除，会出问题，所以分两个foreach写）
            foreach (string path in list) resDic.Remove(path);

            // 等待未使用的资源卸载完成
            yield return Resources.UnloadUnusedAssets();
            // 卸载完毕后通知外部
            callback?.Invoke();
        }
    }
    #endregion

    #region 其余操作
    //获取引用计数
    public int GetRefCount<T>(string path)
    {
        string resName = $"{typeof(T).Name}_{path}";
        if (resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }

    //清空字典
    public void ClearDic(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic());
        IEnumerator ReallyClearDic()
        {
            resDic.Clear();
            yield return Resources.UnloadUnusedAssets();
            //卸载完毕后 通知外部
            callback?.Invoke();
        }
    }
    #endregion
}

