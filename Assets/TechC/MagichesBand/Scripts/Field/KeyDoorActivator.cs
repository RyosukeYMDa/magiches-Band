using TechC.MagichesBand.Game;
using TechC.MagichesBand.Item;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// Keyを所持しているのみDoorを出現させるClass
    /// </summary>
    public class KeyDoorActivator : MonoBehaviour
    {
        [SerializeField] private GameObject door;

        //キャッシュ
        private Inventory inventory;
        
        private const string KeyItemName = "Key";

        private void Start()
        {
            //キャッシュ
            inventory = GameManager.Instance.inventory;
            
            inventory.OnInventoryChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (inventory != null)
                inventory.OnInventoryChanged -= Refresh;
        }

        private void Refresh()
        {
            door.SetActive(inventory.HasItem(KeyItemName));
        }
    }
}
