namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// MonoBehaviourをシングルトン化する汎用クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMonoBehaviour<T> : UnityEngine.MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance { get; private set; }
        protected virtual bool dontDestroyOnLoad => false;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;

                if(dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
