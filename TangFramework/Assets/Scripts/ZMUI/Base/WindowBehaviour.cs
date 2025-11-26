using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehaviour
{
    public GameObject gameObject { get; set; } //当前窗口物体
    public Transform transform { get; set; } //当前窗口物体的Transform组件
    public Canvas Canvas { get; set; } //当前窗口物体的Canvas组件
    public string Name { get; set; } //窗口名称
    public bool Visible { get; set; } //窗口是否可见

    public virtual void OnAwake() { }//只会在窗口创建时调用一次 与Mono Awake调用时机和次数保持一致
    public virtual void OnShow() { }//在物体显示时执行一次 与Mono OnEnable一致
    public virtual void OnUpdate() { }
    public virtual void OnHide() { }//在物体隐藏时执行一次 与Mono OnDisable一致
    public virtual void OnDestroy() { }//窗口销毁时调用
    
    public virtual void SetVisible(bool visible) {} //设置窗口可见性
}
