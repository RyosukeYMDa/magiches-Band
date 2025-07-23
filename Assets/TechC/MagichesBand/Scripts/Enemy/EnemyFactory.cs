using System.Collections.Generic;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Enemy
{
    public enum EnemyTypeEnum
    {
        Slime,
        DRex
    }

    /// <summary>
    /// 敵のプレハブを生成するファクトリークラス
    /// </summary>
    public class EnemyFactory : MonoBehaviour
    {
        [Header("UI Enemy Prefabs")]
        [SerializeField] public GameObject slimePrefab;
        [SerializeField] public GameObject dRexPrefab;
        [SerializeField] public GameObject bossPrefab;
        [SerializeField] public GameObject bossPhase2Prefab;

        // 通常敵のDictionary
        private Dictionary<EnemyTypeEnum, GameObject> enemyPrefabDict;
        
        private void Awake()
        {
            // Enumとプレハブの対応を初期化
            enemyPrefabDict = new Dictionary<EnemyTypeEnum, GameObject>
            {
                { EnemyTypeEnum.Slime, slimePrefab },
                { EnemyTypeEnum.DRex, dRexPrefab }
            };
        }
        
        /// <summary>
        /// ランダムな敵またはボスを生成して返す
        /// </summary>
        /// <param name="localPosition">生成後の敵のローカル座標（Canvas内）</param>
        /// <param name="parent">親Transform（通常はCanvas）</param>
        /// <param name="isPhase2">ボスフェーズ2かどうかを判定するフラグ（通常敵には影響しない）</param>
        /// <returns>ICharacterを実装した敵キャラオブジェクト</returns>
        public ICharacter CreateEnemy(Vector3 localPosition, Transform parent, bool isPhase2 = false)
        {
            GameObject prefab = null;

            var enemyType = GameManager.Instance.enemyType;

            if (enemyType == GameManager.EnemyType.BossEnemy)
            {
                prefab = isPhase2 ? bossPhase2Prefab : bossPrefab;
                Debug.Log(isPhase2 ? "Boss Phase 2" : "Boss Phase 1");
            }
            else
            {
                // 通常敵をランダム選出
                var randomType = (EnemyTypeEnum)Random.Range(0, System.Enum.GetNames(typeof(EnemyTypeEnum)).Length);

                if (!enemyPrefabDict.TryGetValue(randomType, out prefab))
                {
                    Debug.LogWarning($"未定義のEnemyType: {randomType}");
                    return null;
                }
            }

            if (!prefab)
            {
                Debug.LogError("Enemyのプレハブが未設定です。");
                return null;
            }

            // Canvas内にプレハブを生成
            var enemyObj = Instantiate(prefab, parent);
            enemyObj.GetComponent<RectTransform>().anchoredPosition = localPosition;

            // ICharacter取得
            var character = enemyObj.GetComponent<ICharacter>();
            if (character == null)
            {
                Debug.LogError($"プレハブ '{prefab.name}' に ICharacter がアタッチされていません。");
            }

            return character;
        }
    }
}