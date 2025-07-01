namespace TechC.MagichesBand.Enemy
{
    public interface ICharacter
    {
        public enum AttackType
        {
            Physical, // 物理攻撃
            Magical   // 魔法攻撃
        }
        
        void Act();
        void TakeDamage(int damage,AttackType type);

        void NextState();

        void ResetStatus();
        Game.CharacterStatus Status { get; }
    }
}
