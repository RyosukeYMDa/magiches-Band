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
    
        public int turnCount;
    
        public enum TurnPhase
        {
            FirstMove,
            SecondMove
        }
    
        public TurnPhase CurrentTurnPhase { get; set; } = TurnPhase.FirstMove;

        protected override void Awake()
        {
            base.Awake();
            
            Debug.Log("【行動順】");
            foreach (var unit in turnOrder)
            {
                var status = unit.GetComponent<ICharacter>() as MonoBehaviour;
            }
        }

        public void SetupTurnOrder()
        {
            turnOrder.Clear();
            turnOrder.AddRange(enemyObjects); // 敵（BattleManagerなどで生成された）
            turnOrder.Add(player.gameObject); // プレイヤー

            foreach (var obj in turnOrder)
            {
                var enemy = obj.GetComponent<ICharacter>();
                if (enemy != null)
                {
                    Debug.Log($"[TurnManager] {obj.name} AGI: {enemy.Status.agi}");
                }
                else
                {
                    Debug.LogWarning($"[TurnManager] IEnemy 取得失敗: {obj.name}");
                }
            }
        
            turnOrder = turnOrder.OrderByDescending(obj =>
            {
                var comp = obj.GetComponent<ICharacter>(); // IEnemy 取得
                return comp?.Status?.agi ?? 0;         // AGIを取得
            })
            .ThenBy(_ => Random.value)
            .ToList();
            
            
            Debug.Log("[TurnManager] 行動順決定：");
            foreach (var obj in turnOrder)
            {
                var comp = obj.GetComponent<ICharacter>();
                Debug.Log($" {obj.name}  AGI:{comp?.Status?.agi}");
            }
        }

        public void ProceedTurn()
        {
            if (turnOrder.Count == 0) return;

            //turn数を増加
            turnCount++;
            Debug.Log(turnCount);
            GameObject currentUnit = turnOrder[currentTurnIndex];
    
            var enemy = currentUnit.GetComponent<ICharacter>();
            if (enemy != null)
            {
                BattleManager.Instance.enemyDead = false;
                Debug.Log($"[TurnManager] 行動ユニット: {currentUnit.name} AGI: {enemy.Status.agi}");
                Debug.Log("Act");
                enemy.Act();
            }
            else
            {
                Debug.LogWarning($"[TurnManager] IEnemy が見つかりません: {currentUnit.name}");
            }
        }
    
        public void ReplaceEnemy(GameObject newEnemy)
        {
            Debug.Log("ReplayEnemy");
            
            if (enemyObjects.Count > 0)
            {
                enemyObjects[0] = newEnemy;
            }
            ProceedTurn();
        }
    
        public void AddEnemy(GameObject enemy)
        {
            enemyObjects.Add(enemy);
            Debug.Log($"[TurnManager] Enemy added: {enemy.name}");
        }
    }
}
