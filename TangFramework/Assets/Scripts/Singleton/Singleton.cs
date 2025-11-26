using UnityEngine;

//限定 T 必须是 MonoBehaviour 的子类
//该类的目的是确保继承它的类在场景中只有一个实例，并提供全局访问。
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //这是单例模式的核心部分，Instance 是一个静态的属性，表示单例对象。
    //它只能通过 Singleton<T> 的派生类访问，并且每个 Singleton<T> 类只能有一个实例。
    public static T Instance { get; private set; }

    //Unity 中 Awake() 方法会在对象(该脚本)创建时调用
    protected virtual void Awake()
    {
        //检查是否已经存在实例
        //如果有实例了，就销毁当前对象，确保只保留一个实例
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //如果没有实例，则将 Instance 设置为当前对象 (this)
        //使得后续访问 Instance 时能获取到这个对象。
        Instance = this as T;
    }

    //在应用程序退出时重置实例并销毁对象
    //确保在应用程序关闭时不会留下任何实例
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    //这是一个继承自 Singleton<T> 的子类，用来创建持久化的单例。
    //持久化意味着在场景切换时，单例对象不会被销毁，而是保持在内存中直到程序退出。
    /* public abstract class PresistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    } */
    //这段代码为 Unity 中的游戏对象提供了两种类型的单例模式：
    //普通单例(Singleton) : 只在场景中保持一个实例，如果场景切换或者多次创建该对象时，只保留一个实例。
    //持久单例(PresistentSingleton) : 保证单例对象在场景切换中保持不被销毁（例如，游戏管理器、音效管理器等需要在多个场景中共享的资源）
    //这种设计模式在 Unity 中非常有用，尤其是当你希望某个类只存在一个实例，并且在应用程序生命周期内持续存在时。
}
