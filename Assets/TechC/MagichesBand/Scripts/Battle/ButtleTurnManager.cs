using System.Collections.Generic;
using System.Linq;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    ///     バトル時のターン管理
    /// </summary>
    public class ButtleTurnManager : SingletonMonoBehaviour<ButtleTurnManager>
    {
        [SerializeField] private BattlePlayerController player;
        [SerializeField] private List<GameObject> enemyObjects = new List<GameObject>();
    
        private List<GameObject> turnOrder = new List<GameObject>();
        private int currentTurnIndex;

        public void SetupTurnOrder()
        {
            turnOrder.Clear();
            turnOrder.AddRange(enemyObjects);
            turnOrder.Add(player.gameObject);

            turnOrder = turnOrder
                .OrderByDescending(obj => obj.GetComponent<ICharacter>()?.Status?.agi ?? 0)
                .ThenBy(_ => Random.value)
                .ToList();

            Debug.Log("[TurnManager] 行動順決定：");
            foreach (var obj in turnOrder)
            {
                Debug.Log($" {obj.name} AGI: {obj.GetComponent<ICharacter>()?.Status?.agi}");
            }

            currentTurnIndex = -1;
        }

        public void ProceedTurn()
        {
            if (turnOrder.Count == 0) return;

            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

            var currentUnit = turnOrder[currentTurnIndex];
            var character = currentUnit.GetComponent<ICharacter>();

            if (character == null) return;
            
            Debug.Log($"[TurnManager] 行動ユニット: {currentUnit.name} AGI: {character.Status.agi}");
            character.Act();
        }

        public void ReplaceEnemy(GameObject newEnemy)
        {
            if (enemyObjects.Count > 0)
            {
                enemyObjects[0] = newEnemy;
            }
            
            BattleManager.Instance.enemyDead = false; 
            
            SetupTurnOrder();
            ProceedTurn();
        }

        public void AddEnemy(GameObject enemy)
        {
            enemyObjects.Add(enemy);
            Debug.Log($"[TurnManager] Enemy added: {enemy.name}");
        }
    }
}