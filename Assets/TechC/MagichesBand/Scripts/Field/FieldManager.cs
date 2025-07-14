using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// ゲームフィールドのマネージャー
    /// </summary>
    public class FieldManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemy1;
    
        public Vector3 player;


        private void Start()
        {
            if (!GameManager.Instance)
            {
                Debug.LogError("GameManager.Instance が null です！シーンに GameManager が存在しているか確認してください。");
                return;
            }

            if (GameManager.Instance.enemyType == GameManager.EnemyType.NotEncounterEnemy)
            {
                enemy1.SetActive(true);
            }

            if (GameManager.Instance.enemyType == GameManager.EnemyType.Enemy1)
            {
                StartCoroutine(EnableSpawnPoint());
            }
        }

        private IEnumerator EnableSpawnPoint()
        {
            yield return new WaitForSeconds(5f);
            enemy1.SetActive(true);
        }
    }
}
