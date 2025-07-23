using System.Collections.Generic;
using System.Linq;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// バトル時のターン管理を行うシングルトンクラス
    /// プレイヤーと敵の行動順を素早さに基づいて決定し、順番に行動させる
    /// </summary>
    public class ButtleTurnManager : SingletonMonoBehaviour<ButtleTurnManager>
    {
        [SerializeField] private BattlePlayerController player;  // プレイヤーキャラクターの参照
        [SerializeField] private List<GameObject> enemyObjects = new List<GameObject>(); // 現在の敵キャラクターのGameObjectリスト
    
        // 行動順に並べられたユニットリスト
        private List<GameObject> turnOrder = new List<GameObject>();
        private int currentTurnIndex; // 現在のターンのインデックス

        /// <summary>
        /// 行動順を素早さに基づいて決定し、turnOrderに格納する
        /// プレイヤーと敵を統一的に扱う
        /// </summary>
        public void SetupTurnOrder()
        {
            // 敵とプレイヤーをまとめてturnOrderに追加
            turnOrder.Clear();
            turnOrder.AddRange(enemyObjects);
            turnOrder.Add(player.gameObject);

            // AGIの高い順に並び替えて同値の場合はランダム順
            turnOrder = turnOrder
                .OrderByDescending(obj => obj.GetComponent<ICharacter>()?.Status?.agi ?? 0)
                .ThenBy(_ => Random.value)
                .ToList();

            Debug.Log("[TurnManager] 行動順決定：");
            foreach (var obj in turnOrder)
            {
                Debug.Log($" {obj.name} AGI: {obj.GetComponent<ICharacter>()?.Status?.agi}");
            }

            currentTurnIndex = -1; // 初期化
        }

        /// <summary>
        /// 次のユニットの行動処理を呼び出す
        /// ターンを1つ進めて、Act()を実行
        /// </summary>
        public void ProceedTurn()
        {
            if (turnOrder.Count == 0) return;

            // ターンインデックスを進め、リストの範囲でループ
            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

            var currentUnit = turnOrder[currentTurnIndex];
            var character = currentUnit.GetComponent<ICharacter>();

            if (character == null) return;
            
            // ユニットに行動させる
            Debug.Log($"[TurnManager] 行動ユニット: {currentUnit.name} AGI: {character.Status.agi}");
            character.Act();
        }

        /// <summary>
        /// ボスの変化
        /// </summary>
        public void ReplaceEnemy(GameObject newEnemy)
        {
            if (enemyObjects.Count > 0)
            {
                // ボスの変化
                enemyObjects[0] = newEnemy;
            }
            
            BattleManager.Instance.enemyDead = false; 
            
            // 行動順を再決定し、次のターンを進行
            SetupTurnOrder();
            ProceedTurn();
        }

        /// <summary>
        /// 新しい敵をバトルに追加する
        /// </summary>
        /// <param name="enemy">追加する敵GameObject</param>
        public void AddEnemy(GameObject enemy)
        {
            enemyObjects.Add(enemy);
            Debug.Log($"[TurnManager] Enemy added: {enemy.name}");
        }
    }
}