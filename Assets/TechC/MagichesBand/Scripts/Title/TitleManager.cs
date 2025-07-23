using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Title
{
    /// <summary>
    /// タイトル画面の管理を行うクラス
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        private void Start()
        {
            // プレイヤーデータとカメラデータを読み込み、GameManagerに反映
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
    
        /// <summary>
        /// フィールドシーンへ遷移してゲーム開始
        /// </summary>
        public void StartGame()
        {
            Debug.Log("StartGame");
            Sound.Instance.Play(SoundType.ButtonSelect);
            SceneManager.LoadScene("Field");
        }

        /// <summary>
        /// セーブデータのリセット
        /// </summary>
        public void TitleDateReset()
        {
            Sound.Instance.Play(SoundType.ButtonSelect);
            GameManager.Instance.DateReset();
        }
        
        /// <summary>
        /// アプリケーションを終了
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("アプリケーションを終了します");
            Application.Quit();
        }
    }
}
