using UnityEngine;

namespace MyBox
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField, Tooltip("True to not destroy this object when it loads")]
        private bool dontDestroyOnLoad;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError("Singleton Instance caused: " + typeof(T).Name + " not found on scene");
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            var instance = Instance;

            if (dontDestroyOnLoad)
            {
                // Ensure that the instance actually exists before we do a "DontDestroyOnLoad" on it.
                if (instance != null)
                {
                    DontDestroyOnLoad(instance);
                }
            }

            OnAwake();
        }

        protected abstract void OnAwake();
    }
}
