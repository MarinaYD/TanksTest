using UnityEngine;

public class TankHealth : MonoBehaviour, ITankHealth
{
    [SerializeField, Min(1)] private int maxHealth = 3;

    protected int currentHealth;

    public int GetCurrentHealth { get { return currentHealth; } set { value = currentHealth; } }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void TakeDamageToDie()
    {
        currentHealth = 0;
        Die();
    }
    public virtual void SetLoadedHealth(int lives)
    {
        currentHealth = lives;
    }
}


