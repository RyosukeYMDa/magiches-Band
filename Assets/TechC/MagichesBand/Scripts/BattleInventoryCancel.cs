using UnityEngine;
using UnityEngine.InputSystem;

namespace TechC.MagichesBand
{
    public class BattleInventoryCancel : MonoBehaviour
    {
        [SerializeField] private GameObject attackCommand;
    
        [SerializeField] private PlayerInput playerInput;

        private void OnEnable()
        {
            var cancelAction = playerInput.actions["UiCancel"];

            // イベント登録
            cancelAction.performed += OnAdditionCancel;
        }

        private void OnDisable()
        {
            if (playerInput && playerInput.actions)
            {
                var cancelAction = playerInput.actions["UiCancel"];
        
                cancelAction.performed -= OnAdditionCancel;   
            }
        }
    
        private void OnAdditionCancel(InputAction.CallbackContext context)
        {
            Debug.Log("OnAdditionCancel");
            attackCommand.SetActive(true);
        }
    }
}