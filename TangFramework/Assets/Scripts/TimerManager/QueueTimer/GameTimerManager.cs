using System;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// 管理所有的计时器
/// </summary>
public class GameTimerManager : SingletonAutoMono<GameTimerManager>
{
    //note:开始的时候我们要创建一些计时器，否则我们的空闲计时器中一个计时器都没有
    //1.有一个集合用来保存所有的空闲计时器
    //2.有一个集合用来保存当前正在工作的计时器
    //3.更新当前工作中的计时器
    //4.当某个计时器工作完成后，我们需要把它回收到空闲计时器集合中

    [SerializeField, Tooltip("初始计时器数量")] private int _initMaxTimerCount = 3; // 初始化最大的空闲计时器数量 

    // 空闲计时器队列
    private Queue<GameTimer> _idleTimers = new();
    // 工作中的计时器列表
    private List<GameTimer> _workingTimers = new();
    // 计时器ID管理
    private Dictionary<int, GameTimer> _timerDict = new();
    private int _nextTimerId = 1;

    private void Start()
    {
        InitTimerManager();
    }

    private void Update()
    {
        UpdateWorkingTimers();
    }

    private void InitTimerManager()
    {
        for (int i = 0; i < _initMaxTimerCount; i++)
        {
            CreateTimer();
        }
    }

    private void CreateTimer()
    {
        GameTimer timer = new();
        _idleTimers.Enqueue(timer);
    }

    /// <summary>
    /// 启动计时器
    /// </summary>
    /// <param name="time">延时时间（秒）</param>
    /// <param name="task">完成后执行的任务</param>
    /// <returns>计时器ID</returns>
    public int StartTimer(float time, Action task)
    {
        // 如果没有空闲计时器，创建一个新的
        if (_idleTimers.Count == 0)
            CreateTimer();
        
        // 获取空闲计时器并启动
        GameTimer timer = _idleTimers.Dequeue();
        timer.StartTimer(time, task);
        _workingTimers.Add(timer);
        
        // 分配唯一ID并设置到timer中
        int timerId = _nextTimerId++;
        timer.TimerId = timerId;                // 设置到timer对象
        _timerDict[timerId] = timer;
        return timerId;
    }

    /// <summary>
    /// 暂停指定计时器
    /// </summary>
    /// <param name="timerId">计时器ID</param>
    /// <returns>是否成功暂停</returns>
    public bool PauseTimer(int timerId)
    {
        if (_timerDict.TryGetValue(timerId, out GameTimer timer))
        {
            timer.PauseTimer();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 恢复指定计时器
    /// </summary>
    /// <param name="timerId">计时器ID</param>
    /// <returns>是否成功恢复</returns>
    public bool ResumeTimer(int timerId)
    {
        if (_timerDict.TryGetValue(timerId, out GameTimer timer))
        {
            timer.ResumeTimer();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 取消指定计时器
    /// </summary>
    /// <param name="timerId">计时器ID</param>
    /// <returns>是否成功取消</returns>
    public bool CancelTimer(int timerId)
    {
        if (_timerDict.TryGetValue(timerId, out GameTimer timer))
        {
            timer.CancelTimer();
            _timerDict.Remove(timerId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取指定计时器剩余时间
    /// </summary>
    /// <param name="timerId">计时器ID</param>
    /// <returns>剩余时间（秒）</returns>
    public float GetRemainingTime(int timerId)
    {
        if (_timerDict.TryGetValue(timerId, out GameTimer timer))
        {
            return timer.GetRemainingTime();
        }
        return 0f;
    }

    /// <summary>
    /// 获取指定计时器状态
    /// </summary>
    /// <param name="timerId">计时器ID</param>
    /// <returns>计时器状态</returns>
    public E_TimerState GetTimerState(int timerId)
    {
        if (_timerDict.TryGetValue(timerId, out GameTimer timer))
        {
            return timer.GetTimerState();
        }
        return E_TimerState.IDLE;
    }

    /// <summary>
    /// 获取当前工作中的计时器数量
    /// </summary>
    /// <returns>工作中计时器数量</returns>
    public int GetActiveTimerCount()
    {
        return _workingTimers.Count;
    }

    /// <summary>
    /// 获取空闲计时器数量
    /// </summary>
    /// <returns>空闲计时器数量</returns>
    public int GetIdleTimerCount()
    {
        return _idleTimers.Count;
    }

    private void UpdateWorkingTimers()
    {
        if (_workingTimers.Count == 0) return;//没有计时器在工作

        //使用倒序遍历避免移除元素时的索引问题
        for (int i = _workingTimers.Count - 1; i >= 0; i--)
        {
            GameTimer timer = _workingTimers[i];
            E_TimerState state = timer.GetTimerState();
            
            //如果计时器的状态是WORKING，则更新
            if (state == E_TimerState.WORKING)
            {
                timer.UpdateTimer();
            }
            else if (state == E_TimerState.DONE)  //真正完成时才回收
            {
                //任务完成，回收计时器
                _idleTimers.Enqueue(timer);
                
                // 从字典中移除（简化版）
                if (timer.TimerId != 0)
                {
                    _timerDict.Remove(timer.TimerId);
                }
                
                timer.ResetTimer();//重置状态（包括TimerId）
                _workingTimers.RemoveAt(i);//使用RemoveAt避免查找
            }
            // PAUSED状态什么都不做，保持在工作列表中
        }
    }

}
