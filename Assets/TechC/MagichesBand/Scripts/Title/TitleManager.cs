using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Title
{
    public class TitleManager : MonoBehaviour
    {
        private void Start()
        {
            var data = SaveManager.LoadPlayerData();
            var cameraData = SaveManager.LoadCameraData();
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
            
            Sound.Instance.Play(SoundType.TitleBGM,true);
        }
    
        public void StartGame()
        {
            Debug.Log("StartGame");
            Sound.Instance.Play(SoundType.ButtonSelect);
            SceneManager.LoadScene("Field");
        }

        public void TitleDateReset()
        {
            Sound.Instance.Play(SoundType.ButtonSelect);
            GameManager.Instance.DateReset();
        }
        
        public void QuitGame()
        {
            Debug.Log("アプリケーションを終了します");
            Application.Quit();
        }
    }
}
