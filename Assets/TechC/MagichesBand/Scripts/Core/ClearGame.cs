using System.Linq;
using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// ゲームクリア後の処理を担当するクラス
    /// エンディングBGMの再生と入力によるタイトル画面への遷移を行う
    /// </summary>
    public class ClearGame : MonoBehaviour
    {
        private void Start()
        {
            Sound.Instance.Play(SoundType.EndingBGM,true);
        }
        
        private void Update()
        {
            if(Gamepad.current == null)
                return;

            if (!Gamepad.current.allControls.Any(control => control is ButtonControl
                {
                    wasPressedThisFrame: true
                })) return;
            
            GameManager.Instance.DateReset();
            SceneManager.LoadScene("Title");
        }
    }
}