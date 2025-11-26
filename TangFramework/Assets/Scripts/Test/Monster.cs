using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public string MonsterName = "ÎÚÑ»¾«";
    // Start is called before the first frame update
    void Start()
    {
        Dead();
    }

    public void Dead()
    {
        Debug.Log("¹ÖÎïËÀÍöÁË");
        EventCenter.Instance.EventTrigger<Monster>(E_EventType.E_Monster_Dead, this);
    }
}
