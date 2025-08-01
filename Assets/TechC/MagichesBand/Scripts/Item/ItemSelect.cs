using System.Collections.Generic;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Field;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TechC.MagichesBand.Item
{
    /// <summary>
    /// アイテム選択と使用を管理するクラス
    /// </summary>
    public class ItemSelect : MonoBehaviour
    {
        // アイテムUIオブジェクト一覧
        private readonly List<GameObject> itemUiObjects = new();
    
        private int selectedIndex; // 選択中のインデックス
        private const int RecoverMp = 7; // 回復MP量
        private const int RecoverHp = 7; // 回復HP量
        private const float StickThreshold = 0.5f; // スティック感度閾値
        
        [FormerlySerializedAs("disolveController")] [FormerlySerializedAs("loadingShaderController")] [SerializeField] private DissolveController dissolveController;
        [SerializeField] public Transform contentParent;   // アイテムを並べる親（Vertical Layout Group）
        [SerializeField] private CharacterStatus playerStatus;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private GameObject itemTextPrefab; // TextMeshProを含むプレハブ
        [SerializeField] private PlayerInput playerInput;
    
        //キャッシュ
        private Sound sound;
        private GameManager gameManager;
        private MessageWindow messageWindow;

        private void Awake()
        {
            //キャッシュ
            sound = Sound.Instance;
            gameManager = GameManager.Instance;
            messageWindow = MessageWindow.Instance;
        }
        
        private void OnEnable()
        {
            // PlayerInputが設定されていなければ警告だけ出す
            if (!playerInput)
            {
                Debug.LogWarning("PlayerInputが設定されていません！");
                return;
            }
        
            OpenInventory();
        
            var navigateAction = playerInput.actions["Choice"];
            var submitAction = playerInput.actions["UiSubmit"];
            var cancelAction = playerInput.actions["UiCancel"];

            // イベント登録
            navigateAction.performed += OnNavigate;
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
        }

        private void OnDisable()
        {
            if (!playerInput || !playerInput.actions) return;
            
            var navigateAction = playerInput.actions["Choice"];
            var submitAction = playerInput.actions["UiSubmit"];
            var cancelAction = playerInput.actions["UiCancel"];

            // イベント登録
            navigateAction.performed -= OnNavigate;
            submitAction.performed -= OnSubmit;
            cancelAction.performed -= OnCancel;
        }
    
        /// <summary>
        /// インベントリを開く
        /// </summary>
        private void OpenInventory()
        {   
            UpdateUI(); // 忘れずに最新のUIを生成

            if (itemUiObjects.Count <= 0) return;
        
            EventSystem.current.SetSelectedGameObject(itemUiObjects[0]);
            selectedIndex = 0;
            UpdateHighlight();
        }
    
        /// <summary>
        /// ナビゲーション入力処理
        /// </summary>
        /// <param name="context"></param>
        private void OnNavigate(InputAction.CallbackContext context)
        {
            Debug.Log("OnNavigate called!");
        
            if (!inventoryUI.isInventory || itemUiObjects.Count <= 0) return;
            
            var input = context.ReadValue<Vector2>();

            switch (input.y)
            {
                case < -StickThreshold:
                    // 下に移動
                    Sound.Instance.Play(SoundType.ButtonNavi);
                    selectedIndex = (selectedIndex + 1) % itemUiObjects.Count;
                    UpdateHighlight();
                    Debug.Log("Navigate Down");
                    break;
                case > StickThreshold:
                    // 上に移動
                    Sound.Instance.Play(SoundType.ButtonNavi);
                    selectedIndex = (selectedIndex - 1 + itemUiObjects.Count) % itemUiObjects.Count;
                    UpdateHighlight();
                    Debug.Log("Navigate Up");
                    break;
            }
        }
    
        /// <summary>
        /// 決定入力処理：選択中のアイテムを使用
        /// </summary>
        /// <param name="context"></param>
        private void OnSubmit(InputAction.CallbackContext context)
        {
            Debug.Log("OnNavigate called!");
        
            if (!inventoryUI.isInventory || itemUiObjects.Count <= 0) return;

            UseItem(selectedIndex);
            Debug.Log("Submit");
        }
    
        /// <summary>
        /// キャンセル入力処理
        /// </summary>
        /// <param name="context"></param>
        private void OnCancel(InputAction.CallbackContext context)
        {
            CloseInventory();
        }

        /// <summary>
        /// インベントリを閉じる
        /// </summary>
        private void CloseInventory()
        {
            Debug.Log("closeInventory"); 
            if(!inventoryUI.isInventory) return;
        
            inventoryUI.isItem = false;
            contentParent.gameObject.SetActive(false);
            inventoryUI.SetInventoryState(false);
        
        
            Debug.Log("closeInventory");
        }
    
        /// <summary>
        /// UIの表示を更新する（全て削除 → 再生成）
        /// </summary>
        private void UpdateUI()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            itemUiObjects.Clear();

            foreach (var item in inventoryUI.inventory.items)
            {
                //newItem.transform.SetAsLastSibling(); // 新しいのを一番下に
                var newItem = Instantiate(itemTextPrefab, contentParent);
            
                var tmp = newItem.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp)
                    tmp.text = $"{item.itemName} x{item.quantity}";

                itemUiObjects.Add(newItem);
            }

            selectedIndex = 0;
            UpdateHighlight();
        }

        /// <summary>
        /// 選択中のアイテムの文字色を変更
        /// </summary>
        private void UpdateHighlight()
        {
            for (var i = 0; i < itemUiObjects.Count; i++)
            {
                var text = itemUiObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text)
                    text.color = (i == selectedIndex) ? Color.yellow : Color.white;
            }
        }
    
        /// <summary>
        /// アイテム使用処理
        /// </summary>
        /// <param name="index"></param>
        private void UseItem(int index)
        {
            if(index < 0 || index >= inventoryUI.inventory.items.Count) return;
        
            var item = inventoryUI.inventory.items[index];
            Debug.Log($"Use Item {item.itemName}");

            switch (item.itemName)
            {
                case "Potion":
                    gameObject.SetActive(false);
                    RemoveAndSave();
                    playerStatus.hp = Mathf.Min(playerStatus.hp + RecoverHp, playerStatus.maxHp);
                    UpdateUI();
                    MessageWindow.Instance.DisplayMessage("HPを7回復した", () =>
                    {
                        inventoryUI.isItem = true;
                        Sound.Instance.Play(SoundType.Potion);
                    });
                    break;
                case "MpPotion":
                    gameObject.SetActive(false);
                    RemoveAndSave();
                    playerStatus.mp = Mathf.Min(playerStatus.mp + RecoverMp, playerStatus.maxMp);
                    UpdateUI();
                    MessageWindow.Instance.DisplayMessage("MPを7回復した", () =>
                    {
                        inventoryUI.isItem = true;
                        Sound.Instance.Play(SoundType.MPotion);
                    });
                    break;
                case "Key" when inventoryUI.isOpen:
                    Debug.Log("ドアを開けた");
                    CloseInventory();
                    GameManager.Instance.enemyType = GameManager.EnemyType.BossEnemy;
                    //loading用のShaderを再生
                    dissolveController.PlayEffect();
                    MessageWindow.Instance.DisplayMessage("鍵を開けた", () =>
                    {
                        playerController.SavePlayerPosition();
                        Sound.Instance.Play(SoundType.AreaMovement);
                        dissolveController.StopEffect();
                        SceneManager.LoadScene("Boss");
                    });
                    break;
                case "Key":
                    StartCoroutine(inventoryUI.MessageReception("ここでは使えない"));
                    break;
            }

            return;

            //アイテム削除とセーブ処理
            void RemoveAndSave()
            {
                inventoryUI.inventory.RemoveItem(item.itemName, 1);
                GameManager.Instance.inventory.RemoveItem(item.itemName, 1);

                var stillHas = GameManager.Instance.inventory.items.Exists(i => i.itemName == item.itemName);
                if (!stillHas)
                {
                    GameManager.Instance.obtainedItemIds.Remove(item.itemId);
                }

                SaveManager.SaveInventory(GameManager.Instance.inventory);
                SaveManager.SaveObtainedItemIds(GameManager.Instance.obtainedItemIds);
            }
        }
    }
}
