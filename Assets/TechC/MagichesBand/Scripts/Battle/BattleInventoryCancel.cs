using UnityEngine;
using UnityEngine.InputSystem;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// バトル中のインベントリ操作中にキャンセル入力を検出して攻撃コマンドに戻す処理を行うクラス
    /// InputSystemを使用してUiCancelアクションを受け取る
    /// </summary>
    public class BattleInventoryCancel : MonoBehaviour
    {
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private PlayerInput playerInput; // プレイヤーの入力アクション

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
    
        /// <summary>
        /// キャンセル入力を受け取った際の処理
        /// インベントリ表示を終了して攻撃コマンドUIを表示する
        /// </summary>
        private void OnAdditionCancel(InputAction.CallbackContext context)
        {
            Debug.Log("OnAdditionCancel");
            attackCommand.SetActive(true);
        }
    }
}