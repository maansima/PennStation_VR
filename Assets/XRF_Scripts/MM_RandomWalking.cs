using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MM_RandomWalking : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField] float range = 10f;

    void Start()
    {
        Debug.Log("START called on " + gameObject.name);
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Debug.Log("UPDATE running on " + gameObject.name);

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is NOT on a NavMesh! " + gameObject.name);
            return;
        }

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetRandomDestination();
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection.y = 0f;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, range, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
            Debug.Log("New destination: " + navHit.position);
        }
        else
        {
            Debug.LogWarning("Could not find NavMesh point near randomDirection.");
        }
    }
}