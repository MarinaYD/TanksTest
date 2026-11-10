public interface ITankHealth
{

    int GetCurrentHealth { get; set; }

    void TakeDamage(int damage);
    void Die();
    void TakeDamageToDie();
    void SetLoadedHealth(int lives);
}
