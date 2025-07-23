using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// Keyを所持しているのみDoorを出現させるClass
    /// </summary>
    public class KeyDoorActivator : MonoBehaviour
    {
        [SerializeField] private GameObject door;

        private void Update()
        {
            door.SetActive(GameManager.Instance.inventory.items.Exists(i => i.itemName == "Key"));
        }
    }
}
