
using UnityEngine;
using UnityEngine.Events;

public class PlayerTankController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;

    [SerializeField] private UnityEvent<int> PlayersHealthUpdate;

    private ITankHealth health;

    private void Awake()
    {
        health = GetComponent<ITankHealth>();

    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.W))
            moveInput += 1f;

        if (Input.GetKey(KeyCode.S))
            moveInput -= 1f;

        if (Mathf.Abs(moveInput) > 0f)
        {
            transform.position +=
                transform.right * (moveInput * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.A))
            rotationInput += 1f;

        if (Input.GetKey(KeyCode.D))
            rotationInput -= 1f;

        transform.Rotate(
            Vector3.forward,
            rotationInput * rotationSpeed * Time.deltaTime
        );
    }

    private void HandleShooting()
    {
        if (Time.timeScale == 0f)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning(
                "Assign Bullet Prefab and Fire Point in the Inspector.",
                this
            );
            return;
        }

        GameObject projectile = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }
        else
        {
            Debug.LogWarning(
                "Bullet prefab requires a Rigidbody2D.",
                projectile
            );
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            health.TakeDamageToDie();
        ShowPlayersHealthUpdated();
    }

    public void ShowPlayersHealthUpdated()
    {
        PlayersHealthUpdate.Invoke(health.GetCurrentHealth);
    }

}

