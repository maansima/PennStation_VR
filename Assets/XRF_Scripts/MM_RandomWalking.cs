using UnityEngine;
using UnityEngine.AI;

public class MM_StraightLinePatrol : MonoBehaviour
{
    public float distance = 10f;   // how far forward he walks before turning
    public float stopThreshold = 0.2f;

    NavMeshAgent agent;
    Vector3 startPos;
    Vector3 forwardDir;
    bool goingForward = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Record starting position and initial forward direction
        startPos = transform.position;
        forwardDir = transform.forward.normalized;

        // Begin by walking forward
        MoveToTarget();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (!agent.pathPending && agent.remainingDistance < stopThreshold)
        {
            goingForward = !goingForward;  // flip direction
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        Vector3 direction = goingForward ? forwardDir : -forwardDir;
        Vector3 target = startPos + direction * distance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Couldn't find NavMesh at target: " + target);
        }
    }
}
