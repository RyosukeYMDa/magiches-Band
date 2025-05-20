using UnityEngine;

public interface IEnemy
{
    void Attack();
}

public enum EnemyTypeEnum
{
    Slime,
    Dragon
}

public class EnemyFactory : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject slimePrefab;
    public GameObject dragonPrefab;

    public IEnemy CreateEnemy(EnemyTypeEnum type, Vector3 position)
    {
        GameObject enemyObj = null;

        switch (type)
        {
            case EnemyTypeEnum.Slime:
                enemyObj = Instantiate(slimePrefab, position, Quaternion.identity);
                break;
            case EnemyTypeEnum.Dragon:
                enemyObj = Instantiate(dragonPrefab, position, Quaternion.identity);
                break;
        }

        return enemyObj?.GetComponent<IEnemy>();
    }
}
