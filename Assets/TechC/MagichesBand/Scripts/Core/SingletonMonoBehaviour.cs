namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// MonoBehaviourをシングルトン化する汎用クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMonoBehaviour<T> : UnityEngine.MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        // クラス T の唯一のインスタンスを格納する静的プロパティ
        public static T Instance { get; private set; }
        
        // DontDestroyOnLoad を使うかどうかを切り替える
        protected virtual bool dontDestroyOnLoad => false;

        protected virtual void Awake()
        {
            if (!Instance)
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
