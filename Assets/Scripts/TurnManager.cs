using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private BattlePlayerController player;
    [SerializeField] private List<GameObject> enemyObjects = new List<GameObject>();

    private List<GameObject> turnOrder = new List<GameObject>();
    private int currentTurnIndex = 0;

    private void Start()
    {
        SetupTurnOrder();
        Debug.Log("【行動順】");
        foreach (var unit in turnOrder)
        {
            var status = unit.GetComponent<IEnemy>() as MonoBehaviour;
            Debug.Log(status.name);
        }

        ProceedTurn();
    }

    private void SetupTurnOrder()
    {
        turnOrder.Clear();
        turnOrder.Add(player.gameObject); // プレイヤー
        turnOrder.AddRange(enemyObjects); // 敵（BattleManagerなどで生成された）

        foreach (var obj in turnOrder)
        {
            var enemy = obj.GetComponent<IEnemy>();
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
            var comp = obj.GetComponent<IEnemy>(); // IEnemy 取得
            return comp?.Status?.agi ?? 0;         // AGIを取得
        }).ToList();
    }

    private void ProceedTurn()
    {
        if (turnOrder.Count == 0) return;

        GameObject currentUnit = turnOrder[currentTurnIndex];
    
        var enemy = currentUnit.GetComponent<IEnemy>();
        if (enemy != null)
        {
            Debug.Log($"[TurnManager] 行動ユニット: {currentUnit.name} AGI: {enemy.Status.agi}");
            enemy.Attack();
        }
        else
        {
            Debug.LogWarning($"[TurnManager] IEnemy が見つかりません: {currentUnit.name}");
        }

        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
    }
    
    public void AddEnemy(GameObject enemy)
    {
        enemyObjects.Add(enemy);
        Debug.Log($"[TurnManager] Enemy added: {enemy.name}");
    }
}

