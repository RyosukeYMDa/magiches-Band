using UnityEngine;

public interface IEnemy
{
    void Act();
    void TakeDamage(int damage);

    void NextState();
    
    void ResetStatus();
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
    public GameObject bossPhase2Prefab;

    /// <summary>
    /// ランダムな敵をUI上に生成（親はCanvas）
    /// </summary>
    public IEnemy CreateEnemy(Vector3 localPosition, Transform parent, bool isPhase2 = false)
    {
        GameObject prefab = null;
        
        if (GameManager.Instance.enemyType == GameManager.EnemyType.BossEnemy)
        {
            prefab = isPhase2 ? bossPhase2Prefab : bossPrefab;
            Debug.Log(isPhase2 ? "Boss Phase 2" : "Boss Phase 1");
        }
        else
        {
            EnemyTypeEnum randomType = (EnemyTypeEnum)Random.Range(0, System.Enum.GetNames(typeof(EnemyTypeEnum)).Length);
            
            switch (randomType)
            {
                case EnemyTypeEnum.Slime:
                    prefab = slimePrefab;
                    break;
                case EnemyTypeEnum.DRex:
                    prefab = dRexPrefab;
                    break;
            }   
        }

        if (!prefab) return null;

        // Instantiate UI prefab under the canvas
        var enemyObj = Instantiate(prefab, parent);
        
        // UI のローカル座標で表示位置を調整
        enemyObj.GetComponent<RectTransform>().anchoredPosition = localPosition;

        return enemyObj.GetComponent<IEnemy>();
    }
}