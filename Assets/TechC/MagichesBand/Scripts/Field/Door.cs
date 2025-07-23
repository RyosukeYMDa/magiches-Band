using TechC.MagichesBand.Item;
using TechC.MagichesBand.UI;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// プレイヤーが近づいたときにインベントリを開ける扉オブジェクトの処理を管理するクラス
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField] private InventoryUI inventoryUI;

        private void OnEnable()
        {
            MessageWindow.Instance.DisplayMessage("OnDoor");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                inventoryUI.isOpen = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                inventoryUI.isOpen = false;   
            }
        }
    }
}
