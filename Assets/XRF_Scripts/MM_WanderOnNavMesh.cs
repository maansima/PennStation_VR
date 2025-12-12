using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MM_WanderSmooth : MonoBehaviour
{
    public float wanderRadius = 6f;        // overall area size
    public float arrivalDistance = 0.6f;   // how close counts as "arrived"
    public float minMoveDistance = 3f;     // minimum distance to next point
    public float minIdleTime = 0.5f;       // pause when arriving
    public float maxIdleTime = 2f;

    NavMeshAgent agent;
    Vector3 centerPoint;
    float idleTimer = 0f;
    bool isIdling = false;

    [Header("Edge bouncing")]
    public float maxStepDistance = 20f;  // how far to look ahead along the navmesh
    public float edgeMargin = 0.5f;      // how far before the edge we stop

    Vector3 moveDirection;               // current movement direction


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centerPoint = transform.position;

        // Initial direction (use forward if possible, otherwise random)
        moveDirection = transform.forward;
        moveDirection.y = 0f;
        if (moveDirection.sqrMagnitude < 0.0001f)
        {
            moveDirection = Random.insideUnitSphere;
            moveDirection.y = 0f;
        }
        moveDirection.Normalize();


        // Avoid extra early stopping by the agent
        agent.stoppingDistance = 0f;

        PickNewDestination();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (isIdling)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdling = false;
                PickNewDestination();
            }
            return;
        }

        if (agent.pathPending) return;

        if (agent.remainingDistance <= arrivalDistance)
        {
            // Arrived: pause a bit before choosing a new point
            isIdling = true;
            idleTimer = Random.Range(minIdleTime, maxIdleTime);
        }
    }

    void PickNewDestination()
    {
        // Make sure we have a sane horizontal direction
        Vector3 dir = moveDirection;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = transform.forward;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
            {
                dir = Vector3.forward;
            }
            dir.Normalize();
        }

        Vector3 start = transform.position;
        start.y = centerPoint.y; // flatten to navmesh height if needed

        Vector3 end = start + dir * maxStepDistance;

        NavMeshHit hit;
        Vector3 target;

        // Cast along the navmesh to see if we hit an edge between start and end
        bool hitEdge = NavMesh.Raycast(start, end, out hit, NavMesh.AllAreas);

        if (hitEdge)
        {
            // Stop a little before the edge
            target = hit.position - dir * edgeMargin;

            // Next time, go the opposite way (bounce off edge)
            moveDirection = -dir;
        }
        else
        {
            // No edge within maxStepDistance – just go full distance,
            // clamped onto the navmesh.
            if (NavMesh.SamplePosition(end, out hit, 2f, NavMesh.AllAreas))
            {
                target = hit.position;
            }
            else
            {
                // Fallback: if even that fails, nudge direction randomly and try a shorter move
                Vector3 randomOffset = Random.insideUnitSphere * wanderRadius;
                randomOffset.y = 0f;
                if (NavMesh.SamplePosition(centerPoint + randomOffset, out hit, wanderRadius, NavMesh.AllAreas))
                {
                    target = hit.position;
                }
                else
                {
                    // Give up this frame – we'll try again next time
                    return;
                }

                // Also randomize direction a bit so we don't get stuck
                Vector3 newDir = (target - start);
                newDir.y = 0f;
                if (newDir.sqrMagnitude > 0.0001f)
                {
                    moveDirection = newDir.normalized;
                }
            }
        }

        // Optional: enforce a minimum move distance so we don't shuffle in place
        Vector3 toTarget = target - start;
        toTarget.y = 0f;
        if (toTarget.magnitude < minMoveDistance)
        {
            // Push the target a bit further along the current direction if possible
            Vector3 extendedEnd = start + dir * Mathf.Max(minMoveDistance, maxStepDistance * 0.5f);
            if (NavMesh.SamplePosition(extendedEnd, out hit, 2f, NavMesh.AllAreas))
            {
                target = hit.position;
            }
        }

        agent.SetDestination(target);
    }
}
