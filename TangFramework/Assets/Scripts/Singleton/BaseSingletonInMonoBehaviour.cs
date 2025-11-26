// 引入Unity引擎的命名空间
using UnityEngine;

// 声明一个泛型类 BaseSingletonInMonoBehaviour<T>
// 这个类继承自MonoBehaviour，泛型参数 T 必须是MonoBehaviour的派生类
public class BaseSingletonInMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // 静态字段用于存储单例实例
    private static T instance;

    // 公共静态属性，用于获取单例实例
    public static T Instance
    {
        get
        {
            // 如果实例尚未创建
            if (instance == null)
            {
                // 尝试在场景中查找具有类型 T 的对象
                instance = FindObjectOfType<T>();

                // 如果在场景中找不到该类型的对象
                if (instance == null)
                {
                    // 创建一个新的空游戏对象
                    GameObject gameObject = new GameObject();

                    // 向新创建的游戏对象添加一个类型为 T 的组件，并将其赋值给 instance
                    instance = gameObject.AddComponent<T>();

                    // 设置游戏对象的名称为类型 T 的名称
                    gameObject.name = typeof(T).ToString();

                    // 不要在场景切换时销毁这个游戏对象
                    DontDestroyOnLoad(gameObject);
                }
            }

            // 返回单例实例
            return instance;
        }
    }

    // Awake 方法在对象实例被创建时调用
    protected virtual void Awake()
    {
        // 如果单例实例尚未创建
        if (instance == null)
        {
            // 将当前对象实例赋值给 instance
            instance = this as T;

            // 不要在场景切换时销毁该对象
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 如果单例实例已存在，则销毁当前对象
            Destroy(this);
        }
    }
}


