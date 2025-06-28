namespace TechC.MagichesBand.Enemy
{
    public interface ICharacter
    {
        void Act();
        void TakeDamage(int damage);

        void NextState();

        void ResetStatus();
        Game.CharacterStatus Status { get; }
    }
}
