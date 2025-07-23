using TechC.MagichesBand.Core;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.UI
{
    /// <summary>
    /// メッセージウィンドウを管理するClass
    /// </summary>
    public class MessageWindow : SingletonMonoBehaviour<MessageWindow>
    {
        [SerializeField] private TextMeshProUGUI messageText;

        // メッセージ待機時間
        private const float MessageFadeOutTime = 0.1f;

        /// <summary>
        /// メッセージを表示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void DisplayMessage(string message, Action callback = null)
        {
            messageText.gameObject.SetActive(true);
            StartCoroutine(BattleMessage(message, callback));
        }

        /// <summary>
        /// 指定されたメッセージを表示し、ボタン入力があるまで待って非表示にする
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator BattleMessage(string message, Action callback = null)
        {
            Debug.Log("[BattleManager] BattleMessage");
            
            messageText.text = message;
        
            var buttonPressed = false;
            
            yield return new WaitForSeconds(MessageFadeOutTime);

            while (!buttonPressed)
            {
                if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
                {
                    buttonPressed = true;
                    
                    Debug.Log("[BattleManager] ButtonPressed");
                }
                
                yield return null;
            }
            messageText.gameObject.SetActive(false);
            
            callback?.Invoke();
        }
    }
}
