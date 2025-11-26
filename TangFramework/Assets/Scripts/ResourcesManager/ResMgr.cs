using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResMgr : BaseManager<ResMgr>
{
    // 资源管理器的具体实现可以在这里添加
    // 例如，加载资源、卸载资源等功能
    #region 同步加载资源
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        // 同步加载资源的逻辑可以在这里实现
        Debug.Log("从该路径加载资源: " + path);
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError("资源加载失败: " + path);
        }
        return resource;
    }
    #endregion
    
    #region 两种异步加载资源
    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="callback">加载结束后的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
    {
        // 异步加载资源的逻辑可以在这里实现
        Debug.Log("从该路径加载资源: " + path);
        //要通过协同程序去异步架加载资源
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path, callback));

    }
    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
    {
        //异步加载资源
        ResourceRequest request = Resources.LoadAsync<T>(path);
        //等待资源加载结束后 才会执行yield return后面的代码
        yield return request;
        //判断资源是否加载成功
        if (request.asset != null)
        {
            callback?.Invoke(request.asset as T);
        }
        else
        {
            Debug.LogError("资源加载失败: " + path);
            callback?.Invoke(null);
        }
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">加载结束后的回调函数</param>
    public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callback)
    {
        // 异步加载资源的逻辑可以在这里实现
        Debug.Log("从该路径加载资源: " + path);
        //要通过协同程序去异步架加载资源
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type, callback));

    }
    private IEnumerator ReallyLoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callback)
    {
        //异步加载资源
        ResourceRequest request = Resources.LoadAsync(path);
        //等待资源加载结束后 才会执行yield return后面的代码
        yield return request;

        if (request.asset != null)
        {
            callback?.Invoke(request.asset);
        }
        else
        {
            Debug.LogError("资源加载失败: " + path);
            callback?.Invoke(null);
        }
    }
    #endregion

    #region 卸载资源
    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="asset"></param>
    public void UnloadAsset(UnityEngine.Object asset)
    {
        // 卸载资源的逻辑可以在这里实现
        Debug.Log("卸载资源: " + asset.name);
        Resources.UnloadAsset(asset);
    }

    /// <summary>
    /// 异步卸载未使用的资源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callback = null)
    {
        // 卸载未使用的资源
        Debug.Log("卸载未使用的资源");
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callback));
    }
    
    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callback = null)
    {
        // 等待未使用的资源卸载完成
        yield return Resources.UnloadUnusedAssets();
        Debug.Log("未使用的资源已卸载");

        // 调用回调函数
        callback?.Invoke();
    }
   #endregion
}
