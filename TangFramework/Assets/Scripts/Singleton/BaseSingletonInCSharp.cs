// 定义一个泛型基类 BaseSingletonInCSharp，要求泛型类型 T 必须是引用类型（class）并且具有无参数构造函数（new()）
public class BaseSingletonInCSharp<T> where T : class, new()
{
    // 用于存储单例实例的私有静态字段
    private static T instance;

    // 用于线程同步的锁对象
    private static readonly object LockObject = new object();

    // 公共静态属性，用于获取单例实例
    public static T Instance
    {
        get
        {
            // 如果实例尚未创建
            if (instance == null)
            {
                // 使用锁确保线程安全
                lock (LockObject)
                {
                    // 再次检查实例是否为空，因为多个线程可能同时通过第一个检查
                    if (instance == null)
                    {
                        // 如果为空，创建一个新的实例并赋值给 instance
                        instance = new T();
                    }
                }
            }
            // 返回单例实例
            return instance;
        }
    }

    // 受保护的构造函数，只能在类内部或派生类中访问
    protected BaseSingletonInCSharp()
    {
        // 这里可以添加初始化代码，但通常不需要，因为实例化是延迟的
    }
}

