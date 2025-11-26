using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//可寻址资源信息类
public class AddressablesInfo
{
    //异步操作句柄
    public AsyncOperationHandle handle;
    //引用计数
    public uint refCount;
    public AddressablesInfo(AsyncOperationHandle handle)
    {
        this.handle = handle;
        refCount = 1;
    }
}

public class AddressableMgr
{
    private static AddressableMgr instance = new();
    public static AddressableMgr Instance => instance;
    //用于存储异步加载的返回值
    public Dictionary<string, AddressablesInfo> resDic = new();
    //声明私有构造函数，防止外部实例化
    private AddressableMgr() { }
    public void Init()
    {
        //初始化Addressable资源管理器
    }
    #region 加载单个资源
    //异步加载资源的方法
    public void LoadAssetAsync<T>(string address, System.Action<AsyncOperationHandle<T>> callback) where T : UnityEngine.Object
    {
        //生成资源的唯一key
        string key = address + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果资源已经加载过，则直接从字典中获取
        if (resDic.ContainsKey(key))
        {
            handle = resDic[key].handle.Convert<T>();
            //引用计数加一
            resDic[key].refCount += 1;
            //判断这个异步加载是否已经完成
            if (handle.IsDone)
            {
                callback(handle);
            }
            //还没有加载完成
            else
            {
                handle.Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log($"{key}资源已加载过，再加载成功！");
                        callback(obj);
                    }
                };
            }
            return;
        }
        //如果没有加载过资源，则开始异步加载资源并记录
        handle = Addressables.LoadAssetAsync<T>(address);
        handle.Completed += (h) =>
        {
            //判断异步加载是否成功，成功则回调，失败则报错并从字典中移除
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{key}资源加载成功！");
                callback(h);
            }
            else
            {
                Debug.LogError($"{key}资源加载失败！");
                //? 移除前先判断是否存在该key，是为了防止出现连续两次加载同一资源都失败时，第二次加载失败导致的key不存在问题
                if (resDic.ContainsKey(key)) resDic.Remove(key);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(key, info);
    }
    #endregion

    #region 加载多个资源
    //异步加载多个资源或加载指定资源的方法
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<T> callback, params string[] addresses) where T : UnityEngine.Object
    {
        //生成资源的唯一key
        List<string> addrList = new List<string>(addresses);
        string key = "";
        foreach (var addr in addrList)
        {
            key += addr + "_";
        }
        key += typeof(T).Name;
        //检查资源是否已经加载过
        AsyncOperationHandle<IList<T>> handle;
        if (resDic.ContainsKey(key))
        {
            handle = resDic[key].handle.Convert<IList<T>>();
            //引用计数加一
            resDic[key].refCount += 1;
            //判断这个异步加载是否已经完成
            if (handle.IsDone)
            {
                foreach (T item in handle.Result) callback(item);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    //加载成功后才调用外部传入的回调函数
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log($"{key}资源已加载过，再加载成功！");
                        foreach (T item in obj.Result) callback(item);
                    }
                };
            }
            return;
        }
        //如果没有加载过资源，则
        //开始异步加载多个资源
        handle = Addressables.LoadAssetsAsync<T>(addrList, callback, mode);
        handle.Completed += (h) =>
        {
            if (h.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"{key}资源加载失败！");
                if (resDic.ContainsKey(key)) resDic.Remove(key);
            }
            else if (h.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{key}资源加载成功！");
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(key, info);
    }
    //异步加载多个资源的方法（重载）
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<AsyncOperationHandle<IList<T>>> callback, params string[] addresses) where T : UnityEngine.Object
    {
        //和上面的方法类似
    }
    #endregion

    #region 释放资源
    //释放资源的方法
    public void ReleaseAsset<T>(string address) where T : UnityEngine.Object
    {
        //生成资源的唯一key
        string key = address + "_" + typeof(T).Name;
        //如果资源存在于字典中，则释放资源并从字典中移除
        if (resDic.ContainsKey(key))
        {
            //释放时，引用计数减一
            resDic[key].refCount -= 1;
            //当引用计数为0时，才真正释放资源
            if (resDic[key].refCount == 0)
            {
                AsyncOperationHandle<T> handle = resDic[key].handle.Convert<T>();
                Addressables.Release(handle);
                resDic.Remove(key);
                Debug.Log($"{key}资源已释放！");
            }
        }
        else
        {
            Debug.LogWarning($"{key}资源不存在，无法释放！");
        }
    }
    //释放多个资源的方法
    public void ReleaseAssets<T>(params string[] addresses) where T : UnityEngine.Object
    {
        //生成资源的唯一key
        List<string> addrList = new List<string>(addresses);
        string key = "";
        foreach (var addr in addrList)
        {
            key += addr + "_";
        }
        key += typeof(T).Name;
        if (resDic.ContainsKey(key))
        {
            //释放时，引用计数减一
            resDic[key].refCount -= 1;
            //当引用计数为0时，才真正释放资源
            if (resDic[key].refCount == 0)
            {
                AsyncOperationHandle<IList<T>> handle = resDic[key].handle.Convert<IList<T>>();
                Addressables.Release(handle);
                resDic.Remove(key);
                Debug.Log($"{key}资源已释放！");
            }
        }
        else
        {
            Debug.LogWarning($"{key}资源不存在，无法释放！");
        }
    }
    #endregion
    #region 清空资源
    //清空资源的方法(粗暴) 将所有资源全部清一遍
    public void ClearAllAssets()
    {
        foreach (var item in resDic.Values)
        {
            Addressables.Release(item.handle);
        }
        resDic.Clear();

        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Debug.Log("已清空所有资源！");
    }
    #endregion
}
