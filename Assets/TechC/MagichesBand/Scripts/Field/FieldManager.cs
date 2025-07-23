using System;
using System.Collections;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// フィールド内の敵出現などを制御するマネージャークラス
    /// </summary>
    public class FieldManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemy1;
        private const float SpawnEnemyTime = 5f;
        
        private void Start()
        {
            if (!GameManager.Instance)
            {
                Debug.LogError("GameManager.Instance が null です！シーンに GameManager が存在しているか確認してください。");
                return;
            }

            switch (GameManager.Instance.enemyType)
            {
                case GameManager.EnemyType.NotEncounterEnemy:
                    enemy1.SetActive(true);
                    break;
                case GameManager.EnemyType.Enemy1:
                    StartCoroutine(EnableSpawnPoint());
                    break;
                case GameManager.EnemyType.BossEnemy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 一定時間後に敵を出現させる処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator EnableSpawnPoint()
        {
            yield return new WaitForSeconds(SpawnEnemyTime);
            enemy1.SetActive(true);
        }
    }
}
