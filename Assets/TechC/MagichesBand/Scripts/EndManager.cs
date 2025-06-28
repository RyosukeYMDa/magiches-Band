using UnityEngine;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public void ReTry()
    {
        GameManager.Instance.DateReset();
        SceneManager.LoadScene("Title");
    }

    public void EndGame()
    {
        Debug.Log("まだ未実装");
    }
}
