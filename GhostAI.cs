using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 5f;
    
    [Header("Vision")]
    public float visionDistance = 12f;
    public LayerMask obstacleMask;
    
    [Header("Wall Avoidance")]
    public float wallCheckDistance = 2f;
    public float wallAvoidanceForce = 1.5f;
    public int rayCount = 5;
    public float raySpread = 60f;

    [Header("Debug")]
    public bool showDebugRays = true;
    public bool showDebugGizmos = true;

    private Rigidbody rb;
    private Vector3 wanderTarget;
    private float wanderTimer;
    private Vector3 currentVelocity;
    private Vector3 lastPosition;
    private float stuckTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 0.5f;

        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        PickNewWanderTarget();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 desiredDirection;

        if (CanSeePlayer()) {
            desiredDirection = (player.position - transform.position).normalized;
        }
        else
        {
            wanderTimer -= Time.fixedDeltaTime;
            
            if (wanderTimer <= 0f || Vector3.Distance(transform.position, wanderTarget) < 1f)
                PickNewWanderTarget();
            
            desiredDirection = (wanderTarget - transform.position).normalized;
        }

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > 0.3f) {
                PickNewWanderTarget();
                stuckTimer = 0f;
            }
        }
        else {
            stuckTimer = 0f;
        }
        lastPosition = transform.position;

        Vector3 avoidanceDirection = GetWallAvoidanceDirection();
        Vector3 finalDirection = (desiredDirection + avoidanceDirection).normalized;

        Vector3 targetVelocity = finalDirection * speed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 5f);
        
        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);

        if (currentVelocity.magnitude > 0.1f) {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        if (showDebugRays) {
            DrawWallAvoidanceDebug();
        }
    }

    Vector3 GetWallAvoidanceDirection()
    {
        Vector3 avoidanceDir = Vector3.zero;
        Vector3 eyePos = transform.position + Vector3.up * 0.5f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -raySpread / 2f + (raySpread / (rayCount - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(eyePos, direction, out hit, wallCheckDistance, obstacleMask)) {
                float proximity = 1f - (hit.distance / wallCheckDistance);
                Vector3 avoidForce = (transform.position - hit.point).normalized * proximity * wallAvoidanceForce;
                avoidForce.y = 0;
                avoidanceDir += avoidForce;
            }
        }

        return avoidanceDir;
    }

    void DrawWallAvoidanceDebug()
    {
        Vector3 eyePos = transform.position + Vector3.up * 0.5f;

        for (int i = 0; i < rayCount; i++) {
            float angle = -raySpread / 2f + (raySpread / (rayCount - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(eyePos, direction, out hit, wallCheckDistance, obstacleMask))
            {
                Debug.DrawRay(eyePos, direction * hit.distance, Color.yellow);
                Debug.DrawLine(hit.point, hit.point + hit.normal * 0.5f, Color.red);
            }
            else
            {
                Debug.DrawRay(eyePos, direction * wallCheckDistance, Color.green);
            }
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * 0.5f;
        Vector3 targetPos = player.position + Vector3.up * 0.5f;

        Vector3 dir = targetPos - eyePos;
        float dist = dir.magnitude;

        if (dist > visionDistance) return false;

        if (showDebugRays) {
            Debug.DrawRay(eyePos, dir, Color.red);
        }
        
        RaycastHit hit;
        if (Physics.Raycast(eyePos, dir.normalized, out hit, dist, obstacleMask)) {
            return false;
        }

        if (Physics.Raycast(eyePos, dir.normalized, out hit, dist))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    void PickNewWanderTarget()
    {
        for (int i = 0; i < 10; i++) {
            Vector3 randomDir = Random.insideUnitSphere * 6f;
            randomDir.y = 0;
            
            if(player != null) {
                Vector3 toPlayer = (player.position - transform.position).normalized;
                randomDir = Vector3.Lerp(randomDir.normalized, toPlayer, 0.4f) * 6f;
                randomDir.y = 0;
            }
            
            Vector3 potentialTarget = transform.position + randomDir;

            Vector3 eyePos = transform.position + Vector3.up * 0.5f;
            Vector3 dirToTarget = potentialTarget - transform.position;
            
            if (!Physics.Raycast(eyePos, dirToTarget.normalized, dirToTarget.magnitude, obstacleMask))
            {
                if (!Physics.CheckSphere(potentialTarget, 0.3f, obstacleMask)) {
                    wanderTarget = potentialTarget;
                    wanderTimer = Random.Range(2f, 4f);
                    
                    if (showDebugRays)
                        Debug.DrawLine(eyePos, potentialTarget, Color.cyan, 0.5f);
                    
                    return;
                }
            }
            
            if (showDebugRays)
                Debug.DrawLine(eyePos, potentialTarget, Color.red, 0.2f);
        }

        Vector3[] directions = new Vector3[]
        {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            new Vector3(1, 0, 1).normalized, new Vector3(-1, 0, 1).normalized,
            new Vector3(1, 0, -1).normalized, new Vector3(-1, 0, -1).normalized
        };

        foreach (Vector3 dir in directions) {
            Vector3 testTarget = transform.position + dir * 3f;
            Vector3 eyePos = transform.position + Vector3.up * 0.5f;
            Vector3 dirToTarget = testTarget - transform.position;
            
            if (!Physics.Raycast(eyePos, dirToTarget.normalized, dirToTarget.magnitude, obstacleMask))
            {
                if (!Physics.CheckSphere(testTarget, 0.3f, obstacleMask))
                {
                    wanderTarget = testTarget;
                    wanderTimer = Random.Range(2f, 4f);
                    return;
                }
            }
        }

        wanderTarget = transform.position + transform.forward * 2f;
        wanderTimer = Random.Range(1f, 2f);
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        DrawCircle(transform.position, visionDistance, 32);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, visionDistance);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(wanderTarget, 0.5f);
            Gizmos.DrawLine(transform.position, wanderTarget);
            
            if (CanSeePlayer() && player != null) {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, player.position + Vector3.up * 0.5f);
            }
        }
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++) {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}