using UnityEngine;

public interface IEnemy
{
    void Act();
    void TakeDamage(int damage);

    void NextState();
    CharacterStatus Status { get; }
}

public enum EnemyTypeEnum
{
    Slime,
    DRex
}

public class EnemyFactory : MonoBehaviour
{
    [Header("UI Enemy Prefabs")]
    public GameObject slimePrefab;
    public GameObject dRexPrefab;
    public GameObject bossPrefab;

    /// <summary>
    /// ランダムな敵をUI上に生成（親はCanvas）
    /// </summary>
    public IEnemy CreateEnemy(Vector3 localPosition, Transform parent)
    {
        EnemyTypeEnum randomType = (EnemyTypeEnum)Random.Range(0, System.Enum.GetNames(typeof(EnemyTypeEnum)).Length);
        GameObject prefab = null;

        if (GameManager.Instance.enemyType == GameManager.EnemyType.BossEnemy)
        {
            Debug.Log("Boss");
            prefab = bossPrefab;
        }
        else
        {
            switch (randomType)
            {
                case EnemyTypeEnum.Slime:
                    Debug.Log("Slime");
                    prefab = slimePrefab;
                    break;
                case EnemyTypeEnum.DRex:
                    Debug.Log("DRex");
                    prefab = dRexPrefab;
                    break;
            }   
        }

        if (!prefab) return null;

        // Instantiate UI prefab under the canvas
        var enemyObj = Instantiate(prefab, parent);
        if (enemyObj == null)
        {
            Debug.LogError("bossPrefab に IEnemy が実装されていません！");
        }
        
        // UI のローカル座標で表示位置を調整
        enemyObj.GetComponent<RectTransform>().anchoredPosition = localPosition;

        return enemyObj.GetComponent<IEnemy>();
    }
}