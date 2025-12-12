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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centerPoint = transform.position;

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
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Random point around the center
            Vector3 randomOffset = Random.insideUnitSphere * wanderRadius;
            randomOffset.y = 0f;
            Vector3 rawTarget = centerPoint + randomOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(rawTarget, out hit, 2f, NavMesh.AllAreas))
            {
                Vector3 target = hit.position;
                Vector3 toTarget = target - transform.position;
                toTarget.y = 0f;

                // 1) Require a minimum distance so he doesn't just shuffle
                if (toTarget.magnitude < minMoveDistance)
                    continue;

                // 2) Don't choose a point directly behind him (avoid constant 180° turns)
                Vector3 dirToTarget = toTarget.normalized;
                float forwardDot = Vector3.Dot(transform.forward, dirToTarget);
                if (forwardDot < -0.3f) // -1 = straight back, 1 = straight forward
                    continue;

                agent.SetDestination(target);
                return;
            }
        }

        // If we failed to find anything after several tries, just don't change destination this frame
        // (he'll try again next Update)
        // Debug.LogWarning("WanderSmooth: Couldn't find a good new destination.");
    }
}
