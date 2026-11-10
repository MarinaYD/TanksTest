using UnityEngine;


public class EnemyTankHealth : TankHealth
{
    private EnemyTankPool pool;

    public void SetPool(EnemyTankPool owner)
    {
        pool = owner;
    }

    public override void Die()
    {
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
        else
        {
            Debug.LogWarning(
                "Enemy has no pool reference. Destroying it.",
                this
            );

            Destroy(gameObject);
        }
    }
}

