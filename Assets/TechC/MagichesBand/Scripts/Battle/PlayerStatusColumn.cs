using TechC.MagichesBand.Game;
using TMPro;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// プレイヤーのHP・MPステータスをUIに表示するためのクラス
    /// </summary>
    public class PlayerStatusColumn : MonoBehaviour
    {
        [SerializeField] private CharacterStatus playerStatus; // プレイヤーのステータス
        [SerializeField] private TextMeshProUGUI playerHpText; // HP表示用のText
        [SerializeField] private TextMeshProUGUI playerMpText; // MP表示用のText

        private void Update()
        {
            playerHpText.text = $"HP: {playerStatus.hp}";
            playerMpText.text = $"MP: {playerStatus.mp}";
        }
    }
}
