using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
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
        private const int MaxDefBuffValue = 16;
        
        
    
        public void EnableAct()
        {
            if (inventoryUI.isInventory) return;
        
            Debug.Log("Enabling act");
            Sound.Instance.Play(SoundType.ButtonSelect);
            actCommand.SetActive(true);
            actButton.SetActive(false);
        }

        public void EnableAttack()
        {
            if (inventoryUI.isInventory) return;
        
            attackCommand.SetActive(true);
            actCommand.SetActive(false);
        }
    
        public void InventoryDisplay()
        {
            Debug.Log($"InventoryDisplay called at frame {Time.frameCount}");
            if (inventoryUI.isInventory) return;
        
            Debug.Log("InventoryDisplay");
            StartCoroutine(ShowInventoryPanelNextFrame());
        }

        /// <summary>
        /// buttonの受け付けが重複しないようにコルーチンを使う
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowInventoryPanelNextFrame()
        {
            yield return null;

            inventoryPanel.SetActive(true);
            actCommand.SetActive(false);
            inventoryUI.SetInventoryState(true);
        }

        public void ShootText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "敵に物理攻撃";
        }

        public void ExplosionText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "敵に魔法攻撃";
        }

        public void AtkUpText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "攻撃力を少し上昇";

            if (battlePlayerController.atkDoublingValue == MaxAtkBuffValue)
            {
                messageText.text = "これ以上攻撃力は上がらない";
            }
        }

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
