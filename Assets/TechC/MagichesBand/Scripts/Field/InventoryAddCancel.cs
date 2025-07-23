using TechC.MagichesBand.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// インベントリ追加画面でキャンセル操作を検出してサウンドを再生するクラス
    /// </summary>
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
            if (!playerInput || !playerInput.actions) return;
            
            var cancelAction = playerInput.actions["UiCancel"];
        
            cancelAction.performed -= OnAddMainCancel;
        }

        // キャンセル操作が実行された際の処理
        private void OnAddMainCancel(InputAction.CallbackContext context)
        {
            Sound.Instance.Play(SoundType.ButtonCancel);
        }
    }
}
