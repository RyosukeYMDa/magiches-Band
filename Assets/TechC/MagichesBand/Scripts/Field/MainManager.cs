using System.Collections;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    public class MainManager : MonoBehaviour
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
