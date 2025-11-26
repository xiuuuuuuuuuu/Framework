using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameObjectPoolTest : MonoBehaviour
{
    public GameObject CubePrefab;
    //[SerializeField] private Transform TempFayther;
    private Queue<GameObject> gameObjects;
    // Start is called before the first frame update
    void Start()
    {
        //0.初始化对象池管理器
        //要想正常使用这个对象池管理器，必须要先初始化，这点应该在框架根脚本实现
        //由于现在还没有实现框架根脚本，故先在这里实现
        PoolManager.Instance.Init();
        //1.对象池初始化(非必要)
        //如果没有初始化 代表这个池无限空间，没有默认数量
        //PoolManager.Instance.InitGameObjectPool(CubePrefab, 50, 30);
        //2.放进去和拿出来
        GameObject obj = GameObject.Instantiate(CubePrefab);
        obj.name = CubePrefab.name;
        PoolManager.Instance.PushGameObject(obj);
        PoolManager.Instance.InitGameObjectPool(CubePrefab, 50, 20);
        //3.对象池清理
        //PoolManager.Instance.ClearGameObjectPool(CubePrefab.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject bullet2 = PoolManager.Instance.GetGameObject(CubePrefab.name);
        }
        if(Input.GetMouseButtonDown(0))
        {
            PoolManager.Instance.GetGameObject<BulletController>(CubePrefab.name, transform).Init(transform.position, Random.Range(5, 10f));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (gameObjects.Count > 0)
            {
                PoolManager.Instance.PushGameObject(gameObjects.Dequeue());
                Debug.Log("已经放回对象池");
            }
        }
    }
    void 简单使用()
    {
        GameObject bullet = GameObject.Instantiate(CubePrefab);
        bullet.name = CubePrefab.name;
        PoolManager.Instance.PushGameObject(bullet);
        GameObject bullet2 = PoolManager.Instance.GetGameObject(CubePrefab.name);
    }
}
