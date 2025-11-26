using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    void Awake()
    {
        UIModule.Instance.InitModule();
    }
    void Start()
    {
        LogicWindow logicWindow = UIModule.Instance.PopupWindow<LogicWindow>();
        logicWindow.Test();
        LogicWindow logicWindow2 = UIModule.Instance.GetWindow<LogicWindow>() as LogicWindow;
        logicWindow2.Test2();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //UIModule.Instance.HideWindow<LogicWindow>();
            Debug.Log($"测试遮罩系统开关:{UISetting.Instance.SINGMASK_SYSTEM}");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //UIModule.Instance.DestroyWindow<LogicWindow>();
            //UIModule.Instance.DestroyAllWindow();
            UIModule.Instance.PopupWindow<TestAWindow>();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //UIModule.Instance.DestroyAllWindow(new List<string>() { "LogicWindow" });
            UIModule.Instance.PopupWindow<TestBWindow>();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIModule.Instance.HideWindow<TestAWindow>();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UIModule.Instance.HideWindow<TestBWindow>();
        }
    }
}
