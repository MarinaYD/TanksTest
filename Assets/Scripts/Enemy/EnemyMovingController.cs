using UnityEngine;

public class EnemyMovingController : MonoBehaviour
{
    private enum AIState { Forward, Reversing }

    [Header("Moving Settings")]
    public float moveSpeed = 5f;
    public float reverseSpeed = 2.5f;
    public float turningRadius = 4f;

    [Header("DirectionChangeTimer")]
    public float minDirectionTime = 2f;
    public float maxDirectionTime = 6f;

    [Header("ObstaclesDetection")]
    public float obstacleDetectionRange = 5f;
    public float emergencyReverseRange = 1.8f;

    public LayerMask obstacleMask;

    [Header("Reverse")]
    public float reverseDuration = 2f;

    private AIState currentState = AIState.Forward;
    private Vector2 targetDirection;
    private float directionChangeTimer;
    private float reverseTimer;
    private float blockCooldownTimer;

    // Check for stucking
    private Vector3 lastPosition;
    private float stuckCheckTimer;



    void Start()
    {
        ChooseNewDirection();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (blockCooldownTimer > 0) blockCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case AIState.Forward:
                HandleForwardState();
                break;

            case AIState.Reversing:
                HandleReverseState();
                break;
        }

    }

    /// <summary>
    /// Standart Forward Movement
    /// </summary>
    private void HandleForwardState()
    {
        CheckForObstacles180();

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            ChooseNewDirection();
        }

        MoveAndRotate2D(forward: true);

        CheckIfStuck();
    }

    /// <summary>
    /// Reverse Movement if blocked 
    /// </summary>
    private void HandleReverseState()
    {
        reverseTimer -= Time.deltaTime;

        MoveAndRotate2D(forward: false);

        if (reverseTimer <= 0)
        {
            //Finish Reverse Movement and looking for new direction
            currentState = AIState.Forward;
            ChooseNewDirection();
            stuckCheckTimer = 0f;
        }
    }

    private void ChooseNewDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        targetDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
        directionChangeTimer = Random.Range(minDirectionTime, maxDirectionTime);

    }
    private void CheckForObstacles180()
    {
        Vector2 forwardVector = transform.right;
        Collider2D myCollider = GetComponent<Collider2D>();

        // Check for Stuck emergencyReverseRange
        RaycastHit2D emergencyHit = GetFirstValidHit(transform.position, forwardVector, emergencyReverseRange, myCollider);
        if (emergencyHit.collider != null)
        {
            StartReversing();
            return;
        }

        // turn on 180 if there is obstackle near here
        if (blockCooldownTimer <= 0)
        {
            RaycastHit2D hit = GetFirstValidHit(transform.position, forwardVector, obstacleDetectionRange, myCollider);
            if (hit.collider != null)
            {
                targetDirection = -transform.right;

                // reset basic movement timer
                directionChangeTimer = Random.Range(minDirectionTime, maxDirectionTime);
                blockCooldownTimer = 0.5f;
            }
        }
    }

    /// <summary>
    /// Extended checking: if tank stack in collider infront of it
    /// and its coordinates aren't changing
    /// </summary>
    private void CheckIfStuck()
    {
        stuckCheckTimer += Time.deltaTime;
        if (stuckCheckTimer >= 0.4f) // Check for stuck
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            // if tank was moving , but has moved for 0.15 — it is stuck
            if (distanceMoved < 0.15f && moveSpeed > 0)
            {
                StartReversing();
            }

            lastPosition = transform.position;
            stuckCheckTimer = 0f;
        }
    }

    private void StartReversing()
    {
        currentState = AIState.Reversing;
        reverseTimer = reverseDuration;

        targetDirection = -transform.right;
        blockCooldownTimer = 0.5f;
    }

    private void MoveAndRotate2D(bool forward)
    {
        if (targetDirection == Vector2.zero) return;

        // choose speed according to state
        float currentSpeed = forward ? moveSpeed : reverseSpeed;

        // angular speed w = v / R
        float maxAngularSpeedRad = currentSpeed / Mathf.Max(0.1f, turningRadius);
        float maxAngularSpeedDeg = maxAngularSpeedRad * Mathf.Rad2Deg;

        // calculate rotate angle
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // If reverse
        if (!forward)
        {
            targetAngle += 180f;
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);


        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            maxAngularSpeedDeg * Time.deltaTime
        );

        // Moving: forward (Vector3.right) or back (-Vector3.right) by X
        Vector3 moveVector = forward ? Vector3.right : Vector3.left;
        transform.Translate(moveVector * currentSpeed * Time.deltaTime, Space.Self);
    }

    private RaycastHit2D GetFirstValidHit(Vector2 origin, Vector2 direction, float distance, Collider2D selfCollider)
    {

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, obstacleMask);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider != selfCollider)
            {
                return hit;
            }
        }

        return new RaycastHit2D(); // if no hits
    }


    /*   private void OnDrawGizmosSelected()
       {
           // Green circle — turning R
           Gizmos.color = Color.green;
           Gizmos.DrawWireSphere(transform.position, turningRadius);

           // Red circle — emrgency revese distance
           Gizmos.color = Color.red;
           Gizmos.DrawWireSphere(transform.position, emergencyReverseRange);

           // Yellow rays for detection
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.right * obstacleDetectionRange);
         
       }*/

}
