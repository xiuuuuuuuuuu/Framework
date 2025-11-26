using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private bool isInit = false;
    private float moveSpeed;
    //初始化
    public void Init(Vector3 pos, float speed)
    {
        transform.position = pos;
        moveSpeed = speed;
        isInit = true;
        Invoke("Destroy", 1f);
    }
    void Update()
    {
        if(isInit)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
    //反初始化
    public void UnInit()
    {
        isInit = false;
    }
    public void Destroy()
    {
        UnInit();
        PoolManager.Instance.PushGameObject(gameObject);
    }
}
