using System;
using UnityEngine;


/// <summary>
/// 计时器状态
/// </summary>
public enum E_TimerState
{
    IDLE,     // 空闲状态
    WORKING,  // 工作中
    DONE,     // 工作完成
    PAUSED    // 暂停状态
}


public class GameTimer
{
    //1.计时时长
    //2.计时结束后执行的任务
    //3.当前计时器的状态
    //4.是否停止当前计时器

    // 计时器ID
    public int TimerId { get; set; }
    
    private float _startTime;
    private Action _task;
    private bool _isStopTimer;
    private E_TimerState e_timerState;

    public GameTimer()
    {
        ResetTimer();
    }

    //1.开始计时
    public void StartTimer(float time, Action task)
    {
        _startTime = time;
        _task = task;
        _isStopTimer = false;
        e_timerState = E_TimerState.WORKING;
    }

    //2.更新计时器
    public void UpdateTimer()
    {
        if (_isStopTimer || e_timerState == E_TimerState.PAUSED) return;//如果停止或暂停计时器，则不更新

        _startTime -= Time.deltaTime;
        if (_startTime <= 0f)
        {
            _task?.Invoke();
            e_timerState = E_TimerState.DONE;
            _isStopTimer = true;
        }
    }
    //3.确定定时器状态
    public E_TimerState GetTimerState() => e_timerState;

    //4.暂停计时器
    public void PauseTimer()
    {
        if (e_timerState == E_TimerState.WORKING)
        {
            e_timerState = E_TimerState.PAUSED;
        }
    }
    //5.恢复计时器
    public void ResumeTimer()
    {
        if (e_timerState == E_TimerState.PAUSED)
        {
            e_timerState = E_TimerState.WORKING;
        }
    }
    //6.取消计时器
    public void CancelTimer()
    {
        _isStopTimer = true;
        e_timerState = E_TimerState.DONE;
    }
    //7.获取剩余时间
    public float GetRemainingTime()
    {
        return _isStopTimer ? 0f : Mathf.Max(0f, _startTime);
    }
    //8.重置计时器
    public void ResetTimer()
    {
        TimerId = 0;           // 重置ID
        _startTime = 0f;
        _task = null;
        _isStopTimer = true;
        e_timerState = E_TimerState.IDLE;
    }
}