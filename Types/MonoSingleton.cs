using UnityEngine;

namespace MyBox
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField, Tooltip("True to not destroy this object when it loads")]

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
            OnAwake();
        }

        protected abstract void OnAwake();
    }
}
