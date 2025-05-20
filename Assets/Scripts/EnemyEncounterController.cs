using UnityEngine;

public class EnemyEncounterController : MonoBehaviour
{
    [SerializeField] private GameObject enemy1;


    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance が null です！シーンに GameManager が存在しているか確認してください。");
            return;
        }

        if (GameManager.Instance.enemyType == 1)
        {
            enemy1.SetActive(false);
        }
    }
}
