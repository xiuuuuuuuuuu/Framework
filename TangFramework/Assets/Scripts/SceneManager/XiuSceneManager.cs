using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class XiuSceneManager : BaseManager<XiuSceneManager>
{
    //同步加载场景的方法
    public void LoadScene(string sceneName, UnityAction callBack = null)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        //执行回调函数
        callBack?.Invoke();
    }

    //异步加载场景的方法
    public void LoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, callBack));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, UnityAction callBack)
    {
        AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        if (ao == null)
        {
            Debug.LogError("[SceneManager] 异步加载场景失败！请检查场景名称是否正确：" + sceneName);
            yield break;
        }
        //等待场景加载完成
        while (!ao.isDone)
        {
            //发送场景加载进度事件 实现外部获取加载进度
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_Scene_LoadProgress, ao.progress);
            yield return null;
        }
        //确保进度条走完 避免最后一帧直接结束，没有同步100%的进度
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_Scene_LoadProgress, 1f);
        //执行回调函数
        callBack?.Invoke();
        //加载完成后进行内存优化
        ClearAfterLoad();
    }

    private static void ClearAfterLoad()
    {
        // 强制垃圾回收
        System.GC.Collect();
        // 卸载未使用的资源
        Resources.UnloadUnusedAssets();
        Debug.Log("[SceneManager] 内存优化完成");
    }
}
