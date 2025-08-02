using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// バトル中のボタン操作を制御するクラス
    /// 攻撃・アイテム選択・バフ表示など、各種UIボタンの表示/非表示やテキスト表示
    /// </summary>
    public class ButtonController : MonoBehaviour
    {
        //Attackする為のbuttonの参照
        [SerializeField] private GameObject actButton;
        [SerializeField] private GameObject actCommand;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private BattlePlayerController battlePlayerController;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private ItemSelect itemSelect;
        [SerializeField] private GameObject inventoryPanel;
        
        //messageを表示させる
        [SerializeField] private TextMeshProUGUI messageText;
    
        private const int MaxAtkBuffValue = 16; //攻撃力バフの上限
        private const int MaxDefBuffValue = 16; //防御力バフの上限
        
        private Sound sound; //キャッシュ
        
        private void Awake()
        {
            sound = Sound.Instance; //キャッシュ
        }
        
        /// <summary>
        /// 行動ボタンを押したときに、行動コマンドUIを表示する
        /// インベントリ中は無効
        /// </summary>
        public void EnableAct()
        {
            if (inventoryUI.isInventory) return;
        
            Debug.Log("Enabling act");
            Sound.Instance.Play(SoundType.ButtonSelect);
            actCommand.SetActive(true);
            actButton.SetActive(false);
        }

        /// <summary>
        /// 攻撃コマンドを表示する（行動コマンドは閉じる）
        /// </summary>
        public void EnableAttack()
        {
            if (inventoryUI.isInventory) return;
        
            attackCommand.SetActive(true);
            actCommand.SetActive(false);
        }
    
        /// <summary>
        /// インベントリを開く処理
        /// </summary>
        public void InventoryDisplay()
        {
            Debug.Log($"InventoryDisplay called at frame {Time.frameCount}");
            if (inventoryUI.isInventory) return;
        
            Debug.Log("InventoryDisplay");
            StartCoroutine(ShowInventoryPanelNextFrame());
        }

        /// <summary>
        /// ボタンの受け付けが重複しないようにコルーチンを使う
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowInventoryPanelNextFrame()
        {
            yield return null;

            inventoryPanel.SetActive(true);
            actCommand.SetActive(false);
            inventoryUI.SetInventoryState(true);
        }

        /// <summary>
        /// 物理攻撃を選んだときの説明テキスト
        /// </summary>
        public void ShootText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "敵に物理攻撃";
        }

        /// <summary>
        /// 魔法攻撃を選んだときの説明テキスト
        /// </summary>
        public void ExplosionText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Mp:4\n敵に魔法攻撃";
        }

        /// <summary>
        /// 攻撃力アップを選んだときの説明テキストと上限チェック
        /// </summary>
        public void AtkUpText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "攻撃力を少し上昇";

            if (battlePlayerController.atkDoublingValue == MaxAtkBuffValue)
            {
                messageText.text = "これ以上攻撃力は上がらない";
            }
        }

        /// <summary>
        /// 防御力アップを選んだときの説明テキストと上限チェック
        /// </summary>
        public void DefUpText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "防御力を少し上昇";

            if (battlePlayerController.defDoublingValue == MaxDefBuffValue)
            {
                messageText.text = "これ以上防御力は上がらない";
            }
        }
    }
}
