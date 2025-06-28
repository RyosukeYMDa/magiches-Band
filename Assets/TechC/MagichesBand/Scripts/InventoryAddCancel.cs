using UnityEngine;
using UnityEngine.InputSystem;

namespace TechC.MagichesBand
{
    public class InventoryAddCancel : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
    
        [SerializeField] private GameObject slidePanel;
    
        private void OnEnable()
        {
            var cancelAction = playerInput.actions["UiCancel"];

            // イベント登録
            cancelAction.performed += OnAddMainCancel;
        }

        private void OnDisable()
        {
            if (playerInput && playerInput.actions)
            {
                var cancelAction = playerInput.actions["UiCancel"];
        
                cancelAction.performed -= OnAddMainCancel;   
            }
        }

        private void OnAddMainCancel(InputAction.CallbackContext context)
        {
            slidePanel.SetActive(true);
        }
    }
}
