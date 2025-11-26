using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicWindow : WindowBase
{
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("LogicWindow OnAwake");
    }
    public override void OnShow()
    {
        base.OnShow();
        Debug.Log("LogicWindow OnShow");
    }
    public override void OnHide()
    {
        base.OnHide();
        Debug.Log("LogicWindow OnHide");
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("LogicWindow OnDestroy");
    }
    public void Test()
    {
        Debug.Log("LogicWindow Test");
    }
    public void Test2()
    {
        Debug.Log("LogicWindow Test2");
    }
}
