using UnityEngine;
using UnityEngine.AddressableAssets;
public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //异步加载
            ResourcesMgr.Instance.LoadAsync<GameObject>("test", TestFun);
            Debug.Log(ResourcesMgr.Instance.GetRefCount<GameObject>("test"));
            //异步加载
            ResourcesMgr.Instance.LoadAsync<GameObject>("test", TestFun);
            Debug.Log(ResourcesMgr.Instance.GetRefCount<GameObject>("test"));
            //卸载资源
            ResourcesMgr.Instance.UnloadAsset<GameObject>("test", false, TestFun);
            Debug.Log(ResourcesMgr.Instance.GetRefCount<GameObject>("test"));
            //卸载资源
            //ResourcesMgr.Instance.UnloadAsset<GameObject>("test", false,TestFun);
            //Debug.Log(ResourcesMgr.Instance.GetRefCount<GameObject>("test"));
            //同步加载
            //Instantiate(ResourcesMgr.Instance.Load<GameObject>("test"));
        }
        //测试Addressable资源管理器
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddressableMgr.Instance.LoadAssetAsync<GameObject>("Cube", (h) =>
            {
                Instantiate(h.Result);
            });
            AddressableMgr.Instance.LoadAssetAsync<GameObject>("Cube", (sb) =>
            {
                Instantiate(sb.Result, Vector3.right * 2, Quaternion.identity);
                //使用完资源后释放
                AddressableMgr.Instance.ReleaseAsset<GameObject>("Cube");
            });
        }
        //测试加载多个地址标签的资源（交集）
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddressableMgr.Instance.LoadAssetsAsync<GameObject>(Addressables.MergeMode.Intersection, (obj) =>
            {
                print("1" + obj.name);
            }, "Cube", "SD");
            AddressableMgr.Instance.LoadAssetsAsync<GameObject>(Addressables.MergeMode.Intersection, (obj) =>
            {
                print("2" + obj.name);
            }, "Cube", "SD");
        }
        //测试加载多个地址标签的资源（并集）
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddressableMgr.Instance.LoadAssetsAsync<GameObject>(Addressables.MergeMode.Union, (obj) =>
            {
                print(obj.name);
            }, "Cube", "SD");
        }
        //测试引用计数
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddressableMgr.Instance.LoadAssetAsync<GameObject>("Cube", (h) =>
            {
                Instantiate(h.Result);
            });
            Debug.Log("引用计数加1");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddressableMgr.Instance.ReleaseAsset<GameObject>("Cube");
            Debug.Log("引用计数减1");
        }
    }
    private void TestFun(GameObject obj) => Instantiate(obj);
    /* {
        Instantiate(obj);
    } */
}
