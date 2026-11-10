using UnityEngine;

public class EnemyShootingController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float shootingRange = 8f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float aimAngleTolerance = 10f;

    private float nextFireTime;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }
    void Update()
    {
        TryShoot();
    }
    public void TryShoot()
    {
        if (projectilePrefab == null)
            return;

        Vector2 origin = firePoint != null
            ? (Vector2)firePoint.position
            : (Vector2)transform.position;

        Vector2 direction = (Vector2)player.position - origin;
        float distance = direction.magnitude;

        if (distance > shootingRange || distance < 0.001f)
            return;

        Vector2 tankForward = transform.right;
        float angle = Vector2.Angle(tankForward, direction);

        if (angle > aimAngleTolerance)
            return;

        if (Time.time < nextFireTime)
            return;

        Shoot(direction.normalized);
        nextFireTime = Time.time + fireRate;
    }

    private void Shoot(Vector2 direction)
    {
        Vector2 spawnPosition = firePoint != null
            ? (Vector2)firePoint.position
            : (Vector2)transform.position;


        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.Euler(0f, 0f, angle)
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogWarning(
                "Projectile Prefab requires a Rigidbody2D.",
                projectile
            );
        }
    }
}
