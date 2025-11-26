using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIModule : BaseManager<UIModule>
{
    private Camera uiCamera;//UI摄像机
    private Transform uiRoot;//UI根节点

    private Dictionary<string, WindowBase> allWindowDic = new();//存储所有窗口的字典
    private List<WindowBase> allWindowList = new();//存储所有窗口的列表
    private List<WindowBase> visibleWindowList = new();//存储所有显示窗口的列表
    
    #region 初始化
    // 初始化UI模块
    public void InitModule()
    {
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        uiRoot = GameObject.Find("UIRoot").transform;
    }

    // 初始化窗口
    private WindowBase InitWindow(WindowBase windowBase,string windowName)
    {
        //1.生成对应的窗口预制体
        GameObject newWindow = TempLoadWindow(windowName);
        //2.初始化出对应的窗口类
        if (newWindow != null)
        {
            windowBase.gameObject = newWindow;
            windowBase.transform = newWindow.transform;
            windowBase.Canvas = newWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = uiCamera;
            windowBase.transform.SetAsLastSibling();
            windowBase.Name = newWindow.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTransform = newWindow.GetComponent<RectTransform>();
            rectTransform.anchorMax = Vector2.one;//锚点最大值
            rectTransform.offsetMax = Vector2.zero;//相对于锚点最大点的偏移量
            rectTransform.offsetMin = Vector2.zero;//相对于锚点最小点的偏移量
            allWindowDic.Add(windowName, windowBase);
            allWindowList.Add(windowBase);
            visibleWindowList.Add(windowBase);
            SetWindowMaskVisibkle();
            return windowBase;
        }
        Debug.LogError($"{windowName}窗口预制体加载失败");
        return null;
    }
    #endregion

    #region 窗口显示
    /// <summary>
    /// 弹出并显示指定窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    /// <returns></returns>
    public T PopupWindow<T>() where T : WindowBase,new()
    {
        System.Type windowType = typeof(T);
        string windowName = windowType.Name;
        WindowBase window = GetWindow(windowName);//尝试获取已有窗口
        if (window != null) return ShowWindow(windowName) as T;
        //如果窗口不存在则创建新窗口
        T t = new T();
        return InitWindow(t, windowName) as T;
    }
    // 显示窗口
    private WindowBase ShowWindow(string windowName)
    {
        WindowBase window = null;
        if (allWindowDic.ContainsKey(windowName))
        {
            window = allWindowDic[windowName];
            if (window.gameObject != null && window.Visible == false)
            {
                visibleWindowList.Add(window);//将窗口添加到显示列表中
                window.transform.SetAsLastSibling();//将当前物体的层级顺序设置为其父物体下的最后一个子物体（最高的显示顺序）
                window.SetVisible(true);
                SetWindowMaskVisibkle();
                window.OnShow();
            }
            return window;
        }
        else Debug.LogError($"{windowName}窗口不存在,请调用PopUpWindow进行弹出");
        return null;
    }
    #endregion

    #region 窗口获取
    /// <summary>
    /// 获取已经显示的窗口实例
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    /// <returns></returns>
    public T GetWindow<T>() where T : WindowBase,new()
    {
        System.Type windowType = typeof(T);
        foreach (var window in visibleWindowList)
        {
            if (window.Name == windowType.Name)
            {
                return window as T;
            }
        }
        Debug.LogError($"该窗口没有获得到:{windowType.Name} 实例");
        return null;
    }
    // 获取窗口实例
    private WindowBase GetWindow(string windowName)
    {
        if (allWindowDic.ContainsKey(windowName))
        {
            return allWindowDic[windowName];
        }
        else Debug.LogWarning($"{windowName}窗口第一次创建或不存在");
        return null;
    }
    #endregion

    #region 窗口隐藏
    private void HideWindow(string windowName)
    {
        WindowBase window = GetWindow(windowName);
        HideWindow(window);
    }
    /// <summary>
    /// 隐藏指定窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void HideWindow<T>() where T : WindowBase
    {
        HideWindow(typeof(T).Name);
    }
    private void HideWindow(WindowBase window)
    {
        //满足条件则隐藏窗口
        if (window != null && window.Visible)
        {
            visibleWindowList.Remove(window);//将窗口从显示列表中移除
            window.SetVisible(false);//隐藏窗口物体
            SetWindowMaskVisibkle();
            window.OnHide();//调用窗口自定义的OnHide生命周期函数
        }
    }
    #endregion

    #region 窗口销毁
    private void DestroyWindow(string windowName)
    {
        WindowBase window = GetWindow(windowName);
        DestroyWindow(window);
    }
    /// <summary>
    /// 销毁指定窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void DestroyWindow<T>() where T : WindowBase
    {
        DestroyWindow(typeof(T).Name);
    }
    private void DestroyWindow(WindowBase window)
    {
        if (window != null)
        {
            if (allWindowDic.ContainsKey(window.Name))
            {
                allWindowDic.Remove(window.Name);
                allWindowList.Remove(window);
                visibleWindowList.Remove(window);
            }
            window.SetVisible(false);
            SetWindowMaskVisibkle();
            window.OnHide();
            window.OnDestroy();
            GameObject.Destroy(window.gameObject);           
        }
    }
    /// <summary>
    /// 销毁所有窗口
    /// </summary>
    /// <param name="filterList">过滤列表，指定不销毁的窗口名称列表</param>
    public void DestroyAllWindow(List<string> filterList = null)
    {
        for (int i = allWindowList.Count - 1; i >= 0; i--)
        {
            WindowBase window = allWindowList[i];
            if (window == null || (filterList != null && filterList.Contains(window.Name)))
            {
                continue;
            }
            DestroyWindow(window.Name);  
        }
        Resources.UnloadUnusedAssets();
    }
    #endregion

    // 设置窗口遮罩显示状态
    private void SetWindowMaskVisibkle()
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM) return;
        WindowBase maxOrderWindowBase = null;//渲染层级最高的窗口
        int maxOrder = 0;//最大渲染层级值
        int maxIndex = 0;//最大排序索引 在相同父节点下的子物体排序索引
        //1.关闭所有显示窗口的Mask（设为不可见）
        //2.找到渲染层级最高的窗口，打开它的Mask
        for (int i = 0; i < visibleWindowList.Count; i++)
        {
            WindowBase window = visibleWindowList[i];
            if (window != null && window.gameObject != null)
            {
                window.SetMaskVisible(false);
                if (maxOrderWindowBase == null)
                {
                    maxOrderWindowBase = window;
                    maxOrder = window.Canvas.sortingOrder;//渲染层级
                    maxIndex = window.transform.GetSiblingIndex();//在父节点下的子物体排序索引
                }
                else
                {
                    // 找到最大渲染层级的窗口，拿到它
                    if (maxOrder < window.Canvas.sortingOrder)
                    {
                        maxOrderWindowBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                    }
                    // 如果两个窗口的渲染层级相同，就找到同节点下最靠下的那个窗口 优先渲染Mask
                    else if (maxOrder == window.Canvas.sortingOrder && maxIndex < window.transform.GetSiblingIndex())
                    {
                        maxOrderWindowBase = window;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                }
            } 
        }
        if (maxOrderWindowBase != null)
        {
            maxOrderWindowBase.SetMaskVisible(true);
        }
    }
    
    // 加载窗口（临时的方法） 后面可以替换成自己的一套资源加载方式
    private GameObject TempLoadWindow(string windowName)
    {
        //加载窗口预制体并克隆（实例化）出来并设置其父节点
        GameObject windowPrefab = GameObject.Instantiate(Resources.Load<GameObject>($"Windows/{windowName}"),uiRoot);
        //设置窗口预制体transform信息
        windowPrefab.transform.localPosition = Vector3.zero;
        windowPrefab.transform.localScale = Vector3.one;
        windowPrefab.transform.rotation = Quaternion.identity;
        windowPrefab.name = windowName;
        return windowPrefab;
    }
}
