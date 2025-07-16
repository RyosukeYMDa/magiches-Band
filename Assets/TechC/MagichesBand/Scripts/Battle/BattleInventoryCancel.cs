using UnityEngine;
using UnityEngine.InputSystem;

namespace TechC.MagichesBand.Battle
{
    public class BattleInventoryCancel : MonoBehaviour
    {
        [SerializeField] private GameObject attackCommand;
    
        [SerializeField] private PlayerInput playerInput;

        private void OnEnable()
        {
            var cancelAction = playerInput.actions["UiCancel"];
            cancelAction.performed += OnAdditionCancel;  // イベント登録
        }

        private void OnDisable()
        {
            if (!playerInput || !playerInput.actions) return;
            
            var cancelAction = playerInput.actions["UiCancel"];
            cancelAction.performed -= OnAdditionCancel;
        }
    
        private void OnAdditionCancel(InputAction.CallbackContext context)
        {
            Debug.Log("OnAdditionCancel");
            attackCommand.SetActive(true);
        }
    }
}