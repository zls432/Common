//  µ¥ÀýÄ£°å
using Unity.VisualScripting;
using UnityEngine;
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }
    private static bool applicationIsQuitting = false;
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
    public static bool IsDestroy()
    {
        return applicationIsQuitting;
    }
}
public class Singleton<T> where T : new()
{
    private static readonly object objLock = new object();
    private static T instance;
    public static T Instance
    {
        get
        {
            lock (objLock)
            {
                if (instance == null)
                {
                  

                    instance = new T();
                    Debug.Log("New Instance:" + instance.GetType().ToString() + " Hash :" + instance.GetHashCode());
                }
            }
            return instance;
        }
    }
}
