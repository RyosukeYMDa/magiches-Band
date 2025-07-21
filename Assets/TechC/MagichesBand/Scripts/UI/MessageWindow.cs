using TechC.MagichesBand.Core;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.UI
{
    public class MessageWindow : SingletonMonoBehaviour<MessageWindow>
    {
        [SerializeField] private TextMeshProUGUI messageText;

        private const float MessageFadeOutTime = 0.1f;

        public void DisplayMessage(string message, Action callback = null)
        {
            messageText.gameObject.SetActive(true);
            StartCoroutine(BattleMessage(message, callback));
        }

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
