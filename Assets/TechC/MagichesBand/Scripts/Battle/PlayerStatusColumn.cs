using TechC.MagichesBand.Game;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    public class PlayerStatusColumn : MonoBehaviour
    {
        [SerializeField] private CharacterStatus playerStatus;
        [SerializeField] private TextMeshProUGUI playerHpText;
        [SerializeField] private TextMeshProUGUI playerMpText;

        private void Update()
        {
            playerHpText.text = $"HP: {playerStatus.hp}";
            playerMpText.text = $"MP: {playerStatus.mp}";
        }
    }
}
