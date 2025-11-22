namespace Creational;
public class Singleton
{
    public static void Main()
    {
        var obj = SingletonClass.Instance;

        obj?.SomeBusinessLogic();
    }
}

class SingletonClass
{
    private static SingletonClass? _instance;
    private static object _lock = new object();

    private SingletonClass()
    {
    }

    public static SingletonClass? Instance
    {
        get
        {
            if (_instance == null)
            {
                // Thread safe
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SingletonClass();
                    }
                }
            }
            return _instance;
        }
    }

    public void SomeBusinessLogic()
    {
        // Business logic here
    }
}
