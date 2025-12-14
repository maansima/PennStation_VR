using UnityEngine;
using UnityEngine.AI;

public class MM_CrowdManager : MonoBehaviour
{
    [Header("Which people can we spawn?")]
    public GameObject[] pedestrianPrefabs;   // person prefabs

    [Header("How many & where")]
    public int numberToSpawn = 20;
    public float spawnRadius = 12f;
    public Transform center;                 // where the crowd is centered

    [Header("Per-person variation")]
    public float minSpeed = 1.3f;
    public float maxSpeed = 2.1f;

    void Start()
    {
        if (center == null)
            center = transform;

        if (pedestrianPrefabs == null || pedestrianPrefabs.Length == 0)
        {
            Debug.LogError("MM_CrowdManager: No pedestrianPrefabs set!");
            return;
        }

        for (int i = 0; i < numberToSpawn; i++)
        {
            SpawnOnePedestrian();
        }
    }

    void SpawnOnePedestrian()
    {
        // Pick one person prefab at random
        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];

        // Find a point on the NavMesh near the center
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f;
        Vector3 rawPos = center.position + randomOffset;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(rawPos, out hit, 3f, NavMesh.AllAreas))
        {
            // Couldn't find a good spot this time – skip
            return;
        }

        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject npc = Instantiate(prefab, hit.position, rot);

        // Optional: give this person a slightly different speed / wander settings
        var agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = Random.Range(minSpeed, maxSpeed);
        }

        var wander = npc.GetComponent<MM_WanderSmooth>();
        if (wander != null)
        {
            // tiny variations so they don't all move identically
            wander.wanderRadius += Random.Range(-1f, 1f);
            wander.minIdleTime += Random.Range(-0.2f, 0.4f);
            wander.maxIdleTime += Random.Range(-0.3f, 0.5f);
        }
    }
}
