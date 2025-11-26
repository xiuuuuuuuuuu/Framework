//最基础的单例管理类
public class BaseManager<T> where T:class,new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();
            return instance;
        }
    }
}
