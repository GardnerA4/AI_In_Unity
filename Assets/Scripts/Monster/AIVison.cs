using UnityEngine;
using UnityEngine.AI;

public class AIVision : MonoBehaviour
{
    public Transform player;
    public float visionRange = 15f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask; 

    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in vision range and angle
        if (distanceToPlayer < visionRange)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2f)
            {

                if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }
}