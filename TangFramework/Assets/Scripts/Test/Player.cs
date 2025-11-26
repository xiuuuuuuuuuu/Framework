using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        //添加事件监听者
        EventCenter.Instance.AddEventListener<Monster>(E_EventType.E_Monster_Dead, PlayerWaitMonsterDeadDo);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerWaitMonsterDeadDo(Monster info)
    {
        Debug.Log("玩家得到奖励因为：击杀了" + info.MonsterName);
    }
}
