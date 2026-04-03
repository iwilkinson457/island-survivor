namespace ExtractionDeadIsles.Core
{
    public interface IDamageable
    {
        void TakeDamage(float amount, HitZone zone);
        bool IsDead { get; }
    }
}
