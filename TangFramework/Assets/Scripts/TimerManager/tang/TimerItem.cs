using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象 里面存储了计时器的相关数据
/// </summary>
public class TimerItem
{
    public int keyID;// 唯一ID
    public UnityAction overCallBack;// 计时结束后的委托回调
    public UnityAction callBack;// 间隔一定时间去执行的委托回调
    public int allTime;// 表示计时器总的计时时间 毫秒：1s = 1000ms
    public int maxAllTime;// 记录一开始计时时的总时间 用于时间重置
    public int intervalTime;// 间隔执行回调的时间 毫秒 毫秒：1s = 1000ms
    public int maxIntervalTime;// 记录一开始的间隔时间
    public bool isRuning;// 是否在进行计时

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="time">计时时间</param>
    /// <param name="overCallBack">总时间计时结束后的回调</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    public void InitInfo(int keyID, int time, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = time;
        this.overCallBack = overCallBack;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    /// <summary>
    /// 缓存池回收时  清除相关引用数据
    /// </summary>
    public void ResetInfo()
    {
        overCallBack = null;
        callBack = null;
    }
}
