using System.Collections;
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
    
        //messageを表示させる
        [SerializeField] private TextMeshProUGUI messageText;
    
        public GameObject inventoryPanel;
    
        public void EnableAct()
        {
            if (inventoryUI.isInventory) return;
        
            Debug.Log("Enabling act");
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
        IEnumerator ShowInventoryPanelNextFrame()
        {
            yield return null;

            inventoryPanel.SetActive(true);
            actCommand.SetActive(false);
            inventoryUI.SetInventoryState(true);
        }

        public void ShootText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "slash:enemy - 1";
        }

        public void ExplosionText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Explosion:enemy - 1";
        }

        public void AtkUpText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "AtkUp";

            if (battlePlayerController.atkDoublingValue == 16)
            {
                messageText.text = "AtkUpperLimit";
            }
        }

        public void DefUpText()
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "DefUp";

            if (battlePlayerController.defDoublingValue == 16)
            {
                messageText.text = "DefUpperLimit";
            }
        }
    }
}
