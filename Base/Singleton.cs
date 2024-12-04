//  µ¥ÀýÄ£°å
using UnityEngine;

namespace BaseSpace
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
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
        public void Awake()
        {

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

    interface IOverirdeInstance
    {
        public void CreatInstance();

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
                        return instance;


                    }
                }
                return instance;
            }
        }

        protected Singleton()
        {

        }


      

        public virtual void Init()
        {

        }
    }

}