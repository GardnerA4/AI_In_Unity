using UnityEngine;


public class AIController : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public AIRoamer roamer;
    public AIVision vision;

    private bool chasing = false;

    void Update()
    {
        if (vision.CanSeePlayer())
        {
            chasing = true;
        }
        else if (chasing && Vector3.Distance(transform.position, player.position) > vision.visionRange)
        {
            
            chasing = false;
            roamer.enabled = true;
        }

        if (chasing)
        {
            agent.SetDestination(player.position);
            roamer.enabled = false;
        }
    }
}