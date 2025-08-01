using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.Item
{
    /// <summary>
    /// インベントリUIとアイテム取得メッセージ処理を管理するクラス
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public Inventory inventory;

        public bool isOpen; //扉の範囲内にいるかどうか
        public bool isItem; //item使ったかどうか 
        public bool isInventory; //Inventoryが表示されているのかを判別
        
        //messageを表示させる
        [SerializeField] private TextMeshProUGUI messageText;

        private const float TextFadeOutTime = 1f;

        /// <summary>
        /// セーブデータからインベントリを読み込む
        /// </summary>
        private void Start()
        {
            // Inventoryを初期化 or 既存のものを取得
            inventory = SaveManager.LoadInventory() ?? new Inventory();
        }
    
        /// <summary>
        /// アイテム取得音とメッセージ表示を行う
        /// </summary>
        /// <param name="itemName"></param>
        public void GetItem(string itemName)
        {
            Sound.Instance.Play(SoundType.ItemGet);
            
            StartCoroutine(MessageReception($"{itemName}:get"));
        }
    
        /// <summary>
        /// 一定時間メッセージを表示するコルーチン
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IEnumerator MessageReception(string msg)
        {
            Debug.Log("Message reception");
            messageText.gameObject.SetActive(true);
            messageText.text = msg;
            Debug.Log("到達前");
            yield return new WaitForSeconds(TextFadeOutTime);
            Debug.Log("到達");
            messageText.gameObject.SetActive(false);
        }

        /// <summary>
        /// インベントリの外部アクセス用
        /// </summary>
        public void SetInventoryItem(string itemId, string itemName, int quantity)
        {
            Debug.Log($"SetInventoryItem {itemName}");
            inventory.AddItem(itemId, itemName, quantity);
            GameManager.Instance.inventory.AddItem(itemId, itemName, quantity);
        }
        
        /// <summary>
        /// インベントリ表示状態の変更を行う
        /// </summary>
        /// <param name="state"></param>
        public void SetInventoryState(bool state)
        {
            isInventory = state;
            Debug.Log($"SetInventoryState: {isInventory}");
        }
    }
}