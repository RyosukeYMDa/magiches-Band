//定義されているシンボルが DEBUG でないときにコンパイルされる
#if !DEBUG
public static class DebugSuppressor
{
      [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
                // ログ無効化
                UnityEngine.Debug.unityLogger.logEnabled = false;
    }
}
#endif