using UnityEngine;
using MieMieFrameWork;
using UnityEngine.InputSystem;

namespace MieMieFrameWork.Sample
{
    public class TimerMgrTest : MonoBehaviour
    {   
        private int timerId;
        void Update()
        {   
            if(Keyboard.current.qKey.wasPressedThisFrame)
            {
                timerId = GameTimerManager.Instance.StartTimer(5f, () =>
                {
                    Debug.Log("计时器结束");
                });
            }

            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {   
                GameTimerManager.Instance.PauseTimer(timerId);
                print("暂停计时器");
                print(GameTimerManager.Instance.GetTimerState(timerId));
            }
            if(Keyboard.current.aKey.wasPressedThisFrame)
            {
                GameTimerManager.Instance.ResumeTimer(timerId);
                print("恢复计时器");
                print(GameTimerManager.Instance.GetTimerState(timerId));
            }
            if(Keyboard.current.sKey.wasPressedThisFrame)
            {
                GameTimerManager.Instance.CancelTimer(timerId);
                print("取消计时器");
                print(GameTimerManager.Instance.GetTimerState(timerId));
            }
        }
    }
}