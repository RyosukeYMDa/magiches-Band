using System;
using TechC.MagichesBand.Item;
using TechC.MagichesBand.UI;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private InventoryUI inventoryUI;

        private void OnEnable()
        {
            MessageWindow.Instance.DisplayMessage("OnDoor");
        }

        private void OnCollisionEnter(Collision collision)
        {
            inventoryUI.isOpen = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            inventoryUI.isOpen = false;
        }
    }
}
