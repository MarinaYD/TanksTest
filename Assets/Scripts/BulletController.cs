using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Fire;
    [SerializeField] private int DamagePower = 1;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        ITankHealth tankHealth = collision.gameObject.GetComponent<ITankHealth>();
        if (tankHealth != null)
            tankHealth.TakeDamage(DamagePower);
        Fire.SetActive(true);
        Bullet.SetActive(false);
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

    }
    public void ExplosureEnd()
    {
        Destroy(gameObject);
    }

}
