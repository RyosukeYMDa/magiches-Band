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
            CameraData cameraData = SaveManager.LoadCameraData();
            if (data != null)
            {
                GameManager.Instance.playerPosition = data.GetPosition();
                GameManager.Instance.playerRotation = data.GetRotation();
                Debug.Log("Load");
            }
            
            if (cameraData != null)
            {
                GameManager.Instance.cameraOffset = cameraData.GetOffset();
                GameManager.Instance.cameraRotation = cameraData.GetRotation();
                Debug.Log("Camera Load");
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
