using TechC.MagichesBand.Core;
using TechC.MagichesBand.Field;
using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Title
{
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
            SceneManager.LoadScene("Field");
        }

        public void TitleDateReset()
        {
            GameManager.Instance.DateReset();
        }
    }
}
