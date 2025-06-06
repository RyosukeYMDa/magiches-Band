using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        PlayerData data = SaveManager.LoadPlayerData();
        if (data != null)
        {
            GameManager.Instance.playerPosition = data.GetPosition();
            Debug.Log("Load");
        }
    }
    
    public void StartGame()
    {
        Debug.Log("StartGame");
        SceneManager.LoadScene("MainScene");
    }
    
    public void DateReset()
    {
        Debug.Log("DateReset");
        SaveManager.DeleteAllData();
        GameManager.Instance.ReloadPlayerData();
    }
}
