using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件 —— 参数：Monster
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励 —— 参数： int
    /// </summary>
    E_Player_GetReward,

    E_Scene_LoadProgress, // 场景加载进度 —— 参数： 无
}
