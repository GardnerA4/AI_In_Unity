using UnityEngine;
using UnityEngine.AI;

public class AIRoamer : MonoBehaviour
{
    public NavMeshAgent agent;
    public float roamRadius = 10f;
    public float waitTime = 2f;

    private Vector3 destination;
    private float waitTimer;

    void Start()
    {
        SetNewDestination();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, destination) < 1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                SetNewDestination();
                waitTimer = 0f;
            }
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            destination = hit.position;
            agent.SetDestination(destination);
        }
    }
}

