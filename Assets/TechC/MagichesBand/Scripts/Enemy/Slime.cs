using TechC.MagichesBand.Battle;
using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Enemy
{
    public class Slime : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus slimeStatus;
        [SerializeField] private BattlePlayerController battlePlayerController;
    
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
        private int consumptionMp; //消費Mp
        public CharacterStatus Status => slimeStatus;
    
        public void Act()
        {
            if(StickRotationDetector.Instance.defeatedEnemy) return;
        
            var damage = 0;
        
            var randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Charge");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, (slimeStatus.atk - battlePlayerController.Status.def) - battlePlayerController.defDoublingValue);
                    damage = CriticalCalculation(damage);
                    battlePlayerController.TakeDamage(damage);
                    break;
                case 1:
                    consumptionMp = 1;
                    if ((slimeStatus.mp - consumptionMp) >= 0)
                    {
                        Debug.Log("Fire");
                        slimeStatus.mp -= consumptionMp;
                        // プレイヤーへのダメージ計算
                        damage = Mathf.Max(0, (slimeStatus.mAtk - battlePlayerController.Status.mDef) - battlePlayerController.defDoublingValue);
                        damage = CriticalCalculation(damage);
                        battlePlayerController.TakeDamage(damage);   
                    }
                    else
                    {
                        Debug.Log("失敗");
                    }
                    break;
            }
            NextState();
        }
    
        /// <summary>
        /// criticalの処理（クリティカルの確率を個別に変えたいのでまとめておかない）
        /// </summary>
        /// <param name="damage"></param>
        private int CriticalCalculation(int damage)
        {
            // ランダム値を生成
            float randomCritical = Random.Range(0.0f, 1.0f);
        
            //ランダム値よりクリティカル確率が上だったら、クリティカルがでる
            if (randomCritical < CriticalRate)
            {
                damage *= CriticalMultiplier;
                Debug.Log("Critical");
            }
            return damage;
        }
    
        public void TakeDamage(int damage)
        {
            var randomEvasion = Random.Range(0.0f, 1.0f);

            if (randomEvasion < EvasionRate)
            {
                Debug.Log($"回避  残HP: {slimeStatus.hp}");
            }
            else
            {
                slimeStatus.hp -= damage;
                Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {slimeStatus.hp}");   
            }

            if (slimeStatus.hp > 0) return;
        
            StickRotationDetector.Instance.OnRotationCompleted += OnVictoryStickRotate;
            StickRotationDetector.Instance.StartDetection();
        }

        private void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
            SceneManager.LoadScene("MainScene");
            Destroy(gameObject);
        }

        public void NextState()
        {
            Debug.Log("NextState");
            if (ButtleTurnManager.Instance.CurrentTurnPhase == ButtleTurnManager.TurnPhase.FirstMove)
            {
                ButtleTurnManager.Instance.CurrentTurnPhase = ButtleTurnManager.TurnPhase.SecondMove;
                battlePlayerController.Act();
            }else if (ButtleTurnManager.Instance.CurrentTurnPhase == ButtleTurnManager.TurnPhase.SecondMove)
            {
                ButtleTurnManager.Instance.CurrentTurnPhase = ButtleTurnManager.TurnPhase.FirstMove;
                ButtleTurnManager.Instance.ProceedTurn();
            }
        }

        public void ResetStatus()
        {
            slimeStatus.hp = slimeStatus.maxHp;
            slimeStatus.mp = slimeStatus.maxMp;
        }
    }
}